using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    [Area("systemadmin")]
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