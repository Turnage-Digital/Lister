using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lister.Lists.Presentation.Pages;

[Authorize]
public class IndexModel : PageModel
{
    public void OnGet()
    {
        
    }
}