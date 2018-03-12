using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OneSystemManagement.Areas.SystemAdmin.Models;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    public class AreaFunctionController : BaseAdminController
    {
        private readonly IRepository<Function> _funcRepository;
        private readonly IRepository<Area> _areaRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<RoleFunction> _roleFunctionRepository;

        public AreaFunctionController(IOptions<AppSettings> appSettings,
            IRepository<Function> funcRepository, IRepository<Area> areaRepository,
            IRepository<Role> roleRepository,
            IRepository<RoleFunction> roleFunctionRepository) : base(appSettings)
        {
            _funcRepository = funcRepository;
            _areaRepository = areaRepository;
            _roleRepository = roleRepository;
            _roleFunctionRepository = roleFunctionRepository;
        }

        public IActionResult Index()
        {
            var areas = _areaRepository.Query()
                .Include(x => x.Functions)
                .ThenInclude(x => x.Functions)
                    .ThenInclude(x => x.FunctionProp)
                .ToList();

            var functions = _funcRepository.Query()
                .Include(x => x.FunctionProp)
                .ToList();
            ViewBag.Functions = functions.Select(x => new KeyValuePairResource { Id = x.Id, Name = x.FunctionName }).ToList();
            var model = new List<AreaTreeViewVm>();

            foreach (var area in areas)
            {
                var areaTree = new AreaTreeViewVm
                {
                    Area =
                    {
                        Id = area.Id,
                        Name = area.AreaName
                    },
                    AllFunctions = functions.Select(x => new KeyValuePairResource { Id = x.Id, Name = x.FunctionName }).ToList()
                };

                foreach (var f in functions)
                {
                    var treeViewItem = Map(f);
                    treeViewItem.Area.Add(areaTree.Area);
                    treeViewItem.Roles = GetFunctionRoles(f.Id);

                    areaTree.Functions.Add(treeViewItem);
                }

                foreach (var f in area.Functions)
                {
                    var treeViewItem = Map(f);
                    treeViewItem.Area.Add(areaTree.Area);
                    treeViewItem.Roles = GetFunctionRoles(f.Id);
                    areaTree.AreaFunctions.Add(treeViewItem);
                }
                model.Add(areaTree);
            }

            return View(model);
        }

        public async Task<IActionResult> ModifyFunction(string jsonObject, string functionNoArea)
        {
            if (string.IsNullOrEmpty(jsonObject))
                return Json(new
                {
                    status = false,
                    message = "Đã xảy ra lỗi khi lưu dữ liệu"
                });
            if (string.IsNullOrEmpty(functionNoArea))
                return Json(new
                {
                    status = false,
                    message = "Đã xảy ra lỗi khi lưu dữ liệu"
                });

            var functionNoAreaMap = JsonConvert.DeserializeObject<List<int>>(functionNoArea);
            var model = JsonConvert.DeserializeObject<IList<FunctionJsonObject>>(jsonObject);

            foreach (var item in model)
            {
                if (item.FunctionIds.Any())
                {
                    foreach (var fId in item.FunctionIds)
                    {
                        var function = await _funcRepository.GetAsync(fId);
                        function.IdArea = item.AreaId;
                        await _funcRepository.UpdateAsync(function);
                    }
                }
                else
                {
                    foreach (var fId in functionNoAreaMap)
                    {
                        var function = await _funcRepository.GetAsync(fId);
                        function.IdArea = null;
                        await _funcRepository.UpdateAsync(function);
                    }
                }
            }
            return Json(new
            {
                status = true,
                message = "Dữ liệu đã được lưu thành công"
            });
        }

        public async Task<IActionResult> ModifyParentFunction(string data)
        {
            if (string.IsNullOrEmpty(data))
                return Json(new
                {
                    status = false,
                    message = "Đã xảy ra lỗi khi lưu dữ liệu"

                });

            var model = JsonConvert.DeserializeObject<FunctionParentChange>(data);

            var function = await _funcRepository.GetAsync(model.Id);
            if (model.ParentId == 0)
                function.IdFunctionParent = null;
            else
                function.IdFunctionParent = model.ParentId;

            await _funcRepository.UpdateAsync(function);

            return Json(new
            {
                status = true,
                message = "Dữ liệu đã được lưu thành công"
            });
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

        [NonAction]
        private TreeViewVm Map(Function function)
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
                treeViewVm.AddChildItem(childTreeViewVm);
            }

            return treeViewVm;
        }

    }
}