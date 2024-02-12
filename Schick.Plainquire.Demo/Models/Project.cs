using Schick.Plainquire.Filter.Abstractions.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Demo.Models;

/// <summary>
/// Project.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Required by EF")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by reflection")]
public class Project
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Project title
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Description of the project.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Identifier of the owning <see cref="Freelancer"/>.
    /// </summary>
    [Filter(Filterable = false)]
    public Guid FreelancerId { get; set; }
}