using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.Responses.ApiResponses;

namespace OneSystemManagement.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/role")]
    public class RoleApiController : BaseApiController
    {
        #region ctor
        private readonly IRoleService _roleService;

        public RoleApiController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        protected override void Dispose(bool disposing)
        {
            _roleService?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region CRUD Methods

        [HttpGet]
        public IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null)
        {
            return _roleService.GetAll(pageSize, pageNumber, q);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            return await _roleService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleResource resource)
        {
            return await _roleService.Create(resource);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleResource resource)
        {
            return await _roleService.Update(id, resource);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _roleService.Delete(id);
        }

        #endregion
    }
}