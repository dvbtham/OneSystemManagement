using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneSystemManagement.Controllers.Resources
{
    public class SaveFunctionResource
    {
        public int Id { get; set; }
        public int? IdArea { get; set; }
        public int? IdFunctionParent { get; set; }

        [Required]
        public string CodeFuction { get; set; }

        [Required]
        public string FuctionName { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        
        public ICollection<int> Roles { get; set; }
    }
}
