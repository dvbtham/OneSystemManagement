namespace OneSystemManagement.Areas.SystemAdmin.Models
{
    public class ResponseResult
    {
        public string Message { get; set; }
    }
    public class LoginResult
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
