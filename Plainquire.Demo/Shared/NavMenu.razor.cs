#pragma warning disable 1591
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Plainquire.Demo.Shared;

public class NavMenuComponent : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected string OpeApiLink = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var uri = new Uri(NavigationManager.BaseUri);
        // ReSharper disable once StringLiteralTypo
        OpeApiLink = $"{uri.Scheme}://{uri.Authority}/openapi";
    }
}