﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OneSystemManagement.Areas.SystemAdmin.Models;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.Responses.ApiResponses;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using OneSystemManagement.Core.Responses;

namespace OneSystemManagement.Controllers.Api
{
    /// <summary>
    /// This class for management user.
    /// </summary>
    [Produces("application/json")]
    [Route("api/user")]
    public class UserApiController : BaseApiController
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        private readonly IConfiguration _configuration;
        public UserApiController(IUserService userService, IConfiguration configuration)
        {
            _configuration = configuration;
            _userService = userService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _userService?.Dispose();

            base.Dispose(disposing);
        }

        #region CRUD Methods

        /// <summary>
        /// Return user list with paging.
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null)
        {
            return _userService.GetAll(pageSize, pageNumber, q);
        }

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            return await _userService.GetAsync(id);
        }

        /// <summary>
        /// Add new user to database.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveUserResource resource)
        {
            return await _userService.Create(resource);
        }

        /// <summary>
        /// Update existing user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveUserResource resource)
        {
            return await _userService.Update(id, resource);
        }

        /// <summary>
        /// Delete existing user by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _userService.Delete(id);
        }

        /// <summary>
        /// Login by email and password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel request)
        {
            var loginCode = await _userService.Login(request.Email, request.Password, isAdminLogin: false);

            switch (loginCode)
            {
                case (int)LoginStatus.Success:
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, request.Email)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: "oneoffice.vn",
                        audience: "oneoffice.vn",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: creds);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                    
                case (int)LoginStatus.NotActived:
                    return BadRequest("Tài khoản của bạn chưa được kích hoạt");

                case (int)LoginStatus.NotConfirmed:
                    return BadRequest("Tài khoản của bạn chưa được kiểm duyệt");
            }

            return BadRequest("Không tìm thất kết quả phù hợp");
        }

        /// <summary>
        /// Login by email and password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("adminlogin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginViewModel request, bool isAdminLogin = true)
        {
            var loginCode = await _userService.Login(request.Email, request.Password, isAdminLogin: isAdminLogin);
            var response = new SingleModelResponse<ResponseResult>();
            switch (loginCode)
            {
                case (int)LoginStatus.Success:
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, request.Email)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: "oneoffice.vn",
                        audience: "oneoffice.vn",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: creds);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    });

                case (int)LoginStatus.NotAdmin:
                    var notAdminResponse = new ResponseResult();
                    notAdminResponse.Message = "Bạn không có quyền truy cập.";
                    response.Message = notAdminResponse.Message;
                    return response.ToHttpResponse();

                case (int)LoginStatus.NotActived:
                    var notActivedResponse = new ResponseResult();
                    notActivedResponse.Message = "Tài khoản của bạn chưa được kích hoạt.";
                    response.Message = notActivedResponse.Message;
                    return response.ToHttpResponse();

                case (int)LoginStatus.NotConfirmed:
                    var notConfirmedResponse = new ResponseResult();
                    notConfirmedResponse.Message = "Tài khoản của bạn chưa được xét duyệt.";
                    response.Message = notConfirmedResponse.Message;
                    return response.ToHttpResponse();
            }

            response.Message = "Sai email hoặc mật khẩu.";
            return response.ToHttpResponse();

        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("password/change")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordViewModel changePassword)
        {
            if (ModelState.IsValid)
                return await _userService.ChangePassword(id, changePassword);
            return BadRequest(ModelState);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByUsername(string email)
        {
            return await _userService.GetByEmail(email);
        }

        [HttpPost("password/reset")]
        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {
            if (!string.IsNullOrEmpty(newPassword))
                return await _userService.ResetPassword(id, newPassword);
            return BadRequest("New password is not set.");
        }

        #endregion
    }
}