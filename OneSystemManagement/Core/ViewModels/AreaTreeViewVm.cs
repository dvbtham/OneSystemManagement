using System.Collections.Generic;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.ViewModels
{
    public class AreaTreeViewVm
    {
        public KeyValuePairResource Area { get; set; } = new KeyValuePairResource();
        public IList<TreeViewVm> Functions { get; set; } = new List<TreeViewVm>();
        public IList<TreeViewVm> AreaFunctions { get; set; } = new List<TreeViewVm>();
    }

    public class RoleFunctionTreeViewVm
    {
        public KeyValuePairResource Area { get; set; } = new KeyValuePairResource();
        public IList<TreeViewVm> Functions { get; set; } = new List<TreeViewVm>();
        public IList<KeyValuePairResource> RoleAreas { get; set; } = new List<KeyValuePairResource>();
        public IList<TreeViewVm> RoleFunctions { get; set; } = new List<TreeViewVm>();
    }
}
