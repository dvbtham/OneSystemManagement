using System.Runtime.Serialization;

namespace OneSystemAdmin.Lib.elFinder.Models.Response
{
    [DataContract]
    internal class GetResponseModel
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}