using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Responses.ApiResponses;

namespace OneSystemManagement.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/area")]
    public class AreaApiController : Controller
    {
        private readonly IAreaService _areaService;

        #region ctor
        public AreaApiController(IAreaService areaService)
        {
            _areaService = areaService;
        }
        protected override void Dispose(bool disposing)
        {
            _areaService?.Dispose();

            base.Dispose(disposing);
        }
        #endregion

        #region CRUD Methods

        [HttpGet]
        public IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null)
        {
            return _areaService.GetAll(pageSize, pageNumber, q);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            return await _areaService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AreaResource resource)
        {
            return await _areaService.Create(resource);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AreaResource resource)
        {
            return await _areaService.Update(id, resource);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _areaService.Delete(id);
        }

        #endregion

    }
}