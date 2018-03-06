using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OneSystemManagement.Areas.SystemAdmin.Models;

namespace OneSystemManagement.Controllers.Resources
{
    /// <summary>
    /// Use this class for create and update user.
    /// </summary>
    public class SaveUserResource
    {
        public int Id { get; set; }

        [Required]
        public UserInfoResource UserInfo { get; set; } = new UserInfoResource();

        public DateTime? LastLogin { get; set; }
        public DateTime? CreateDate { get; set; }

        public ICollection<int> Roles { get; set; } = new List<int>();

        public void Modify(SaveUserViewModel model)
        {
            Id = model.Id;
            UserInfo.Email = model.Email;
            UserInfo.Password = model.Password;
            UserInfo.Phone = model.Phone;
            UserInfo.FullName = model.FullName;
            UserInfo.IsAccTwitter = model.IsAccTwitter;
            UserInfo.IsAccFacebook = model.IsAccFacebook;
            UserInfo.IsAccOutlook = model.IsAccOutlook;
            UserInfo.IsAccGoogle = model.IsAccGoogle;
            UserInfo.IsActive = model.IsActive;

            UserInfo.UserCode = model.UserCode;
            UserInfo.UserIdentifier = model.UserIdentifier;
            UserInfo.ConfirmPassword = model.ConfirmPassword;
            UserInfo.IsConfirm = model.IsConfirm;
            UserInfo.IsMember = model.IsMember;
            UserInfo.IsAdmin = model.IsAdmin;
            UserInfo.IsPartner = model.IsPartner;
            UserInfo.Avatar = model.Avatar;
            UserInfo.LoginFailed = model.LoginFailed;
            UserInfo.QuestionAnswer = model.QuestionAnswer;
            UserInfo.QuestionCode = model.QuestionCode;
            UserInfo.Address = model.Address;

            LastLogin = model.LastLogin;
            CreateDate = model.CreateDate;
            Roles = model.RoleIds;
        }
    }
}
