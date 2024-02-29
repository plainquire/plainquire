#pragma warning disable 1591
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Plainquire.Demo.Pages;

public class IndexPage : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected string DemoAppLink = string.Empty;
    protected string OpeApiLink = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var uri = new Uri(NavigationManager.BaseUri);
        DemoAppLink = $"{uri.Scheme}://{uri.Authority}/demo";
        OpeApiLink = $"{uri.Scheme}://{uri.Authority}/openapi";
    }
}