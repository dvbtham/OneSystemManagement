using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace OneSystemManagement.Core.Extensions.TagHelpers
{
    [HtmlTargetElement("one-checkbox")]
    public class CheckBoxTagHelper : TagHelper
    {
        [HtmlAttributeName("for")]
        public int Id { get; set; }

        [HtmlAttributeName("is-checked")]
        public bool? IsChecked { get; set; } = false;

        [HtmlAttributeName("label")]
        public string Label { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            var input = new TagBuilder("input");
            input.Attributes.Add("type", "checkbox");
            input.Attributes.Add("id", Id.ToString());
            var label = new TagBuilder("label");
            label.Attributes.Add("for", Id.ToString());
            label.InnerHtml.Append(Label);
            input.InnerHtml.AppendHtml(label);
            output.Attributes.Add("class", "checkbox checkbox-success");

            if (IsChecked != null && IsChecked.Value)
                input.Attributes.Add("checked", "checked");
            else
                input.Attributes.Remove("checked");

           
            output.Content.SetHtmlContent(input);
        }
    }
}
