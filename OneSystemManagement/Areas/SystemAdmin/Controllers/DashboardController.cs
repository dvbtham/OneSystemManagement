using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    public class DashboardController : BaseAdminController
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<RoleFunction> _roleFuncRepository;

        public IActionResult Index(int id)
        {
            var data = _userRepository.Query()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.RoleFunctions)
                .ThenInclude(x => x.Area)
                .ThenInclude(x => x.RoleFunctions)
                .ThenInclude(x => x.Function)

                .SingleOrDefault(x => x.Id == id);

            if (data == null) return View();
            var dataEx = new UserEx
            {
                MyRoles = data.UserRoles.Select(x => new MyRole
                {
                    Id = x.Role.Id,
                    Name = x.Role.RoleName,
                    MyAreas = x.Role.RoleFunctions.GroupBy(g => g.AreaId)
                    .Select(g => g.First())
                    .Select(rf => new MyArea
                    {
                        Id = rf.Area.Id,
                        Name = rf.Area.AreaName,
                        MyFunctions = rf.Area.RoleFunctions.Where(ff => ff.IdRole == x.IdRole && ff.AreaId == rf.AreaId)
                                        .Select(k => new KeyValuePairResource
                                        {
                                            Id = k.Function.Id,
                                            Name = k.Function.FunctionName
                                        })
                                        .ToList()
                    }).ToList()
                }).ToList()
            };

            return View(dataEx);
        }

        public DashboardController(IOptions<AppSettings> appSettings, IRepository<User> userRepository, IRepository<RoleFunction> funcRepository) : base(appSettings)
        {
            _userRepository = userRepository;
            _roleFuncRepository = funcRepository;
        }
    }
}