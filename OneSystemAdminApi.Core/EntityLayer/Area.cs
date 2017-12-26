using System.Collections.Generic;

namespace OneSystemAdminApi.Core.EntityLayer
{
    public class Area
    {
        public Area()
        {
            Functions = new HashSet<Function>();
        }

        public int Id { get; set; }
        public string CodeArea { get; set; }
        public string AreaName { get; set; }
        public string Description { get; set; }

        public ICollection<Function> Functions { get; set; }
    }
}
