using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Responses.ApiResponses
{
    public interface IUserService : IDisposable
    {
        IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null);
        Task<IActionResult> GetAsync(int id);
        Task<IActionResult> Create([FromBody] SaveUserResource resource);
        Task<IActionResult> Update(int id, [FromBody] SaveUserResource resource);
        Task<IActionResult> Delete(int id);
        Task<User> GetUserWithRelated(int id);
    }
}
