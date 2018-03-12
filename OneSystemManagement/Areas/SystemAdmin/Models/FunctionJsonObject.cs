using System.Collections.Generic;

namespace OneSystemManagement.Areas.SystemAdmin.Models
{
    public class FunctionJsonObject
    {
        public int AreaId { get; set; }
        public IList<int> FunctionIds { get; set; } = new List<int>();
    }
}
