using System.Diagnostics.CodeAnalysis;
using Plainquire.Filter.Abstractions.Attributes;
using Plainquire.Sort.Sorts;

namespace Plainquire.Demo.Models.FilterSets;

/// <summary>
/// Common sort order set for <see cref="Freelancer"/> and <see cref="Address"/>
/// </summary>
[EntitySortSet]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Required by ModelBinders")]
public class FreelancerSortSet
{
    /// <summary>
    /// Gets or sets the sort order for freelancers.
    /// </summary>
    public EntitySort<Freelancer> Freelancer { get; set; } = new();

    /// <summary>
    /// Gets or sets the sort order for projects.
    /// </summary>
    public EntitySort<Address> Address { get; set; } = new();
}