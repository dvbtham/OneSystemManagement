using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;

namespace OneSystemManagement.Areas.SystemAdmin.Controllers
{
    public class DashboardController : BaseAdminController
    {
        public IActionResult Index()
        {
            return View();
        }

        public DashboardController(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }
    }
}