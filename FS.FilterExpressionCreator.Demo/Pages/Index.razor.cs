#pragma warning disable 1591
using FS.FilterExpressionCreator.Demo.Extensions;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;

namespace FS.FilterExpressionCreator.Demo.Pages
{
    public class IndexPage : ComponentBase
    {
        private const string README_FILE = "README.md";

        protected string ReadmeMarkup = string.Empty;

        [Inject] private IWebHostEnvironment WebHostEnvironment { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var readmePath = System.IO.Path.Combine(WebHostEnvironment.GetWebRootPath(), README_FILE);
            if (!System.IO.File.Exists(readmePath))
                readmePath = System.IO.Path.Combine(WebHostEnvironment.ContentRootPath, "..", README_FILE);

            var pipeline = new MarkdownPipelineBuilder().UseSoftlineBreakAsHardlineBreak().Build();
            var uri = new Uri(NavigationManager.BaseUri);
            ReadmeMarkup = Markdown
                .ToHtml(await System.IO.File.ReadAllTextAsync(readmePath), pipeline)
                .Replace("https://filterexpressioncreator.schick-software.de", $"{uri.Scheme}://{uri.Authority}");
        }
    }
}
