using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class SelectionFieldControl<TValue, TKey>
{
    private TKey _selectedKey;
    private TValue _selectedItem;
    private IEnumerable<TValue> _itemsSource;
    private string _selectedKeyValue;

    [CascadingParameter()]
    public IFieldControl FieldControl { get; set; }
    [CascadingParameter()]
    public EditContext EditContext { get; set; }

    [Parameter]
    public IEnumerable<TValue> ItemsSource
    {
        get { return _itemsSource; }
        set
        {
            if (Equals(_itemsSource, value))
            {
                return;
            }

            _itemsSource = value;
        }
    }

    private void FutureList_ListLoaded()
    {
        StateHasChanged();
        if (GetFromValueMember(SelectedItem)?.Equals(SelectedKey) != true)
        {
            OnSelectedKeyHasChanged(SelectedKey);
        }
    }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; }

    [Parameter]
    public bool AllowNullValue { get; set; }

    private bool _readOnly;

    [Parameter]
    public bool ReadOnly
    {
        get { return _readOnly; }
        set
        {
            SetProperty(ref _readOnly, value, ReadOnlyChanged);
            if (FieldControl != null)
            {
                FieldControl.ReadOnly = value;
            }
        }
    }

    [Parameter]
    public EventCallback<bool> ReadOnlyChanged { get; set; }

    [Parameter]
    public TValue SelectedItem
    {
        get { return _selectedItem; }
        set
        {
            if (SetProperty(ref _selectedItem, value, SelectedItemChanged))
            {
                SelectedItemHasChanged();
            }
        }
    }

    [Parameter]
    public TKey SelectedKey
    {
        get { return _selectedKey; }
        set
        {
            if (SetProperty(ref _selectedKey, value, SelectedKeyChanged))
            {
                OnSelectedKeyHasChanged(value);
            }
        }
    }

    private string SelectedKeyValue
    {
        get { return _selectedKeyValue; }
        set
        {
            if (SetProperty(ref _selectedKeyValue, value, SelectedKeyValueChanged))
            {
                OnSelectedKeyValueHasChanged(value);
            }
        }
    }

    private void SelectedItemHasChanged()
    {
        var value = SelectedItem;
        if (value != null)
        {
            SetProperty(ref _selectedKey, GetFromValueMember(value), SelectedKeyChanged);
            SetProperty(ref _selectedKeyValue, _selectedKey?.ToString(), SelectedKeyValueChanged);
        }
        else
        {
            SetProperty(ref _selectedKey, default, SelectedKeyChanged);
            SetProperty(ref _selectedKeyValue, null, SelectedKeyValueChanged);
        }
        OnSelectedItemChanged.Raise(_selectedItem);
    }

    private void OnSelectedKeyHasChanged(TKey value)
    {
        if (value != null)
        {
            var itemFromKey = ItemsSource.FirstOrDefault(e =>
            {
                var fromValueMember = GetFromValueMember(e);
                return value.Equals(fromValueMember);
            });
            SetProperty(ref _selectedItem, itemFromKey, SelectedItemChanged);
            SetProperty(ref _selectedKeyValue, GetFromValueMember(_selectedItem)?.ToString(), SelectedKeyValueChanged);
        }
        else
        {
            SetProperty(ref _selectedItem, default, SelectedItemChanged);
            SetProperty(ref _selectedKeyValue, null, SelectedKeyValueChanged);
        }
        OnSelectedItemChanged.Raise(_selectedItem);
    }

    private void OnSelectedKeyValueHasChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var itemFromKey = ItemsSource.FirstOrDefault(e => Equals(value, GetFromValueMember(e).ToString()));
            SetProperty(ref _selectedItem, itemFromKey, SelectedItemChanged);
            SetProperty(ref _selectedKey, GetFromValueMember(itemFromKey), SelectedKeyChanged);
        }
        else
        {
            SetProperty(ref _selectedKey, default, SelectedKeyChanged);
            SetProperty(ref _selectedItem, default, SelectedItemChanged);
        }

        OnSelectedItemChanged.Raise(_selectedItem);
    }


    [Parameter]
    public EventCallback<TValue> SelectedItemChanged { get; set; }
    [Parameter]
    public EventCallback<TValue> OnSelectedItemChanged { get; set; }
    [Parameter]
    public EventCallback<TKey> SelectedKeyChanged { get; set; }
    [Parameter]
    public EventCallback<string> SelectedKeyValueChanged { get; set; }

    [Parameter]
    public Func<TValue, TKey> ValueMemberExpression { get; set; }

    [Parameter]
    public Func<TValue, object> DisplayMemberExpression { get; set; }

    [Parameter]
    public RenderFragment<TValue> DisplayMemberTemplate { get; set; }

    private TKey GetFromValueMember(TValue option)
    {
        if (option == null)
        {
            return default;
        }

        if (ValueMemberExpression != null)
        {
            var valueMemberExpression = ValueMemberExpression(option);
            return valueMemberExpression;
        }

        if (typeof(TValue) == typeof(TKey))
        {
            return (TKey)(object)(option);
        }
        return ((TKey)(object)option.GetId());
    }

    private RenderFragment GetFromDisplayMember(TValue option)
    {
        if (DisplayMemberExpression != null)
        {
            var displayMemberExpression = DisplayMemberExpression(option);
            if (displayMemberExpression is LocalizableString locString)
            {
                return AsLoc(locString);
            }

            return __renderer =>
            {
                __renderer.AddContent(0, displayMemberExpression?.ToString());
            };
        }

        if (DisplayMemberTemplate != null)
        {
            return DisplayMemberTemplate(option);
        }

        return __renderer =>
        {
            __renderer.AddContent(0, option?.ToString());
        };
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (ItemsSource is FutureList<TValue> futureList)
        {
            futureList.WhenLoadedOnce(FutureList_ListLoaded);
        }
        else
        {
            if (GetFromValueMember(SelectedItem)?.Equals(SelectedKey) != true)
            {
                OnSelectedKeyHasChanged(SelectedKey);
            }
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ValueMemberExpression = ValueMemberExpression ?? (value => (TKey)((object)(value)));
    }
}