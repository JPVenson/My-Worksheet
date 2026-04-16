using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class ServerErrorsComponent : ComponentViewBase
{
    private ServerErrorManager _serverErrorManager;

    public ServerErrorsComponent()
    {
    }

    private void ServerErrors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Render();
    }

    public IEnumerable<ServerError> ServerErrors
    {
        get
        {
            return ServerErrorManager?.ServerErrors ?? Enumerable.Empty<ServerError>();
        }
    }

    [Parameter]
    public ServerErrorManager ServerErrorManager
    {
        get { return _serverErrorManager; }
        set
        {

            SetProperty(ref _serverErrorManager, value, ServerErrorManagerChanged);
        }
    }

    private void ValueOnServerErrorsChanged(object sender, EventArgs e)
    {
        StateHasChanged();
    }

    [Parameter]
    public EventCallback<ServerErrorManager> ServerErrorManagerChanged { get; set; }

    protected override void OnInitialized()
    {
        if (ServerErrorManager != null)
        {
            ServerErrorManager.ServerErrors.CollectionChanged -= ServerErrors_CollectionChanged;
            ServerErrorManager.ServerErrorsChanged -= ValueOnServerErrorsChanged;
        }
        base.OnInitialized();
        ServerErrorManager.ServerErrors.CollectionChanged += ServerErrors_CollectionChanged;
        ServerErrorManager.ServerErrorsChanged += ValueOnServerErrorsChanged;
    }
}

public class ServerErrorManager
{
    private readonly WaiterService _waiterService;

    public ServerErrorManager(WaiterService waiterService)
    {
        _waiterService = waiterService;
        ServerErrors = new ObservableCollection<ServerError>();
    }

    public ObservableCollection<ServerError> ServerErrors { get; set; }
    public event EventHandler ServerErrorsChanged;

    public TApiResult EvalAndUnbox<TApiResult>(ApiResult<TApiResult> result)
    {
        return Eval(result).Object;
    }

    public Task<TApiResult> EvalAndUnbox<TApiResult>(Task<ApiResult<TApiResult>> result)
    {
        return result.ContinueWith(t => EvalAndUnbox(t.Result));
    }

    public Task<TApi> Eval<TApi>(Task<TApi> result) where TApi : IApiResult
    {
        return result.ContinueWith(t => Eval(t.Result));
    }

    public TApi Eval<TApi>(TApi result) where TApi : IApiResult
    {
        if (!result.Success)
        {
            if (result.ErrorResult is ServerProvidedTranslation servTrans)
            {
                ServerErrors.Add(new ServerError()
                {
                    ServerErrorText = servTrans
                });
                return result;
            }
            if (result.ErrorResult is IEnumerable<ServerProvidedTranslation> listOfServTrans)
            {
                foreach (var serverProvidedTranslation in listOfServTrans)
                {
                    ServerErrors.Add(new ServerError()
                    {
                        ServerErrorText = serverProvidedTranslation
                    });
                }
                return result;
            }

            ServerErrors.Add(new ServerError()
            {
                ServerErrorText = new ServerProvidedTranslation()
                {
                    Key = result.StatusMessage
                }
            });
        }

        return result;
    }

    public void SendChanged()
    {
        OnServerErrorsChanged();
    }

    protected virtual void OnServerErrorsChanged()
    {
        ServerErrorsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void DisplayStatus(LocalizableString okMessage = null)
    {
        if (ServerErrors.Any())
        {
            _waiterService.DisplayError();
        }
        else
        {
            _waiterService.DisplayOk(okMessage);
        }
        SendChanged();
    }

    public void Clear()
    {
        ServerErrors.Clear();
    }
}

public class ServerError
{
    public ServerProvidedTranslation ServerErrorText { get; set; }
}