using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.Extensions.HttpClient;
using OneSystemManagement.Core.Responses;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    public class AccountController : BaseAdminController
    {
        private readonly IRepository<User> _userRepository;

        public AccountController(IOptions<AppSettings> appSettings, IRepository<User> userRepository) : base(appSettings)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult UnAuthorized()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        private async Task<SingleModelResponse<UserResource>> GetByEmailAsync(string email)
        {
            var userResponse = await HttpRequestFactory.Get(BaseUrl + "/api/user/email/" + email);
            var outputUser = userResponse.ContentAsType<SingleModelResponse<UserResource>>();

            return outputUser;

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var outputUser = await GetByEmailAsync(User.Identity.Name);

            if (outputUser.Model == null)
            {
                ViewBag.Error = outputUser.ErrorMessage;
                return View(model);
            }

            var response = await HttpRequestFactory.Post(BaseUrl + "/api/user/password/change?id=" + outputUser.Model.Id, model);
            var outputModel = response.ContentAsType<SingleModelResponse<ChangePasswordViewModel>>();
            if (!response.IsSuccessStatusCode || outputModel.DidError)
            {
                ViewBag.Error = outputModel.ErrorMessage ?? response.ReasonPhrase;
                return View(model);
            }

            AlertShow();

            await HttpContext.SignOutAsync();

            return RedirectToAction("ChangePassword");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            var response = await HttpRequestFactory.Post(BaseUrl + "/api/user/login?isAdminLogin=true", model);

            var outputModel = response.ContentAsType<SingleModelResponse<ResponseResult>>();
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("loginStatus", outputModel.ErrorMessage);
                return View(model);
            }
            var user = await _userRepository.Query()
                .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                .SingleOrDefaultAsync(x => x.Email == model.Email);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("Identifer", user.UserIdentifier)
            };
            claims.AddRange(user.UserRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole.Role.CodeRole)));
            var userIdentity = new ClaimsIdentity(claims, "SecureLogin");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false,
                    AllowRefresh = false
                });

            return GoToReturnUrl(returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        private IActionResult GoToReturnUrl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }
    }
}