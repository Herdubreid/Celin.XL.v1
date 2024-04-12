import { get } from "svelte/store";
import { stateStore, optionsStore, tableMenuStore } from "./stores";
import type { ICql, IDetails } from "./types";
import { getItem, setItem } from "./persist";
import { onMenuChanged, MENU_KEY } from "./menus"; 

const SHEET = /^'?([^']*)'?!(.*)/;
const DATETYPE =
  /^(?:19|20)[0-9][0-9]-(?:0[1-9]|1[0-2])-(?:0[1-9]|[1-2][0-9]|3[0-1])$/;
const ALIASTRIMVIEW = /\S*_/;
const ALIASTRIMAGGR = /\S*\.|_\S*/;

async function getDetails(id: string): Promise<IDetails> {
  const details = await getItem<IDetails>(`cql-${id}`);
  if (details === null) {
    throw "No data available!";
  }
  return details;
}

export async function setValues(values: object) {
  try {
    await Excel.run(async (ctx) => {
      const names = ctx.workbook.names;
      for (const [alias, value] of Object.entries(values)) {
        const name = names.getItemOrNullObject(alias);
        const ob = Object.prototype.toString.call(value) === "[object Object]";
        const ar = Object.prototype.toString.call(value) === "[object Array]";
        await ctx.sync();
        if (!name.isNullObject) {
          const range = name.getRangeOrNullObject();
          range.values = ar ? value : [[ob ? value?.value : value]];
        }
      }
    });
  } catch (ex) {
    stateStore.error("Set Value Failed", ex.message, null);
  }
}

export async function setFormula(name: string, formula: string) {
  try {
    await Excel.run(async (ctx) => {
      const names = ctx.workbook.names;
      const cell = names.getItemOrNullObject(name);
      await ctx.sync();
      if (!cell.isNullObject) {
        const range = cell.getRange();
        range.formulas = [[`=${formula}`]];
        range.numberFormat = [[""]];
        await ctx.sync();
      }
    });
  } catch (ex) {
    stateStore.error("Set Formula Failed", ex.message, null);
  }
}

export async function getValue(name: string) {
  try {
    const value = await Excel.run(async (ctx) => {
      const names = ctx.workbook.names;
      const cell = names.getItemOrNullObject(name);
      await ctx.sync();
      if (!cell.isNullObject) {
        const range = cell.getRange();
        range.load("values");
        await ctx.sync();
        return range.values;
      } else {
        stateStore.error("Get Value Failed", `Name '${name}' not Found!`, null);
      }
      return null;
    });
    return value;
  } catch (ex) {
    stateStore.error("Get Value Failed", ex.message, null);
  }
  return null;
}

export async function deleteTable(header) {
  try {
    if (!header.id) throw "Missing name!";
    await Excel.run(async (context) => {
      // Test if table exists
      const exists = context.workbook.tables.getItemOrNullObject(header.id);
      exists.load();
      await context.sync();
      if (exists.isNullObject) throw `Table "${header.id}" does not exists!`;

      // Delete table
      exists.delete();
      await context.sync();
    });
  } catch (ex) {
    stateStore.error("Failed to delete Table", ex, null);
  }
}

export async function createTable(header: ICql) {
  try {
    if (!header.id) throw "Missing name!";
    const rows = (await getDetails(header.id)).results;
    if (rows.length == 0) throw "No data!";
    await Excel.run(async (context) => {
      // Test if table exists
      const exists = context.workbook.tables.getItemOrNullObject(header.id);
      exists.load();
      await context.sync();
      if (!exists.isNullObject) throw `Table "${header.id}" exists!`;

      // Create table
      const sheet = context.workbook.worksheets.getActiveWorksheet();
      const cell = context.workbook.getActiveCell();
      const columns = rows[0].length + (header.withMenu ? 1 : 0);
      const range = cell.getResizedRange(0, columns - 1);
      const fmt = rows[0].map((c) =>
        typeof c === "string"
          ? c.length === 0 || c[0] === "=" || DATETYPE.test(c)
            ? null
            : "@"
          : null
      );
      range.numberFormat = [header.withMenu ? ["@", ...fmt] : fmt];
      const table = sheet.tables.add(range, true);
      table.name = header.id;
      const cols = Object.entries(header.columns).map((c) =>
        header.aliasHeader
        ? c[1] === "" ? c[0].replace(ALIASTRIMAGGR, "") : c[0].replace(ALIASTRIMVIEW, "")
        : c[1] === "" ? c[0] : c[1]
      );
      table.getHeaderRowRange().values = [
        header.withMenu ? [".", ...cols] : cols,
      ];
      if (header.withMenu) {
        table.onChanged.add(onMenuChanged);
      }
      await context.sync();
      if (header.withMenu) {
        const menus = (await getItem<string[]>(MENU_KEY)) ?? [];
        await setItem(MENU_KEY, [...menus, table.id]);
      }
    });
  } catch (ex) {
    stateStore.error("Failed to create Table", ex, null);
  }
}

