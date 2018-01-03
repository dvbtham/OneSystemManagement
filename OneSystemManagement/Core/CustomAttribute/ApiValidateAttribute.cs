using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OneSystemManagement.Core.CustomAttribute
{
    /// <summary>
    /// Validate api for each user.
    /// </summary>
    public class ApiValidateAttribute : ResultFilterAttribute
    {
        private readonly string _apiKey;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apiKey"></param>
        public ApiValidateAttribute(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        /// Check user api.
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var response = new ListModelResponse<string>();
            // Check given api key and current api key.
            // this api key for demo purpose.
            if (_apiKey == Ultilities.Constants.ApiValue)
            {
                base.OnResultExecuting(context);
                return;
            }

            response.DidError = true;
            response.ErrorMessage = "Your api is incorrect, please check and try again.";
            context.Result = new BadRequestObjectResult(response);
            base.OnResultExecuting(context);
        }
    }
}
