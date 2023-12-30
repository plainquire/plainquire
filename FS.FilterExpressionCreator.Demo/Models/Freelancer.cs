using FS.FilterExpressionCreator.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Demo.Models;

/// <summary>
/// Freelancer.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
[FilterEntity(Prefix = "")]
public class Freelancer
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The seed this freelancer belongs too.
    /// </summary>
    [Filter(Filterable = false, Sortable = false)]
    public int Seed { get; set; }

    /// <summary>
    /// First name.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Last name.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Gender.
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    /// Birthday.
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// Hourly rate.
    /// </summary>
    public double HourlyRate { get; set; }

    /// <summary>
    /// Years of Experience.
    /// </summary>
    public int YearsOfExperience { get; set; }

    /// <summary>
    /// Address.
    /// </summary>
    public Address Address { get; set; } = new();

    /// <summary>
    /// The projects done.
    /// </summary>
    public List<Project> Projects { get; set; } = [];
}