using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Plainquire.Integration.Tests.Extensions;

[ExcludeFromCodeCoverage]
internal static class ControllerExtensions
{
    public static string GetRoute<TController, TResult>(Expression<Func<TController, TResult>> controllerFunc, IApiDescriptionGroupCollectionProvider apiDescriptionProvider)
    {
        var controllerAction = Expression.Lambda<Action<TController>>(controllerFunc.Body, controllerFunc.Parameters);
        return GetRoute(controllerAction, apiDescriptionProvider);
    }

    public static string GetRoute<TController>(Expression<Action<TController>> controllerAction, IApiDescriptionGroupCollectionProvider apiDescriptionProvider)
    {
        if (controllerAction.Body is not MethodCallExpression controllerActionCall)
            throw new InvalidOperationException($"No route for {controllerAction} found.");

        var apiMethodDescription = apiDescriptionProvider.ApiDescriptionGroups.Items
            .SelectMany(x => x.Items)
            .FirstOrDefault(x =>
                ImplementedByController<TController>(x) &&
                HasMethodName(x, controllerActionCall.Method.Name) &&
                MethodParametersMatch(x, controllerActionCall)
            );

        if (apiMethodDescription == null)
            throw new InvalidOperationException($"No route for {controllerAction} found.");

        var route = GetRelativePath(apiMethodDescription, controllerActionCall);
        return route;
    }

    private static bool ImplementedByController<TController>(this ApiDescription apiDescription)
        => (apiDescription.ActionDescriptor as ControllerActionDescriptor)?.ControllerTypeInfo == typeof(TController);

    private static bool HasMethodName(this ApiDescription apiDescription, string methodName)
        => (apiDescription.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.Name == methodName;

    private static bool MethodParametersMatch(this ApiDescription apiDescription, MethodCallExpression controllerActionCall)
    {
        var apiMethodParameterTypes = (apiDescription.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.GetParameters().Select(parameter => parameter.ParameterType).ToList();
        if (apiMethodParameterTypes == null)
            return false;

        var controllerActionParameterTypes = controllerActionCall.Arguments.Select(parameter => parameter.Type).ToList();

        var result = apiMethodParameterTypes
            .Zip(controllerActionParameterTypes)
            .All(x => x.First.IsAssignableFrom(x.Second));

        return result;
    }

    private static string GetRelativePath(this ApiDescription apiMethodDescription, MethodCallExpression controllerActionCall)
    {
        var route = apiMethodDescription.RelativePath;
        if (route == null)
            throw new InvalidOperationException($"Path to action {controllerActionCall.Method.Name} could not be found.");

        var controllerActionCallParameters = controllerActionCall.Method.GetParameters().ToList();
        foreach (var parameter in apiMethodDescription.ParameterDescriptions)
        {
            var parameterIndex = controllerActionCallParameters.FindIndex(x => x.Name == parameter.Name);
            if (parameterIndex == -1)
                continue;

            var parameterExpression = controllerActionCall.Arguments[parameterIndex];
            try
            {
                var parameterValue = Expression.Lambda(parameterExpression).Compile().DynamicInvoke();
                var parameterValueString = parameterValue?.ToString();
                if (parameter.Source == BindingSource.Query && parameterValueString != null)
                    route = QueryHelpers.AddQueryString(route, parameter.Name, parameterValueString);
                else if (parameter.Source == BindingSource.Path)
                    route = Regex.Replace(route, $"{{{parameter.Name}(\\W.*?)?}}", parameterValueString ?? string.Empty);
            }
            catch (Exception)
            {
                throw new NotSupportedException($"Unable to evaluate parameter {parameter.Name} of {controllerActionCall}.");
            }
        }

        return route;
    }
}