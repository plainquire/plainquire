using Plainquire.Page.Abstractions;
using System;

namespace Plainquire.Page;

/// <summary>
/// Extension methods for <see cref="EntityPage"/>
/// </summary>
public static class EntityPageExtensions
{
    /// <summary>
    /// Get the skip and take values for the given <paramref name="page"/> and <paramref name="configuration"/>.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static (int? Skip, int? Take) GetSkipAndTake(this EntityPage page, PageConfiguration? configuration)
    {
        configuration ??= EntityPage.DefaultConfiguration;

        var pageNumber = ParsePageNumber(page, configuration);
        var pageSize = ParsePageSize(page, configuration);

        var skip = (pageNumber - 1) * pageSize;

        return (skip, pageSize);
    }

    private static int? ParsePageNumber(EntityPage page, PageConfiguration configuration)
    {
        var pageNumberNotSet = string.IsNullOrEmpty(page.PageNumberValue);
        if (pageNumberNotSet)
            return null;

        var parseFailed = !int.TryParse(page.PageNumberValue, out var pageNumber);
        if (parseFailed && !configuration.IgnoreParseExceptions)
            throw new FormatException($"Error while parsing page number '{page.PageNumberValue}'");

        if (pageNumber >= 1)
            return pageNumber;

        if (configuration.IgnoreParseExceptions)
            return 1;

        throw new ArgumentOutOfRangeException(nameof(EntityPage.PageNumber), page.PageNumber, $"{nameof(EntityPage.PageNumber)} must be a positive integer.");
    }

    private static int? ParsePageSize(EntityPage page, PageConfiguration configuration)
    {
        var pageSizeNotSet = string.IsNullOrEmpty(page.PageSizeValue);
        if (pageSizeNotSet && !configuration.IgnoreParseExceptions)
            throw new ArgumentException("Page size not set", nameof(EntityPage.PageSize));

        var parseFailed = !int.TryParse(page.PageSizeValue, out var pageSize);
        if (parseFailed && !configuration.IgnoreParseExceptions)
            throw new FormatException($"Error while parsing page size '{page.PageSizeValue}'");

        if (pageSize >= 1)
            return pageSize;

        if (configuration.IgnoreParseExceptions)
            return int.MaxValue;

        throw new ArgumentOutOfRangeException(nameof(EntityPage.PageSize), page.PageSize, $"{nameof(EntityPage.PageSize)} must be a positive integer.");
    }
}