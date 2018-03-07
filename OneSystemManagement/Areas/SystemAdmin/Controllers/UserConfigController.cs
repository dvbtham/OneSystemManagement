using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.Extensions;
using OneSystemManagement.Core.Extensions.HttpClient;
using OneSystemManagement.Core.Responses;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    public class UserConfigController : BaseAdminController
    {
        private readonly IMapper _mapper;

        public async Task<IActionResult> Index(bool isAlert = false)
        {
            if (isAlert) AlertShow();
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/userconfig");
            var outputModel = response.ContentAsType<ListModelResponse<UserConfigViewModel>>();
            return View(outputModel);
        }

        public async Task<IActionResult> Create()
        {
            var model = new UserConfigViewModel();
            await PrepareUserList(model);
            return View("UserConfigForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserConfigViewModel model, string saveCommand = null)
        {
            await PrepareUserList(model);
            if (!ModelState.IsValid) return View("UserConfigForm", model);

            var entity = new UserConfigResource
            {
                Id = model.Id,
                ApiKey = model.ApiKey,
                ApiCode = model.ApiCode,
                Description = model.Description,
                IdUser = model.IdUser,
                IsActive = model.IsActive
            };

            var response = await HttpRequestFactory.Post(BaseUrl + "/api/userconfig", entity);
            var outmodel = response.ContentAsType<SingleModelResponse<UserConfigViewModel>>();
            if (outmodel.DidError || !response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMsg = outmodel.ErrorMessage ?? response.ReasonPhrase;
                return View("UserConfigForm", model);
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

            var model = new UserConfigViewModel();
            _mapper.Map(outputModel.Model, model);
            await PrepareUserList(model);
            return View("UserConfigForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserConfigViewModel model, string saveCommand = null)
        {
            await PrepareUserList(model);
            if (!ModelState.IsValid) return View("UserConfigForm", model);

            var entity = new UserConfigResource
            {
                Id = model.Id,
                ApiKey = model.ApiKey,
                ApiCode = model.ApiCode,
                Description = model.Description,
                IdUser = model.IdUser,
                IsActive = model.IsActive
            };

            var response = await HttpRequestFactory.Put(BaseUrl + "/api/userconfig/" + model.Id, entity);
            var outmodel = response.ContentAsType<SingleModelResponse<UserConfigViewModel>>();

            if (outmodel.DidError || !response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMsg = outmodel.ErrorMessage ?? response.ReasonPhrase;
                return View("UserConfigForm", model);
            }
            AlertShow();
            if (saveCommand != Constants.SaveContinute) return RedirectToAction("Index");

            model = outmodel.Model;
            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await HttpRequestFactory.Delete(BaseUrl + "/api/userconfig/" + id);
            var outmodel = response.ContentAsType<SingleModelResponse<UserConfigViewModel>>();
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
        private async Task PrepareUserList(UserConfigViewModel model)
        {
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/user");
            var outputModel = response.ContentAsType<ListModelResponse<UserGridResource>>();
            model.UserList = outputModel.Model
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FullName }).ToList();
        }

        [NonAction]
        private async Task<SingleModelResponse<UserConfigViewModel>> GetSingle(int id)
        {
            var response = await HttpRequestFactory.Get(BaseUrl + "/api/userconfig/" + id);
            var outputModel = response.ContentAsType<SingleModelResponse<UserConfigViewModel>>();
            return outputModel;
        }

        public UserConfigController(IOptions<AppSettings> appSettings, IMapper mapper) : base(appSettings)
        {
            _mapper = mapper;
        }
    }
}