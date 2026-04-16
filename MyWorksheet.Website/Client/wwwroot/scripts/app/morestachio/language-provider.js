/*!-----------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * monaco-languages version: 1.10.0(1b4729c63bdb0d1e06d4e637e5c3977ddeb714dd)
 * Released under the MIT license
 * https://github.com/Microsoft/monaco-languages/blob/master/LICENSE.md
 *-----------------------------------------------------------------------------*/
define("vs/basic-languages/morestachio/morestachio", ["require", "exports"],
	(function (e, t) {
		"use strict";
		t.conf = {
		};

		var blockKeywords = [
			"if",
			"else",
			"ifelse",
			"declare",
			"each",
			"foreach",
			"do",
			"while",
			"repeat",
			"switch",
			"case",
			"default",
			"scope",
		];

		t.language = {

			//defaultToken: '',
			tokenPostfix: '.Morestachio',
			ignoreCase: true,

			brackets: [
				["{{", "}}", "expression"],
				["(", ")", "formatter"]
			],

			whitespaceControl: [
				["--|", "-|", "preWhitespaceTag"],
				["|--", "|-", "postWhitespaceTag"]
			],

			blockKeywords: blockKeywords.map(e => "#" + e),

			keywords: [
				"^if",
				"^scope",
				"#var",
				"#isolate",
				"#let",
				"#nl",
				"#tnl",
				"#tnls",
				"#import",
				"/",
				"AS",
				"|-",
				"|--",
				"-|",
				"--|",
				"this",
				"$",
				"null",
			].concat(blockKeywords.map(e => "#" + e)).concat(blockKeywords.map(e => "/" + e)),

			reservedKeywords: [
				"$first",
				"$last",
				"$middel",
				"$index",
				"$odd",
				"$even",
				"$name",
			],

			escapes: /\\(?:[abfnrtv\\"']|x[0-9A-Fa-f]{1,4}|u[0-9A-Fa-f]{4}|U[0-9A-Fa-f]{8})/,

			autoClosingPairs: [
				{ open: "{", close: "}" },
				{ open: "(", close: ")" },
				{ open: '"', close: '"' },
				{ open: "'", close: "'" }
			],
			surroundingPairs: [
				{ open: '"', close: '"' },
				{ open: "'", close: "'" }
			],

			// The main tokenizer for our languages
			tokenizer: {
				root: [
					[/{{!}}/, { token: "comment", next: "@comment" }],
					[/{{!\?}}/, { token: "escaped", next: "@escaped" }],
					[/{{!.+}}/, { token: "comment" }],
					[/{{/, { token: "@brackets.expression", next: "morestachioRoot" }],
					[/}}/, { token: "@brackets.expression" }],
				],
				morestachioRoot: [
					[/![^\}]*/, ["comment"]],
					[/-\|/, { token: "@whitespaceControl.preWhitespaceTag", next: "handlebarsRoot" }],
					[/--\|/, { token: "@whitespaceControl.preWhitespaceTag", next: "handlebarsRoot" }],
					{ include: "handlebarsRoot" }
				],
				handlebarsRoot: [
					[
						/[#/][^\s}}]+/, {
							cases: {
								// "@blockKeywords": {
								//     token: "@rematch",
								//     next: "blockToken"
								// },
								"@keywords": {
									token: "@rematch",
									next: "path"
								},
								"@default": "identifier",
							}
						}
					],
					[/[^}]/, { token: "@rematch", next: "path" }],
					{ include: "endOfExpression" }
				],
				// blockToken: [
				//     [/[#/][^\s}}]+/, {token: "blockkeyword", next: "@push"}]
				// ],
				path: [
					[/}}/, { token: "@brackets.expression", bracket: '@close', next: "@popall" }],
					[/\.\?/, { token: "keyword" }],
					[/~\./, { token: "keyword" }],
					[/\.\.\//, { token: "keyword" }],
					[/(\d+(?:\.\d*)*)/, { token: "keyword.number" }],
					[/[.,]+|\s+this(?:[.]*|[(])/, { token: "keyword" }],
					[/(null)\s*([}),\s])+?/, [{ token: "keyword" }, { token: "@rematch" }]],
					[/&/, { token: "expression.unescaped" }],
					[/\$[^.\s}]+/, { token: "keyword.reserved" }],
					[/(\w+)(\()/, ["formatter.name", "@brackets.formatter"]],
					[/\(/, { token: "@brackets.formatter", bracket: '@open', next: "@push" }],
					[/\)/, { token: "@brackets.formatter", bracket: '@close', next: "@pop" }],
					[/"/, { token: 'string.quote', bracket: '@open', next: '@doubleQuoteStrings' }],
					[/'/, { token: 'string.quote', bracket: '@open', next: '@singleQuoteStrings' }],
					[/[#/^][^\s}}]+/, { token: "keyword.token" }],
					[/\./, { token: "keyword" }],

					[/\|--/, { token: "@whitespaceControl.postWhitespaceTag", next: "endOfExpression" }],
					[/\|-/, { token: "@whitespaceControl.postWhitespaceTag", next: "endOfExpression" }],
					[/[^()]/, { token: "path" }],
				],
				nearEndOfExpression: [
					[/\|--/, { token: "@whitespaceControl.postWhitespaceTag", next: "endOfExpression" }],
					[/\|-/, { token: "@whitespaceControl.postWhitespaceTag", next: "endOfExpression" }],
					{ include: "endOfExpression" }
				],
				endOfExpression: [
					[/}}/, { token: "@brackets.expression", bracket: '@close', next: "@popall" }],
				],
				doubleQuoteStrings: [
					[/[^\\"]+/, 'string'],
					[/@escapes/, 'string.escape'],
					[/"/, { token: 'string.quote', bracket: '@close', next: '@pop' }]
				],
				singleQuoteStrings: [
					[/[^\\']+/, 'string'],
					[/@escapes/, 'string.escape'],
					[/'/, { token: 'string.quote', bracket: '@close', next: '@pop' }]
				],
				comment: [
					[/{{\/!}}/, 'comment', "@pop"],
					[/./, "comment.content"]
				],
				escaped: [
					[/{{\/!\?}}/, 'escaped', "@pop"],
					[/./, "escaped.content"]
				],
			}
		};


		var keywordColor = "DF0DFF";
		var keywordTokenColor = "185911";
		var bracketColor = "569cd6";
		var commentColor = "999988";
		var unescapedOutputColor = "e2f725";
		var formatterNameColor = "c5c2c6";

		t.theme = {
			base: 'vs-dark',
			name: "morestachioTheme",
			inherit: true,
			rules: [
				{ "token": "keyword.Morestachio", "foreground": keywordColor, "fontStyle": "bold", },
				{ "token": "keyword.token.Morestachio", "foreground": keywordTokenColor, "fontStyle": "bold", },
				{ "token": "keyword.reserved.Morestachio", "foreground": keywordColor, "fontStyle": "italic", },
				{ "token": "escaped.Morestachio", "foreground": keywordColor, "fontStyle": "italic", },
				{ "token": "expression.Morestachio.tag", "foreground": keywordColor, },
				{ "token": "formatter.Morestachio.formatter", "foreground": bracketColor, },
				{ "token": "formatter.name.Morestachio", foreground: formatterNameColor, "fontStyle": "italic", },
				{ "token": "expression.unescaped.Morestachio", "foreground": unescapedOutputColor, "fontStyle": "bold" },
				{ "token": "expression.Morestachio.expression", "foreground": keywordColor, },
				{ "token": "@whitespaceControl.postWhitespaceTag.Morestachio", "foreground": keywordColor, "fontStyle": "bold" },
				{ "token": "@whitespaceControl.preWhitespaceTag.Morestachio", "foreground": keywordColor, "fontStyle": "bold" },

			],
			colors: {
			}
		};
		t.foldingProvider = {
			provideFoldingRanges: function (model, context, token) {
				var value = model.getValue();
				var lines = value.split(/\r?\n/);

				var foldStack = [];
				var folds = [];
				for (var i = 0; i < lines.length; i++) {
					let line = lines[i].toLowerCase();
					var keywordsFound = blockKeywords.find(e => line.search("/" + e) != -1);
					if (keywordsFound != null && foldStack.length > 0) {

						let fold;
						for (var j = foldStack.length; j > 0; j--) {
							if (foldStack[j - 1].key == keywordsFound) {
								fold = foldStack.splice(j - 1, 1)[0];
								break;
							}
						}
						if (fold == null) {
							continue;
						}

						folds.push(
							{
								start: fold.index,
								end: i + 1,
								kind: monaco.languages.FoldingRangeKind.Region
							}
						)
					}

					keywordsFound = blockKeywords.find(e => line.search("#" + e) != -1);
					if (keywordsFound != null) {
						foldStack.push({
							key: keywordsFound,
							index: i + 1
						})
					}
				}
				return folds;
			}
		};
		//monaco.languages.registerHoverProvider('morestachio', {
		t.formatterProvider = {
			provideHover: function (model, position) {
				return null;
				var wordInfo = model.getWordAtPosition(position);
				var word = wordInfo.word;
				var nextChar = model.getValueInRange(new monaco.Range(
					position.lineNumber,
					wordInfo.endColumn,
					position.lineNumber,
					wordInfo.endColumn + 1
				));
				if (nextChar != '(') {
					return null;
				}
				return xhr("/api/MorestachioApi/GetFormatterInfoFormatted?formatterName=" + encodeURIComponent(word))
					.then(function (res) {
						return {
							range: new monaco.Range(
								position.lineNumber,
								wordInfo.startColumn,
								position.lineNumber,
								wordInfo.endColumn
							),
							contents: [
								{ value: '**' + word + '**' },
								{
									value: res.responseText,
									isTrusted: true,
									supportHtml: true,
								}
							]
						};
					});
			}
		};
	}
	));


function xhr(url) {
	var req = null;
	return new Promise(
		function (c, e) {
			req = new XMLHttpRequest();
			req.onreadystatechange = function () {
				if (req._canceled) {
					return;
				}

				if (req.readyState === 4) {
					if ((req.status >= 200 && req.status < 300) || req.status === 1223) {
						c(req);
					} else {
						e(req);
					}
					req.onreadystatechange = function () { };
				}
			};

			req.open('GET', url, true);
			req.responseType = '';

			req.send(null);
		},
		function () {
			req._canceled = true;
			req.abort();
		}
	);
}
