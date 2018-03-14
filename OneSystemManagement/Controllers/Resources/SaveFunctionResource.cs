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
        public string CodeFunction { get; set; }

        [Required]
        public string FunctionName { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        [Display(Name = "Quyền đọc")]
        public bool IsRead { get; set; } 

        [Display(Name = "Quyền ghi")]
        public bool IsWrite { get; set; }

        public ICollection<int> Roles { get; set; } = new List<int>();
    }
}
