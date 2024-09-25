
// Types
export type detail = (string | number | boolean)[][];

export interface ISubject {
  value: string;
  label: string;
}

export interface IAlias extends ISubject {
  alias: string;
}

export interface ISubjectDemo {
  subject: ISubject,
  error: string,
  list: IAlias[];
}

export interface IDetails {
  transposed: detail;
  results: detail;
}

export interface ISummary {
  moreRecords: boolean;
  records: number;
}

export interface ITableMenu {
  id: string;
  index: number;
  option: any;
  row: any[];
}

export interface IServer {
  id: number;
  name: string;
  baseUrl: string;
  authResponse: object;
}

export enum CommandType {
  onMenu,
  onCql,
  onCsl,
  func,
  onTable,
}

export interface IAction {
  id: string;
  title: string;
  source: string;
  busy: boolean;
  error: string;
}

export interface ICmd extends IAction {
  type: CommandType;
  isAsync: boolean;
  fn: any | null;
  unsub: any | null;
}

export interface ICslResponse {
  id: string;
  error: string;
  msg: string;
  data: any;
}

export interface ICslProgress {
  id: string;
  row: number;
  of: number;
  errors: number;
  msg: string,
  msgs: string[];
}

export interface ICsl extends IAction {
  template: string;
}

export interface ICql extends IAction {
  user: string;
  query: string;
  template: string;
  columns: object;
  environment: string;
  submitted: string;
  summary: ISummary;
  demo: boolean;
  autoUpdate: boolean;
  withMenu: boolean;
  aliasHeader: boolean;
  insertOption: number;
  timer: NodeJS.Timer;
}

export interface IState {
  busy: boolean;
  login: boolean;
  loginMsg: unknown;
  active: boolean;
  notify: unknown;
  table: unknown;
  info: unknown;
  selected: unknown;
}
