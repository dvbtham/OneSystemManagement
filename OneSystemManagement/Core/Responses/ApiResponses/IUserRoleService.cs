using System.Collections.Generic;
using System.Threading.Tasks;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemManagement.Core.Responses.ApiResponses
{
    public interface IUserRoleService
    {
        Task Delete(IList<UserRole> entities);
    }
}