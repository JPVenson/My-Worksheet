using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.AspNetCore.Components.Authorization;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Collapse;

public delegate void StateDelegateFull(bool fromValue, bool toValue, string key, Action transitionCompleted);
public delegate void StateDelegate(bool fromValue, bool toValue, string key);

[SingletonService()]
public class CollapseService
{
    public CollapseService()
    {
        States = new Dictionary<string, StateHandler>();
    }

    private class StateHandler
    {
        public StateHandler()
        {
            Actions = new List<Delegate>();
        }

        public bool IsTransitioning { get; set; }
        public bool State { get; set; }
        public IList<Delegate> Actions { get; set; }
    }

    IDictionary<string, StateHandler> States { get; set; }

    public void Collapse(string key)
    {
        SetState(key, false);
    }

    public void Show(string key)
    {
        SetState(key, true);
    }

    public IDisposable WhenChanged(string key, StateDelegate handler)
    {
        return WhenChanged(key, handler as Delegate);
    }

    public IDisposable WhenChanged(string key, StateDelegateFull handler)
    {
        return WhenChanged(key, handler as Delegate);
    }

    private IDisposable WhenChanged(string key, Delegate handler)
    {
        if (!States.TryGetValue(key, out var stateHandler))
        {
            stateHandler = States[key] = new StateHandler();
        }

        stateHandler.Actions.Add(handler);
        return new Disposable(() =>
        {
            stateHandler.Actions.Remove(handler);
        });
    }

    private void OnStateChanged(bool toValue, string key)
    {
        if (key.Contains(".") && toValue)
        {
            var group = key.Split('.')[0];
            var states = States
                .Where(f => f.Key.StartsWith(group) && f.Key != key && f.Value.State)
                .ToDictionary(e => e.Key, e => false);
            foreach (var keyValuePair in states)
            {
                StateChangeExecute(false, keyValuePair.Key, () =>
                {
                });
            }
        }
        StateChangeExecute(toValue, key, () => { });
    }

    private void StateChangeExecute(bool toValue, string key, Action done)
    {
        var stateHandler = States[key];
        var oldValue = stateHandler.State;
        stateHandler.State = toValue;
        stateHandler.IsTransitioning = true;
        var transitions = stateHandler.Actions.ToDictionary(e => e, e => false);

        void CheckState()
        {
            stateHandler.IsTransitioning = transitions.Any(e => e.Value);
            if (!stateHandler.IsTransitioning)
            {
                done();
            }
        }

        foreach (var stateDelegate in stateHandler.Actions)
        {
            transitions[stateDelegate] = true;
            if (stateDelegate is StateDelegate normState)
            {
                normState(oldValue, toValue, key);
                transitions[stateDelegate] = false;
            }
            else if (stateDelegate is StateDelegateFull fullState)
            {
                fullState(oldValue, toValue, key, () =>
                {
                    transitions[stateDelegate] = false;
                    CheckState();
                });
            }
        }

        CheckState();
    }

    public bool GetState(string key)
    {
        return States.GetOrDefault(key, null)?.State ?? false;
    }

    public bool SetState(string key, bool value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key), "You have to set a Key for collapsed");
        }

        if (!States.TryGetValue(key, out var currentValue) || currentValue.State == value)
        {
            return false;
        }

        lock (currentValue)
        {
            if (currentValue.IsTransitioning)
            {
                return false;
            }

            OnStateChanged(value, key);
        }
        return true;
    }
}