#pragma warning disable 1591
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace FS.FilterExpressionCreator.Demo.Pages
{
    public class IndexPage : ComponentBase
    {
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected string DemoAppLink;
        protected string OpeApiLink;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var uri = new Uri(NavigationManager.BaseUri);
            DemoAppLink = $"{uri.Scheme}://{uri.Authority}/demo";
            // ReSharper disable once StringLiteralTypo
            OpeApiLink = $"{uri.Scheme}://{uri.Authority}/openapi";
        }
    }
}
