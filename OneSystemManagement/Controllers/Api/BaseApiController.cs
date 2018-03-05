
using Microsoft.AspNetCore.Authorization;
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
    public class TokenRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}