using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Core.ViewModels;

namespace OneSystemManagement.Controllers
{
    public class FileManagerController : Controller
    {
        [Route("file-manager/{subFolder?}")]
        public virtual IActionResult Index(string subFolder)
        {
            var model = new FileViewModel()
            {
                Folder = "UserUploads",
                SubFolder = subFolder
            };

            return View(model);
        }
    }
}