using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;
using OneSystemManagement.Core.Extensions;
using OneSystemManagement.Core.Extensions.HttpClient;
using OneSystemManagement.Core.Responses;
using System.Threading.Tasks;
using OneSystemAdminApi.Core.DataLayer;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    public class AreaController : BaseAdminController
    {
        private readonly IMapper _mapper;

        public async Task<IActionResult> Index(bool isAlert = false)
        {
            if (isAlert) AlertShow();
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/area");
            var outputModel = response.ContentAsType<ListModelResponse<AreaViewModel>>();
            return View(outputModel);
        }

        public async Task<IActionResult> Create()
        {
            var model = new AreaViewModel {Heading = "Thêm mới khu vực"};
            return View("AreaForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AreaViewModel model, string saveCommand = null)
        {
            model.Heading = "Thêm mới khu vực";
            if (!ModelState.IsValid) return View("AreaForm", model);

            var entity = new Area();
            _mapper.Map(model, entity);

            var response = await HttpRequestFactory.Post(BaseUrl + "/api/area", entity);
            var outmodel = response.ContentAsType<SingleModelResponse<AreaViewModel>>();
            if (outmodel.DidError || !response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMsg = outmodel.ErrorMessage ?? response.ReasonPhrase;
                return View("AreaForm", model);
            }
            AlertShow();
            if (saveCommand != Constants.SaveContinute) return RedirectToAction("Index");


            model = outmodel.Model;
            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var outputModel = await GetSingle(id);

            var model = new AreaViewModel();
            _mapper.Map(outputModel.Model, model);
            
            return View("AreaForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AreaViewModel model, string saveCommand = null)
        {
            model.Heading = "Cập nhật khu vực";
            if (!ModelState.IsValid) return View("AreaForm", model);

            var entity = new Area();
            _mapper.Map(model, entity);

            var response = await HttpRequestFactory.Put(BaseUrl + "/api/area/" + model.Id, entity);
            var outmodel = response.ContentAsType<SingleModelResponse<AreaViewModel>>();

            if (outmodel.DidError || !response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMsg = outmodel.ErrorMessage ?? response.ReasonPhrase;
                return View("AreaForm", model);
            }
            AlertShow();
            if (saveCommand != Constants.SaveContinute) return RedirectToAction("Index");

            model = outmodel.Model;
            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await HttpRequestFactory.Delete(BaseUrl + "/api/area/" + id);
            var outmodel = response.ContentAsType<SingleModelResponse<AreaViewModel>>();
            if (outmodel.DidError || !response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    error = outmodel.ErrorMessage ?? response.ReasonPhrase
                });
            }

            return RedirectToAction("Index");
        }

        [NonAction]
        private async Task<SingleModelResponse<AreaViewModel>> GetSingle(int id)
        {
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/area/" + id);
            var outputModel = response.ContentAsType<SingleModelResponse<AreaViewModel>>();
            return outputModel;
        }

        public AreaController(IOptions<AppSettings> appSettings, IMapper mapper) : base(appSettings)
        {
            _mapper = mapper;
        }
    }
}