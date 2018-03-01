using System.Runtime.Serialization;

namespace OneSystemManagement.Lib.elFinder.Models.Response.Open
{
    [DataContract]
    internal class DebugResponseModel
    {
        [DataMember(Name = "connector")]
        public string Connector
        {
            get { return ".net"; }
        }
    }
}