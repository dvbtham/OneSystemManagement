using System.Collections.Generic;

namespace OneSystemManagement.Controllers.Resources
{
    public class FunctionResource
    {
        public FunctionResource()
        {
            Functions = new List<KeyValuePairResource>();
            Roles = new List<KeyValuePairResource>();
        }
        public int Id { get; set; }
        public int? IdArea { get; set; }
        public int? IdFunctionParent { get; set; }
        public string CodeFuction { get; set; }
        
        public string FuctionName { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public AreaResource Area { get; set; }
        public FunctionResource FunctionProp { get; set; }
        public ICollection<KeyValuePairResource> Functions { get; set; }
        public ICollection<KeyValuePairResource> Roles { get; set; }
    }
}
