#pragma warning disable 1591
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Schick.Plainquire.Demo.Extensions;

namespace Schick.Plainquire.Demo.Shared;

public class MainLayoutPage : LayoutComponentBase
{
    protected string ProductName = string.Empty;
    protected string ProductVersion = string.Empty;
    protected string Copyright = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        ProductName = AssemblyExtensions.GetProgramProduct() ?? string.Empty;
        ProductVersion = AssemblyExtensions.GetProgramProductVersion() ?? string.Empty;
        Copyright = AssemblyExtensions.GetProgramCopyright() ?? string.Empty;
    }
}