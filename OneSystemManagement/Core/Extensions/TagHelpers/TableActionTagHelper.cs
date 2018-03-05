using Microsoft.AspNetCore.Razor.TagHelpers;

namespace OneSystemManagement.Core.Extensions.TagHelpers
{
    [HtmlTargetElement("tbl-actions")]
    public class TableActionTagHelper : TagHelper
    {
        [HtmlAttributeName("edit-label")]
        public string EditLabel { get; set; }

        [HtmlAttributeName("del-class-name")]
        public string DeleteClassName { get; set; }

        [HtmlAttributeName("del-label")]
        public string DeleteLabel { get; set; }

        [HtmlAttributeName("tag-id")]
        public string DataId { get; set; }

        [HtmlAttributeName("tag-name")]
        public string DataName { get; set; }

        [HtmlAttributeName("url")]
        public string Url { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var html = $@"<a href='{Url}' data-id='{DataId}' data-name='{DataName}'><i class='fa fa-edit text-success'></i> {EditLabel}</a> 
            &nbsp;<a href = '#' data-id = '{DataId}' data-name = '{DataName}' class='{DeleteClassName}'><i class='fa fa-trash text-danger'></i> {DeleteLabel}</a>";
            output.Content.SetHtmlContent(html);
        }
    }
}
