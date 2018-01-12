using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Core.ViewModels;

namespace OneSystemManagement.Controllers
{
    public class AreaController : Controller
    {
        private readonly IRepository<Area> _areaRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<Function> _functionRepository;

        public AreaController(IRepository<Area> areareRepository, IMapper mapper,
            IRepository<Function> functionRepository)
        {
            _areaRepository = areareRepository;
            _mapper = mapper;
            _functionRepository = functionRepository;
        }

        public IActionResult Index()
        {
            return View(_areaRepository.Query().ToList());
        }

        public IActionResult Create()
        {
            var areaVm = new AreaViewModel
            {
                Heading = "Add new area"
            };
            return View("AreaForm", areaVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AreaViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Heading = "Add new area";
                return View("AreaForm", viewModel);
            }

            var area = new Area();

            _mapper.Map(viewModel, area);

            await _areaRepository.AddAsync(area);

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var area = _areaRepository.Query().FirstOrDefault(x => x.Id == id);
            var areaVm = new AreaViewModel
            {
                Heading = "Update area"
            };
            _mapper.Map(area, areaVm);
            return View("AreaForm", areaVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AreaViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Heading = "Update area";
                return View("AreaForm", viewModel);
            }

            var area = _areaRepository.Query().FirstOrDefault(x => x.Id == viewModel.Id);
            _mapper.Map(viewModel, area);
            await _areaRepository.UpdateAsync(area);
            return RedirectToAction("Index");
        }
    }
}