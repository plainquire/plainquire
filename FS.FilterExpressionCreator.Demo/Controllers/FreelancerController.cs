using Bogus;
using Bogus.DataSets;
using Bogus.Extensions;
using FS.FilterExpressionCreator.Demo.Database;
using FS.FilterExpressionCreator.Demo.DTOs;
using FS.FilterExpressionCreator.Demo.Models;
using FS.FilterExpressionCreator.Demo.Routing;
using FS.FilterExpressionCreator.Models;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FS.FilterExpressionCreator.Demo.Controllers
{
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
        /// <param name="freelancerFilter">The freelancer filter.</param>
        /// <param name="projectFilter">The project filter.</param>
        /// <param name="seed">A seed. Using the same seed returns predictable result.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        [HttpGet]
        public Task<FreelancerDto> GetFreelancers([FromQuery] EntityFilter<Freelancer> freelancerFilter, [FromQuery] EntityFilter<Project> projectFilter, int seed = 0, CancellationToken cancellationToken = default)
        {
            var dataForSeedAvailable = _dbContext.Set<Freelancer>().Any(x => x.Seed == seed);
            if (!dataForSeedAvailable)
                RecreateFreelancers(seed: seed);

            freelancerFilter ??= new EntityFilter<Freelancer>();
            freelancerFilter
                .Replace(x => x.Projects, projectFilter)
                .Replace(x => x.Seed, seed);

            var filter = freelancerFilter.CreateFilterPredicate() ?? (x => true);
            var query = _dbContext.Set<Freelancer>().Include(x => x.Projects).Where(filter);
            var freelancers = new FreelancerDto
            {
                Data = query.ToList(),
                SqlQuery = query.ToQueryString(),
                FilterExpression = filter.ToString(),
                HttpQuery = WebUtility.UrlDecode(HttpContext.Request.Path.Value + HttpContext.Request.QueryString.Value)
            };

            return Task.FromResult(freelancers);
        }

        /// <summary>
        /// Generates freelancers.
        /// </summary>
        /// <param name="amount">The amount of records to generate.</param>
        /// <param name="locale">The locale to use. Available locales can be found here: https://github.com/bchavez/Bogus#locales</param>
        /// <param name="seed">A seed. Using the same seed returns predictable data.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        [HttpPut]
        public Task<List<Freelancer>> GenerateFreelancers(int amount = 10, string locale = "en_US", int seed = 0, CancellationToken cancellationToken = default)
        {
            var freelancers = RecreateFreelancers(amount, locale, seed);
            return Task.FromResult(freelancers);
        }

        private List<Freelancer> RecreateFreelancers(int amount = 10, string locale = "en_US", int seed = 0)
        {
            var freelancers = GenerateFreelancersInternal(amount, locale, seed);
            _dbContext.Set<Freelancer>().Where(x => x.Seed == seed).Delete();
            _dbContext.AddRange(freelancers);
            _dbContext.SaveChanges();
            return freelancers;
        }

        private static List<Freelancer> GenerateFreelancersInternal(int amount = 10, string locale = "en_US", int seed = 0)
        {
            const int minAge = 15;
            const int maxAge = 80;
            const int minHourlyRate = 20;
            const int maxHourlyRate = 350;
            var today = DateTime.UtcNow.Date;
            amount = Math.Max(0, Math.Min(amount, 100));
            locale = locale.Replace('-', '_');

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
                .RuleFor(x => x.YearsOfExperience, (faker, freelancer) => faker.Random.Int(0, (today.Year - freelancer.Birthday?.Year ?? maxAge) - minAge))
                .RuleFor(x => x.Projects, faker => projects.Take(faker.Random.Int(0, 5)).ToList())
                .FinishWith((_, freelancer) => freelancer.Projects.ForEach(x => x.FreelancerId = freelancer.Id))
                .UseSeed(seed)
                .Generate(amount)
                .ToList();
        }
    }
}
