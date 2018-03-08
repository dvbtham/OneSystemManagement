using System.IO;

namespace OneSystemAdmin.Lib.elFinder.Drawing
{
    public class ImageWithMimeType
    {
        public ImageWithMimeType(string mimeType, Stream stream)
        {
            MimeType = mimeType;
            ImageStream = stream;
        }

        public Stream ImageStream { get; private set; }

        public string MimeType { get; private set; }
    }
}