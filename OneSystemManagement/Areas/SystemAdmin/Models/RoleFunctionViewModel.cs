using System.Collections.Generic;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.ViewModels;

namespace OneSystemManagement.Areas.SystemAdmin.Models
{
    public class RoleFunctionViewModel
    {
        public IList<KeyValuePairResource> Roles { get; set; } = new List<KeyValuePairResource>();

        public int RoleId { get; set; }

        public IList<RoleFunctionTreeViewVm> Areas { get; set; } = new List<RoleFunctionTreeViewVm>();
        public IList<KeyValuePairResource> RoleAreas{ get; set; } = new List<KeyValuePairResource>();
    }

    public class RoleFunctionFormJson
    {
        public int RoleId { get; set; }
        public IList<AreaJson> Areas = new List<AreaJson>();
    }

    public class AreaJson
    {
        public int Id { get; set; }
        public IList<int> Functions = new List<int>();
    }

    public class DeleteRoleFunctionJson
    {
        public int RoleId { get; set; }
        public int AreaId { get; set; }
        public int FunctionId { get; set; }
    }

    public class UpdateRoleFunctionJson
    {
        public int RoleId { get; set; }
        public int AreaId { get; set; }
        public int FunctionId { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class RoleFormResource
    {
        public int Id { get; set; }

        public string CodeRole { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }

        public IList<int> Functions { get; set; } = new List<int>();
        public IList<int> Areas { get; set; } = new List<int>();
    }
}
