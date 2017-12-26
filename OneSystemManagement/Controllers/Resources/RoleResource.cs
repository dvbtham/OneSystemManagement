using System.ComponentModel.DataAnnotations;

namespace OneSystemManagement.Controllers.Resources
{
    public class RoleResource
    {
        public int Id { get; set; }

        [Required]
        public string CodeRole { get; set; }

        [Required]
        public string RoleName { get; set; }
        public string Description { get; set; }
        
    }
}
