#pragma warning disable 1591
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Plainquire.Demo.DTOs;
using Plainquire.Filter.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Plainquire.Demo.Pages;

public class DemoPage : ComponentBase
{
    [Inject] private HttpClient HttpClient { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private readonly Random _randomizer = new();

    protected FreelancerQueryModel QueryModel = new();

    protected FreelancerDto QueryResult = new();

    protected int PageCount => GetPageCount();

    protected readonly NumberFormatInfo NumberFormat;

    public DemoPage()
    {
        NumberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
        NumberFormat.CurrencySymbol = new CultureInfo("en-US").NumberFormat.CurrencySymbol;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        SetQueryModelFromUrl(NavigationManager.Uri);
        await LoadData();
    }

    protected async Task LoadPredefined(string identifier)
    {
        QueryModel.Clear();

        if (identifier == "Older-than-40")
            QueryModel.Filter.Birthday = "<forty-years-ago";
        else if (identifier == "Unknown-Age")
            QueryModel.Filter.Birthday = "ISNULL";
        else if (identifier == "Known-Age")
            QueryModel.Filter.Birthday = "NOTNULL";
        else if (identifier == "Gender-Contains")
            QueryModel.Filter.Gender = "~male";
        else if (identifier == "Address-Street")
            QueryModel.Filter.AddressStreet = "A";
        else if (identifier == "Project-Contains")
            QueryModel.Filter.ProjectTitle = "Z";

        await UpdateQuery();
    }

    protected async Task UpdateQuery()
    {
        SetUrlFromQueryModel();
        await LoadData();
    }

    protected async Task ClearQuery()
    {
        QueryModel.Clear();
        await UpdateQuery();
    }

    protected async Task ChangeSeed()
    {
        QueryModel.Seed = _randomizer.Next().ToString();
        await UpdateQuery();
    }

    protected async Task SetPage(int page)
    {
        QueryModel.PageNumber = Math.Max(page, 1);
        await UpdateQuery();
    }

    protected async Task SetPageSize(int pageSize)
    {
        QueryModel.PageSize = pageSize;
        await UpdateQuery();
    }

    protected async Task FormatSql()
        => QueryResult.SqlQuery = await FormatSql(QueryResult.SqlQuery!);

    private int GetPageCount()
    {
        if (QueryResult.FilteredCount == 0)
            return 1;

        var pageFraction = QueryResult.FilteredCount / (float)QueryModel.PageSize;
        return (int)Math.Ceiling(pageFraction);
    }

    private void SetQueryModelFromUrl(string url)
    {
        var uri = new Uri(url);
        var queryParameters = QueryHelpers.ParseQuery(uri.Query);
        QueryModel = FreelancerQueryModel.FromQuery(queryParameters);
        if (!queryParameters.ContainsKey("Seed"))
            QueryModel.Seed = _randomizer.Next().ToString();
    }

    private void SetUrlFromQueryModel()
    {
        var currentUrl = new Uri(NavigationManager.Uri);
        var newUrl = $"{currentUrl.Scheme}://{currentUrl.Authority}{currentUrl.LocalPath}";
        newUrl = QueryHelpers.AddQueryString(newUrl, QueryModel.ToQuery());
        NavigationManager.NavigateTo(newUrl);
    }

    private async Task LoadData()
    {
        try
        {
            var requestUri = $"{NavigationManager.BaseUri}api/Freelancer/GetFreelancers";
            requestUri = QueryHelpers.AddQueryString(requestUri, QueryModel.ToQuery());

            using var response = await HttpClient.GetAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                QueryResult = new FreelancerDto { ErrorMessage = message };
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            QueryResult = JsonConvert.DeserializeObject<FreelancerDto>(json) ?? new();

            if (QueryModel.PageNumber > 1 && QueryModel.PageNumber > PageCount)
            {
                await SetPage(PageCount);
                await LoadData();
            }
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<string?> FormatSql(string sql)
    {
        var requestUri = "https://sqlformat.org/api/v1/format";
        requestUri = QueryHelpers.AddQueryString(requestUri, "sql", sql);
        // ReSharper disable once StringLiteralTypo
        requestUri = QueryHelpers.AddQueryString(requestUri, "reindent", "1");

        using var response = await HttpClient.GetAsync(requestUri);
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return $"Error formatting sql: {json}";

        return JsonConvert.DeserializeObject<SqlFormatResult>(json)?.Result;
    }

    protected class FreelancerQueryModel
    {
        public FreelancerFilter Filter { get; private set; } = new();

        public string[] Sort { get; private set; } = ["", "", "", ""];

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Seed { get; set; }

        public static FreelancerQueryModel FromQuery(Dictionary<string, StringValues> queryParameters)
        {
            var result = new FreelancerQueryModel();
            foreach (var (key, value) in queryParameters)
                result.SetFilterByName(key, value);

            if (queryParameters.TryGetValue("orderBy", out var orderBy))
            {
                var sort = orderBy
                    .SelectMany(value => value?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [])
                    .Take(4)
                    .ToArray();

                Array.Copy(sort, result.Sort, sort.Length);
            }

            if (queryParameters.TryGetValue("page", out var page))
                result.PageNumber = int.Parse(page!);

            if (queryParameters.TryGetValue("pageSize", out var pageSize))
                result.PageSize = int.Parse(pageSize!);

            if (queryParameters.TryGetValue("seed", out var seed))
                result.Seed = seed;

            return result;
        }

        public Dictionary<string, StringValues> ToQuery()
        {
            var query = Filter
                .GetType()
                .GetProperties()
                .Select(x => new { x.Name, Value = (string?)x.GetMethod?.Invoke(Filter, null) })
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .ToDictionary(x => x.Name.LowercaseFirstChar(), x => new StringValues(x.Value));

            var sort = Sort.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (sort.Count != 0)
                query.Add("orderBy", string.Join(',', sort));

            if (PageNumber != 1)
                query.Add("page", new StringValues(PageNumber.ToString()));

            if (PageSize != 10)
                query.Add("pageSize", new StringValues(PageSize.ToString()));

            if (!string.IsNullOrEmpty(Seed))
                query.Add("seed", new StringValues(Seed));

            return query;
        }

        private void SetFilterByName(string propertyName, string? value)
            => Filter.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance)?.SetMethod?.Invoke(Filter, [value]);

        public void Clear()
        {
            Filter = new FreelancerFilter();
            Sort = ["", "", "", ""];
        }

        public class FreelancerFilter
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Gender { get; set; }
            public string? Birthday { get; set; }
            public string? HourlyRate { get; set; }
            public string? AddressStreet { get; set; }
            public string? ProjectTitle { get; set; }
        }
    }

    private class SqlFormatResult
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string? Result { get; set; }
    }
}