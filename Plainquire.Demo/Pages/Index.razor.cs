#pragma warning disable 1591
using Microsoft.AspNetCore.Components;

namespace Plainquire.Demo.Pages;

public class IndexPage : ComponentBase
{
    [CascadingParameter] protected string? Theme { get; set; }
}