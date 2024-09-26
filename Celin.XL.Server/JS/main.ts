import "./style.css";
import "./functions";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import type { detail, ICql, ICsl, ICslProgress, IServer } from "./types";
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
  init: (lib:DotNet.DotNetObject, version:string) => {
    global.blazorLib = lib;
    const target = document.getElementById("editor-app") as HTMLElement;
    new App({
      target,
      props: {
        version,
      },
    });
  },
  login: (flag: boolean) => stateStore.login(flag),
  authenticated: (context:IServer) => {
    serversStore.update(context);
    stateStore.last();
    stateStore.login(false);
  },
  setServers: (servers:IServer[]) => {
    serversStore.set(servers);
  },
  updateCql: (data:any) => {
    cqlStore.update(data);
    cqlStore.edit(data);
  },
  deleteCql: (id:any) => cqlStore.delete(id),
  setCqlDetails: async (data:ICql, results: detail) => {
    const d = cqlStore.get(data.id);
    await setItem(`cql-${data.id}`, { results });
    if (d?.autoUpdate) {
      insertData(d);
    }
  },
  updateCsl: (data:any) => {
    Object.keys(data).forEach((k) => {
      if (data[k] === null) delete data[k];
    });
    cslStateStore.set(data);
    cslStore.edit(data);
  },
  setScriptResponse: (response:any) => {
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
  setScriptStatus: (status:ICslProgress) => {
    cslProgressStore.update(status);
  },
  getQueryInfo: (id:string) => {
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
  notifyError: (title:string, detail:string, timeout:any) =>
    stateStore.error(title, detail, timeout),
  loginMsg: (msg:string) =>
    stateStore.loginMsg(msg),
};
