#pragma warning disable 1591
using FS.FilterExpressionCreator.Demo.Extensions;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace FS.FilterExpressionCreator.Demo.Shared
{
    public class MainLayoutPage : LayoutComponentBase
    {
        protected string ProductName = string.Empty;
        protected string ProductVersion = string.Empty;
        protected string Copyright = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            ProductName = AssemblyExtensions.GetProgramProduct();
            ProductVersion = AssemblyExtensions.GetProgramProductVersion();
            Copyright = AssemblyExtensions.GetProgramCopyright();
        }
    }
}
