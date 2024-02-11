using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Demo.Models;

/// <summary>
/// Gender enum
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Accessed by reflection")]
public enum Gender
{
    /// <summary>
    /// Divers
    /// </summary>
    Divers = 0,

    /// <summary>
    /// Male
    /// </summary>
    Male = 1,

    /// <summary>
    /// Female
    /// </summary>
    Female = 2
}