using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Responses.ApiResponses;

namespace OneSystemManagement.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/userconfig")]
    public class UserConfigApiController : Controller
    {
        private readonly IUserConfigService _userConfigService;

        public UserConfigApiController(IUserConfigService userConfigService)
        {
            _userConfigService = userConfigService;
        }


        #region CRUD Methods

        [HttpGet]
        public IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null)
        {
            return _userConfigService.GetAll(pageSize, pageNumber, q);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            return await _userConfigService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserConfigResource resource)
        {
            return await _userConfigService.Create(resource);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserConfigResource resource)
        {
            return await _userConfigService.Update(id, resource);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _userConfigService.Delete(id);
        }

        #endregion

    }
}