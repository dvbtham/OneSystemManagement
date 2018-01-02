using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Core.CustomAttribute;

namespace OneSystemManagement.Controllers.Api
{
    // this api key for demo purpose, 
    // change it to ONEDANANG to try it works.
    [ApiValidate("ONEDANANG")] 
    public class BaseApiController : Controller
    {
    }
}