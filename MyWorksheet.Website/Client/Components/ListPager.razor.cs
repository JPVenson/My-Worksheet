using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.TimeTracking;
using MyWorksheet.Website.Client.Pages.TimeTracking.Projects;
using MyWorksheet.Website.Client.Util.View.List;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components;

public partial class ListPager<T> where T : class
{
    public ListPager()
    {
        NoItems = 4;
    }

    public int NoItems { get; set; }

    public int DisplayStartRange { get; set; }
    public int DisplayEndRange { get; set; }

    public void ReevaluateRanges()
    {
        var start = (int)Math.Floor(NoItems / 2D);
        var end = (int)Math.Ceiling(NoItems / 2D);
        if (ItemsSource.Page - start > 1)
        {
            DisplayStartRange = start;
        }
        else
        {
            DisplayStartRange = ItemsSource.Page - 1;
        }

        if (ItemsSource.Page + end > ItemsSource.MaxPages)
        {
            DisplayEndRange = (ItemsSource.MaxPages - ItemsSource.Page);
        }
        else
        {
            DisplayEndRange = end;
        }
    }

    [Parameter]
    public PagedList<T> ItemsSource { get; set; }

    private async Task LoadPage(int i)
    {
        ItemsSource.Page = i;
        await ItemsSource.SearchAsync();
    }
}