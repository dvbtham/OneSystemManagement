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
        public string CodeArea { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập thông tin vùng")]
        [Display(Name = "Tên vùng")]
        public string AreaName { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mô tả")]
        [Display(Name = "Mô tả")]
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
}
