using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Demo.Routing;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal class V1ApiController : ControllerAttribute, IApiBehaviorMetadata, IRouteTemplateProvider, IApiDescriptionGroupNameProvider
{
    public const string API_PREFIX = "api";
    public const string API_VERSION = "v1";

    /// <inheritdoc />
    public string GroupName { get; } = API_VERSION;

    /// <inheritdoc />
    public string Template { get; } = $"{API_PREFIX}/{API_VERSION}/[controller]/[action]";

    /// <inheritdoc />
    public int? Order { get; } = 0;

    /// <inheritdoc />
    public string Name { get; } = "[controller]_[action]";
}