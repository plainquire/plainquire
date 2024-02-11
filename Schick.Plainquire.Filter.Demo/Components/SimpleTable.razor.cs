#pragma warning disable 1591
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Schick.Plainquire.Demo.Components;

public class FreelancerTableComponent<TItem> : ComponentBase
{
    [Parameter] public required RenderFragment HeaderTemplate { get; set; }
    [Parameter] public required RenderFragment<TItem> RowTemplate { get; set; }
    [Parameter] public required IReadOnlyList<TItem> Items { get; set; }
}