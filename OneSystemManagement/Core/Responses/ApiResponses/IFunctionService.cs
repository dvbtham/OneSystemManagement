using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.Responses.ApiResponses
{
    public interface IFunctionService : IDisposable
    {
        IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null, bool isPaging = false);
        Task<IActionResult> GetAsync(int id);
        Task<IActionResult> Create(SaveFunctionResource resource);
        Task<IActionResult> Update(int id, SaveFunctionResource resource);
        Task<IActionResult> Delete(int id);
        Task<Function> GetFunctionWithRelated(int id, bool include = true);
    }
}
