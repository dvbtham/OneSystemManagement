using System.Collections.Generic;

namespace OneSystemAdminApi.Core.EntityLayer
{
    public class Role
    {
        public Role()
        {
            RoleFunctions = new HashSet<RoleFunction>();
            UserRoles = new HashSet<UserRole>();
        }

        public int Id { get; set; }
        public string CodeRole { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public ICollection<RoleFunction> RoleFunctions { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
