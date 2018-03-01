using System.Runtime.Serialization;

namespace OneSystemManagement.Lib.elFinder.Models
{
    [DataContract]
    internal class ImageModel : FileModel
    {
        [DataMember(Name = "tmb")]
        public object Thumbnail { get; set; }

        [DataMember(Name = "dim")]
        public string Dimension { get; set; }
    }
}