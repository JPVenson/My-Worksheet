window.GridStackInterop = (() => {
    let grid = null;

    return {
        init(dotNetRef, cols) {
            // Destroy any previous instance (e.g. after hot-reload)
            if (grid) {
                grid.destroy(false);
                grid = null;
            }

            grid = GridStack.init({
                column: cols,
                float: true,
                animate: true,
                cellHeight: 120,
                margin: 8,
                disableResize: true   // resize off by default; enabled when edit mode starts
            });

            grid.on('change', (_, items) => {
                if (!items || !dotNetRef) return;
                const data = items.map(i => ({
                    id: i.el ? i.el.getAttribute('data-id') : null,
                    x: i.x,
                    y: i.y,
                    w: i.w,
                    h: i.h
                })).filter(i => i.id !== null);
                dotNetRef.invokeMethodAsync('OnGridChanged', data);
            });
            grid.enableMove(false);
            grid.enableResize(false);
        },

        setEditMode(enabled) {
            if (!grid) return;
            grid.enableMove(enabled);
            grid.enableResize(enabled);
        },

        addWidget(id, x, y, w, h) {
            // Element was already rendered by Blazor; just register it with GridStack
            const el = document.querySelector(`[data-id="${CSS.escape(id)}"]`);
            if (el && grid) {
                grid.makeWidget(el);
            }
        },

        removeWidget(id) {
            const el = document.querySelector(`[data-id="${CSS.escape(id)}"]`);
            if (el && grid) {
                grid.removeWidget(el, false); // false = keep DOM (Blazor manages it)
            }
        },

        destroy() {
            if (grid) {
                grid.destroy(false);
                grid = null;
            }
        }
    };
})();
