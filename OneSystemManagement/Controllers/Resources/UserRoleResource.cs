using System;

namespace OneSystemManagement.Controllers.Resources
{
    public class UserRoleResource
    {
        public int Id { get; set; }
        public int IdRole { get; set; }
        public int IdUser { get; set; }
        public DateTime CreateDate { get; set; }

        public RoleResource Role { get; set; }
        public UserResource User { get; set; }

        public UserRoleResource()
        {
            CreateDate = DateTime.Now;
        }
    }
}
