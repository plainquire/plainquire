#pragma warning disable 1591
using Microsoft.AspNetCore.Components;
using Plainquire.Demo.Startup;
using System;
using System.Threading.Tasks;

namespace Plainquire.Demo.Shared;

public class NavMenuComponent : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected string OpeApiLink = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var uri = new Uri(NavigationManager.BaseUri);
        OpeApiLink = $"{uri.Scheme}://{uri.Authority}/{OpenApi.API_UI_ROUTE}".TrimEnd('/');
    }
}