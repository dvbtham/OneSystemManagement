using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace OneSystemManagement.Core.Extensions.HttpClient
{
    public class PatchContent : StringContent
    {
        public PatchContent(object value)
            : base (JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json-patch+json")
        {
        }
    }
}
