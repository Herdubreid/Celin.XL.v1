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
  tableMenuStore,
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
      console.log(servers);
      if (servers) {

        const ctx = servers.find((c) => c.id === id);
        if (ctx) {
          global.blazorLib.invokeMethodAsync("SelectContext", id);
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
  } catch (ex) {
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
function oncql(
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
        const unsubscibe = cqlStateStore.subscribe((data) => {
          if (data && (!name || data?.id === name)) {
            invocation.setResult(runCmd(cmd, [data, parameters]));
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
  } catch (ex) {
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
function oncsl(
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
        const unsubscibe1 = cslStateStore.subscribe((data) => {
          if (data && (!name || data.id === name)) {
            last = { ...last, ...data };
            invocation.setResult(runCmd(cmd, [data, parameters]));
          }
        });
        const unsubscibe2 = cslResponseStateStore.subscribe((data) => {
          if (!data || data.id === name) {
            last = { ...last, ...data };
            invocation.setResult(runCmd(cmd, [data, parameters]));
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
  } catch (ex) {
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
function callstream(
  name: string,
  params: any[][][],
  invocation: CustomFunctions.StreamingInvocation<any[][]>
) {
  try {
    const fn = get(cmdStore).find((c) => c.id === name);
    if (fn) {
      if (fn.type === CommandType.func) {
        runCmd(fn, [null, params, invocation]);
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
  } catch (ex) {
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
function callvolatile(name: string, params: any[][][]) {
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
  } catch (ex) {
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
  } catch (ex) {
    console.error(ex);
    return new CustomFunctions.Error(
      CustomFunctions.ErrorCode.invalidValue,
      ex.message
    );
  }
}

/**
 * Menu event
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/onmenu.html
 * @param {string} menu Menu name
 * @param {any} option Menu option
 * @param {number[]} columns Columns to display
 * @param {CustomFunctions.StreamingInvocation<any[][]>} invocation Attribute value
 */
function onmenu(
  menu: string,
  option: any,
  columns: number[],
  invocation: CustomFunctions.StreamingInvocation<any[][]>
) {
  invocation.setResult(
    new CustomFunctions.Error(CustomFunctions.ErrorCode.notAvailable)
  );
  const unsubscibe = tableMenuStore.subscribe(async (mnu) => {
    try {
      if (mnu && mnu.id === menu && (!option || option === mnu.option)) {
        await Excel.run(async (ctx) => {
          const table = ctx.workbook.tables.getItem(mnu.id);
          const range = table.rows.getItemAt(mnu.index);
          range.load();
          await ctx.sync();
          if (range.values.length > 0) {
            const row = range.values[0];
            const values = columns.map(
              (c) => row[Math.max(0, Math.min(row.length, c))]
            );
            invocation.setResult([values.length > 0 ? values : [""]]);
          }
        });
      }
    } catch (ex) {
      console.error(ex);
      invocation.setResult(
        new CustomFunctions.Error(
          CustomFunctions.ErrorCode.invalidValue,
          ex.message
        )
      );
    }
  });

  invocation.onCanceled = () => {
    unsubscibe();
  };
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
  } catch (ex) {
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
  } catch (ex) {
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
      } catch (ex) {
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
  } catch (ex) {
    console.error(ex);
    invocation.setResult(
      new CustomFunctions.Error(
        CustomFunctions.ErrorCode.invalidValue,
        ex.message
      )
    );
  }
}

const FMT = /{(\d+)}/g;
const lastUpdate = new Map<string, Number>();

/**
 * Get named data results
 * @customfunction
 * @helpUrl https://celin.io/xl-docs/functions/data.html
 * @param {string} name Data Name
 * @param {number} from From Row
 * @param {number} to To Row
 * @param {any[]} [format] Format
 * @param {CustomFunctions.StreamingInvocation<any[][]>} invocation Custom function invocation
 */
function data(
  name: string,
  from: number,
  to: number,
  format: any[],
  invocation: CustomFunctions.StreamingInvocation<any[][]>
) {
  const unsubscibe = cqlStore.subscribe((current) => {
    try {
      const data = current.find((d) => d?.id === name);
      if (data) {
        if (data.busy) {
          lastUpdate.set(name, 0);
          invocation.setResult([["#BUSY"]]);
        } else {
          const submitted = new Date(data.submitted);
          const last = lastUpdate.get(name);
          if (submitted.getTime() < last) {
            return;
          }
          lastUpdate.set(name, Date.now());
          getItem<IDetails>(data.id).then((allrows) => {
            const rows =
              from < 0
                ? allrows.results
                : allrows.results.slice(
                  from,
                  Math.min(
                    Math.max(to, -1),
                    (allrows.transposed ?? allrows.results).length
                  )
                );
            if (rows.length > 0) {
              if (format.length > 0) {
                const frows = rows.map((r) =>
                  format.map((c: any) => {
                    if (typeof c === "string") {
                      const fmts = c.matchAll(FMT);
                      if (fmts) {
                        const fs = [...fmts].reduce(
                          (f, fmt) => f.replace(fmt[0], r[fmt[1]]),
                          c
                        );
                        return fs;
                      }
                      return c;
                    }
                    return r[c];
                  })
                );
                invocation.setResult(frows);
              } else {
                invocation.setResult(rows);
              }
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
