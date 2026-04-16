using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Util.View;

public static class EventCallbackExtensions
{
    public static async void Raise(this EventCallback callback)
    {
        if (callback.HasDelegate)
        {
            await callback.InvokeAsync();
        }
    }
    public static async void Raise(this EventCallback callback, object value)
    {
        if (callback.HasDelegate)
        {
            await callback.InvokeAsync(value);
        }
    }

    public static async void Raise<T>(this EventCallback<T> callback)
    {
        if (callback.HasDelegate)
        {
            await callback.InvokeAsync();
        }
    }
    public static async void Raise<T>(this EventCallback<T> callback, T value)
    {
        if (callback.HasDelegate)
        {
            await callback.InvokeAsync(value);
        }
    }

    public static async Task RaiseAsync(this EventCallback callback)
    {
        if (callback.HasDelegate)
        {
            await callback.InvokeAsync();
        }
    }
    public static async Task RaiseAsync(this EventCallback callback, object value)
    {
        if (callback.HasDelegate)
        {
            await callback.InvokeAsync(value);
        }
    }

    public static async Task RaiseAsync<T>(this EventCallback<T> callback)
    {
        if (callback.HasDelegate)
        {
            await callback.InvokeAsync();
        }
    }
    public static async Task RaiseAsync<T>(this EventCallback<T> callback, T value)
    {
        if (callback.HasDelegate)
        {
            await callback.InvokeAsync(value);
        }
    }
}