//
import { writable } from "svelte/store";
import { getItem, setItem, removeItem } from "./persist";
import { buildCmd, runCmd } from "./submit";
import {
  type ICslResponse,
  type ICmd,
  type ICql,
  type ICsl,
  type ICslProgress,
  type IServer,
  type ITableMenu,
  type ISubject,
  type ISubjectDemo,
  CommandType,
} from "./types";

//#region options
const initOptions = {
  includeHeader: true,
};
const options = () => {
  const { update, subscribe } = writable(initOptions);

  return {
    update,
    subscribe,
  };
};
export const optionsStore = options();
//#endregion

//#region state
const initFlags = {
  busy: false,
  login: false,
  server: false,
  active: false,
  notify: null,
  action: false,
};
const resetState = {
  table: null,
  info: null,
  contextId: 0,
};
const initState = {
  ...initFlags,
  ...resetState,
  selected: null,
};
let lastState = initState;
const state = () => {
  const { set, subscribe, update } = writable<any>(initState);

  return {
    busy: (busy) => update((s) => ({ ...s, busy })),
    action: (action) => update((s) => ({ ...s, action })),
    subPrompt: (subPrompt) =>
      update((s) => ({ ...s, ...initFlags, subPrompt, active: subPrompt })),
    alPrompt: (alPrompt) =>
      update((s) => ({ ...s, ...initFlags, alPrompt, active: alPrompt })),
    opPrompt: (opPrompt) =>
      update((s) => ({ ...s, ...initFlags, opPrompt, active: opPrompt })),
    login: (login) =>
      update((s) => {
        lastState = s;
        return { ...s, ...initFlags, login, active: login };
      }),
    loginMsg: (loginMsg) =>
      update((s) => ({
        ...s,
        loginMsg,
      })),
    server: (server) =>
      update((s) => ({ ...s, ...initFlags, server, active: server })),
    context: (contextId) =>
      update((s) => ({ ...s, contextId})),
    selected: (data) =>
      update((s) => {
        const selected = s.selected === data ? null : data;
        const info = s.info === null ? null : selected;
        const table = s.table == null ? null : selected;
        return { ...s, selected, info, table };
      }),
    info: () =>
      update((s) => ({
        ...s,
        ...resetState,
        info: s.info === null ? s.selected : null,
      })),
    table: () =>
      update((s) => ({
        ...s,
        ...resetState,
        table: s.table === null ? s.selected : null,
      })),
    last: () => set(lastState),
    error: (title, details, timeout) => 
      update((s) => ({
        ...s,
        busy: false,
        notify: {
          title,
          details,
          type: "e",
          timeout,
        },
      })),
    clear: () => update((s) => ({ ...s, notify: null })),
    subscribe,
  };
};
export const stateStore = state();
//#endregion

//#region servers
const SERVER_KEY = "servers";
const servers = () => {
  const { set, subscribe, update } = writable<IServer[]>(null);

  return {
    init: () => {
      getItem<any[]>(SERVER_KEY).then((s) => {
        const servers = s ?? [];
        set(servers);
        stateStore.server(servers.length === 0);
        if (servers.length > 0) {
          global.blazorLib.invokeMethodAsync("SelectContext", 0);
        }
      });
    },
    set: (servers) => {
      setItem(SERVER_KEY, servers);
      set(servers);
      stateStore.server(servers.length === 0);
      if (servers.length > 0) {
        global.blazorLib.invokeMethodAsync("SelectContext", 0);
      }
    },
    update: (server) =>
      update((s) => {
        const ndx = s.findIndex((e) => e.id === server.id);
        if (ndx !== -1) {
          s = [...s.slice(0, ndx), server, ...s.slice(ndx + 1)];
          setItem(SERVER_KEY, s);
        }
        return s;
      }),
    subscribe,
  };
};
export const serversStore = servers();
//#endregion

//#region query
const QUERY_KEY = "query";
const query = () => {
  const { set, subscribe, update } = writable("");

  getItem<string>(QUERY_KEY).then((q) => {
    set(q ?? "");
    subscribe((current) => {
      setItem(QUERY_KEY, current);
    });
  });
  return {
    set,
    subscribe,
    replace: (text, from, to) =>
      update((q) => `${q.slice(0, from)}${text}${q.slice(to)}`),
    append: (text) => update((q) => (q = `${q}\n${text}`)),
  };
};
export const queryStore = query();
//#endregion

