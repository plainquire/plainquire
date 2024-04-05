using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Plainquire.Sort.Abstractions;
using System;

namespace Plainquire.Sort.Swashbuckle.Models;

public class SortParameterReplacement
{
    public OpenApiParameter OpenApiParameter { get; set; }

    public ApiParameterDescription OpenApiDescription { get; set; }

    public Type SortedType { get; set; }

    public SortConfiguration Configuration { get; set; }
}