export async function insertData(header: ICql) {
  try {
    if (!header.id) throw "Missing name!";
    let rows = (await getDetails(header.id)).results;
    await Excel.run(async (context) => {
      const table = context.workbook.tables.getItem(header.id);
      const cols = table.getHeaderRowRange().load("values");

      table.load("showTotals");

      await context.sync();

      if (rows.length > 0 && rows[0].length < cols.values[0].length) {
        if (header.withMenu) {
          rows = rows.map((r) => [
            null,
            ...r,
            ...Array(cols.values[0].length - rows[0].length - 1).fill(null),
          ]);
        } else {
          rows = rows.map((r) => [
            ...r,
            ...Array(cols.values[0].length - rows[0].length).fill(null),
          ]);
        }
      }

      switch (header.insertOption ?? 0) {
        case 0:
          table.rows.add(0, rows.length > 0 ? rows : null);
          break;
        case 1:
          table.rows.add(null, rows.length > 0 ? rows : null);
          break;
        case 2:
          const body = table.getDataBodyRange();
          const showTotals = table.showTotals;
          table.showTotals = false;
          table.rows.add(null, rows.length > 0 ? rows : null);
          table.showTotals = showTotals;
          body.delete(Excel.DeleteShiftDirection.up);

          break;
      }
      await context.sync();
    });
  } catch (ex) {
    if (ex.code !== "InvalidBinding") {
      stateStore.error("Table Insert Error", ex.message, null);
    }
    console.error(JSON.stringify(ex));
  }
}

export async function pasteData(header) {
  const options = get(optionsStore);
  try {
    const rows = (await getDetails(header.id)).results;
    await Excel.run(async (context) => {
      let startCell = context.workbook.getActiveCell();
      if (options.includeHeader) {
        const heading = startCell.getResizedRange(0, rows[0].length - 1);
        const titles = Object.entries(header.columns).map((c) =>
          c[1] === "" ? c[0] : c[1]
        );
        heading.values = [titles];
        startCell = startCell.getCell(1, 0);
      }
      const detail = startCell.getResizedRange(
        rows.length - 1,
        rows[0].length - 1
      );
      detail.numberFormat = rows.map((r) =>
        r.map((c) =>
          typeof c === "string"
            ? c.length === 0 || c[0] === "=" || DATETYPE.test(c)
              ? null
              : "@"
            : null
        )
      );
      detail.values = rows;
      await context.sync();
    });
  } catch (ex) {
    stateStore.error("Paste data error", ex.message, null);
  }
}

export async function pasteGrid(data: object[][]) {
  if (data && data.length > 0) {
    try {
      await Excel.run(async (context) => {
        const startCell = context.workbook.getActiveCell();
        const range = startCell.getResizedRange(
          data.length - 1,
          data[0].length - 1
        );
        range.numberFormat = data.map((r) =>
          r.map((c) =>
            typeof c === "string" ? (DATETYPE.test(c) ? null : "@") : null
          )
        );
        range.values = data;
        await context.sync();
      });
    } catch (ex) {
      stateStore.error("Grid paste error", ex.message, null);
    }
  } else {
    stateStore.error("Grid paste error", "Grid Empty!", null);
  }
}

export async function pasteSpecs(data: object[][]) {
  try {
    await Excel.run(async (context) => {
      const startCell = context.workbook.getActiveCell();
      const range = startCell.getResizedRange(
        data.length - 1,
        data[0].length - 1
      );
      range.values = data;
      await context.sync();
    });
  } catch (ex) {
    stateStore.error("Specs paste error", ex.message, null);
  }
}
