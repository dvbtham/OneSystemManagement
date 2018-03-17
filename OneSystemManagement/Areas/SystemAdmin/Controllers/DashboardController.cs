using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    public class DashboardController : BaseAdminController
    {
        private readonly IRepository<User> _userRepository;

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
                    MyAreas = usr.Role.RoleFunctions.GroupBy(grf => grf.AreaId)
                        .Select(grf => grf.First())
                        .Select(rf => new MyArea
                        {
                            Id = rf.Area.Id,
                            Name = rf.Area.AreaName,
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
            var dataEx = FetchData();
            return dataEx == null ? View() : View(dataEx);
        }

        [HttpGet]
        public async Task<IActionResult> GetAreasAsync(int roleId)
        {
            var data = FetchData();
            var role = data.MyRoles.Single(x => x.Id == roleId);

            return PartialView("AreaList", role.MyAreas);
        }

        public DashboardController(IOptions<AppSettings> appSettings,
           IRepository<User> userRepository) : base(appSettings)
        {
            _userRepository = userRepository;
        }
    }
}