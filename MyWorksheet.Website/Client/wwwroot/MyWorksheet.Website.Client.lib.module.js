var allResourcesBeingLoaded = [];

export function beforeWebAssemblyStart(options, extensions) {
	var progressbar = document.getElementById('progressbar');
	var progressbarContent = document.getElementById('current-progress');

	progressbarContent.textContent = "...";
	var pgbMax = 0;
	var pgbValue = 0;

	options.loadBootResource = function (type, name, defaultUri, integrity) {
		if (type == "dotnetjs")
			return defaultUri;
		return LoadWithProgress(type, name, defaultUri, integrity,
			(nameVal) => {
				//addToUiList(name);
			},
			(maxVal) => {
				pgbMax += maxVal;
				progressbar.setAttribute("max", pgbMax);

				var percent = Math.round(pgbValue / pgbMax * 100);
				UpdateWaiterText(`${percent} %`);
			},
			(valVal) => {
				pgbValue += valVal;
				progressbar.setAttribute("value", pgbValue);

				var percent = Math.round(pgbValue / pgbMax * 100);
				UpdateWaiterText(`${percent} %`);
			});
	}
}

async function LoadWithProgress(type, name, defaultUri, integrity,
	setContent,
	addToMax,
	addToValue) {
	var val = {
		url: defaultUri,
		progress: 0
	};
	allResourcesBeingLoaded.push(val);

	let response = await fetch(val.url,
		{
			cache: 'no-cache',
			method: "HEAD"
		});
	const contentEncoding = response.headers.get('content-encoding');
	const contentLength = response.headers.get(contentEncoding ? 'x-file-size' : 'content-length');
	if (contentLength === null) {
		throw Error('Response size header unavailable');
	}
	response = await fetch(defaultUri,
		{
			cache: 'no-cache',
			//integrity: integrity,
			method: "GET"
		});
	
	setContent(name);
	return new Response(new ReadableStream({
		async start(controller) {
			const reader = response.body.getReader();
			if (mode === "xor") {
				key = key.split(' ').map(function (h) { return parseInt(h, 16) });
				iv = iv.split(' ').map(function (h) { return parseInt(h, 16) });
				var position = 1;
				while (true) {
					const { done, value } = await reader.read();
					if (done) break;
					addToValue(value.byteLength);
					for (var i = 0; i < value.length; i++) {
						value[i] = (value[i] ^ key[position++ % key.length]);
					}
					controller.enqueue(value);
				}
			} else {
				while (true) {
					const { done, value } = await reader.read();
					if (done) break;
					addToValue(value.byteLength);
					controller.enqueue(value);
				}
			}
			controller.close();
		}
	}));
}

function UpdateWaiterText(text) {
	var progressbarContent = document.getElementById('current-progress');
	progressbarContent.textContent = text;
}

window.UpdateWaiterText = UpdateWaiterText;

export function afterWebAssemblyStarted(blazor) {
}