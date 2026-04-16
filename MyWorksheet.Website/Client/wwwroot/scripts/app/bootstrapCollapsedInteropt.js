function SetCollapsedAction(id, state) {
    var collapseElementList = [].slice.call(document.querySelectorAll(`[collapse-key="${id}"]`));
    collapseElementList.map(function(collapseEl) {
        return new bootstrap.Collapse(collapseEl);
    }).forEach(function(item) {
        if (state) {
            item.hide();
        } else {
            item.show();
        }
    });
}

MyWorksheet.Blazor.SetCollapsedAction = SetCollapsedAction;