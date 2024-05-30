#pragma warning disable 1591
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Plainquire.Demo.Extensions;
using System.Threading.Tasks;

namespace Plainquire.Demo.Shared;

public class MainLayoutPage : LayoutComponentBase
{
    protected string ProductName = string.Empty;
    protected string ProductVersion = string.Empty;
    protected string Copyright = string.Empty;

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    protected string? Theme;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        ProductName = AssemblyExtensions.GetProgramProduct() ?? string.Empty;
        ProductVersion = AssemblyExtensions.GetProgramProductVersion() ?? string.Empty;
        Copyright = AssemblyExtensions.GetProgramCopyright() ?? string.Empty;
        Theme = await JsRuntime.InvokeAsync<string>("getTheme");
    }

    protected async Task ToggleTheme()
    {
        Theme = Theme == "dark" ? "light" : "dark";
        await JsRuntime.InvokeVoidAsync("setTheme", Theme);
    }
}