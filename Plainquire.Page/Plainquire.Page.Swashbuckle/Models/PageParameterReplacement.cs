using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Plainquire.Page.Abstractions;
using System;

namespace Plainquire.Page.Swashbuckle.Models;

/// <summary>
/// Page parameter replacement infos.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PageParameterReplacement"/> class.
/// </remarks>
/// <param name="OpenApiParameter">The OpenAPI parameter to replace.</param>
/// <param name="OpenApiDescription">The OpenAPI description of the parameter.</param>
/// <param name="PagedType">The type of the entity to page.</param>
/// <param name="Configuration">The page configuration for the paged type.</param>
public record PageParameterReplacement(
    OpenApiParameter OpenApiParameter,
    ApiParameterDescription OpenApiDescription,
    Type? PagedType,
    PageConfiguration Configuration
);