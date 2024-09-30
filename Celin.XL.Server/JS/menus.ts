import { getItem, setItem } from "./persist";
import { tableMenuStore, stateStore } from "./stores";

export const MENU_KEY = "menus";

export async function onMenuChanged(eventArgs: Excel.TableChangedEventArgs | null) {
  if (!eventArgs || eventArgs.details.valueTypeAfter === "Empty") return;
  await Excel.run(async (context) => {
    const table = context.workbook.tables.getItem(eventArgs.tableId);
    table.load(["name"]);
    const body = table.getDataBodyRange();
    body.load(["rowIndex", "columnIndex", "columnCount"]);
    const range = eventArgs.getRange(context);
    range.load();
    await context.sync();
    const index = range.rowIndex - body.rowIndex;
    const col = range.columnIndex - body.columnIndex;
    if (col === 0 && index !== -1) {
      range.clear("Contents");
      const row = range.getResizedRange(0, body.columnCount - 1);
      row.load("values");
      await context.sync();
      const ev = {
        id: table.name,
        index,
        option: eventArgs.details.valueAfter,
        row: row.values[0],
      };
      tableMenuStore.set(ev);
    }
  });
}

export async function initMenus() {
  onMenuChanged(null);
  const menus = (await getItem<string[]>(MENU_KEY)) ?? [];
  let valid: string[] = [];
  for (let i = 0; i < menus.length; i++) {
    const exist = await Excel.run(async (context) => {
      const wb = context.workbook;
      const table = wb.tables.getItemOrNullObject(menus[i]);
      await context.sync();
      if (!table.isNullObject) {
        table.onChanged.add(onMenuChanged);
        await context.sync();
        return true;
      }
      return false;
    });
    if (exist) valid = [...valid, menus[i]];
  }
  await setItem(MENU_KEY, valid);
}

export async function createMenu() {
  try {
    await Excel.run(async (context) => {
      // Create table
      const sheet = context.workbook.worksheets.getActiveWorksheet();
      const cell = context.workbook.getActiveCell();
      const range = cell.getResizedRange(0, 1);
      const table = sheet.tables.add(range, false);

      const header = table.getHeaderRowRange();
      header.values = [["-", "Menu"]];
      table.showBandedColumns = false;
      table.showBandedRows = false;
      table.showFilterButton = false;
      // table.showHeaders = false;
      table.showTotals = false;
      table.style = "TableStyleDark6";
      const fmt = table.columns.getItemAt(0).getDataBodyRange().format;
      fmt.horizontalAlignment = "Center";
      table.onChanged.add(onMenuChanged);
      await context.sync();
      const menus = (await getItem<string[]>(MENU_KEY)) ?? [];
      await setItem(MENU_KEY, [...menus, table.id]);
    });
  } catch (ex: any) {
    stateStore.error("Failed to create Menu", ex, null);
  }
}
