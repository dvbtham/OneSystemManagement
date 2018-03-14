using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Areas.SystemAdmin.Controllers;
using OneSystemManagement.Controllers.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OneSystemManagement.Areas.SystemAdmin.Models
{
    public class FunctionViewModel
    {
        public FunctionViewModel()
        {
            Functions = new List<KeyValuePairResource>();
        }
        public int Id { get; set; }

        [Display(Name = "Khu vực")]
        public int? IdArea { get; set; }

        [Display(Name = "Chức năng cha")]
        public int? IdFunctionParent { get; set; }

        [Display(Name = "Mã code")]
        [Required(ErrorMessage = "Bạn chưa nhập mã code")]
        public string CodeFunction { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tên chức năng")]
        [Display(Name = "Tên chức năng")]
        public string FunctionName { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mô tả")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Url")]
        public string Url { get; set; }

        [Display(Name = "Quyền đọc")]
        public bool IsRead { get; set; } = false;

        [Display(Name = "Quyền ghi")]
        public bool IsWrite { get; set; } = false;

        public AreaResource Area { get; set; }

        public ICollection<KeyValuePairResource> Functions { get; set; }
        
        public ICollection<KeyValuePairResource> Roles { get; set; } = new List<KeyValuePairResource>();
        public ICollection<KeyValuePairResource> AllRole { get; set; } = new List<KeyValuePairResource>();
        
        [Display(Name = "Phân vào nhóm")]
        public IList<int> RoleIds { get; set; } = new List<int>();

        public FunctionResource FunctionProp { get; set; }
        public IList<SelectListItem> AreaList { get; set; } = new List<SelectListItem>();
        public IList<SelectListItem> ParentFuncList { get; set; } = new List<SelectListItem>();

        [IgnoreMap]
        public string Action
        {
            get
            {
                async Task<IActionResult> Update(FunctionController c) => await c.Update(this);
                async Task<IActionResult> Create(FunctionController c) => await c.Create(this);

                var action = (Id != 0) ? Update : (Func<FunctionController, Task<IActionResult>>)Create;

                var getActionName = action.Method.Name.Replace("<get_Action>g__", "");
                var actionName = getActionName.Split("|");
                return actionName[0];
            }
        }
    }
}
