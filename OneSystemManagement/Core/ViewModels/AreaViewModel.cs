using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Controllers;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.ViewModels
{
    public class AreaViewModel
    {
        public AreaViewModel()
        {
            Functions = new HashSet<KeyValuePairResource>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Area Code")]
        public string CodeArea { get; set; }

        [Required]
        [Display(Name = "Area Name")]
        public string AreaName { get; set; }

        [Required]
        public string Description { get; set; }

        public string Heading { get; set; }

        public string Action
        {
            get
            {
                async Task<IActionResult> Update(AreaController c) => await c.Update(this);
                async Task<IActionResult> Create(AreaController c) => await c.Create(this);

                var action = (Id != 0) ? Update : (Func<AreaController, Task<IActionResult>>) Create;

                var getActionName = action.Method.Name.Replace("<get_Action>g__", "");
                var actionName = getActionName.Split("|");
                return actionName[0];
            }
        }

        public ICollection<KeyValuePairResource> Functions { get; set; }
    }
}
