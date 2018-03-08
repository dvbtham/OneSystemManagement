using System.Collections.Generic;
using System.Runtime.Serialization;
using elFinder.NetCore.Models;

namespace OneSystemAdmin.Lib.elFinder.Models.Response
{
    [DataContract]
    internal class TreeResponseModel
    {
        public TreeResponseModel()
        {
            Tree = new List<BaseModel>();
        }

        [DataMember(Name = "tree")]
        public List<BaseModel> Tree { get; private set; }
    }
}