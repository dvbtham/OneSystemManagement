using System;

namespace OneSystemAdminApi.Core.EntityLayer
{
    public class UserRole
    {
        public int Id { get; set; }
        public int IdRole { get; set; }
        public int IdUser { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public Role Role { get; set; }
        public User User  { get; set; }
    }
}
