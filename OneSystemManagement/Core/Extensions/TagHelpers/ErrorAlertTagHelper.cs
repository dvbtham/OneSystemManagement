using Microsoft.AspNetCore.Razor.TagHelpers;

namespace OneSystemManagement.Core.Extensions.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("error")]
    public class ErrorAlertTagHelper : TagHelper
    {
        [HtmlAttributeName("msg")]
        public string ErrorMsg { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(ErrorMsg)) return;

            var html = $@"
                <div class='alert alert-danger alert-dismissable'>
                    <button type = 'button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>
                    <i class='zmdi zmdi-block pr-15 pull-left'></i><p class='pull-left'>{ErrorMsg}</p>
                    <div class='clearfix'></div>
                </div>";
            output.Content.SetHtmlContent(html);
        }
    }
}
