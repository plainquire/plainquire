using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Plainquire.Sort.Abstractions;
using System;

namespace Plainquire.Sort.Swashbuckle.Models;

/// <summary>
/// Sort parameter replacement infos.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SortParameterReplacement"/> class.
/// </remarks>
/// <param name="Parameter">The OpenAPI parameter to replace.</param>
/// <param name="Description">The OpenAPI description of the parameter.</param>
/// <param name="SortedType">The type of the entity to sort.</param>
/// <param name="Configuration">The sort configuration for the sorted type.</param>
public record SortParameterReplacement(
    IOpenApiParameter Parameter,
    ApiParameterDescription Description,
    Type SortedType,
    SortConfiguration Configuration
);