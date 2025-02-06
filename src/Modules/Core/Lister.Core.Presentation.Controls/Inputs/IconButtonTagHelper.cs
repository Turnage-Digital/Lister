using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Lister.Core.Presentation.Controls.Inputs;

[HtmlTargetElement("lister-icon-button")]
public class IconButtonTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        const string classes =
            "my-4 flex h-10 w-10 items-center justify-center rounded-full text-gray-600 transition-colors hover:bg-gray-100";

        output.TagName = "button";
        output.Attributes.SetAttribute("type", "button");
        output.Attributes.SetAttribute("class", classes);

        var childContent = output.GetChildContentAsync().Result;
        output.Content.SetHtmlContent(childContent);
    }
}