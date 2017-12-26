namespace OneSystemAdminApi.Core.EntityLayer
{
    public class Language
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public bool IsDefault { get; set; }
    }
}
