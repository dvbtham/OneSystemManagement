using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace OneSystemManagement.Controllers.Resources
{
    public class AreaResource
    {
        public int Id { get; set; }

        [Required]
        public string CodeArea { get; set; }

        [Required]
        public string AreaName { get; set; }
        public string Description { get; set; }
        
        public ICollection<FunctionResource> Functions { get; set; }
    }
}
