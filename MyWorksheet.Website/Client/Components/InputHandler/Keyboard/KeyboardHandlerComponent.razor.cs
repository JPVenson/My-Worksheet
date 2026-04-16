using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MyWorksheet.Website.Client.Components.InputHandler.Keyboard;

public partial class KeyboardHandlerComponent
{
    public KeyboardHandlerComponent()
    {
        ReferenceId = Guid.NewGuid().ToString("N");
    }

    [Inject]
    public IJSRuntime JsRuntime { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; }

    public IList<KeyInputHandler> Handler { get; set; }

    public string ReferenceId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JsRuntime.InvokeVoidAsync("MyWorksheet.Blazor.Window.KeyInputCallback", DotNetObjectReference.Create(this), ReferenceId);
        await base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable("JsIOOnKeyDown")]
    public async Task<JsKeyboardEvent> JsIOOnKeyDown(JsKeyboardEvent eventArgs)
    {
        Console.WriteLine("Down: " + eventArgs.Key);
        return eventArgs;
    }

    [JSInvokable("JsIOOnKeyUp")]
    public async Task<JsKeyboardEvent> JsIOOnKeyUp(JsKeyboardEvent eventArgs)
    {
        Console.WriteLine("Up: " + eventArgs.Key);
        return eventArgs;
    }

    public class JsKeyboardEvent
    {
        [JsonPropertyName("charCode")]
        public int CharCode { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("repeat")]
        public bool Repeat { get; set; }
        [JsonPropertyName("altKey")]
        public bool AltKey { get; set; }
        [JsonPropertyName("ctrlKey")]
        public bool ControlKey { get; set; }
        [JsonPropertyName("shiftKey")]
        public bool ShiftKey { get; set; }
        [JsonPropertyName("handled")]
        public bool Handled { get; set; }
    }
}

public class KeyInputHandler
{
    public KeyInputHandler()
    {

    }

    public KeyboardEventArgs KeyboardEventArgs { get; set; }
}