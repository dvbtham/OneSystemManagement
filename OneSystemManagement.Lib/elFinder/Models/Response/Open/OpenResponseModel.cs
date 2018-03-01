using System.Runtime.Serialization;

namespace OneSystemManagement.Lib.elFinder.Models.Response.Open
{
    [DataContract]
    internal class OpenResponse : BaseOpenResponseModel
    {
        public OpenResponse(BaseModel currentWorkingDirectory, FullPath fullPath)
            : base(currentWorkingDirectory)
        {
            Options = new Options(fullPath);
            files.Add(currentWorkingDirectory);
        }
    }
}