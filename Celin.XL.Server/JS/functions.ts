import { get } from "svelte/store";
import { CommandType, type IDetails } from "./types";
import {
  stateStore,
  serversStore,
  cmdStore,
  cqlStateStore,
  cqlStore,
  cslResponseStateStore,
  cslStateStore,
  cslStore,
} from "./stores";
import { runCmd, submitQuery, submitScript } from "./submit";
import { getItem } from "./persist";

/**
 * Set Context
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/context.html
 * @param {number} id
 * @param {boolean} lock
 * @param {CustomFunctions.StreamingInvocation<any[][]>} invocation Function Return
 */
function context(
  id: number,
  lock: boolean,
  invocation: CustomFunctions.StreamingInvocation<any[][]>
) {
  try {
    const unsubscibe = serversStore.subscribe((servers) => {
      if (servers) {

        const ctx = servers.find((c) => c.id === id);
        if (ctx) {
          global.blazorLib?.invokeMethodAsync("SelectContext", id);
          stateStore.context(id);
          stateStore.lockContext(lock);
          invocation.setResult([[ctx.name]]);
        } else {
          invocation.setResult(
            new CustomFunctions.Error(
              CustomFunctions.ErrorCode.invalidValue,
              `Unknown context: ${id}`
            ));
        }
      }
    });
    invocation.onCanceled = () => unsubscibe();
  } catch (ex: any) {
    console.error(ex);
    invocation.setResult(
      new CustomFunctions.Error(
        CustomFunctions.ErrorCode.invalidValue,
        ex.message
      )
    );
  }
}

/**
 * Call a Function on CQL Update
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/oncql.html
 * @param {string} name
 * @param {string} func
 * @param {any[][][]} parameters
 * @param {CustomFunctions.StreamingInvocation<any[][]>} invocation Function Return
 */
async function oncql(
  name: string,
  func: string,
  parameters: any[][][],
  invocation: CustomFunctions.StreamingInvocation<any[][]>
) {
  try {
    const cmd = get(cmdStore).find((c) => c.id === func);
    if (cmd) {
      if (cmd.type != CommandType.func) {
        invocation.setResult(
          new CustomFunctions.Error(
            CustomFunctions.ErrorCode.invalidValue,
            `${name} is not 'func'`
          )
        );
      } else {
        const unsubscibe = await cqlStateStore.subscribe(async (data) => {
          if (data && (!name || data?.id === name)) {
            invocation.setResult(await runCmd(cmd, [data, parameters]));
          }
        });

        invocation.onCanceled = () => unsubscibe();
      }
    } else {
      invocation.setResult(
        new CustomFunctions.Error(
          CustomFunctions.ErrorCode.invalidValue,
          `'${func}' not found!`
        )
      );
    }
  } catch (ex: any) {
    console.error(ex);
    invocation.setResult(
      new CustomFunctions.Error(
        CustomFunctions.ErrorCode.invalidValue,
        ex.message
      )
    );
  }
}

/**
 * Call a Function on CSL Update
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/oncsl.html
 * @param {string} name
 * @param {string} func
 * @param {any[][][]} parameters
 * @param {CustomFunctions.StreamingInvocation<any[][]>} invocation Function Return
 */
async function oncsl(
  name: string,
  func: string,
  parameters: any[][][],
  invocation: CustomFunctions.StreamingInvocation<any[][]>
) {
  let last = {};
  try {
    const cmd = get(cmdStore).find((c) => c.id === func);
    if (cmd) {
      if (cmd.type != CommandType.func) {
        invocation.setResult(
          new CustomFunctions.Error(
            CustomFunctions.ErrorCode.invalidValue,
            `${name} is not 'func'`
          )
        );
      } else {
        const unsubscibe1 = await cslStateStore.subscribe(async (data) => {
          if (data && (!name || data.id === name)) {
            last = { ...last, ...data };
            invocation.setResult(await runCmd(cmd, [data, parameters]));
          }
        });
        const unsubscibe2 = await cslResponseStateStore.subscribe(async (data) => {
          if (!data || data.id === name) {
            last = { ...last, ...data };
            invocation.setResult(await runCmd(cmd, [data, parameters]));
          }
        });
        invocation.onCanceled = () => {
          unsubscibe1();
          unsubscibe2();
        };
      }
    } else {
      invocation.setResult(
        new CustomFunctions.Error(
          CustomFunctions.ErrorCode.invalidValue,
          `'${func}' not found!`
        )
      );
    }
  } catch (ex: any) {
    console.error(ex);
    invocation.setResult(
      new CustomFunctions.Error(
        CustomFunctions.ErrorCode.invalidValue,
        ex.message
      )
    );
  }
}

