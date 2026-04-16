using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Shared.ViewModels;

namespace MyWorksheet.Website.Client.Shared.Layout;

public class LayoutController : ViewModelBase
{
    public LayoutController()
    {
        _modifiers = new List<Action<LayoutData>>();
        Layout = new LayoutData();
    }

    public LayoutData Layout { get; private set; }

    private List<Action<LayoutData>> _modifiers;

    public IDisposable Modifier(Action<LayoutData> layoutAction)
    {
        _modifiers.Add(layoutAction);
        Reevaluate();
        return new Disposable(() =>
        {
            _modifiers.Remove(layoutAction);
            Reevaluate();
        });
    }

    private void Reevaluate()
    {
        Layout = new LayoutData();
        foreach (var modifier in _modifiers)
        {
            modifier(Layout);
        }
    }
}

public class LayoutData
{
    public bool FullHeightContent { get; set; }
}