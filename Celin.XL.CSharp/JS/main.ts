import { DotNet } from "@microsoft/dotnet-js-interop";
import { TestStringComplete } from "./utils"

Office.onReady(async (info) => {
    console.log(info);
    // Workaround for https://github.com/OfficeDev/office-js/issues/429
    delete history.pushState;
    delete history.replaceState;
});

var blazorLib: DotNet.DotNetObject;

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
    }
}

export const xl = {
    setRange: async (sheet: string, address: string, values: any[][]) =>
        await Excel.run(async (ctx) => {
            const sh = sheet == null
                ? ctx.workbook.worksheets.getActiveWorksheet()
                : ctx.workbook.worksheets.getItem(sheet);
            const range = sh.getRange(address);
            range.values = values;
            return ctx.sync();
        }),
    getRange: async (sheet: string, address: string) =>
        Excel.run(async (ctx) => {
            const sh = sheet == null
                ? ctx.workbook.worksheets.getActiveWorksheet()
                : ctx.workbook.worksheets.getItem(sheet);
            const range = sh.getRange(address);
            range.load();
            await ctx.sync();
            return range.values;
        }),
}