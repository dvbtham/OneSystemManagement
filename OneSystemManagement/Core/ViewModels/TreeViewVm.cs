using System.Collections.Generic;
using System.Linq;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.ViewModels
{
    public class TreeViewVm
    {
        public TreeViewVm()
        {
            ChildItems = new List<TreeViewVm>();
        }

        public IList<KeyValuePairResource> Area { get; set; } = new List<KeyValuePairResource>();

        public IList<KeyValuePairResource> Roles { get; set; } = new List<KeyValuePairResource>();

        public int Id { get; set; }

        public string Name { get; set; }

        public string CodeFunction { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public TreeViewVm Parent { get; set; }
        
        public IList<TreeViewVm> ChildItems { get; set; }

        public IList<string> FunctionRoles { get; set; } = new List<string>();

        public void AddChildItem(TreeViewVm childItem)
        {
            childItem.Parent = this;
            ChildItems.Add(childItem);
        }
    }
}