using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Areas.SystemAdmin.Controllers;
using OneSystemManagement.Controllers.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OneSystemManagement.Areas.SystemAdmin.Models
{
    public class AreaViewModel
    {
        public AreaViewModel()
        {
            Functions = new HashSet<KeyValuePairResource>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mã Code")]
        [Display(Name = "Mã code")]
        [StringLength(30, ErrorMessage = "{0} chỉ nhập tối đa {1} ký tự")]
        public string CodeArea { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập thông tin vùng")]
        [Display(Name = "Tên vùng")]
        [StringLength(300, ErrorMessage = "{0} chỉ nhập tối đa {1} ký tự")]
        public string AreaName { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mô tả")]
        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "{0} chỉ nhập tối đa {1} ký tự")]
        public string Description { get; set; }

        [IgnoreMap]
        public string Heading { get; set; }

        [IgnoreMap]
        public string Action
        {
            get
            {
                async Task<IActionResult> Update(AreaController c) => await c.Update(this);
                async Task<IActionResult> Create(AreaController c) => await c.Create(this);

                var action = (Id != 0) ? Update : (Func<AreaController, Task<IActionResult>>)Create;

                var getActionName = action.Method.Name.Replace("<get_Action>g__", "");
                var actionName = getActionName.Split("|");
                return actionName[0];
            }
        }

        public ICollection<KeyValuePairResource> Functions { get; set; }
    }

    public class UserEx
    {
        public IList<MyRole> MyRoles = new List<MyRole>();
    }

    public class MyRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RoleCode { get; set; }
        public IList<MyArea> MyAreas { get; set; } = new List<MyArea>();
        public IList<KeyValuePairResource> AllAreas { get; set; } = new List<KeyValuePairResource>();
    }

    public class MyArea
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AreaCode { get; set; }
        public IList<FunctionWithRole> MyFunctions { get; set; } = new List<FunctionWithRole>();
        public IList<KeyValuePairResource> AllFunctions { get; set; } = new List<KeyValuePairResource>();
    }

    public class FunctionWithRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FunctionCode { get; set; }
        public string Url { get; set; }
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }

        public int Parent { get; set; }

        public IList<FunctionWithRole> ChildItems { get; set; } = new List<FunctionWithRole>();

    }
}
