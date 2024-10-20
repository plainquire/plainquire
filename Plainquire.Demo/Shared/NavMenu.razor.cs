﻿#pragma warning disable 1591
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Plainquire.Demo.Startup;
using System;
using System.Threading.Tasks;

namespace Plainquire.Demo.Shared;

public partial class NavMenu : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IHostEnvironment Environment { get; set; } = default!;

    [Parameter] public string Theme { get; set; } = default!;
    [Parameter] public EventCallback ToggleTheme { get; set; }

    private bool IsDevelopment => Environment.IsDevelopment();
    private string _opeApiLink = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var uri = new Uri(NavigationManager.BaseUri);
        _opeApiLink = $"{uri.Scheme}://{uri.Authority}/{OpenApi.API_UI_ROUTE}".TrimEnd('/');
    }
}