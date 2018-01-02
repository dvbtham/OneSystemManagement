namespace OneSystemManagement.Controllers.Resources
{
    public class UserConfigResource
    {
        public int Id { get; set; }
        public int? IdUser { get; set; }
        public string ApiCode { get; set; }
        public string ApiKey { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        public UserGridResource User { get; set; }
    }
}
