using System.Collections.Generic;

namespace OneSystemManagement.Controllers.Resources
{
    public class UserResultResource
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
    }

    public class UserResultListResource
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string UserIdentifier { get; set; }

        public IEnumerable<string> RoleCodes { get; set; } = new List<string>();
    }
}
