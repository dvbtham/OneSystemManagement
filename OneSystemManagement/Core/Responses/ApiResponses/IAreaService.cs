using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.Responses.ApiResponses
{
    public interface IAreaService : IDisposable
    {
        IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null);
        Task<IActionResult> GetAsync(int id);
        Task<IActionResult> Create(AreaResource resource);
        Task<IActionResult> Update(int id, AreaResource resource);
        Task<IActionResult> Delete(int id);
    }
}
