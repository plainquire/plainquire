using FS.FilterExpressionCreator.Abstractions.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Demo.Models;

/// <summary>
/// Project.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public class Project
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Project title
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Identifier of the owning <see cref="Freelancer"/>.
    /// </summary>
    [Filter(Filterable = false)]
    public Guid FreelancerId { get; set; }
}