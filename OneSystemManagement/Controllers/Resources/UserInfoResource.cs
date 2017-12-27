using System.ComponentModel.DataAnnotations;

namespace OneSystemManagement.Controllers.Resources
{
    public class UserInfoResource
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public bool IsAccFacebook { get; set; }
        public bool IsAccGoogle { get; set; }
        public bool IsAccTwitter { get; set; }
        public bool IsAccOutlook { get; set; }
        public bool IsActive { get; set; }

        [Required]
        public string UserIdentifier { get; set; }
        public string UserCode { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsConfirm { get; set; }
        public bool IsMember { get; set; }
        public bool IsPartner { get; set; }
        public bool IsAdmin { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        
        public int LoginFailed { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionAnswer { get; set; }
    }
}
