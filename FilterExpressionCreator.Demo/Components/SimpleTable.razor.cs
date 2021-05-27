#pragma warning disable 1591
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace FilterExpressionCreator.Demo.Components
{
    public class FreelancerTableComponent<TItem> : ComponentBase
    {
        [Parameter] public RenderFragment HeaderTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> RowTemplate { get; set; }
        [Parameter] public IReadOnlyList<TItem> Items { get; set; }
    }
}
