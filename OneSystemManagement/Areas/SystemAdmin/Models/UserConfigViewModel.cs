using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Areas.SystemAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Areas.SystemAdmin.Models
{
    public class UserConfigViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Người dùng")]
        public int? IdUser { get; set; }

        [Display(Name = "Api code")]
        [StringLength(250, ErrorMessage = "{0} chỉ nhập tối đa {1} ký tự")]
        public string ApiCode { get; set; }

        [Display(Name = "Api key")]
        [StringLength(500, ErrorMessage = "{0} chỉ nhập tối đa {1} ký tự")]
        public string ApiKey { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "{0} chỉ nhập tối đa {1} ký tự")]
        public string Description { get; set; }

        public UserResultResource User { get; set; }

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        public IList<SelectListItem> UserList { get; set; } = new List<SelectListItem>();

        [IgnoreMap]
        public string Action
        {
            get
            {
                async Task<IActionResult> Update(UserConfigController c) => await c.Update(this);
                async Task<IActionResult> Create(UserConfigController c) => await c.Create(this);

                var action = (Id != 0) ? Update : (Func<UserConfigController, Task<IActionResult>>)Create;

                var getActionName = action.Method.Name.Replace("<get_Action>g__", "");
                var actionName = getActionName.Split("|");
                return actionName[0];
            }
        }
    }
}
