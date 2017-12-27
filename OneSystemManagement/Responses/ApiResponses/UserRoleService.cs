using System.Collections.Generic;
using System.Threading.Tasks;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemManagement.Responses.ApiResponses
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IRepository<UserRole> _userRoleRepository;

        public UserRoleService(IRepository<UserRole> userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }
        public async Task Delete(IList<UserRole> entities)
        {
            foreach (var userRole in entities)
            {
                await _userRoleRepository.DeleteAsync(userRole.Id);
            }
        }
    }
}
