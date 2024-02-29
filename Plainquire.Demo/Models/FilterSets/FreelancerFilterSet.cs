using Plainquire.Filter;
using Plainquire.Filter.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Demo.Models.FilterSets;

/// <summary>
/// Common filter set for <see cref="Freelancer"/> and <see cref="Project"/>
/// </summary>
[EntityFilterSet]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Required by ModelBinders")]
public class FreelancerFilterSet
{
    /// <summary>
    /// Gets or sets the filter for freelancers.
    /// </summary>
    public EntityFilter<Freelancer> Freelancer { get; set; } = new();

    /// <summary>
    /// Gets or sets the filter for projects.
    /// </summary>
    public EntityFilter<Project> Project { get; set; } = new();

    /// <summary>
    /// Gets or sets the filter for addresses.
    /// </summary>
    public EntityFilter<Address> Address { get; set; } = new();
}