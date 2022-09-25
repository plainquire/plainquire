using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.FilterExpressionCreator.Filters;

namespace FS.FilterExpressionCreator.Demo.Models.FilterSets
{
    /// <summary>
    /// Common filter set for <see cref="Freelancer"/> and <see cref="Project"/>
    /// </summary>
    [EntityFilterSet]
    public class FreelancerFilterSet
    {
        /// <summary>
        /// Gets or sets the filter for freelancers.
        /// </summary>
        public EntityFilter<Freelancer> Freelancer { get; set; }

        /// <summary>
        /// Gets or sets the filter for projects.
        /// </summary>
        public EntityFilter<Project> Project { get; set; }
    }
}
