using System.Collections.Generic;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.ViewModels
{
    public class AreaViewModel
    {
        public AreaViewModel()
        {
            Functions = new HashSet<KeyValuePairResource>();
        }

        public int Id { get; set; }
        public string CodeArea { get; set; }
        public string AreaName { get; set; }
        public string Description { get; set; }

        public ICollection<KeyValuePairResource> Functions { get; set; }
    }
}
