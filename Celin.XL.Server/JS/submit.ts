import { get } from "svelte/store";
import { CommandType, type ICmd, type IDetails } from "./types";
import {
  cmdStore,
  cqlStateStore,
  cqlStore,
  cslResponseStateStore,
  cslStore,
  stateStore,
  tableMenuStore,
} from "./stores";
import { AsyncFunction } from "./helper";
import { getValue, setFormula, setValues } from "./excel";
import { getItem } from "./persist";

// const VARS = /"[^"]*"|\W@(\w+)/g;
const VARS = /"[^"]*"|(\W+|\w+)\s*@(\w+)/g;
const CMD =
  /^(onMenu|onCql|onCsl|func|onTable)\s+(async\s+)?(\w+)((.|\n)*?)(?=^\n)/gm;

const lib = {
  cql: (id: string): boolean => {
    const data = get(cqlStore).find((d) => d?.id === id);
    if (data) {
      submitQuery(data.template ?? data.query);
      return true;
    }
    return false;
  },
  csl: (id: string): boolean => {
    const csl = get(cslStore).find((d) => d?.id === id);
    if (csl) {
      submitScript(csl.template ?? csl.source, false);
    }
    return false;
  },
  set: async (values: object) => {
    await setValues(values);
  },
  get: async (name: string) => {
    return await getValue(name);
  },
  formula: async (name: string, formula: string) => {
    return await setFormula(name, formula);
  },
  data: async (name: string) => {
    const d = await getItem<IDetails>(`cql-${name}`);
    return d.results;
  },
  table: async (name: string) => {
    const id = await Excel.run(async (ctx) => {
      const table = ctx.workbook.tables.getItem(name);
      await ctx.sync();
      return table.id;
    });
    return {
      id,
      col: async (column: number) => {
        const columnValues = await Excel.run(async (ctx) => {
          const c = ctx.workbook.tables
            .getItem(id)
            .getDataBodyRange()
            .getColumn(column)
            .load("values");
          await ctx.sync();
          return c.values;
        });
        const colObj = {
          id,
          column,
          columnValues,
          set: async (values: any[], start?: number) => {
            const updated = await Excel.run(async (ctx) => {
              const c = ctx.workbook.tables
                .getItem(id)
                .getDataBodyRange()
                .getColumn(column)
                .load("values");
              await ctx.sync();
              const v = c.values.map((e, i) =>
                start
                  ? i < start
                    ? e
                    : i - start > values.length - 1
                    ? e
                    : [values[i - start]]
                  : i > values.length - 1
                  ? e
                  : [values[i]]
              );
              c.values = v;
              return v.flat();
            });
            return {
              ...colObj,
              columnValues: updated,
            };
          },
        };
        return colObj;
      },
      row: async (rownum: number) => {
        const rowValues = await Excel.run(async (ctx) => {
          const r = ctx.workbook.tables
            .getItem(id)
            .getDataBodyRange()
            .getRow(rownum)
            .load("values");
          await ctx.sync();
          return r.values;
        });
        const rowObj = {
          id,
          rownum,
          rowValues,
          set: async (values: any[], start?: number) => {
            const updated = await Excel.run(async (ctx) => {
              const r = ctx.workbook.tables
                .getItem(id)
                .getDataBodyRange()
                .getRow(rownum)
                .load("values");
              await ctx.sync();
              const v = r.values[0].map((e, i) =>
                start
                  ? i < start
                    ? e
                    : i - start > values.length - 1
                    ? e
                    : values[i - start]
                  : i > values.length - 1
                  ? e
                  : values[i]
              );
              r.values = [v];
              return v;
            });
            return {
              ...rowObj,
              rowValues: updated,
            };
          },
        };
        return rowObj;
      },
    };
  },
  error: (id: string, error: string) => {
    const cmd = get(cmdStore).find((c) => c.id === id);
    if (cmd) {
      cmdStore.edit({ ...cmd, error });
    }
  },
};