/**
 * Stream a Function
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/callstream.html
 * @param {string} name
 * @param {any[][][]} parameters
 * @param {CustomFunctions.StreamingInvocation<any[][]>} invocation Function Return
 */
async function callstream(
  name: string,
  params: any[][][],
  invocation: CustomFunctions.StreamingInvocation<any[][]>
) {
  try {
    const fn = get(cmdStore).find((c) => c.id === name);
    if (fn) {
      if (fn.type === CommandType.func) {
        await runCmd(fn, [null, params, invocation]);
      } else {
        invocation.setResult(
          new CustomFunctions.Error(
            CustomFunctions.ErrorCode.invalidValue,
            `${name} is not 'func'`
          )
        );
      }
    } else {
      invocation.setResult(
        new CustomFunctions.Error(
          CustomFunctions.ErrorCode.notAvailable,
          `${name} not found!`
        )
      );
    }
  } catch (ex: any) {
    invocation.setResult(
      new CustomFunctions.Error(
        CustomFunctions.ErrorCode.invalidValue,
        ex.message
      )
    );
  }
}

/**
 * Volatile Call a Function
 * @customfunction
 * @volatile
 * @helpUrl https://celin.io/xl-docs/functions/callvolatile.html
 * @param {string} name
 * @param {any[][][]} parameters
 * @returns {any[][]}
 */
async function callvolatile(name: string, params: any[][][]) {
  try {
    const fn = get(cmdStore).find((c) => c.id === name);
    if (fn) {
      if (fn.type === CommandType.func) {
        return await runCmd(fn, [null, params]);
      } else {
        return new CustomFunctions.Error(
          CustomFunctions.ErrorCode.invalidValue,
          `${name} is not 'func'`
        );
      }
    } else {
      return new CustomFunctions.Error(
        CustomFunctions.ErrorCode.invalidValue,
        `${name} not found!`
      );
    }
  } catch (ex: any) {
    console.error(ex);
    return new CustomFunctions.Error(
      CustomFunctions.ErrorCode.invalidValue,
      ex.message
    );
  }
}

/**
 * Call a Function
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/call.html
 * @param {string} name
 * @param {any[][][]} parameters
 * @returns {any[][]}
 */
function call(name: string, params: any[][][]) {
  try {
    const fn = get(cmdStore).find((c) => c.id === name);
    if (fn) {
      if (fn.type === CommandType.func) {
        return runCmd(fn, [null, params]);
      } else {
        return new CustomFunctions.Error(
          CustomFunctions.ErrorCode.invalidValue,
          `${name} is not 'func'`
        );
      }
    } else {
      return new CustomFunctions.Error(
        CustomFunctions.ErrorCode.invalidValue,
        `${name} not found!`
      );
    }
  } catch (ex: any) {
    console.error(ex);
    return new CustomFunctions.Error(
      CustomFunctions.ErrorCode.invalidValue,
      ex.message
    );
  }
}

/**
 * Regular Expression function
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/regex.html
 * @param {string} match
 * @param {string} pattern
 * @returns pattern match.
 */
function regex(match: string, pattern: string) {
  const rgx = new RegExp(pattern);
  const m = rgx.exec(match);

  if (m && m.length > 1) {
    return m[1];
  }

  return "";
}

