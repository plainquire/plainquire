using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Demo.Models;

/// <summary>
/// Freelancer.
/// </summary>
[FilterEntity(Prefix = "", PageSize = 10)]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public class Freelancer
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The seed this freelancer belongs too.
    /// </summary>
    [Filter(Filterable = false, Sortable = false)]
    public int Seed { get; init; }

    /// <summary>
    /// First name.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Last name.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Gender.
    /// </summary>
    public Gender? Gender { get; init; }

    /// <summary>
    /// Birthday.
    /// </summary>
    public DateTime? Birthday { get; init; }

    /// <summary>
    /// Hourly rate.
    /// </summary>
    public double HourlyRate { get; init; }

    /// <summary>
    /// Address.
    /// </summary>
    public Address Address { get; init; } = new();

    /// <summary>
    /// The projects done.
    /// </summary>
    public List<Project> Projects { get; init; } = [];
}