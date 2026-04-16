using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Shared.Data.Filtering;

public partial class FilterRowComponent
{
    public FilterRowComponent()
    {
        FilterColumns = new List<FilterColumn>();
    }

    [Parameter]
    public RenderFragment Columns { get; set; }

    public IList<FilterColumn> FilterColumns { get; set; }

}

public class FilterColumn : ComponentBase
{
    public FilterColumn()
    {
        FieldType = FieldTypes.Text;
    }

    [CascadingParameter]
    public FilterRowComponent FilterRowComponent { get; set; }

    public FieldTypes FieldType { get; set; }
    public FilterOperator FilterOperator { get; set; }

    public string FieldName { get; set; }
}

public enum FilterOperator
{
    Contains,
    StartsWith,
    EndsWith,
    Equals
}