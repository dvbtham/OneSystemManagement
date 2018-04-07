using System;
using System.Collections.Generic;

namespace OneSystemManagement.Controllers.Resources
{
    /// <summary>
    /// Return user object without password.
    /// </summary>
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
        public ICollection<UserConfigResource> UserConfigs { get; set; }  = new List<UserConfigResource>();
    }
}