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

function parseRangeAddress(address: string|null) {
    if (address === null) return {};
    let m = address?.match(/(?:^'?([^']+)'?!)?(.*)$/);
    let sheet: string | null = m ? m[1] : null;
    let cells: string | null = m ? m[2] : null;
    return { sheet, cells };
}

function isNullOrEmpty(value: string | null | undefined): boolean {
    return value === null || value === undefined || value.trim().length === 0;
}

function getProperty<T, K extends keyof T>(obj: T, key: K): T[K] {
    return obj[key];
}

function setProperty<T, K extends keyof T>(obj: T, key: K, value: T[K]): void {
    obj[key] = value;
}

function getRange(ctx: Excel.RequestContext, address: string|null): Excel.Range {
    let a = parseRangeAddress(address);
    const sh = isNullOrEmpty(a.sheet)
    ? ctx.workbook.worksheets.getActiveWorksheet()
    : ctx.workbook.worksheets.getItem(a.sheet!);
    const range = isNullOrEmpty(a.cells)
    ? isNullOrEmpty(a.sheet)
        ? ctx.workbook.getSelectedRange()
        : sh.getUsedRange()
    : sh.getRange(a.cells!);
    return range;
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
    syncFill: async (key: string, values: any) => {
        let result = await Excel.run(async (ctx) => {
            const range = getRange(ctx, key);
            if (assignNonNullProperties(values, range.format.fill)) {
                await ctx.sync();
            }
            range.load("format/fill");
            await ctx.sync();
            return range.format.fill;
        });
        return result;
    },
    syncFormat: async (key: string, values: any) => {
        let result = await Excel.run(async (ctx) => {
            const range = getRange(ctx, key);
            if (assignNonNullProperties(values, range.format)) {
                await ctx.sync();
            }
            range.load("format");
            await ctx.sync();
            return range.format;
        });
        return result;
    },
    syncList: async (key: string, props: string, values: any) => {
        let result = await Excel.run(async (ctx) => {
            const range = getRange(ctx, key);
            setProperty(range, props as keyof Excel.Range, values);
            await ctx.sync();
            range.load(props);
            await ctx.sync();
            return getProperty(range, props as keyof Excel.Range);
        });
        return result;
    },
    syncRange: async (key: string, values: Excel.Range) => {
        let result = await Excel.run(async (ctx) => {
            const range = getRange(ctx, key);
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
            const sh = isNullOrEmpty(key)
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
