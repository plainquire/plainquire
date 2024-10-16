using Bogus;
using Bogus.DataSets;
using ExpressionTreeToString;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plainquire.Demo.Database;
using Plainquire.Demo.DTOs;
using Plainquire.Demo.Models;
using Plainquire.Demo.Models.FilterSets;
using Plainquire.Demo.Routing;
using Plainquire.Filter;
using Plainquire.Page;
using Plainquire.Sort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Plainquire.Filter.Abstractions;
using Address = Plainquire.Demo.Models.Address;

namespace Plainquire.Demo.Controllers;

/// <summary>
/// Controller to list and generate freelancers.
/// Implements <see cref="Controller"/>
/// </summary>
/// <seealso cref="Controller"/>
[V1ApiController]
public class FreelancerController : Controller
{
    private readonly FreelancerDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="FreelancerController"/> class.
    /// </summary>
    /// <param name="dbContext">The freelancer store.</param>
    public FreelancerController(FreelancerDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureCreated();
    }

    /// <summary>
    /// Gets filtered freelancers.
    /// </summary>
    /// <param name="filter">The freelancer/project filter set.</param>
    /// <param name="orderBy">The freelancer/address sort order set.</param>
    /// <param name="page">The freelancer pagination set.</param>
    /// <param name="seed">A seed. Using seed returns predictable result.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    [HttpGet]
    public async Task<FreelancerDto> GetFreelancers([FromQuery] FreelancerFilterSet filter, [FromQuery] FreelancerSortSet orderBy, [FromQuery] EntityPage<Freelancer> page, int seed = 0, CancellationToken cancellationToken = default)
    {
        var freelancerFilter = filter.Freelancer;
        var projectFilter = filter.Project;
        var addressFilter = filter.Address;

        freelancerFilter
            .ReplaceNested(x => x.Projects, projectFilter)
            .ReplaceNested(x => x.Address, addressFilter)
            .Replace(x => x.Seed, seed);

        var freelancerOrderBy = orderBy.Freelancer;
        freelancerOrderBy
            .AddNested(freelancer => freelancer.Address, orderBy.Address);

        var unfilteredCount = await _dbContext.Set<Freelancer>().CountAsync(x => x.Seed == seed, cancellationToken);
        if (unfilteredCount == 0)
        {
            await GenerateFreelancers(seed: seed, cancellationToken: cancellationToken);
            unfilteredCount = await _dbContext.Set<Freelancer>().CountAsync(x => x.Seed == seed, cancellationToken);
        }

        var filteredCount = await _dbContext.Set<Freelancer>().CountAsync(freelancerFilter, cancellationToken);

        var query = _dbContext
            .Set<Freelancer>()
            .OrderBy(freelancerOrderBy)
            .Include(x => x.Projects)
            .Where(freelancerFilter)
            .Page(page);

        var data = await query.ToListAsync(cancellationToken);

        var freelancers = new FreelancerDto
        {
            Data = data,
            FilteredCount = filteredCount,
            UnfilteredCount = unfilteredCount,
            SqlQuery = GetSqlQuery(query),
            FilterExpression = freelancerFilter.CreateFilter()?.ToString("C#"),
            HttpQuery = WebUtility.UrlDecode(HttpContext.Request.Path.Value + HttpContext.Request.QueryString.Value)
        };

        return freelancers;
    }

    /// <summary>
    /// Generates freelancers.
    /// </summary>
    /// <param name="amount">The amount of records to generate.</param>
    /// <param name="locale">The locale to use. Available locales can be found here: https://github.com/bchavez/Bogus#locales</param>
    /// <param name="seed">A seed. Using seed returns predictable data.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    [HttpPut]
    public Task<ICollection<Freelancer>> GenerateFreelancers(int amount = 12, string locale = "en_US", int seed = 0, CancellationToken cancellationToken = default)
    {
        var freelancers = RecreateFreelancers(amount, locale, seed);
        return Task.FromResult<ICollection<Freelancer>>(freelancers);
    }

    private List<Freelancer> RecreateFreelancers(int amount, string locale, int seed)
    {
        var freelancers = GenerateFreelancersInternal(amount, locale, seed);
        _dbContext.Set<Freelancer>().Where(x => x.Seed == seed).ExecuteDelete();
        _dbContext.AddRange(freelancers);
        _dbContext.SaveChanges();
        return freelancers;
    }

    private static List<Freelancer> GenerateFreelancersInternal(int amount, string locale, int seed)
    {
        const int minAge = 15;
        const int maxAge = 80;
        const int minHourlyRate = 20;
        const int maxHourlyRate = 350;
        var today = DateTime.UtcNow.Date;
        amount = Math.Max(0, Math.Min(amount, 100));
        locale = locale.Replace('-', '_');

        var address = new Faker<Address>(locale)
            .StrictMode(true)
            .RuleFor(x => x.Street, faker => faker.Address.StreetName())
            .RuleFor(x => x.City, faker => faker.Address.City());

        var projects = new Faker<Project>()
            .StrictMode(true)
            .RuleFor(x => x.Id, faker => faker.Random.Uuid())
            .RuleFor(x => x.Title, faker => faker.PickRandom(ProjectCodeNames.CodeNames))
            .RuleFor(x => x.Description, (_, project) => ProjectCodeNames.GetDescription(project.Title))
            .RuleFor(x => x.FreelancerId, _ => default)
            .UseSeed(seed)
            .GenerateForever();

        return new Faker<Freelancer>(locale)
            .StrictMode(true)
            .RuleFor(x => x.Id, faker => faker.Random.Uuid())
            .RuleFor(x => x.Seed, _ => seed)
            .RuleFor(x => x.Gender, faker => faker.PickRandom<Gender>().OrNull(faker, .1f))
            .RuleFor(x => x.FirstName, (faker, freelancer) => freelancer.Gender switch
            {
                Gender.Male => faker.Name.FirstName(Name.Gender.Male),
                Gender.Female => faker.Name.FirstName(Name.Gender.Female),
                _ => faker.Name.FirstName()
            })
            .RuleFor(x => x.LastName, faker => faker.Name.LastName())
            .RuleFor(x => x.Birthday, faker => faker.Date.Between(today.AddYears(-maxAge), today.AddYears(-minAge)).Date.OrNull(faker, .25f))
            .RuleFor(x => x.HourlyRate, faker => Math.Round(faker.Random.Double(minHourlyRate, maxHourlyRate), 2))
            .RuleFor(x => x.Address, _ => address.Generate())
            .RuleFor(x => x.Projects, faker => projects.Take(faker.Random.Int(0, 5)).ToList())
            .FinishWith((_, freelancer) => { foreach (var project in freelancer.Projects) project.FreelancerId = freelancer.Id; })
            .UseSeed(seed)
            .Generate(amount)
            .ToList();
    }

    private static string GetSqlQuery(IQueryable query)
    {
        var queryString = query.ToQueryString();

        var select = Regex.Match(queryString, "SELECT.*", RegexOptions.Singleline, RegexDefaults.Timeout).Value;

        var parameterMatches = Regex.Matches(queryString, @"^\s*.param set (?<paramName>@__p_\d+)\s+(?<paramValue>.*)\s*$", RegexOptions.Multiline, RegexDefaults.Timeout);
        var parameters = parameterMatches.Select(x => new { Name = x.Groups["paramName"].Value.Trim(), Value = x.Groups["paramValue"].Value.Trim() }).ToDictionary(x => x.Name, x => x.Value, StringComparer.Ordinal);

        foreach (var parameter in parameters)
            select = select.Replace(parameter.Key, parameter.Value);

        return select;
    }
}