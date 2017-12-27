using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneSystemManagement.Controllers.Resources
{
    public class SaveUserResource
    {
        public int Id { get; set; }

        [Required]
        public UserInfoResource UserInfo { get; set; }

        public DateTime? LastLogin { get; set; }
        public DateTime? CreateDate { get; set; }

        public ICollection<int> Roles { get; set; }
    }

    public class UserGridResource
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
        public string FullName { get; set; }
        public bool IsAccFacebook { get; set; }
        public bool IsAccGoogle { get; set; }
        public bool IsAccTwitter { get; set; }
        public bool IsAccOutlook { get; set; }
        public bool IsActive { get; set; }
        public string UserIdentifier { get; set; }
        public string UserCode { get; set; }

        public bool IsConfirm { get; set; }
        public bool IsMember { get; set; }
        public bool IsPartner { get; set; }
        public bool IsAdmin { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }

        public int LoginFailed { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionAnswer { get; set; }

        public DateTime? LastLogin { get; set; }
        public DateTime? CreateDate { get; set; }

        public ICollection<KeyValuePairResource> Roles { get; set; }  = new List<KeyValuePairResource>();
    }
}