/**
. * Run a named Script
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/csl.html
 * @param {string} name Name
 * @param {any[][][]} trigger Trigger
 * @param {CustomFunctions.StreamingInvocation<string>} invocation Results
 */
async function csl(
  name: string,
  _: any[][][],
  invocation: CustomFunctions.StreamingInvocation<string>
) {
  try {
    const dt = get(cslStore);
    const csl = dt.find((d) => d?.id === name);
    let timer;
    if (csl) {
      clearTimeout(timer);
      timer = setTimeout(async () => {
        if (csl.busy) {
          invocation.setResult("");
        }

        await submitScript(csl.template ?? csl.source);
      }, 600);
    } else {
      invocation.setResult(
        new CustomFunctions.Error(
          CustomFunctions.ErrorCode.invalidValue,
          `"${name}" not found!`
        )
      );
    }
  } catch (ex: any) {
    console.error(ex);
    invocation.setResult(
      new CustomFunctions.Error(
        CustomFunctions.ErrorCode.invalidValue,
        ex.message
      )
    );
  }
}

/**
 * Run a named Query
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/cql.html
 * @param {string} name CQL Name
 * @param {any[][][]} trigger Trigger
 * @param {CustomFunctions.StreamingInvocation<string>} invocation Results
 */
async function cql(
  name: string,
  _: any[][][],
  invocation: CustomFunctions.StreamingInvocation<string>
) {
  try {
    const dt = get(cqlStore);

    const data = dt.find((d) => d?.id === name);

    if (data) {
      clearTimeout(data.timer);
      data.timer = setTimeout(async () => {
        invocation.setResult("");

        await submitQuery(data.template ?? data.query);
      }, 1000);
    } else {
      invocation.setResult(
        new CustomFunctions.Error(
          CustomFunctions.ErrorCode.invalidValue,
          `"${name}" not found!`
        )
      );
    }
  } catch (ex: any) {
    console.error(ex);
    invocation.setResult(
      new CustomFunctions.Error(
        CustomFunctions.ErrorCode.invalidValue,
        ex.message
      )
    );
  }
}

/**
 * Get CQL State
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/cqlstate.html
 * @param {string} name CQL Name
 * @param {string} attribute Attribute
 * @param {any} default Default Attribute Value
 * @param {CustomFunctions.StreamingInvocation<any>} invocation Attribute value
 * @returns The request attribute
 */
async function cqlstate(
  name: string,
  attribute: string,
  _default: any,
  invocation: CustomFunctions.StreamingInvocation<any>
) {
  invocation.setResult(_default);
  const unsubscibe = cqlStateStore.subscribe((data) => {
    if (data && (!name || data.id === name)) {
      try {
        const fn = Function(
          `try{const state=arguments[0];return ${attribute};}catch(ex){return ex.message;}`
        );
        const state = fn(data) ?? _default;
        invocation.setResult(state);
      } catch (ex: any) {
        console.error(ex);
        invocation.setResult(
          new CustomFunctions.Error(
            CustomFunctions.ErrorCode.invalidValue,
            ex.message
          )
        );
      }
    }
  });

  invocation.onCanceled = () => {
    unsubscibe();
  };
}

/**
 * Get CSL State
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/cslstate.html
 * @param {string} name CSL Name
 * @param {string} attribute Attribute
 * @param {any} default Default Attribute Value
 * @param {CustomFunctions.StreamingInvocation<any>} invocation Attribute value
 * @returns The request attribute
 */
