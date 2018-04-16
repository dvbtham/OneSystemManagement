using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.Responses.ApiResponses
{
    public interface IUserService : IDisposable
    {
        IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null, bool isPaging = false);
        Task<IActionResult> GetAsync(int id);
        Task<IActionResult> Create(SaveUserResource resource);
        Task<IActionResult> Update(int id, SaveUserResource resource);
        Task<IActionResult> UpdateEmail(string email, UserResultListResource resource);
        Task<IActionResult> Delete(int id);
        Task<User> GetUserWithRelated(int id, bool include = true);
        Task<int> Login(string email, string password, bool isAdminLogin = false);
        Task<IActionResult> GetByEmail(string email);
        Task<IActionResult> GetAreasByEmail(string email);

        Task<bool> UserVerifyAsync(string email, string password);
        Task<IActionResult> ChangePassword(int id, ChangePasswordViewModel changePassword);
        Task<IActionResult> ResetPassword(int id, string newPassword);
    }
}
