using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.SortQueryableCreator.Sorts;

namespace FS.FilterExpressionCreator.Demo.Models.FilterSets;

/// <summary>
/// Common sort order set for <see cref="Freelancer"/> and <see cref="Address"/>
/// </summary>
[EntitySortSet]
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