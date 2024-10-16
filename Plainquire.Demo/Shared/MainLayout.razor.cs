#pragma warning disable 1591
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Plainquire.Demo.Extensions;
using Plainquire.Filter.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Plainquire.Demo.Shared;

public partial class MainLayout : LayoutComponentBase
{
    protected string ProductName = string.Empty;
    protected string ProductVersion = string.Empty;
    protected string Copyright = string.Empty;

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected string Theme = "light";

    [SuppressMessage("Design", "MA0119:JSRuntime must not be used in OnInitialized or OnInitializedAsync", Justification = "False positive")]
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        ProductName = AssemblyExtensions.GetProgramProduct() ?? string.Empty;
        ProductVersion = AssemblyExtensions.GetProgramProductVersion() ?? string.Empty;
        Copyright = AssemblyExtensions.GetProgramCopyright() ?? string.Empty;
        Theme = await JsRuntime.InvokeAsync<string>("getTheme");
    }

    protected string GetRoute()
    {
        var uri = new Uri(NavigationManager.Uri);
        return uri.AbsolutePath.Trim('/');
    }

    protected async Task ToggleTheme()
    {
        Theme = Theme.EqualsOrdinal("dark") ? "light" : "dark";
        await JsRuntime.InvokeVoidAsync("setTheme", Theme);
    }
}