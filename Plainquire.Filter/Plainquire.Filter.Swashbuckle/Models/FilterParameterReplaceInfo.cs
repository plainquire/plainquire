using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace Plainquire.Filter.Swashbuckle.Models;

internal record FilterParameterReplaceInfo(List<OpenApiParameter> Parameters, List<Type> EntityFilters);