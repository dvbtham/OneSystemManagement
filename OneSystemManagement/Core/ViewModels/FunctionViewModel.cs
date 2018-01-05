using System.Collections.Generic;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.ViewModels
{
    public class FunctionViewModel
    {
        public FunctionViewModel()
        {
            Functions = new List<KeyValuePairResource>();
        }
        public int Id { get; set; }
        public int? IdArea { get; set; }
        public int? IdFunctionParent { get; set; }
        public string CodeFuction { get; set; }

        public string FuctionName { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public AreaViewModel Area { get; set; }
        public ICollection<KeyValuePairResource> Functions { get; set; }
    }
}
