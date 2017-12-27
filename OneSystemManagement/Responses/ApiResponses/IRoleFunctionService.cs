using System.Collections.Generic;
using System.Threading.Tasks;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemManagement.Responses.ApiResponses
{
    public interface IRoleFunctionService
    {
        Task Delete(IList<RoleFunction> entities);
    }
}