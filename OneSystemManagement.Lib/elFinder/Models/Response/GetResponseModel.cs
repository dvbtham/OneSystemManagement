using System.Runtime.Serialization;

namespace OneSystemManagement.Lib.elFinder.Models.Response
{
    [DataContract]
    internal class GetResponseModel
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}