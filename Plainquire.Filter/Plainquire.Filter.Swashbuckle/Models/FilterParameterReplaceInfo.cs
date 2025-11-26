using Microsoft.OpenApi;
using System;
using System.Collections.Generic;

namespace Plainquire.Filter.Swashbuckle.Models;

internal record FilterParameterReplaceInfo(List<IOpenApiParameter> Parameters, List<Type> EntityFilters);