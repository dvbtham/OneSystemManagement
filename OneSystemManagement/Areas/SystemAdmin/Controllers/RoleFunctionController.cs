using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.ViewModels;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    public class RoleFunctionController : BaseAdminController
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Function> _funcRepository;
        private readonly IRepository<Area> _areaRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<RoleFunction> _roleFunctionRepository;

        public RoleFunctionController(IOptions<AppSettings> appSettings,
            IRepository<User> userRepository,
            IRepository<Function> funcRepository,
            IRepository<Area> areaRepository,
            IRepository<Role> roleRepository,
            IRepository<RoleFunction> roleFunctionRepository) : base(appSettings)
        {
            _userRepository = userRepository;
            _funcRepository = funcRepository;
            _areaRepository = areaRepository;
            _roleRepository = roleRepository;
            _roleFunctionRepository = roleFunctionRepository;
        }

        private UserEx FetchData()
        {
            var data = _userRepository.Query()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.RoleFunctions)
                .ThenInclude(x => x.Area)
                .ThenInclude(x => x.RoleFunctions)
                .ThenInclude(x => x.Function)
                .SingleOrDefault(x => x.Email == User.Identity.Name);

            if (data == null) return null;
            var dataEx = new UserEx
            {
                MyRoles = data.UserRoles.Select(usr => new MyRole
                {
                    Id = usr.Role.Id,
                    Name = usr.Role.RoleName,
                    AllAreas = _areaRepository.Query().Select(x => new KeyValuePairResource { Id = x.Id, Name = x.AreaName }).ToList(),
                    MyAreas = usr.Role.RoleFunctions.GroupBy(grf => grf.AreaId)
                        .Select(grf => grf.First())
                        .Select(rf => new MyArea
                        {
                            Id = rf.Area.Id,
                            Name = rf.Area.AreaName,
                            AllFunctions = _funcRepository.Query().Select(x => new KeyValuePairResource { Id = x.Id, Name = x.FunctionName }).ToList(),
                            MyFunctions = rf.Area.RoleFunctions
                                .Where(mfrf => mfrf.IdRole == usr.IdRole && mfrf.AreaId == rf.AreaId && rf.Area.Functions.Select(mf => mf.Id).Contains(mfrf.IdFunction))
                                .Select(kvrf => new FunctionWithRole
                                {
                                    Id = kvrf.Function.Id,
                                    Name = kvrf.Function.FunctionName,
                                    IsWrite = kvrf.IsWrite,
                                    IsRead = kvrf.IsRead
                                }).ToList()
                        }).ToList()
                }).ToList()
            };
            return dataEx;
        }

        public IActionResult Index()
        {
            var result = new RoleFunctionViewModel();
            var user = _userRepository.Query()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefault(x => x.Email == User.Identity.Name);

            result.Roles = user?.UserRoles.Select(x =>
                                                    new KeyValuePairResource
                                                    {
                                                        Id = x.Role.Id,
                                                        Name = x.Role.RoleName
                                                    }).ToList();


            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAreasAsync(int roleId)
        {
            var data = FetchData();

            var result = new RoleFunctionViewModel();

            var areas = _areaRepository.Query()
                .Include(x => x.Functions)
                .ThenInclude(x => x.Functions)
                .ThenInclude(x => x.FunctionProp)
                .ToList();

            var functions = _funcRepository.Query()
                .Include(x => x.FunctionProp)
                .ToList();

            var model = new List<RoleFunctionTreeViewVm>();
            var myRole = data.MyRoles.FirstOrDefault(x => x.Id == roleId);
            foreach (var area in areas)
            {
                var areaTree = new RoleFunctionTreeViewVm
                {
                    Area =
                    {
                        Id = area.Id,
                        Name = area.AreaName
                    },
                    RoleAreas = myRole?.MyAreas.Select(x =>
                    new KeyValuePairResource { Id = x.Id, Name = x.Name }).ToList()
                };

                foreach (var f in functions)
                {
                    var treeViewItem = Map(f, roleId);
                    treeViewItem.Area.Add(areaTree.Area);
                    treeViewItem.Roles = GetFunctionRoles(f.Id);
                    treeViewItem.FunctionRoles = FetchRoles(f.Id, roleId);
                    areaTree.Functions.Add(treeViewItem);
                }
                var myArea = myRole?.MyAreas.FirstOrDefault(x => x.Id == areaTree.Area.Id);
                if (myArea != null)
                    areaTree.RoleFunctions = myArea.MyFunctions.Select(x => new TreeViewVm { Id = x.Id, Name = x.Name }).ToList();

                model.Add(areaTree);
            }
            result.RoleAreas = myRole?.MyAreas.Select(x => new KeyValuePairResource { Id = x.Id, Name = x.Name }).ToList();
            result.Areas = model;
            result.RoleId = roleId;
            return PartialView("_RoleFunction", result);
        }

        private IList<string> FetchRoles(int functionId, int roleId)
        {
            var roles = new List<string>();
            var roleFunctions = _roleFunctionRepository.Query()
                .Where(x => x.IdRole == roleId && x.IdFunction == functionId);

            foreach (var rf in roleFunctions)
            {
                if (rf.IsRead) roles.Add("Đọc");
                if (rf.IsWrite) roles.Add("Ghi");
            }

            return roles;
        }

        [NonAction]
        private IList<KeyValuePairResource> GetFunctionRoles(int functionId)
        {
            var roles = _roleFunctionRepository.Query()
                .Where(x => x.IdFunction == functionId)
                .Select(x => new KeyValuePairResource
                {
                    Id = x.IdRole,
                    Name = x.Role.RoleName
                }).ToList();

            return roles;
        }

        public async Task<IActionResult> SaveChanges(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json)) return Json(new { status = false, message = "Dữ liệu rỗng" });

                var model = JsonConvert.DeserializeObject<RoleFunctionFormJson>(json);
                var role = await _roleRepository.Query().Include(x => x.RoleFunctions)
                    .FirstOrDefaultAsync(x => x.Id == model.RoleId);
                var functionRoleToDelete = new List<RoleFunction>();

                var deletedarea = role.RoleFunctions
                    .Where(x => x.IdRole == model.RoleId)
                    .Select(x => x.AreaId)
                    .Where(x => !model.Areas.Select(a => a.Id).Contains(x)).ToList();

                if (deletedarea.Any())
                {
                    foreach (var i in deletedarea)
                    {
                        var temp = _roleFunctionRepository.Query()
                            .Where(x => x.AreaId == i);
                        await _roleFunctionRepository.DeleteRangeAsync(temp.ToList());
                    }
                    return Json(new { status = true, message = "Phân quyền chức năng thành công" });
                }

                foreach (var area in model.Areas)
                {
                    var deletedFunctions = role.RoleFunctions
                        .Where(x => x.AreaId == area.Id)
                        .Select(x => x.IdFunction)
                        .Where(x => !area.Functions.Contains(x)).ToList();

                    foreach (var function in area.Functions)
                    {
                        var roleFunctionAdd = await _roleFunctionRepository.Query()
                            .FirstOrDefaultAsync(x => x.AreaId == area.Id && x.IdRole == model.RoleId && x.IdFunction == function);
                        foreach (var fId in deletedFunctions)
                        {
                            var roleFunctionsToDelete = role.RoleFunctions
                            .FirstOrDefault(x => x.AreaId == area.Id && x.IdRole == model.RoleId && x.IdFunction == fId);
                            functionRoleToDelete.Add(roleFunctionsToDelete);
                        }

                        if (roleFunctionAdd == null)
                        {
                            var entity = new RoleFunction
                            {
                                IdFunction = function,
                                AreaId = area.Id,
                                IdRole = model.RoleId,
                                CreateDate = DateTime.Now,
                                IsRead = true,
                                IsWrite = true
                            };

                            _roleFunctionRepository.Add(entity);
                        }
                    }
                }
                var finalResult = functionRoleToDelete.Distinct().ToList();
                foreach (var roleFunction in finalResult)
                {
                    _roleFunctionRepository.Delete(roleFunction);
                }
                _roleFunctionRepository.SaveChanges();
                return Json(new { status = true, message = "Phân quyền chức năng thành công" });
            }
            catch (Exception e)
            {
                return Json(new { status = false, message = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json)) return Json(new { status = false, message = "Dữ liệu rỗng" });

                var model = JsonConvert.DeserializeObject<DeleteRoleFunctionJson>(json);

                var roleFunction = _roleFunctionRepository.Query()
                    .SingleOrDefault(x => x.AreaId == model.AreaId && x.IdRole == model.RoleId && x.IdFunction == model.FunctionId);
                if (roleFunction == null) return Json(new { status = false, message = "Không tìm thấy dữ liệu" });

                _roleFunctionRepository.Delete(roleFunction);
                _roleRepository.SaveChanges();
                return Json(new { status = true, message = "Cập nhật thành công" });
            }
            catch (Exception e)
            {
                return Json(new { status = false, message = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json)) return Json(new { status = false, message = "Dữ liệu rỗng" });

                var model = JsonConvert.DeserializeObject<UpdateRoleFunctionJson>(json);

                var roleFunction = _roleFunctionRepository.Query()
                    .SingleOrDefault(x => x.AreaId == model.AreaId && x.IdRole == model.RoleId && x.IdFunction == model.FunctionId);
                if (roleFunction == null) return Json(new { status = false, message = "Bạn chưa thể phân quyền cho chức năng này!" });

                roleFunction.IsWrite = model.Roles.Contains("IsWrite");
                roleFunction.IsRead = model.Roles.Contains("IsRead");

                _roleFunctionRepository.SaveChanges();

                return Json(new { status = true, message = "Cập nhật thành công" });
            }
            catch (Exception e)
            {
                return Json(new { status = false, message = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRole(int roleId, int areaId, int functionId)
        {
            try
            {
                var roleFunction = _roleFunctionRepository.Query()
                    .SingleOrDefault(x => x.AreaId == areaId && x.IdRole == roleId && x.IdFunction == functionId);

                if (roleFunction == null) return Json(new { status = false, message = "Không tìm thấy dữ liệu" });

                _roleFunctionRepository.SaveChanges();
                var roles = new List<string>();

                if (roleFunction.IsRead)
                    roles.Add("IsRead");
                if (roleFunction.IsWrite)
                    roles.Add("IsWrite");

                return Json(new { status = true, data = roles, message = "Cập nhật thành công" });
            }
            catch (Exception e)
            {
                return Json(new { status = false, message = e.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteArea(int id)
        {
            try
            {
                var roleFunctions = _roleFunctionRepository.Query()
                    .Where(x => x.AreaId == id);

                foreach (var roleFunction in roleFunctions)
                {
                    _roleFunctionRepository.Delete(roleFunction);
                }
                _roleFunctionRepository.SaveChanges();
                return Json(new { status = true, message = "Cập nhật thành công" });
            }
            catch (Exception e)
            {
                return Json(new { status = false, message = e.Message });
            }
        }

        [NonAction]
        private TreeViewVm Map(Function function, int roleId = 0)
        {
            var treeViewVm = new TreeViewVm
            {
                Id = function.Id,
                Name = function.FunctionName,
                CodeFunction = function.CodeFunction,
                Description = function.Description,
                Url = function.Url
            };

            var childFunctions = function.Functions;
            foreach (var childCategory in childFunctions)
            {
                var childTreeViewVm = Map(childCategory);
                childTreeViewVm.FunctionRoles = FetchRoles(childCategory.Id, roleId);
                treeViewVm.AddChildItem(childTreeViewVm);
            }

            return treeViewVm;
        }

    }
}