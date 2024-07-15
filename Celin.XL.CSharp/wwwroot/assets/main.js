(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
    typeof define === 'function' && define.amd ? define(['exports'], factory) :
    (global = typeof globalThis !== 'undefined' ? globalThis : global || self, factory(global.lib = {}));
})(this, (function (exports) { 'use strict';

    //
    function TestStringComplete(str) {
        // Test Quotes
        let qoutes = (str.match(/"/g) || []).length;
        if (qoutes % 2 != 0)
            return false;
        // Test unmatched brackets
        var depth = 0;
        // for each char in the string : 2 cases
        for (let i = 0; i < str.length; i++) {
            if (str[i] == '[') {
                // if the char is an opening parenthesis then we increase the depth
                depth++;
            }
            else if (str[i] == ']') {
                // if the char is an closing parenthesis then we decrease the depth
                depth--;
            }
            //  if the depth is negative we have a closing parenthesis 
            //  before any matching opening parenthesis
            if (depth < 0)
                return false;
        }
        // If the depth is not null then a closing parenthesis is missing
        if (depth > 0)
            return false;
        // OK !
        return true;
    }

    Office.onReady(async (info) => {
        console.log(info);
        // Workaround for https://github.com/OfficeDev/office-js/issues/429
        delete history.pushState;
        delete history.replaceState;
    });
    var blazorLib;
    const app = {
        init: (lib) => {
            blazorLib = lib;
        },
        initCommandPrompt: (id) => {
            let txt = document.getElementById(id);
            txt.addEventListener('keydown', function (event) {
                if (event.key === 'Enter') {
                    if (TestStringComplete(txt.value)) {
                        if (txt.value.trim()) {
                            blazorLib.invokeMethodAsync('PromptCommand', txt.value);
                        }
                        txt.style.height = '19px';
                        txt.value = '';
                        event.preventDefault();
                    }
                }
            });
        }
    };

    exports.app = app;

}));
