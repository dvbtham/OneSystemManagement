using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemManagement.Controllers
{
    public class AreaController : Controller
    {
        private readonly IRepository<Area> _areareRepository;
        private readonly IRepository<Function> _functionRepository;

        public AreaController(IRepository<Area> areareRepository, IRepository<Function> functionRepository)
        {
            _areareRepository = areareRepository;
            _functionRepository = functionRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Get()
        {
            var areas = _areareRepository.Query().Include(x => x.Functions);

            var result = areas.Select(a => new
            {
                id = a.Id,
                text = a.AreaName,
                children = _functionRepository.Query()
                .Select(c => new
                {
                    id = c.Id,
                    text = c.FuctionName,
                    @checked = a.Functions.Any(x => x.Id == c.Id),
                    children = _functionRepository.Query().Where(x => x.Id == c.Id).Include(x => x.Functions).Select(x => new
                    {
                        id = x.Id,
                        text = x.FuctionName,
                        @checked = x.Functions.Any(y => y.Id == x.Id),
                        children = a.Functions.Select(v => new
                        {
                            id = v.Id,
                            text = v.FuctionName
                        })
                    })
                })
            });
            return Json(result);
        }
    }
}