using System;

namespace OneSystemManagement.Controllers.Resources
{
    public class RoleFunctionResource
    {
        public int Id { get; set; }

        public int IdRole { get; set; }

        public int IdFunction { get; set; }
        
        public int AreaId { get; set; }

        public bool IsRead { get; set; }

        public bool IsWrite { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public FunctionResource Function { get; set; }
        public RoleResource Role { get; set; }
        public AreaResource Area { get; set; }

    }
}
