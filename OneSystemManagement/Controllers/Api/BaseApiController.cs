using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Core.CustomAttribute;

namespace OneSystemManagement.Controllers.Api
{
    [ApiValidate("one")]
    public class BaseApiController : Controller
    {
    }
}