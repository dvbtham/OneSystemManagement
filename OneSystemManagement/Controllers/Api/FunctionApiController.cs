using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Responses.ApiResponses;

namespace OneSystemManagement.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/function")]
    public class FunctionApiController : BaseApiController
    {
        private readonly IFunctionService _functionService;

        public FunctionApiController(IFunctionService functionService)
        {
            _functionService = functionService;
        }

        protected override void Dispose(bool disposing)
        {
            _functionService?.Dispose();
            base.Dispose(disposing);
        }

        #region CRUD Methods

        [HttpGet]
        public IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null)
        {
            return _functionService.GetAll(pageSize, pageNumber, q);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            return await _functionService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveFunctionResource resource)
        {
            return await _functionService.Create(resource);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveFunctionResource resource)
        {
            return await _functionService.Update(id, resource);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            return await _functionService.Delete(id);
        }

        #endregion
    }
}
