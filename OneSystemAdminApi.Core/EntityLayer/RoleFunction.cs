using System;

namespace OneSystemAdminApi.Core.EntityLayer
{
    public class RoleFunction
    {
        public int Id { get; set; }
        public int IdRole { get; set; }
        public int AreaId { get; set; }
        public int IdFunction { get; set; }
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public Function Function { get; set; }
        public Role Role { get; set; }
        public Area Area { get; set; }
    }
}
