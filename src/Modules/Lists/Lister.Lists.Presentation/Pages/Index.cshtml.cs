using Lister.Lists.Application.Queries.GetListNames;
using Lister.Lists.Domain.Views;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lister.Lists.Presentation.Pages;

public class IndexModel(IMediator mediator) : PageModel
{
    public IEnumerable<ListName> ListNames { get; private set; } = new List<ListName>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        GetListNamesQuery query = new();
        ListNames = await mediator.Send(query, cancellationToken);
    }
} 