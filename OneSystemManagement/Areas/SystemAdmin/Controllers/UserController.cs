using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.Extensions;
using OneSystemManagement.Core.Extensions.HttpClient;
using OneSystemManagement.Core.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    public class UserController : BaseAdminController
    {
        private readonly IMapper _mapper;

        public async Task<IActionResult> Index(bool isAlert = false)
        {
            if (isAlert) AlertShow();
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/user");
            var outputModel = response.ContentAsType<ListModelResponse<UserGridResource>>();
            return View(outputModel);
        }

        public async Task<IActionResult> Create()
        {
            var model = new SaveUserViewModel();
            await PrepareRoleList(model);
            return View("UserForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveUserViewModel model, string saveCommand = null)
        {
            await PrepareRoleList(model);
            if (!ModelState.IsValid) return View("UserForm", model);

            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "Bạn chưa nhập mật khẩu");
                return View("UserForm", model);
            }

            var saveUserResource = new SaveUserResource();
            saveUserResource.Modify(model);

            var response = await HttpRequestFactory.Post(BaseUrl + "/api/user", saveUserResource);
            var outmodel = response.ContentAsType<SingleModelResponse<SaveUserViewModel>>();
            if (outmodel.DidError || !response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMsg = outmodel.ErrorMessage ?? response.ReasonPhrase;
                return View("UserForm", model);
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

            var model = new SaveUserViewModel();
            _mapper.Map(outputModel.Model, model);

            await PrepareRoleList(model);
            return View("UserForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SaveUserViewModel model, string saveCommand = null)
        {
            await PrepareRoleList(model);
            if (!ModelState.IsValid) return View("UserForm", model);

            var saveUserResource = new SaveUserResource();
            saveUserResource.Modify(model);

            var response = await HttpRequestFactory.Put(BaseUrl + "/api/user/" + model.Id, saveUserResource);
            var outmodel = response.ContentAsType<SingleModelResponse<SaveUserViewModel>>();

            if (outmodel.DidError || !response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMsg = outmodel.ErrorMessage ?? response.ReasonPhrase;
                return View("UserForm", model);
            }
            AlertShow();
            if (saveCommand != Constants.SaveContinute) return RedirectToAction("Index");

            model = outmodel.Model;
            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await HttpRequestFactory.Delete(BaseUrl + "/api/user/" + id);
            var outmodel = response.ContentAsType<SingleModelResponse<SaveUserViewModel>>();
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
        private async Task<SingleModelResponse<SaveUserViewModel>> GetSingle(int id)
        {
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/user/" + id);
            var outputModel = response.ContentAsType<SingleModelResponse<SaveUserViewModel>>();
            return outputModel;
        }

        [NonAction]
        private async Task PrepareRoleList(SaveUserViewModel model)
        {
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/role");
            var outputModel = response.ContentAsType<ListModelResponse<RoleResource>>();
            model.AllRole = outputModel.Model
                .Select(x => new KeyValuePairResource { Id = x.Id, Name = x.RoleName }).ToList();
        }

        public UserController(IOptions<AppSettings> appSettings, IMapper mapper) : base(appSettings)
        {
            _mapper = mapper;
        }
    }
}