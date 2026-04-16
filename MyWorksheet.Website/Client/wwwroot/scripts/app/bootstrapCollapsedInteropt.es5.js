'use strict';

function SetCollapsedAction(id, state) {
    var collapseElementList = [].slice.call(document.querySelectorAll('#' + id));
    collapseElementList.map(function (collapseEl) {
        return new bootstrap.Collapse(collapseEl);
    }).forEach(function (item) {
        if (state) {
            item.show();
        } else {
            item.hide();
        }
    });
}

MyWorksheet.Blazor.SetCollapsedAction = SetCollapsedAction;

