import { DotNet } from "@microsoft/dotnet-js-interop";
import { openEditorDlg, openLoginDlg, messageDlg, closeDlg } from "./dialog";
import { globalState } from "./common";

Office.onReady(async (info) => {
    // Workaround for https://github.com/OfficeDev/office-js/issues/429
    // @ts-ignore
    delete history.pushState;
    // @ts-ignore
    delete history.replaceState;
});

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
    let m = address?.match(/(?:'?([^']+)'?!)?(.+)/);
    let sheet: string | null = m ? m[1] : null;
    let cells: string | null = m ? m[2] : null;
    console.log(`Sheet: ${sheet}, Cells: ${cells}`);
    return { sheet, cells };
}

function getProperty<T, K extends keyof T>(obj: T, key: K): T[K] {
    return obj[key];
}

function setProperty<T, K extends keyof T>(obj: T, key: K, value: T[K]): void {
    obj[key] = value;
}

export const app = {
    init: (lib: DotNet.DotNetObject) => {
        globalState.blazorLib = lib;
    },
    initCommandPrompt: (id: string) => {
        let txt = document.getElementById(id) as HTMLInputElement;
        txt.addEventListener('keydown', function (event) {
            if (event.key === 'Enter' && event.shiftKey) {
                event.preventDefault();
            }
        });
    },
    openEditorDlg: (key: string, title: string, doc: string) => {
        openEditorDlg(key, title, doc);
    },
    openLoginDlg: (title: string, username: string) => {
        openLoginDlg(title, username);
    },
    messageDlg: (notice: string) => {
        messageDlg(notice);
    },
    closeDlg: () => {
        closeDlg();
    },
}

export const xl = {
    syncList: async (key: string, props: string, values: any) => {
        let a = parseRangeAddress(key);
        let result = await Excel.run(async (ctx) => {
            const sh = a.sheet == null
                ? ctx.workbook.worksheets.getActiveWorksheet()
                : ctx.workbook.worksheets.getItem(a.sheet);
            const range = a.cells == null
                ? sh.getUsedRange()
                : sh.getRange(a.cells);
            setProperty(range, props as keyof Excel.Range, values);
            await ctx.sync();
            range.load(props);
            await ctx.sync();
            return getProperty(range, props as keyof Excel.Range);
        });
        console.log(JSON.stringify(result));
        return result;
    },
    syncRange: async (key: string, values: Excel.Range) => {
        let a = parseRangeAddress(key);
        let result = await Excel.run(async (ctx) => {
            const sh = a.sheet == null
                ? ctx.workbook.worksheets.getActiveWorksheet()
                : ctx.workbook.worksheets.getItem(a.sheet);
            const range = a.cells == null
                ? sh.getUsedRange()
                : sh.getRange(a.cells);
            if (assignNonNullProperties(values, range)) {
                await ctx.sync();
            }
            range.load();
            await ctx.sync();
            return range;
        });
        return result;
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
            return sh;
        });
        return result;
    },
}
