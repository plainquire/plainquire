using Plainquire.Filter.Abstractions;
using System.Text.RegularExpressions;

namespace Plainquire.Page;

/// <summary>
/// Extension methods for parsing page parameter names.
/// </summary>
public static class ParameterExtensions
{
    private const string PAGE_PARAMETER_PATTERN = @"^(?<page>.*?)(\s*,\s*(?<pageSize>.+))?$";

    /// <summary>
    /// Get the page parameter names from the action and binding parameter name.
    /// </summary>
    /// <param name="actionParameterName">Original name of the action parameter</param>
    /// <param name="bindingParameterName">Binding name of the action parameter</param>
    /// <returns></returns>
    public static (string PageNumberParameterName, string PageSizeParameterName) GetPageParameterNames(string actionParameterName, string? bindingParameterName)
    {
        if (string.IsNullOrEmpty(bindingParameterName))
            return (actionParameterName, $"{actionParameterName}Size");

        var values = Regex.Match(bindingParameterName, PAGE_PARAMETER_PATTERN, RegexOptions.ExplicitCapture, RegexDefaults.Timeout);
        var pageNumberBinderName = values.Groups["page"].Value;
        var pageSizeBinderName = values.Groups["pageSize"].Value;

        var pageNumberName = !string.IsNullOrWhiteSpace(pageNumberBinderName) ? pageNumberBinderName : actionParameterName;
        var pageSizeName = !string.IsNullOrWhiteSpace(pageSizeBinderName) ? pageSizeBinderName : $"{pageNumberName}Size";

        return (pageNumberName, pageSizeName);
    }
}