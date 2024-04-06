// ReSharper disable All
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Swashbuckle.AspNetCore.TestSupport
{
    [ExcludeFromCodeCoverage]
    public static class ModelMetadataFactory
    {
        public static ModelMetadata CreateForType(Type type)
        {
            return new EmptyModelMetadataProvider().GetMetadataForType(type);
        }

        public static ModelMetadata CreateForProperty(Type containingType, string propertyName)
        {
            return new EmptyModelMetadataProvider().GetMetadataForProperty(containingType, propertyName);
        }

        public static ModelMetadata CreateForParameter(ParameterInfo parameter)
        {
            return new EmptyModelMetadataProvider().GetMetadataForParameter(parameter);
        }
    }
}