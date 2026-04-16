using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Shared.Data.Display.Views;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MyWorksheet.Website.Client.Shared.Data.Display;

public class DataDisplay<T> : ComponentViewBase
{
    public DataDisplay()
    {
        DataProvider = new ValueProvider<T>();
        ColumnManager = new ColumnManager<T>();
    }

    public ValueProvider<T> DataProvider { get; }

    [Parameter]
    public Func<FilterOptions, Task<IEnumerable<T>>> AsyncItemsProvider
    {
        get { return DataProvider.AsyncItemsProvider; }
        set { DataProvider.AsyncItemsProvider = value; }
    }

    [Parameter]
    public Func<FilterOptions, IEnumerable<T>> ItemsProvider
    {
        get { return DataProvider.ItemsProvider; }
        set { DataProvider.ItemsProvider = value; }
    }

    [Parameter]
    public IEnumerable<T> ItemsSource
    {
        get { return DataProvider.ItemsSource; }
        set { DataProvider.ItemsSource = value; }
    }

    public ColumnManager<T> ColumnManager { get; set; }
    [Parameter]
    public RenderFragment Columns { get; set; }

    [Parameter]
    public RenderFragment<object> LabelDataTemplate { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<ColumnManager<T>>>(0);
        builder.AddAttribute(1, nameof(CascadingValue<T>.Value), ColumnManager);
        builder.AddAttribute(2, nameof(CascadingValue<T>.IsFixed), true);
        builder.AddAttribute(3, nameof(CascadingValue<T>.ChildContent), Columns);
        builder.CloseComponent();

        builder.OpenComponent<DataDisplayTableView<T>>(4);
        builder.AddAttribute(5, nameof(DataDisplayTableView<T>.DataDisplay), this);
        builder.CloseComponent();
        base.BuildRenderTree(builder);
    }

    public FilterOptions GetFilter()
    {
        return new FilterOptions();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (DataProvider.NeedsReload)
        {
            DataProvider.NeedsReload = false;
            DataProvider.BoundValues = await DataProvider.GetValues(GetFilter());
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}

public class ColumnManager<T>
{
    public ColumnManager()
    {
        Columns = new List<GridColumnBase<T>>();
    }

    public IList<GridColumnBase<T>> Columns { get; set; }
}

public class GridColumnBase<T> : ComponentBase
{
    [CascadingParameter()]
    public ColumnManager<T> ColumnManager { get; set; }

    [Parameter]
    public object Label { get; set; }

    [Parameter]
    public RenderFragment HeaderContent { get; set; }

    [Parameter]
    public RenderFragment<T> CellTemplate { get; set; }

    [Parameter]
    [Required]
    public Expression<Func<T, object>> ValueExpression { get; set; }

    [Parameter]
    public Func<T, object> ValueFactory { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        ColumnManager.Columns.Add(this);
    }

    public RenderFragment GetTH(DataDisplay<T> grid)
    {
        if (HeaderContent != null)
        {
            return HeaderContent;
        }

        if (grid.LabelDataTemplate != null)
        {
            return builder =>
            {
                builder.AddContent(0, grid.LabelDataTemplate, Label);
            };
        }

        var property = GetProperty(ValueExpression);
        var display = property.GetCustomAttribute<DisplayAttribute>();
        if (display != null && Label == null)
        {
            return builder =>
            {
                builder.OpenComponent<Translatable>(0);
                builder.AddAttribute(1, nameof(Translatable.Key), display.Name);
                builder.CloseComponent();
            };
        }

        return builder => builder.AddContent(0, Label);
    }

    public RenderFragment<T> GetTD()
    {
        if (CellTemplate == null && ValueExpression != null)
        {
            if (ValueFactory != null)
            {
                return t => builder =>
                {
                    if (t == null)
                    {
                        return;
                    }

                    var val = ValueFactory?.Invoke(t);
                    builder.OpenElement(0, "span");
                    builder.AddContent(1, val);
                    builder.CloseElement();
                };
            }
            else if (ValueExpression != null)
            {
                return t => builder =>
                {
                    if (t == null)
                    {
                        return;
                    }


                    var valueExpression = ValueExpression;
                    var compile = valueExpression?.Compile();
                    var val = compile?.Invoke(t);
                    builder.OpenElement(0, "span");
                    builder.AddContent(1, val);
                    builder.CloseElement();
                };
            }
        }
        return CellTemplate;
    }

    /// <summary>
    ///     Helper for getting the property info of an expression
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="property">The property.</param>
    /// <returns></returns>
    public static PropertyInfo GetProperty<TProperty, TObject>(Expression<Func<TObject, TProperty>> property)
    {
        var lambda = (LambdaExpression)property;

        MemberExpression memberExpression;
        var body = lambda.Body as UnaryExpression;

        if (body != null)
        {
            var unaryExpression = body;
            memberExpression = (MemberExpression)unaryExpression.Operand;
        }
        else
        {
            memberExpression = (MemberExpression)lambda.Body;
        }

        return memberExpression.Member as PropertyInfo;
    }
}