using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Lister.Core.Presentation.Controls.Inputs;

[HtmlTargetElement("lister-button")]
public class ButtonTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        const string classes =
            "rounded-md bg-blue-600 px-3.5 py-2.5 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-600";

        output.TagName = "button";
        output.Attributes.SetAttribute("class", classes);

        var childContent = output.GetChildContentAsync().Result;
        output.Content.SetHtmlContent(childContent);
    }
}