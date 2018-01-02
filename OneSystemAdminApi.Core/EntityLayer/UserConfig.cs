namespace OneSystemAdminApi.Core.EntityLayer
{
    public class UserConfig
    {
        public int Id { get; set; }
        public int? IdUser { get; set; }
        public string ApiCode { get; set; }
        public string ApiKey { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        public User User { get; set; }
    }
}