//#region cqlState
const cqlState = () => {
  const { set, subscribe } = writable<ICql>();
  return {
    set,
    subscribe,
  };
};
export const cqlStateStore = cqlState();
//#endregion

//#region cql
const DATA_KEY = "data";
const cql = () => {
  const { set, subscribe, update } = writable<ICql[]>([]);
  getItem<ICql[]>(DATA_KEY).then((d) => {
    set(d ?? []);
    subscribe((current) =>
      setItem(
        DATA_KEY,
        current.map((c) => ({ ...c, busy: false }))
      )
    );
  });
  return {
    edit: (data: ICql) =>
      update((d) => {
        Object.keys(data).forEach((k) => {
          if (data[k] === null) delete data[k];
        });
        const ndx = d.findIndex((e) => e.id === data.id);
        if (ndx !== -1) {
          const e = { ...d[ndx], error: null, ...data };
          const a = [...d.slice(0, ndx), e, ...d.slice(ndx + 1)];
          return a;
        }
        return [data, ...d];
      }),
    update: (data: ICql) => {
      Object.keys(data).forEach((k) => {
        if (data[k] === null) delete data[k];
      });
      cqlStateStore.set(data);
    },
    delete: (id) => {
      update((d) => {
        const ndx = d.findIndex((d) => d.id === id);
        if (ndx !== -1) return [...d.slice(0, ndx), ...d.slice(ndx + 1)];
        return d;
      });
      removeItem(`cql-${id}`);
    },
    get: (id) => {
      let result;
      const unsubscibe = subscribe(
        (current) => (result = current.find((e) => e.id === id))
      );
      unsubscibe();
      return result;
    },
    subscribe,
  };
};
export const cqlStore = cql();
//#endregion

//#region cslState
const cslState = () => {
  const { set, subscribe } = writable<ICsl>();
  return {
    set,
    subscribe,
  };
};
export const cslStateStore = cslState();
//#endregion

//#region csl
const CSL_KEY = "csl";
const csl = () => {
  const { set, subscribe, update } = writable<ICsl[]>([]);
  getItem<ICsl[]>(CSL_KEY).then((d) => {
    set(d ?? []);
    subscribe((current) =>
      setItem(
        CSL_KEY,
        current.map((c) => ({ ...c, busy: false }))
      )
    );
  });
  return {
    edit: (data: ICsl) =>
      update((d) => {
        Object.keys(data).forEach((k) => {
          if (data[k] === null) delete data[k];
        });
        const ndx = d.findIndex((e) => e.id === data.id);
        if (ndx !== -1) {
          const e = { ...d[ndx], error: null, ...data };
          const a = [...d.slice(0, ndx), e, ...d.slice(ndx + 1)];
          return a;
        }
        return [data, ...d];
      }),
    delete: (id) => {
      update((d) => {
        const ndx = d.findIndex((d) => d.id === id);
        if (ndx !== -1) return [...d.slice(0, ndx), ...d.slice(ndx + 1)];
        return d;
      });
    },
    subscribe,
  };
};
export const cslStore = csl();
//#endregion csl

//#region cslResponseState
const cslResponseState = () => {
  const { set, subscribe } = writable<ICslResponse>();
  return {
    set,
    subscribe,
  };
};
export const cslResponseStateStore = cslResponseState();
//#endregion

//#region cslResponse
const cslResponse = () => {
  const { subscribe, update } = writable([]);
  return {
    add: (response) => update((d) => [response, ...d]),
    clear: (id) => update((d) => [...d.filter((e) => e.id !== id)]),
    subscribe,
  };
};
export const cslResponseStore = cslResponse();
//#endregion

