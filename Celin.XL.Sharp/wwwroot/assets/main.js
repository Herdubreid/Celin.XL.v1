!function(e,t){"object"==typeof exports&&"undefined"!=typeof module?t(exports):"function"==typeof define&&define.amd?define(["exports"],t):t((e="undefined"!=typeof globalThis?globalThis:e||self).lib={})}(this,(function(e){"use strict";var t;let n;function o(e,t){return!!Object.values(t).some((e=>null!==e))&&(Object.keys(e).forEach((n=>{if(null!==e[n])try{t[n]=e[n]}catch(e){}})),!0)}function r(e){let t=e.match(/(?:'?([^']+)'?!)?(.+)/);return{sheet:t?t[1]:null,cells:t?t[2]:null}}!function(e){const t=[],n="__jsObjectId",o="__dotNetObject",r="__byte[]",s="__dotNetStream",i="__jsStreamReferenceLength";let a,c;class l{constructor(e){this._jsObject=e,this._cachedFunctions=new Map}findFunction(e){const t=this._cachedFunctions.get(e);if(t)return t;let n,o=this._jsObject;if(e.split(".").forEach((t=>{if(!(t in o))throw new Error(`Could not find '${e}' ('${t}' was undefined).`);n=o,o=o[t]})),o instanceof Function)return o=o.bind(n),this._cachedFunctions.set(e,o),o;throw new Error(`The value '${e}' is not a function.`)}getWrappedObject(){return this._jsObject}}const h=0,u={[h]:new l(window)};u[0]._cachedFunctions.set("import",(e=>("string"==typeof e&&e.startsWith("./")&&(e=new URL(e.substr(2),document.baseURI).toString()),import(e))));let d,f=1;function y(e){t.push(e)}function p(e){if(e&&"object"==typeof e){u[f]=new l(e);const t={[n]:f};return f++,t}throw new Error(`Cannot create a JSObjectReference from the value '${e}'.`)}function m(e){let t=-1;if(e instanceof ArrayBuffer&&(e=new Uint8Array(e)),e instanceof Blob)t=e.size;else{if(!(e.buffer instanceof ArrayBuffer))throw new Error("Supplied value is not a typed array or blob.");if(void 0===e.byteLength)throw new Error(`Cannot create a JSStreamReference from the value '${e}' as it doesn't have a byteLength.`);t=e.byteLength}const o={[i]:t};try{const t=p(e);o[n]=t[n]}catch(t){throw new Error(`Cannot create a JSStreamReference from the value '${e}'.`)}return o}function g(e,n){c=e;const o=n?JSON.parse(n,((e,n)=>t.reduce(((t,n)=>n(e,t)),n))):null;return c=void 0,o}function w(){if(void 0===a)throw new Error("No call dispatcher has been set.");if(null===a)throw new Error("There are multiple .NET runtimes present, so a default dispatcher could not be resolved. Use DotNetObject to invoke .NET instance methods.");return a}e.attachDispatcher=function(e){const t=new v(e);return void 0===a?a=t:a&&(a=null),t},e.attachReviver=y,e.invokeMethod=function(e,t,...n){return w().invokeDotNetStaticMethod(e,t,...n)},e.invokeMethodAsync=function(e,t,...n){return w().invokeDotNetStaticMethodAsync(e,t,...n)},e.createJSObjectReference=p,e.createJSStreamReference=m,e.disposeJSObjectReference=function(e){const t=e&&e[n];"number"==typeof t&&D(t)},function(e){e[e.Default=0]="Default",e[e.JSObjectReference=1]="JSObjectReference",e[e.JSStreamReference=2]="JSStreamReference",e[e.JSVoidResult=3]="JSVoidResult"}(d=e.JSCallResultType||(e.JSCallResultType={}));class v{constructor(e){this._dotNetCallDispatcher=e,this._byteArraysToBeRevived=new Map,this._pendingDotNetToJSStreams=new Map,this._pendingAsyncCalls={},this._nextAsyncCallId=1}getDotNetCallDispatcher(){return this._dotNetCallDispatcher}invokeJSFromDotNet(e,t,n,o){const r=g(this,t),s=J(b(e,o)(...r||[]),n);return null==s?null:O(this,s)}beginInvokeJSFromDotNet(e,t,n,o,r){const s=new Promise((e=>{const o=g(this,n);e(b(t,r)(...o||[]))}));e&&s.then((t=>O(this,[e,!0,J(t,o)]))).then((t=>this._dotNetCallDispatcher.endInvokeJSFromDotNet(e,!0,t)),(t=>this._dotNetCallDispatcher.endInvokeJSFromDotNet(e,!1,JSON.stringify([e,!1,S(t)]))))}endInvokeDotNetFromJS(e,t,n){const o=t?g(this,n):new Error(n);this.completePendingCall(parseInt(e,10),t,o)}invokeDotNetStaticMethod(e,t,...n){return this.invokeDotNetMethod(e,t,null,n)}invokeDotNetStaticMethodAsync(e,t,...n){return this.invokeDotNetMethodAsync(e,t,null,n)}invokeDotNetMethod(e,t,n,o){if(this._dotNetCallDispatcher.invokeDotNetFromJS){const r=O(this,o),s=this._dotNetCallDispatcher.invokeDotNetFromJS(e,t,n,r);return s?g(this,s):null}throw new Error("The current dispatcher does not support synchronous calls from JS to .NET. Use invokeDotNetMethodAsync instead.")}invokeDotNetMethodAsync(e,t,n,o){if(e&&n)throw new Error(`For instance method calls, assemblyName should be null. Received '${e}'.`);const r=this._nextAsyncCallId++,s=new Promise(((e,t)=>{this._pendingAsyncCalls[r]={resolve:e,reject:t}}));try{const s=O(this,o);this._dotNetCallDispatcher.beginInvokeDotNetFromJS(r,e,t,n,s)}catch(e){this.completePendingCall(r,!1,e)}return s}receiveByteArray(e,t){this._byteArraysToBeRevived.set(e,t)}processByteArray(e){const t=this._byteArraysToBeRevived.get(e);return t?(this._byteArraysToBeRevived.delete(e),t):null}supplyDotNetStream(e,t){if(this._pendingDotNetToJSStreams.has(e)){const n=this._pendingDotNetToJSStreams.get(e);this._pendingDotNetToJSStreams.delete(e),n.resolve(t)}else{const n=new _;n.resolve(t),this._pendingDotNetToJSStreams.set(e,n)}}getDotNetStreamPromise(e){let t;if(this._pendingDotNetToJSStreams.has(e))t=this._pendingDotNetToJSStreams.get(e).streamPromise,this._pendingDotNetToJSStreams.delete(e);else{const n=new _;this._pendingDotNetToJSStreams.set(e,n),t=n.streamPromise}return t}completePendingCall(e,t,n){if(!this._pendingAsyncCalls.hasOwnProperty(e))throw new Error(`There is no pending async call with ID ${e}.`);const o=this._pendingAsyncCalls[e];delete this._pendingAsyncCalls[e],t?o.resolve(n):o.reject(n)}}function S(e){return e instanceof Error?`${e.message}\n${e.stack}`:e?e.toString():"null"}function b(e,t){const n=u[t];if(n)return n.findFunction(e);throw new Error(`JS object instance with ID ${t} does not exist (has it been disposed?).`)}function D(e){delete u[e]}e.findJSFunction=b,e.disposeJSObjectReferenceById=D;class N{constructor(e,t){this._id=e,this._callDispatcher=t}invokeMethod(e,...t){return this._callDispatcher.invokeDotNetMethod(null,e,this._id,t)}invokeMethodAsync(e,...t){return this._callDispatcher.invokeDotNetMethodAsync(null,e,this._id,t)}dispose(){this._callDispatcher.invokeDotNetMethodAsync(null,"__Dispose",this._id,null).catch((e=>console.error(e)))}serializeAsArg(){return{[o]:this._id}}}e.DotNetObject=N,y((function(e,t){if(t&&"object"==typeof t){if(t.hasOwnProperty(o))return new N(t[o],c);if(t.hasOwnProperty(n)){const e=t[n],o=u[e];if(o)return o.getWrappedObject();throw new Error(`JS object instance with Id '${e}' does not exist. It may have been disposed.`)}if(t.hasOwnProperty(r)){const e=t[r],n=c.processByteArray(e);if(void 0===n)throw new Error(`Byte array index '${e}' does not exist.`);return n}if(t.hasOwnProperty(s)){const e=t[s],n=c.getDotNetStreamPromise(e);return new k(n)}}return t}));class k{constructor(e){this._streamPromise=e}stream(){return this._streamPromise}async arrayBuffer(){return new Response(await this.stream()).arrayBuffer()}}class _{constructor(){this.streamPromise=new Promise(((e,t)=>{this.resolve=e,this.reject=t}))}}function J(e,t){switch(t){case d.Default:return e;case d.JSObjectReference:return p(e);case d.JSStreamReference:return m(e);case d.JSVoidResult:return null;default:throw new Error(`Invalid JS call result type '${t}'.`)}}let A=0;function O(e,t){A=0,c=e;const n=JSON.stringify(t,R);return c=void 0,n}function R(e,t){if(t instanceof N)return t.serializeAsArg();if(t instanceof Uint8Array){c.getDotNetCallDispatcher().sendByteArray(A,t);const e={[r]:A};return A++,e}return t}}(t||(t={})),Office.onReady((async e=>{console.log(e),delete history.pushState,delete history.replaceState})),e.blazorLib=void 0;const s={init:t=>{e.blazorLib=t},initCommandPrompt:t=>{let n=document.getElementById(t);n.addEventListener("keydown",(function(t){"Enter"===t.key&&t.shiftKey&&function(e){if((e.match(/"/g)||[]).length%2!=0)return!1;var t=0;for(let n=0;n<e.length;n++)if("["==e[n]?t++:"]"==e[n]&&t--,t<0)return!1;return!(t>0)}(n.value)&&(n.value.trim()&&e.blazorLib.invokeMethodAsync("PromptCommand"),t.preventDefault())}))},openLoginDlg:(t,o)=>{console.log(`Login: ${t}, ${o}`),function(t,o){const r=location.protocol+"//"+location.hostname+(location.port?":"+location.port:"")+"/assets/login.html";console.log(r),Office.context.ui.displayDialogAsync(r,{height:28,width:15,displayInIframe:!0},(r=>{r.status===Office.AsyncResultStatus.Failed?console.error(`${r.error.code} ${r.error.message}`):(n=r.value,n.addEventHandler(Office.EventType.DialogMessageReceived,(async r=>{const s=JSON.parse(r.message);switch(!0){case s.loaded:n.messageChild(JSON.stringify({username:o,title:t}));break;case s.ok:await e.blazorLib.invokeMethodAsync("Authenticate",s.username,s.password);break;case s.cancel:e.blazorLib.invokeMethodAsync("DialogCancelled"),n.close()}})),n.addEventHandler(Office.EventType.DialogEventReceived,(t=>{12006===t.error&&(e.blazorLib.invokeMethodAsync("DialogCancelled"),n.close())})))}))}(t,o)},messageDlg:e=>{console.log(`Msg: ${e}`),function(e){null==n||n.messageChild(JSON.stringify({notice:e}))}(e)},closeDlg:()=>{console.log("Close Dialog"),null==n||n.close()}},i={syncValues:async(e,t)=>{let n=r(e),o=await Excel.run((async e=>{const o=(null==n.sheet?e.workbook.worksheets.getActiveWorksheet():e.workbook.worksheets.getItem(n.sheet)).getRange(n.cells);return o.values=t,await e.sync(),o.load("values"),await e.sync(),o.values}));return JSON.stringify(o)},syncRange:async(e,t)=>{let n=r(e),s=await Excel.run((async e=>{const r=(null==n.sheet?e.workbook.worksheets.getActiveWorksheet():e.workbook.worksheets.getItem(n.sheet)).getRange(n.cells);return o(t,r)&&await e.sync(),r.load(),await e.sync(),r}));return JSON.stringify(s)},syncSheet:async(e,t)=>await Excel.run((async n=>{const r=null==e?n.workbook.worksheets.getActiveWorksheet():n.workbook.worksheets.getItem(e);return o(t,r)&&await n.sync(),r.load(),await n.sync(),r}))};e.app=s,e.xl=i}));