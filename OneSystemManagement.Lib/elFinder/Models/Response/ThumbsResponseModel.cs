using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneSystemManagement.Lib.elFinder.Models.Response
{
    [DataContract]
    internal class ThumbsResponseModel
    {
        private Dictionary<string, string> images;

        public ThumbsResponseModel()
        {
            images = new Dictionary<string, string>();
        }

        [DataMember(Name = "images")]
        public Dictionary<string, string> Images
        {
            get { return images; }
        }
    }
}