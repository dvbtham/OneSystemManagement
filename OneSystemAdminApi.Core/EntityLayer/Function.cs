using System.Collections.Generic;

namespace OneSystemAdminApi.Core.EntityLayer
{
    public class Function
    {
        public Function()
        {
            Functions = new HashSet<Function>();
            RoleFunctions = new HashSet<RoleFunction>();
        }

        public int Id { get; set; }
        public int? IdArea { get; set; }
        public int? IdFunctionParent { get; set; }
        public string CodeFunction { get; set; }
        public string FunctionName { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public Area Area { get; set; }
        public Function FunctionProp { get; set; }
        public ICollection<Function> Functions { get; set; }
        public ICollection<RoleFunction> RoleFunctions { get; set; }
    }
}
