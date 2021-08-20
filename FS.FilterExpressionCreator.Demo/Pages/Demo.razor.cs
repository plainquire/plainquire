#pragma warning disable 1591
using FS.FilterExpressionCreator.Demo.DTOs;
using FS.FilterExpressionCreator.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FS.FilterExpressionCreator.Demo.Pages
{
    public class DemoPage : ComponentBase
    {
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        private readonly Random _randomizer = new();

        protected FreelancerQueryModel QueryModel = new();
        protected FreelancerDto QueryResult = new();
        protected NumberFormatInfo NumberFormat;

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
                QueryModel.Birthday = "<forty-years-ago";
            else if (identifier == "Unknown-Age")
                QueryModel.Birthday = "ISNULL";
            else if (identifier == "Known-Age")
                QueryModel.Birthday = "NOTNULL";
            else if (identifier == "Gender-Contains")
                QueryModel.Gender = "~male";
            else if (identifier == "Low-Experience")
                QueryModel.YearsOfExperience = "<2";
            else if (identifier == "Project-Contains")
                QueryModel.ProjectTitle = "Z";

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

        protected async Task FormatSql()
            => QueryResult.SqlQuery = await FormatSql(QueryResult.SqlQuery);

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
            var requestUri = $"{NavigationManager.BaseUri}api/v1/Freelancer/GetFreelancers";
            requestUri = QueryHelpers.AddQueryString(requestUri, QueryModel.ToQuery());
            using var response = await HttpClient.GetAsync(requestUri);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Unable to query database.");

            var json = await response.Content.ReadAsStringAsync();
            QueryResult = JsonConvert.DeserializeObject<FreelancerDto>(json);
        }

        private async Task<string> FormatSql(string sql)
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
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public string Birthday { get; set; }
            public string HourlyRate { get; set; }
            public string YearsOfExperience { get; set; }
            public string ProjectTitle { get; set; }
            public string Seed { get; set; }

            public static FreelancerQueryModel FromQuery(Dictionary<string, StringValues> queryParameters)
            {
                var result = new FreelancerQueryModel();
                foreach (var (key, value) in queryParameters)
                    result.SetByName(key, value);
                return result;
            }

            public Dictionary<string, StringValues> ToQuery()
                => GetType()
                     .GetProperties()
                     .Select(x => new { x.Name, Value = (string)x.GetMethod?.Invoke(this, null) })
                     .Where(x => !string.IsNullOrEmpty(x.Value))
                     .ToDictionary(x => x.Name.LowercaseFirstChar(), x => new StringValues(x.Value));

            public void SetByName(string propertyName, string value)
                => GetType().GetProperty(propertyName)?.SetMethod?.Invoke(this, new object[] { value });

            public void Clear()
            {
                FirstName = null;
                LastName = null;
                Gender = null;
                Birthday = null;
                HourlyRate = null;
                YearsOfExperience = null;
                ProjectTitle = null;
            }
        }

        private class SqlFormatResult
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Result { get; set; }
        }
    }
}
