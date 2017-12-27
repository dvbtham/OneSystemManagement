using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Responses.ApiResponses;

namespace OneSystemManagement.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/user")]
    public class UserApiController : Controller
    {
        private readonly IUserService _userService;
        public UserApiController(IUserService userService)
        {
            _userService = userService;
        }

        #region CRUD Methods

        [HttpGet]
        public IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null)
        {
            return _userService.GetAll(pageSize, pageNumber, q);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            return await _userService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveUserResource resource)
        {
            return await _userService.Create(resource);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveUserResource resource)
        {
            return await _userService.Update(id, resource);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _userService.Delete(id);
        }
        
        #endregion
    }
}