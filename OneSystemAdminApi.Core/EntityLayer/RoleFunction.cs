using System;

namespace OneSystemAdminApi.Core.EntityLayer
{
    public class RoleFunction
    {
        public int Id { get; set; }
        public int IdRole { get; set; }
        public int IdFunction { get; set; }
        public DateTime CreateDate { get; set; }

        public Function Function { get; set; }
        public Role Role { get; set; }
    }
}
