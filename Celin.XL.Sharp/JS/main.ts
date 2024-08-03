import { DotNet } from "@microsoft/dotnet-js-interop";
import { TestStringComplete } from "./utils"

Office.onReady(async (info) => {
    console.log(info);
    // Workaround for https://github.com/OfficeDev/office-js/issues/429
    delete history.pushState;
    delete history.replaceState;
});

var blazorLib: DotNet.DotNetObject;

function assignNonNullProperties(source: any, target: any) {
    if (Object.values(target).some(value => value !== null)) {
        Object.keys(source).forEach(key => {
            if (source[key] !== null) {
                try {
                    target[key] = source[key];
                } catch { }
            }
        });
        return true;
    }
    return false;
}

function parseRangeAddress(address: string) {
    let m = address.match(/(?:'?([^']+)'?!)?(.+)/);
    let sheet: string = m ? m[1] : null;
    let cells: string = m ? m[2] : null;
    return { sheet, cells };
}

export const app = {
    init: (lib: DotNet.DotNetObject) => {
        blazorLib = lib;
    },
    initCommandPrompt: (id: string) => {
        let txt = document.getElementById(id) as HTMLInputElement;
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
    },
}

export const xl = {
    syncValues: async (key: string, values: any) => {
        let a = parseRangeAddress(key);
        console.log(`Address:${a.sheet},${a.cells},${values}`);
        let result = await Excel.run(async (ctx) => {
            const sh = a.sheet == null
                ? ctx.workbook.worksheets.getActiveWorksheet()
                : ctx.workbook.worksheets.getItem(a.sheet);
            const range = sh.getRange(a.cells);
            range.values = values;
            await ctx.sync();
            range.load("values")
            await ctx.sync();
            return range.values;
        });
        return JSON.stringify(result);
    },
    syncRange: async (key: string, values: Excel.Range) => {
        let a = parseRangeAddress(key);
        console.log(`Address:${a.sheet},${a.cells}`);
        let result = await Excel.run(async (ctx) => {
            const sh = a.sheet == null
                ? ctx.workbook.worksheets.getActiveWorksheet()
                : ctx.workbook.worksheets.getItem(a.sheet);
            const range = sh.getRange(a.cells);
            if (assignNonNullProperties(values, range)) {
                await ctx.sync();
            }
            range.load();
            await ctx.sync();
            return range;
        });
        return JSON.stringify(result);
    },
    syncSheet: async (key: string, values: Excel.Worksheet) => {
        let result = await Excel.run(async (ctx) => {
            const sh = key == null
                ? ctx.workbook.worksheets.getActiveWorksheet()
                : ctx.workbook.worksheets.getItem(key);
            if (assignNonNullProperties(values, sh)) {
                await ctx.sync();
            }
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
}