//#region cslProgress
const cslProgress = () => {
  const { update, subscribe } = writable<ICslProgress[]>([]);
  return {
    clear: (id: string) => {
      update((p) => {
        const ndx = p.findIndex((e) => e.id === id);
        if (ndx === -1) return p;
        const e: ICslProgress = {
          ...p[ndx],
          row: 0,
          of: 0,
          errors: 0,
          msgs: [],
        };
        return [...p.slice(0, ndx), e, ...p.slice(ndx + 1)];
      });
    },
    update: (progress: ICslProgress) =>
      update((p) => {
        const ndx = p.findIndex((e) => e.id === progress.id);
        if (ndx !== -1) {
          const e = {
            ...p[ndx],
            ...progress,
            msgs: [
              progress.msg,
              ...p[ndx].msgs.slice(0, Math.min(p[ndx].msgs.length, 10)),
            ],
          };
          return [...p.slice(0, ndx), e, ...p.slice(ndx + 1)];
        }
        return [...p, { ...progress, msgs: [progress.msg] }];
      }),
    subscribe,
  };
};
export const cslProgressStore = cslProgress();
//#endregion

//#region subject
const subject = () => {
  const { set, subscribe } = writable<ISubject[]>([]);
  return {
    set,
    subscribe,
  };
};
export const subjectStore = subject();
//#endregion

//#region subjectDemo
const subjectDemo = () => {
  const { update, subscribe } = writable<ISubjectDemo>(null);
  return {
    subject: (subject: ISubject) => update((s) => ({ ...s, subject })),
    results: (rs: any) => update((s) => ({ ...s, ...rs })),
    subscribe,
  };
};
export const subjectDemoStore = subjectDemo();
//#endregion

//#region script
const SCRIPT_KEY = "script";
const script = () => {
  const { set, subscribe, update } = writable("");

  getItem<string>(SCRIPT_KEY).then((t) => {
    set(t ?? "");
    subscribe((current) => setItem(SCRIPT_KEY, current));
  });
  return {
    set,
    subscribe,
    append: (text) => update((q) => (q = `${q}\n${text}`)),
  };
};
export const scriptStore = script();
//#endregion

//#region cmdEdit
const CMD_EDIT_KEY = "cmd-edit";
const cmdEdit = () => {
  const { set, subscribe, update } = writable("");

  getItem<string>(CMD_EDIT_KEY).then((t) => {
    set(t ?? "");
    subscribe((current) => setItem(CMD_EDIT_KEY, current));
  });
  return {
    set,
    subscribe,
    append: (text) => update((c) => (c = `${c}\n${text}`)),
  };
};
export const cmdEditStore = cmdEdit();
//#endregion

//#region cmdState
const cmdState = () => {
  const { set, subscribe } = writable<ICmd>();
  return {
    set,
    subscribe,
  };
};
export const cmdStateStore = cmdState();
//#endregion

//#region  cmd
const CMD_KEY = "cmd";
const cmd = () => {
  const { set, subscribe, update } = writable<ICmd[]>([]);

  getItem<ICmd[]>(CMD_KEY).then((items) => {
    const cmds = (items ?? []).map<ICmd>((c) => {
      try {
        const fn = buildCmd(c);
        const cmd = {
          ...c,
          fn,
        };
        const unsub = c.unsub ? runCmd(cmd, null) : null;
        return {
          ...cmd,
          unsub,
        };
      } catch (ex) {
        return {
          ...c,
          error: ex.toString(),
        };
      }
    });
    set(cmds);
    subscribe((current) => {
      setItem(CMD_KEY, [
        ...current?.map((e) => ({
          ...e,
          fn: null,
          unsub: e.type == CommandType.func ? false : e.unsub == null ? false : true,
          error: null,
        })),
      ]);
    });
  });
  return {
    set,
    subscribe,
    edit: (cmd: ICmd) => {
      update((c) => {
        const ndx = c.findIndex((e) => e.id === cmd.id);
        if (ndx !== -1) {
          try {
            c[ndx]?.unsub();
          } catch {}
          const a = [...c.slice(0, ndx), cmd, ...c.slice(ndx + 1)];
          return a;
        }
        return [...c, cmd];
      });
      cmdStateStore.set(cmd);
    },
    delete: (id) => {
      update((d) => {
        const ndx = d.findIndex((d) => d.id === id);
        if (ndx !== -1) return [...d.slice(0, ndx), ...d.slice(ndx + 1)];
        return d;
      });
    },
  };
};
export const cmdStore = cmd();
//#endregion

//#region tableMenu
const tableMenu = () => {
  const { subscribe, set } = writable<ITableMenu>();
  return {
    set,
    subscribe,
  };
};
export const tableMenuStore = tableMenu();
//#endregion