export const runCmd = (cmd: ICmd, param: any[]): any => {
  switch (cmd.type) {
    case CommandType.onMenu:
      return cmd.fn(lib, tableMenuStore);
    case CommandType.onCql:
      return cmd.fn(lib, cqlStateStore);
    case CommandType.onCsl:
      return cmd.fn(lib, cslResponseStateStore);
    case CommandType.func:
      return cmd.fn(lib, ...(param ?? []));
  }
};

export const toggleCmd = async (cmd: ICmd) => {
  try {
    switch (cmd.type) {
      case CommandType.onTable:
        if (cmd.unsub) {
          console.log(cmd.unsub);
          cmd.unsub.remove();
          cmdStore.edit({ ...cmd, unsub: null, error: null });
        } else {
          const unsub = await Excel.run(async (ctx) => {
            const tb = ctx.workbook.tables.getItemOrNullObject(cmd.id);
            await ctx.sync();
            if (!tb.isNullObject) {
              const h = tb.onChanged.add(cmd.fn);
              return h;
            }
            return null;
          });
          unsub.remove();
          cmdStore.edit({ ...cmd, unsub, error: null });
        }
        break;
      default:
        if (cmd.unsub) {
          cmd.unsub();
          cmdStore.edit({ ...cmd, unsub: null, error: null });
        } else {
          const unsub = runCmd(cmd, null);
          cmdStore.edit({ ...cmd, unsub, error: null });
        }
    }
  } catch (ex) {
    cmdStore.edit({ ...cmd, error: ex.toString() });
  }
};

export const buildCmd = (cmd: ICmd): any => {
  console.log(cmd);
  const strict = "'use strict';";
  const isAsync = cmd.isAsync ? "async " : "";
  const slib = "const lib=arguments[0];";
  const subs = "return arguments[1].subscribe";
  const err = `catch(ex){console.error(ex);lib.error("${cmd.id}",ex);return ex.message;}`;
  const msg = `(msg)=>{if (msg?.id==='${cmd.id}')try{${cmd.source}}${err}}`;
  const fmsg = "const msg=arguments[1];";
  const range = "const params=arguments[2];";
  const invocation = "const invocation=arguments.length>3?arguments[3]:null;";
  switch (cmd.type) {
    case CommandType.onTable:
      return Function(`${strict}${cmd.source}`);
    case CommandType.onMenu:
      return Function(`${strict}${slib}${subs}(${isAsync}${msg})`);
    case CommandType.onCql:
      return Function(`${strict}${slib}${subs}(${isAsync}${msg})`);
    case CommandType.onCsl:
      return Function(`${strict}${slib}${subs}(${isAsync}${msg})`);
    case CommandType.func:
      return isAsync
        ? AsyncFunction(
            `${strict}${slib}${fmsg}${range}${invocation}try{${cmd.source}}${err}`
          )
        : Function(
            `${strict}${slib}${fmsg}${range}${invocation}try{${cmd.source}}${err}`
          );
  }
};

export const parseCmd = (cmd: string) => {
  if (!cmd && !cmd.trim()) return;

  let c: ICmd = null;
  try {
    const f = [...(cmd + "\n\n").matchAll(CMD)];
    if (f.length > 0) {
      c = {
        id: f[0][3],
        title: "",
        type: CommandType[f[0][1]],
        source: f[0][4].trim(),
        isAsync: f[0][2] > "",
        fn: null,
        unsub: false,
        busy: false,
        error: null,
      };
      const fn = buildCmd(c);
      c = { ...c, fn };
      // const unsub = runCmd(c, []);
      cmdStore.edit({ ...c });
    }
  } catch (ex) {
    stateStore.error("Command Error", ex.message, null);
  }
};

