using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Lister.Core.Presentation.Inputs;

[HtmlTargetElement("a", Attributes = "lister-icon-button")]
public class IconButtonTagHelper : TagHelper
{
    private const string DefaultButtonClass = "lister-icon-button";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.SetAttribute("class", DefaultButtonClass);
        output.Attributes.RemoveAll("lister-icon-button");
    }
}