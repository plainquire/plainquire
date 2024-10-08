using Plainquire.Filter.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Page.Tests.Models;

[ExcludeFromCodeCoverage]
[EntityFilter(Prefix = "", PageSize = 10)]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class TestAddress;