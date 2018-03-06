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
    public class SaveUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập email")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; }
        
        [Display(Name = "Điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        //[Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        //[Required(ErrorMessage = "Bạn chưa nhập mật khẩu để xác nhận")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Tài khoản Facebook")]
        public bool IsAccFacebook { get; set; }

        [Display(Name = "Tài khoản Google")]
        public bool IsAccGoogle { get; set; }

        [Display(Name = "Tài khoản Twitter")]
        public bool IsAccTwitter { get; set; }

        [Display(Name = "Tài khoản Outlook")]
        public bool IsAccOutlook { get; set; }

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Bạn chưa nhập nhận dạng người dùng")]
        [Display(Name = "Nhận dạng")]
        public string UserIdentifier { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mã code")]
        [Display(Name = "Mã code")]
        public string UserCode { get; set; }

        [Display(Name = "Đã xác nhận")]
        public bool IsConfirm { get; set; } = true;

        [Display(Name = "Thành viên")]
        public bool IsMember { get; set; } = true;

        [Display(Name = "Đối tác")]
        public bool IsPartner { get; set; } = false;

        [Display(Name = "Ban quản trị")]
        public bool IsAdmin { get; set; } = false;

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string Avatar { get; set; }

        [Display(Name = "Đăng nhập thất bại")]
        public int LoginFailed { get; set; } = 0;

        [Display(Name = "Code câu hỏi")]
        public string QuestionCode { get; set; }

        [Display(Name = "Câu trả lời")]
        public string QuestionAnswer { get; set; }

        [Display(Name = "Đăng nhập gần đây")]
        public DateTime LastLogin { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreateDate { get; set; }

        public ICollection<KeyValuePairResource> AllRole { get; set; } = new List<KeyValuePairResource>();

        [Display(Name = "Quyền hạn")]
        [Required(ErrorMessage = "Bạn chưa cấp quyền cho người dùng này")]
        public ICollection<int> RoleIds { get; set; } = new List<int>();

        public ICollection<KeyValuePairResource> Roles { get; set; } = new List<KeyValuePairResource>();

        [IgnoreMap]
        public string Action
        {
            get
            {
                async Task<IActionResult> Update(UserController c) => await c.Update(this);
                async Task<IActionResult> Create(UserController c) => await c.Create(this);

                var action = (Id != 0) ? Update : (Func<UserController, Task<IActionResult>>)Create;

                var getActionName = action.Method.Name.Replace("<get_Action>g__", "");
                var actionName = getActionName.Split("|");
                return actionName[0];
            }
        }
    }
}
