using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Lister.Core.Presentation.Inputs;

[HtmlTargetElement("a", Attributes = "lister-button")]
public class ButtonTagHelper : TagHelper
{
    private const string DefaultButtonClass = "lister-button";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.SetAttribute("class", DefaultButtonClass);
        output.Attributes.RemoveAll("lister-button");
    }
}