export const submitScript = async (
  sc: string,
  validateOnly: boolean = false
) => {
  if (!global.ready) return;
  // Test for blank script
  if (!sc && !sc.trim()) return;

  try {
    let parsed = sc;
    const range = await Promise.all(
      [...parsed.matchAll(/"[^"]*"|.each@(\w*)/g)]
        .map((e) => e[1])
        .filter((e, i, a) => e !== undefined && a.indexOf(e) === i)
        .sort((a, b) => b.length - a.length)
        .map(async (name) => {
          return await Excel.run(async (context) => {
            const table = context.workbook.tables.getItemOrNullObject(name);
            const spill = context.workbook.names.getItemOrNullObject(name);
            await context.sync();
            if (table.isNullObject && spill.isNullObject) {
              throw { message: `Range "${name}" not found!` };
            }
            const data = spill.isNullObject
              ? table.getDataBodyRange().getVisibleView().load("values")
              : spill.getRange().getSpillingToRange().load("values");
            await context.sync();
            return { name, data: JSON.stringify(data.values) };
          });
        })
    );

    range.forEach((t) => (parsed = parsed.replaceAll(`@${t.name}`, t.data)));

    const vars = await Promise.all(
      [...parsed.matchAll(VARS)]
        .map((e) => e[2])
        .filter((e, i, a) => e !== undefined && a.indexOf(e) === i)
        .sort((a, b) => b.length - a.length)
        .map(async (name) => {
          return await Excel.run(async (context) => {
            try {
              const data = context.workbook.names
                .getItem(name)
                .getRange()
                .load("values");
              await context.sync();
              return { name, data: data.values };
            } catch (ex) {
              if (ex.code === "ItemNotFound")
                throw { message: `Variable Name "${name}" not defined!` };
              else throw ex;
            }
          });
        })
    );

    vars.forEach((t) => {
      const val = t.data.join(`","`);
      parsed = parsed.replaceAll(
        `@${t.name}`,
        /\W/.test(val)
          ? `"${val}"`
          : typeof +val === "number"
          ? val
          : `"${val}"`
      );
    });

    global.blazorLib.invokeMethodAsync("SubmitCsl", parsed, sc, validateOnly);
  } catch (ex) {
    stateStore.error("Error in Script", ex.message, null);
  }
};

export const submitQuery = async (query) => {
  if (!global.ready) return;
  // Test for blank query
  if (!query && !query.trim()) return;

  try {
    const vars = await Promise.all(
      [...query.matchAll(VARS)]
        .map((e) => ({
          literal: e[0],
          operator: e[1]?.trim(),
          name: e[2],
          index: e.index,
        }))
        .filter((e, i, a) => {
          const test =
            e.name !== undefined &&
            a.findIndex((e1) => e.name === e1.name) === i;
          return test;
        })
        .sort((a, b) => b.name.length - a.name.length)
        .map(async (e) => {
          return await Excel.run(async (context) => {
            try {
              const data = context.workbook.names
                .getItem(e.name)
                .getRange()
                .load("values");
              await context.sync();
              return { ...e, data: data.values };
            } catch (ex) {
              if (ex.code === "ItemNotFound")
                throw { message: `Variable Name "${e.name}" not defined!` };
              else throw ex;
            }
          });
        })
    );

    let parsed = null;
    if (vars.length > 0) {
      parsed = query;
      vars.forEach((t) => {
        const value = t.data.flat().join(`","`);
        if (value.trim()) {
          parsed = parsed.replaceAll(`@${t.name}`, `"${value}"`);
        } else {
          if (t.operator === "=") {
            parsed = parsed.replaceAll(t.literal, "_blank");
          } else {
            parsed = parsed.replaceAll(t.literal, "!blank");
          }
        }
      });
    }

    global.blazorLib.invokeMethodAsync(
      "SubmitCql",
      parsed ?? query,
      parsed ? query : null
    );
  } catch (ex) {
    stateStore.error("Error in Query", ex.message, null);
  }
};
