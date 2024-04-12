import "./style.css";
import "./functions";
import type { detail } from "./types";
import { get } from "svelte/store";
import App from "./Components/EditorApp.svelte";
import {
  stateStore,
  serversStore,
  subjectStore,
  subjectDemoStore,
  cqlStore,
  cslResponseStore,
  cslProgressStore,
  cslStore,
  cslStateStore,
  cslResponseStateStore,
} from "./stores";
import { insertData } from "./excel";
import { setItem } from "./persist";
import { initMenus } from "./menus";

Office.onReady(async (info) => {
  await Office.addin.setStartupBehavior(Office.StartupBehavior.load);
  await initMenus();
});

export const app = {
  init: (lib, version) => {
    global.blazorLib = lib;
    const target = document.getElementById("editor-app");
    new App({
      target,
      props: {
        version,
      },
    });
  },
  login: () => stateStore.login(true),
  authenticated: (context) => {
    serversStore.update(context);
    stateStore.last();
    stateStore.login(false);
  },
  setServers: (servers) => {
    console.log("Set Servers");
    console.log(servers);
    serversStore.set(servers);
  },
  updateCql: (data) => {
    cqlStore.update(data);
    cqlStore.edit(data);
  },
  deleteCql: (id) => cqlStore.delete(id),
  setCqlDetails: async (data, results: detail) => {
    const d = cqlStore.get(data.id);
    await setItem(`cql-${data.id}`, { results });
    if (d.autoUpdate) {
      insertData(d);
    }
  },
  updateCsl: (data) => {
    Object.keys(data).forEach((k) => {
      if (data[k] === null) delete data[k];
    });
    cslStateStore.set(data);
    cslStore.edit(data);
  },
  setScriptResponse: (response) => {
    Object.keys(response).forEach((k) => {
      if (response[k] === null) delete response[k];
    });
    cslResponseStateStore.set(response);
    cslResponseStore.add(response);
  },
  clearScriptResponse: (id: string) => {
    cslProgressStore.clear(id);
    cslResponseStore.clear(id);
  },
  setScriptStatus: (status) => {
    cslProgressStore.update(status);
  },
  getQueryInfo: (id) => {
    const data = get(cqlStore).find((d) => d?.id === id);
    if (data) {
      return { qry: data.template ?? data.query, id: data.id };
    }
    return {};
  },
};

export const lookups = {
  subjectResponse: (response) => subjectStore.set(response),
  subjectDemoResponse: (response) => subjectDemoStore.results(response),
};

export const utils = {
  init: (lib) => (global.blazorLib = lib),
  focus: (id) => document.getElementById(id)?.focus(),
  defaultFocus: () => {
    if (document.activeElement.tagName === "BODY") {
      document.getElementById("defaultElement")?.focus();
    }
  },
  notifyError: (title, detail, timeout) =>
    stateStore.error(title, detail, timeout),
  loginMsg: (msg) =>
    stateStore.loginMsg(msg),
};
