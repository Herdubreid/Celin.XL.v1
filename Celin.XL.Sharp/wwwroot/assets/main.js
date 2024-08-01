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
    function assignNonNullProperties(source, target) {
        Object.keys(source).forEach(key => {
            if (source[key] !== null) {
                target[key] = source[key];
            }
        });
    }
    function parseRangeAddress(address) {
        let m = address.match(/('?([^']+)'?!)?(.+)/);
        let sheet = m ? m[1] : null;
        let cells = m ? m[3] : null;
        return { sheet, cells };
    }
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
        },
        dummy: async () => {
            let sheet = null;
            let address = "B2";
            console.log(`${sheet}, ${address}`);
            let result = await Excel.run(async (ctx) => {
                const sh = ctx.workbook.worksheets.getActiveWorksheet()
                    ;
                const range = sh.getRange(address);
                range.load();
                await ctx.sync();
                return range.values;
            });
            let toreturn = JSON.stringify(result);
            console.log(`Result: ${toreturn}`);
            return toreturn;
        },
    };
    const xl = {
        getRange: async (key) => {
            let a = parseRangeAddress(key);
            console.log(`Address:${a.sheet},${a.cells}`);
            let result = await Excel.run(async (ctx) => {
                const sh = a.sheet == null
                    ? ctx.workbook.worksheets.getActiveWorksheet()
                    : ctx.workbook.worksheets.getItem(a.sheet);
                const range = sh.getRange(a.cells);
                range.load();
                await ctx.sync();
                return range;
            });
            return JSON.stringify(result);
        },
        setRange: async (key, values) => {
            let result = await Excel.run(async (ctx) => {
                const sh = ctx.workbook.worksheets.getItem(key);
                assignNonNullProperties(JSON.parse(values), sh);
                await ctx.sync();
                sh.load();
                await ctx.sync();
                console.log(`SheetFrom:${JSON.stringify(sh)}`);
                return sh;
            });
            return JSON.stringify(result);
        },
        getSheet: async (key) => {
            let result = await Excel.run(async (ctx) => {
                const sh = key == null
                    ? ctx.workbook.worksheets.getActiveWorksheet()
                    : ctx.workbook.worksheets.getItem(key);
                sh.load();
                await ctx.sync();
                console.log(`SheetFrom:${JSON.stringify(sh)}`);
                return sh;
            });
            return result;
        },
        setSheet: async (key, values) => {
            let result = await Excel.run(async (ctx) => {
                const sh = ctx.workbook.worksheets.getItem(key);
                assignNonNullProperties(values, sh);
                await ctx.sync();
                sh.load();
                await ctx.sync();
                console.log(`SheetFrom:${JSON.stringify(sh)}`);
                return sh;
            });
            return result;
        },
        /*setRange: async (sheet: string, address: string, values: any) => {
            console.log(`${sheet}, ${address}, ${values}`);
            await Excel.run(async (ctx) => {
                const sh = sheet == null
                    ? ctx.workbook.worksheets.getActiveWorksheet()
                    : ctx.workbook.worksheets.getItem(sheet);
                const range = sh.getRange(address);
                range.values = values;
                await ctx.sync();
            });
        },
        getRange: async (sheet: string, address: string) => {
            console.log(`${sheet}, ${address}`);
            let result = await Excel.run(async (ctx) => {
                const sh = sheet == null
                    ? ctx.workbook.worksheets.getActiveWorksheet()
                    : ctx.workbook.worksheets.getItem(sheet);
                const range = sh.getRange(address);
                range.load();
                await ctx.sync();
                return range.values;
            });
            let toreturn = JSON.stringify(result);
            console.log(`Result: ${toreturn}`);
            return toreturn;
        },*/
    };

    exports.app = app;
    exports.xl = xl;

}));
