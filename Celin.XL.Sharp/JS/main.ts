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
    Object.keys(source).forEach(key => {
        if (source[key] !== null) {
            target[key] = source[key];
        }
    });
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
    syncSheetFrom: async (key: string) => {
        let result = await Excel.run(async (ctx) => {
            const sh = name == null
            ? ctx.workbook.worksheets.getActiveWorksheet()
            : ctx.workbook.worksheets.getItem(key);
            sh.load();
            await ctx.sync();
            console.log(`SheetFrom:${JSON.stringify(sh)}`);
            return sh;
        });
        return result;
    },
    syncSheetTo: async (key: string, values: Excel.Worksheet) => {
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
    setRange: async (sheet: string, address: string, values: any) => {
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
    },
}
