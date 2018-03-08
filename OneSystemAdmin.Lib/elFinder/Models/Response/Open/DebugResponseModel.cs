using System.Runtime.Serialization;

namespace OneSystemAdmin.Lib.elFinder.Models.Response.Open
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