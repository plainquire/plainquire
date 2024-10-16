#pragma warning disable 1591
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Plainquire.Demo.Components;

public partial class SimpleTable<TItem> : ComponentBase
{
    [Parameter] public required RenderFragment HeaderTemplate { get; set; }
    [Parameter] public required RenderFragment<TItem> RowTemplate { get; set; }
    [Parameter] public required ICollection<TItem> Items { get; set; }
}