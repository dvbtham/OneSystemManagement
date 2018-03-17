using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.DataLayer;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    [Area("systemadmin")]
    [Authorize(Roles = "Admin")]
    public class BaseAdminController : Controller
    {
        public BaseAdminController(IOptions<AppSettings> appSettings)
        {
            BaseUrl = appSettings.Value.BaseUrl;
        }

        protected string BaseUrl { get; }

        protected void AlertShow()
        {
            TempData["IsAlert"] = "success";
        }
    }
}