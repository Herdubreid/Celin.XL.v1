import * as localforage from "localforage";

/* global */

global.blazorLib = undefined;

export const isExcel = new Promise<boolean>(async (resolve) => {
  await Office.onReady((info) => {
    const xl = info.host === Office.HostType.Excel;
    resolve(xl);
    /*
    if (xl) {
      listSettings();
      await initMenus();
    }*/
  });
});

export function listSettings() {
  return Excel.run(async (context) => {
    const settings = context.workbook.settings;
    settings.load();
    await context.sync();
    console.log(settings.items);
  });
}

export async function getItem<Type>(key: string): Promise<Type | null> {
  return (await isExcel)
    ? Excel.run<Type>(async (context) => {
      const item = context.workbook.settings.getItem(key);
      try {
        item.load("value");
        await context.sync();
      } catch {
        return null;
      }
      return item.value;
    })
    : localforage.getItem<Type>(key);
}

export async function setItem(key: string, value: any) {
  return (await isExcel)
    ? Excel.run(async (context) => {
      context.workbook.settings.add(key, value);
      await context.sync();
    })
    : localforage.setItem(key, value);
}

export async function removeItem(key: string) {
  return (await isExcel)
    ? Excel.run(async (context) => {
      const item = context.workbook.settings.getItem(key);
      item.delete();
      await context.sync();
    })
    : localforage.removeItem(key);
}