function cslstate(
  name: string,
  attribute: string,
  _default: any,
  invocation: CustomFunctions.StreamingInvocation<any>
): void {
  invocation.setResult(_default);
  let last = {};
  try {
    const fn = Function(
      `try{const state=arguments[0];return ${attribute};}catch(ex){return ex.message;}`
    );
    const unsubscibe1 = cslStateStore.subscribe((data) => {
      if (data && (!name || data.id === name)) {
        last = { ...last, ...data };
        const state = fn(last) ?? _default;
        invocation.setResult(state);
      }
    });
    const unsubscibe2 = cslResponseStateStore.subscribe((data) => {
      if (data && (!name || data.id === name)) {
        last = { ...last, ...data };
        const state = fn(last) ?? _default;
        invocation.setResult(state);
      }
    });
    invocation.onCanceled = () => {
      unsubscibe1();
      unsubscibe2();
    };
  } catch (ex: any) {
    console.error(ex);
    invocation.setResult(
      new CustomFunctions.Error(
        CustomFunctions.ErrorCode.invalidValue,
        ex.message
      )
    );
  }
}

const lastUpdate = new Map<string, number>();

/**
 * Get named data results
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/data.html
 * @param {string} name Data Name
 * @param {number[][]} coords Row and Column numbers
 * @param {CustomFunctions.StreamingInvocation<any>} invocation Custom function invocation
 */
function data(
  name: string,
  coords: number[],
  invocation: CustomFunctions.StreamingInvocation<any>
) {
  const unsubscibe = cqlStore.subscribe((current) => {
    try {
      const data = current.find((d) => d?.id === name);
      console.log(data, coords);
      if (data && coords.length > 0) {
        if (data.busy) {
          lastUpdate.set(name, 0);
          invocation.setResult([["#BUSY"]]);
        } else {
          const submitted = new Date(data.submitted);
          const last = lastUpdate.get(name);
          if (submitted.getTime() < last!) {
            return;
          }
          lastUpdate.set(name, Date.now());
          getItem<IDetails>(`cql-${data.id}`).then((details) => {
            if (!details) return;
            if (
              details.results.length > coords[0] &&
              details.results[0].length > coords[1]
            ) {
              const r = details.results[coords[0]];
              console.log(r);
              const c = r[coords[1]];
              console.log(c);
              invocation.setResult(details.results[coords[0]][coords[1]]);
            } else {
              invocation.setResult(
                new CustomFunctions.Error(
                  CustomFunctions.ErrorCode.notAvailable
                )
              );
            }
          });
        }
      } else {
        invocation.setResult(
          new CustomFunctions.Error(
            CustomFunctions.ErrorCode.invalidValue,
            `"${name}" not found!`
          )
        );
      }
    } catch (ex) {
      console.error(ex);
    }
  });

  invocation.onCanceled = () => {
    unsubscibe();
    lastUpdate.delete(name);
  };
}

/**
 * Get the Row nd Column number of current table cell
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/tablecoords.html
 * @param {CustomFunctions.Invocation} invocation Invocation object
 * @returns {number[][]} Array with Row and Column number
 * @requiresAddress
 */
async function tableCoords(
  invocation: CustomFunctions.Invocation
) {
  const address = invocation.address!.split("!");
  try {
    const coords = await Excel.run(async (ctx) => {
      const sh = ctx.workbook.worksheets.getItem(address[0]);
      const rng = sh.getRange(address[1]);
      const tbls = rng.getTables();

      rng.load(["rowIndex", "columnIndex"]);
      await ctx.sync();

      const tbl = tbls.getFirst();
      if (tbl) {
        const tblRng = tbl.getDataBodyRange();

        tblRng.load(["rowIndex", "columnIndex"]);
        await ctx.sync();

        const row = rng.rowIndex - tblRng.rowIndex;
        const column = rng.columnIndex - tblRng.columnIndex;
        return [
          [row, column],
          [column, column]
        ];
      }
    });

    console.log(coords);
    return coords;
  } catch (ex: any) {
    console.error(ex);
  }
}

/**
 * Incremental ticker function
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/ticker.html
 * @param {number} delay Delay seconds
 * @param {CustomFunctions.StreamingInvocation<number>} invocation
 */
function ticker(
  delay: number,
  invocation: CustomFunctions.StreamingInvocation<number>
) {
  let counter = 0;
  const timer = setInterval(() => {
    invocation.setResult(++counter);
  }, delay * 1000);

  invocation.onCanceled = () => {
    clearTimeout(timer);
  };
}
