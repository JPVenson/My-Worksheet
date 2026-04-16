function setTitle(title) {
    document.title = "My Worksheet - " + title;
}

function getCsKeyEventFromJsEvent(jsEventArgs) {
    return {
        altKey: jsEventArgs.altKey,
        charCode: jsEventArgs.charCode,
        code: jsEventArgs.code,
        ctrlKey: jsEventArgs.ctrlKey,
        key: jsEventArgs.key,
        repeat: jsEventArgs.repeat,
        shiftKey: jsEventArgs.shiftKey,
        handled: false
    };
}

function keyInputCallback(keyboardHandlerComponent, containerRefId) {

    var containerRef = document.getElementById(containerRefId);

    containerRef.onKeyDown = function(jsEventArgs) {
        var csEventArgs = getCsKeyEventFromJsEvent(jsEventArgs);
        csEventArgs = keyboardHandlerComponent.invokeMethodAsync("JsIOOnKeyDown", csEventArgs);
        if (csEventArgs.handled) {
            jsEventArgs.stopPropagation();
        }
    }

    containerRef.onkeyup = function(jsEventArgs) {
        var csEventArgs = getCsKeyEventFromJsEvent(jsEventArgs);
        csEventArgs = keyboardHandlerComponent.invokeMethodAsync("JsIOOnKeyUp", csEventArgs);
        if (csEventArgs.handled) {
            jsEventArgs.stopPropagation();
        }
    }
}

MyWorksheet.Blazor.Window.SetTitle = setTitle;
MyWorksheet.Blazor.Window.KeyInputCallback = keyInputCallback;