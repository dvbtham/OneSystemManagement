using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.Extensions;
using OneSystemManagement.Core.Extensions.HttpClient;
using OneSystemManagement.Core.Responses;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    public class FunctionController : BaseAdminController
    {
        private readonly IMapper _mapper;

        public async Task<IActionResult> Index(int roleId, bool isAlert = false, bool functionByRole = false)
        {
            if (isAlert) AlertShow();
            var response = new HttpResponseMessage();
            var outputModel = new ListModelResponse<FunctionViewModel>();
            if (functionByRole)
            {
                response = await HttpRequestFactory.Get(BaseUrl + "/api/function/byroleId/" + roleId);
                outputModel = response.ContentAsType<ListModelResponse<FunctionViewModel>>();
                return View(outputModel);
            }
            response = await HttpRequestFactory.Get(BaseUrl + "/api/function");
            outputModel = response.ContentAsType<ListModelResponse<FunctionViewModel>>();
            return View(outputModel);
        }

        public FunctionController(IOptions<AppSettings> appSettings, IMapper mapper) : base(appSettings)
        {
            _mapper = mapper;
        }

        public async Task<ActionResult> Create()
        {
            var model = new FunctionViewModel();
            await PrepareAreaList(model);
            await PrepareFunctionList(model);
            await PrepareRoleList(model);
            return View("FunctionForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FunctionViewModel model, string saveCommand = null)
        {
            await PrepareAreaList(model);
            await PrepareFunctionList(model);
            await PrepareRoleList(model);

            if (!ModelState.IsValid)
                return View("FunctionForm", model);

            var saveFunctionResource = new SaveFunctionResource
            {
                CodeFunction = model.CodeFunction,
                FunctionName = model.FunctionName,
                IdArea = model.IdArea,
                IdFunctionParent = model.IdFunctionParent,
                Description = model.Description,
                Roles = model.RoleIds,
                Url = model.Url
            };

            var response = await HttpRequestFactory.Post(BaseUrl + "/api/function", saveFunctionResource);
            var outmodel = response.ContentAsType<SingleModelResponse<FunctionViewModel>>();
            if (outmodel.DidError || !response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMsg = outmodel.ErrorMessage ?? response.ReasonPhrase;
                return View("FunctionForm", model);
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

            var model = new FunctionViewModel();

            _mapper.Map(outputModel.Model, model);

            await PrepareAreaList(model);
            await PrepareFunctionList(model);
            await PrepareRoleList(model);

            return View("FunctionForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(FunctionViewModel model, string saveCommand = null)
        {
            await PrepareAreaList(model);
            await PrepareFunctionList(model);
            await PrepareRoleList(model);
            if (!ModelState.IsValid) return View("FunctionForm", model);

            var saveFunctionResource = new SaveFunctionResource
            {
                CodeFunction = model.CodeFunction,
                FunctionName = model.FunctionName,
                IdArea = model.IdArea,
                IdFunctionParent = model.IdFunctionParent,
                Description = model.Description,
                Roles = model.RoleIds,
                Url = model.Url
            };

            var response = await HttpRequestFactory.Put(BaseUrl + "/api/function/" + model.Id, saveFunctionResource);
            var outmodel = response.ContentAsType<SingleModelResponse<FunctionViewModel>>();

            if (outmodel.DidError || !response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMsg = outmodel.ErrorMessage ?? response.ReasonPhrase;
                return View("FunctionForm", model);
            }
            AlertShow();
            if (saveCommand != Constants.SaveContinute) return RedirectToAction("Index");

            model = outmodel.Model;
            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await HttpRequestFactory.Delete(BaseUrl + "/api/function?id=" + id);
            var outmodel = response.ContentAsType<SingleModelResponse<FunctionViewModel>>();
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
        private async Task<SingleModelResponse<FunctionViewModel>> GetSingle(int id)
        {
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/function/" + id);
            var outputModel = response.ContentAsType<SingleModelResponse<FunctionViewModel>>();
            return outputModel;
        }

        [NonAction]
        private async Task PrepareAreaList(FunctionViewModel model)
        {
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/area");
            var outputModel = response.ContentAsType<ListModelResponse<AreaViewModel>>();
            model.AreaList = outputModel.Model
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.AreaName }).ToList();
        }

        [NonAction]
        private async Task PrepareRoleList(FunctionViewModel model)
        {
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/role");
            var outputModel = response.ContentAsType<ListModelResponse<RoleResource>>();
            model.AllRole = outputModel.Model
                .Select(x => new KeyValuePairResource { Id = x.Id, Name = x.RoleName }).ToList();
        }

        [NonAction]
        private async Task PrepareFunctionList(FunctionViewModel model)
        {
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/function");
            var outputModel = response.ContentAsType<ListModelResponse<FunctionViewModel>>();
            model.ParentFuncList = outputModel.Model
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FunctionName }).ToList();
        }
    }
}