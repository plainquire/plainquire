using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Schick.Plainquire.Demo.Extensions;
using Schick.Plainquire.Demo.Routing;

namespace Schick.Plainquire.Demo.Controllers;

/// <summary>
/// Controller to retrieve common application information.
/// Implements <see cref="Controller"/>
/// </summary>
/// <seealso cref="Controller"/>
[V1ApiController]
public class InformationController : Controller
{
    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    [HttpGet]
    public Task<string?> GetProductName(CancellationToken cancellationToken = default)
        => Task.FromResult(AssemblyExtensions.GetProgramProduct());

    /// <summary>
    /// Gets the product version.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    [HttpGet]
    public Task<string?> GetProductVersion(CancellationToken cancellationToken = default)
        => Task.FromResult(AssemblyExtensions.GetProgramProductVersion());

    /// <summary>
    /// Gets the product copyright.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    [HttpGet]
    public Task<string?> GetProductCopyright(CancellationToken cancellationToken = default)
        => Task.FromResult(AssemblyExtensions.GetProgramCopyright());
}