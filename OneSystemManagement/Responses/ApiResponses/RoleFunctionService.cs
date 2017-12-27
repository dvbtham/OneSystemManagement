using System.Collections.Generic;
using System.Threading.Tasks;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemManagement.Responses.ApiResponses
{
    public class RoleFunctionService : IRoleFunctionService
    {
        private readonly IRepository<RoleFunction> _roleFuncRepository;

        public RoleFunctionService(IRepository<RoleFunction> roleFuncRepository)
        {
            _roleFuncRepository = roleFuncRepository;
        }

        public async Task Delete(IList<RoleFunction> entities)
        {
            foreach (var roleFunction in entities)
            {
                await _roleFuncRepository.DeleteAsync(roleFunction.Id);
            }
           
        }
    }
}
