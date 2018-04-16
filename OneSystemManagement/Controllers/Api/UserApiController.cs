using Microsoft.AspNetCore.Authorization;
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
        /// Danh sách người dùng
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
        /// Người dùng theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            return await _userService.GetAsync(id);
        }

        /// <summary>
        /// Thêm mới người dùng
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveUserResource resource)
        {
            return await _userService.Create(resource);
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveUserResource resource)
        {
            return await _userService.Update(id, resource);
        }

        [HttpPut("update/{email}")]
        public async Task<IActionResult> UpdateEmail(string email, [FromBody] UserResultListResource resource)
        {
            return await _userService.UpdateEmail(email, resource);
        }

        /// <summary>
        /// Xóa người dùng theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _userService.Delete(id);
        }

        /// <summary>
        /// Đăng nhập hệ thống
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel request, bool isAdminLogin = false)
        {
            var loginCode = await _userService.Login(request.Email, request.Password, isAdminLogin: isAdminLogin);
            var response = new SingleModelResponse<LoginResult>();
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

                    response.Message = "Đăng nhập thành công";
                    response.DidError = false;
                    response.Model = new LoginResult
                    {
                        Email = request.Email,
                        Token = new JwtSecurityTokenHandler().WriteToken(token)
                    };
                    return response.ToHttpResponse();

                case (int)LoginStatus.NotAdmin:
                    response.ErrorMessage = "Bạn không có quyền truy cập.";
                    response.DidError = true;
                    return response.ToHttpResponse();

                case (int)LoginStatus.NotActived:
                    response.ErrorMessage = "Tài khoản của bạn chưa được kích hoạt.";
                    response.DidError = true;
                    return response.ToHttpResponse();

                case (int)LoginStatus.NotConfirmed:
                    response.ErrorMessage = "Tài khoản của bạn chưa được xét duyệt.";
                    response.DidError = true;
                    return response.ToHttpResponse();
            }

            response.ErrorMessage = "Sai email hoặc mật khẩu.";
            response.DidError = true;
            return response.ToHttpResponse();
        }

        /// <summary>
        /// Đổi mật khẩu
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

        /// <summary>
        /// Người dùng theo email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            return await _userService.GetByEmail(email);
        }

        /// <summary>
        /// Lấy tất cả vùng, chức năng của người dùng theo quyền hạn
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("areas/{email}")]
        public async Task<IActionResult> GetAreasByEmail(string email)
        {
            return await _userService.GetAreasByEmail(email);
        }

        /// <summary>
        /// Đặt lại mật khẩu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
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