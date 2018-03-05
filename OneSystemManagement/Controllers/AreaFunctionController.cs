using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.ViewModels;

namespace OneSystemManagement.Controllers
{
    public class AreaFunctionController : Controller
    {
        private readonly IRepository<Area> _areaRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<RoleFunction> _roleFunctionRepository;

        public AreaFunctionController(IRepository<Area> areaRepository,
            IRepository<Role> roleRepository, IRepository<RoleFunction> roleFunctionRepository)
        {
            _areaRepository = areaRepository;
            _roleRepository = roleRepository;
            _roleFunctionRepository = roleFunctionRepository;
        }
        public IActionResult Index()
        {
            var areas = _areaRepository.Query()
                .Include(x => x.Functions)
                    .ThenInclude(x => x.Functions)
                    .ToList();

            var roles = _roleRepository.Query().Select(x => new KeyValuePairResource { Id = x.Id, Name = x.RoleName }).ToList();
            ViewBag.Roles = roles;
            var model = new List<AreaTreeViewVm>();

            foreach (var area in areas)
            {
                var areaTree = new AreaTreeViewVm
                {
                    Area =
                    {
                        Id = area.Id,
                        Name = area.AreaName
                    }
                };

                foreach (var f in area.Functions)
                {
                    var treeViewItem = Map(f);
                    treeViewItem.Area.Add(areaTree.Area);
                    treeViewItem.Roles = GetFunctionRoles(f.Id);

                    areaTree.Functions.Add(treeViewItem);
                }
                model.Add(areaTree);
            }

            return View(model);
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