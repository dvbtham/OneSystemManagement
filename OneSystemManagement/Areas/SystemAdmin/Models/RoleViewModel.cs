using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Areas.SystemAdmin.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OneSystemManagement.Areas.SystemAdmin.Models
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Mã code")]
        [Display(Name = "Mã code")]
        [StringLength(50, ErrorMessage = "{0} chỉ nhập tối đa {1} ký tự")]
        public string CodeRole { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tên quyền")]
        [Display(Name = "Tên quyền")]
        [StringLength(300, ErrorMessage = "{0} chỉ nhập tối đa {1} ký tự")]
        public string RoleName { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "{0} chỉ nhập tối đa {1} ký tự")]
        public string Description { get; set; }

        [IgnoreMap]
        public string Action
        {
            get
            {
                async Task<IActionResult> Update(RoleController c) => await c.Update(this);
                async Task<IActionResult> Create(RoleController c) => await c.Create(this);

                var action = (Id != 0) ? Update : (Func<RoleController, Task<IActionResult>>)Create;

                var getActionName = action.Method.Name.Replace("<get_Action>g__", "");
                var actionName = getActionName.Split("|");
                return actionName[0];
            }
        }
    }
}
