#pragma warning disable 1591
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace FS.FilterExpressionCreator.Demo.Components;

public class FreelancerTableComponent<TItem> : ComponentBase
{
    [Parameter] public required RenderFragment HeaderTemplate { get; set; }
    [Parameter] public required RenderFragment<TItem> RowTemplate { get; set; }
    [Parameter] public required IReadOnlyList<TItem> Items { get; set; }
}