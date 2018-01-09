using System.Collections.Generic;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.ViewModels
{
    public class AreaTreeViewVm
    {
        public KeyValuePairResource Area { get; set; } = new KeyValuePairResource();
        public IList<TreeViewVm> Functions { get; set; } = new List<TreeViewVm>();
    }
}
