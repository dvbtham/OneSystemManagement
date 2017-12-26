using System;
using System.Collections.Generic;

namespace OneSystemManagement.Controllers.Resources
{
    public class UserResource
    {
        public UserResource()
        {
            Roles = new HashSet<UserRoleResource>();
        }

        public int Id { get; set; }
        
        public UserInfoResource UserInfoResource { get; set; }

        public DateTime? LastLogin { get; set; }
        public DateTime? CreateDate { get; set; }

        public ICollection<UserRoleResource> Roles { get; set; }
    }
}
