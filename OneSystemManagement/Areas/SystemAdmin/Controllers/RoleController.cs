using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;
using OneSystemManagement.Core.Extensions;
using OneSystemManagement.Core.Extensions.HttpClient;
using OneSystemManagement.Core.Responses;
using System.Threading.Tasks;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    public class RoleController : BaseAdminController
    {
        private readonly IMapper _mapper;

        public async Task<IActionResult> Index(bool isAlert = false)
        {
            if (isAlert) AlertShow();
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/role");
            var outputModel = response.ContentAsType<ListModelResponse<RoleViewModel>>();
            return View(outputModel);
        }

        public IActionResult Create()
        {
            var model = new RoleViewModel();
            return View("RoleForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel model, string saveCommand = null)
        {
            if (!ModelState.IsValid) return View("RoleForm", model);

            var entity = new Role();
            _mapper.Map(model, entity);

            var response = await HttpRequestFactory.Post(BaseUrl + "/api/role", entity);
            var outmodel = response.ContentAsType<SingleModelResponse<RoleViewModel>>();
            if (outmodel.DidError || !response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMsg = outmodel.ErrorMessage ?? response.ReasonPhrase;
                return View("RoleForm", model);
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

            var model = new RoleViewModel();
            _mapper.Map(outputModel.Model, model);

            return View("RoleForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(RoleViewModel model, string saveCommand = null)
        {
            if (!ModelState.IsValid) return View("RoleForm", model);

            var entity = new Role();
            _mapper.Map(model, entity);

            var response = await HttpRequestFactory.Put(BaseUrl + "/api/role/" + model.Id, entity);
            var outmodel = response.ContentAsType<SingleModelResponse<RoleViewModel>>();

            if (outmodel.DidError || !response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMsg = outmodel.ErrorMessage ?? response.ReasonPhrase;
                return View("RoleForm", model);
            }
            AlertShow();
            if (saveCommand != Constants.SaveContinute) return RedirectToAction("Index");

            model = outmodel.Model;
            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await HttpRequestFactory.Delete(BaseUrl + "/api/role/" + id);
            var outmodel = response.ContentAsType<SingleModelResponse<RoleViewModel>>();
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
        private async Task<SingleModelResponse<RoleViewModel>> GetSingle(int id)
        {
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/role/" + id);
            var outputModel = response.ContentAsType<SingleModelResponse<RoleViewModel>>();
            return outputModel;
        }

        public RoleController(IOptions<AppSettings> appSettings, IMapper mapper) : base(appSettings)
        {
            _mapper = mapper;
        }
    }
}