using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneSystemManagement.Controllers.Resources
{
    /// <summary>
    /// Use this class for create and update user.
    /// </summary>
    public class SaveUserResource
    {
        public int Id { get; set; }

        [Required]
        public UserInfoResource UserInfo { get; set; }

        public DateTime? LastLogin { get; set; }
        public DateTime? CreateDate { get; set; }

        public ICollection<int> Roles { get; set; }
    }
}
