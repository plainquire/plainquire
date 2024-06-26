﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace Plainquire.Demo.Routing;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal class V1ApiController : ControllerAttribute, IApiBehaviorMetadata, IRouteTemplateProvider, IApiDescriptionGroupNameProvider
{
    private const string API_PREFIX = "api";
    public const string API_VERSION = "v1";

    /// <inheritdoc />
    public string GroupName => API_VERSION;

    /// <inheritdoc />
    public string Template => $"{API_PREFIX}/[controller]/[action]";

    /// <inheritdoc />
    public int? Order => 0;

    /// <inheritdoc />
    public string Name => "[controller]_[action]";
}