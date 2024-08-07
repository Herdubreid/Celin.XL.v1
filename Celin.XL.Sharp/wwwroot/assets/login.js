var app=function(){"use strict";function t(){}const e=t=>t;function n(t){return t()}function o(){return Object.create(null)}function r(t){t.forEach(n)}function s(t){return"function"==typeof t}function i(t,e){return t!=t?e==e:t!==e||t&&"object"==typeof t||"function"==typeof t}const c="undefined"!=typeof window;let a=c?()=>window.performance.now():()=>Date.now(),l=c?t=>requestAnimationFrame(t):t;const u=new Set;function d(t){u.forEach((e=>{e.c(t)||(u.delete(e),e.f())})),0!==u.size&&l(d)}function f(t,e){t.appendChild(e)}function p(t){if(!t)return document;const e=t.getRootNode?t.getRootNode():t.ownerDocument;return e&&e.host?e:t.ownerDocument}function h(t){const e=m("style");return e.textContent="/* empty */",function(t,e){f(t.head||t,e),e.sheet}(p(t),e),e.sheet}function g(t){t.parentNode&&t.parentNode.removeChild(t)}function m(t){return document.createElement(t)}function b(t){return document.createElementNS("http://www.w3.org/2000/svg",t)}function $(t){return document.createTextNode(t)}function y(){return $(" ")}function v(t,e,n,o){return t.addEventListener(e,n,o),()=>t.removeEventListener(e,n,o)}function w(t,e,n){null==n?t.removeAttribute(e):t.getAttribute(e)!==n&&t.setAttribute(e,n)}function x(t,e){e=""+e,t.data!==e&&(t.data=e)}function _(t,e){t.value=null==e?"":e}const O=new Map;let E,k=0;function N(t,e,n,o,r,s,i,c=0){const a=16.666/o;let l="{\n";for(let t=0;t<=1;t+=a){const o=e+(n-e)*s(t);l+=100*t+`%{${i(o,1-o)}}\n`}const u=l+`100% {${i(n,1-n)}}\n}`,d=`__svelte_${function(t){let e=5381,n=t.length;for(;n--;)e=(e<<5)-e^t.charCodeAt(n);return e>>>0}(u)}_${c}`,f=p(t),{stylesheet:g,rules:m}=O.get(f)||function(t,e){const n={stylesheet:h(e),rules:{}};return O.set(t,n),n}(f,t);m[d]||(m[d]=!0,g.insertRule(`@keyframes ${d} ${u}`,g.cssRules.length));const b=t.style.animation||"";return t.style.animation=`${b?`${b}, `:""}${d} ${o}ms linear ${r}ms 1 both`,k+=1,d}function S(t,e){const n=(t.style.animation||"").split(", "),o=n.filter(e?t=>t.indexOf(e)<0:t=>-1===t.indexOf("__svelte")),r=n.length-o.length;r&&(t.style.animation=o.join(", "),k-=r,k||l((()=>{k||(O.forEach((t=>{const{ownerNode:e}=t.stylesheet;e&&g(e)})),O.clear())})))}function C(t){E=t}const L=[],P=[];let A=[];const z=[],M=Promise.resolve();let R=!1;function D(t){A.push(t)}const j=new Set;let q,J=0;function B(){if(0!==J)return;const t=E;do{try{for(;J<L.length;){const t=L[J];J++,C(t),H(t.$$)}}catch(t){throw L.length=0,J=0,t}for(C(null),L.length=0,J=0;P.length;)P.pop()();for(let t=0;t<A.length;t+=1){const e=A[t];j.has(e)||(j.add(e),e())}A.length=0}while(L.length);for(;z.length;)z.pop()();R=!1,j.clear(),C(t)}function H(t){if(null!==t.fragment){t.update(),r(t.before_update);const e=t.dirty;t.dirty=[-1],t.fragment&&t.fragment.p(t.ctx,e),t.after_update.forEach(D)}}function T(t,e,n){t.dispatchEvent(function(t,e,{bubbles:n=!1,cancelable:o=!1}={}){return new CustomEvent(t,{detail:e,bubbles:n,cancelable:o})}(`${e?"intro":"outro"}${n}`))}const F=new Set;let U;const V={duration:0};function G(n,o,i,c){let f,p=o(n,i,{direction:"both"}),h=c?0:1,g=null,m=null,b=null;function $(){b&&S(n,b)}function y(t,e){const n=t.b-h;return e*=Math.abs(n),{a:h,b:t.b,d:n,duration:e,start:t.start,end:t.start+e,group:t.group}}function v(o){const{delay:s=0,duration:i=300,easing:c=e,tick:v=t,css:w}=p||V,x={start:a()+s,b:o};o||(x.group=U,U.r+=1),"inert"in n&&(o?void 0!==f&&(n.inert=f):(f=n.inert,n.inert=!0)),g||m?m=x:(w&&($(),b=N(n,h,o,i,s,c,w)),o&&v(0,1),g=y(x,i),D((()=>T(n,o,"start"))),function(t){let e;0===u.size&&l(d),new Promise((n=>{u.add(e={c:t,f:n})}))}((t=>{if(m&&t>m.start&&(g=y(m,i),m=null,T(n,g.b,"start"),w&&($(),b=N(n,h,g.b,g.duration,0,c,p.css))),g)if(t>=g.end)v(h=g.b,1-h),T(n,g.b,"end"),m||(g.b?$():--g.group.r||r(g.group.c)),g=null;else if(t>=g.start){const e=t-g.start;h=g.a+g.d*c(e/g.duration),v(h,1-h)}return!(!g&&!m)})))}return{run(t){s(p)?(q||(q=Promise.resolve(),q.then((()=>{q=null}))),q).then((()=>{p=p({direction:t?"in":"out"}),v(t)})):v(t)},end(){$(),g=m=null}}}function I(t,e){const n=t.$$;null!==n.fragment&&(!function(t){const e=[],n=[];A.forEach((o=>-1===t.indexOf(o)?e.push(o):n.push(o))),n.forEach((t=>t())),A=e}(n.after_update),r(n.on_destroy),n.fragment&&n.fragment.d(e),n.on_destroy=n.fragment=null,n.ctx=[])}function K(t,e){-1===t.$$.dirty[0]&&(L.push(t),R||(R=!0,M.then(B)),t.$$.dirty.fill(0)),t.$$.dirty[e/31|0]|=1<<e%31}function Q(e,i,c,a,l,u,d=null,f=[-1]){const p=E;C(e);const h=e.$$={fragment:null,ctx:[],props:u,update:t,not_equal:l,bound:o(),on_mount:[],on_destroy:[],on_disconnect:[],before_update:[],after_update:[],context:new Map(i.context||(p?p.$$.context:[])),callbacks:o(),dirty:f,skip_bound:!1,root:i.target||p.$$.root};d&&d(h.root);let m=!1;if(h.ctx=c?c(e,i.props||{},((t,n,...o)=>{const r=o.length?o[0]:n;return h.ctx&&l(h.ctx[t],h.ctx[t]=r)&&(!h.skip_bound&&h.bound[t]&&h.bound[t](r),m&&K(e,t)),n})):[],h.update(),m=!0,r(h.before_update),h.fragment=!!a&&a(h.ctx),i.target){if(i.hydrate){const t=function(t){return Array.from(t.childNodes)}(i.target);h.fragment&&h.fragment.l(t),t.forEach(g)}else h.fragment&&h.fragment.c();i.intro&&((b=e.$$.fragment)&&b.i&&(F.delete(b),b.i($))),function(t,e,o){const{fragment:i,after_update:c}=t.$$;i&&i.m(e,o),D((()=>{const e=t.$$.on_mount.map(n).filter(s);t.$$.on_destroy?t.$$.on_destroy.push(...e):r(e),t.$$.on_mount=[]})),c.forEach(D)}(e,i.target,i.anchor),B()}var b,$;C(p)}class W{$$=void 0;$$set=void 0;$destroy(){I(this,1),this.$destroy=t}$on(e,n){if(!s(n))return t;const o=this.$$.callbacks[e]||(this.$$.callbacks[e]=[]);return o.push(n),()=>{const t=o.indexOf(n);-1!==t&&o.splice(t,1)}}$set(t){var e;this.$$set&&(e=t,0!==Object.keys(e).length)&&(this.$$.skip_bound=!0,this.$$set(t),this.$$.skip_bound=!1)}}function X(t,{delay:n=0,duration:o=400,easing:r=e}={}){const s=+getComputedStyle(t).opacity;return{delay:n,duration:o,easing:r,css:t=>"opacity: "+t*s}}function Y(t){let e,n,o,s,i,c,a,l,u,d,p,h,O,E,k,N,S,C,L,P,A,z,M,R,j,q,J,B,H;return{c(){e=m("div"),n=m("form"),o=m("div"),s=m("div"),i=m("p"),c=$(t[2]),a=y(),l=m("div"),u=m("input"),d=y(),p=m("div"),h=m("input"),O=y(),E=m("div"),k=m("div"),N=m("button"),S=b("svg"),C=b("path"),L=y(),P=m("button"),A=b("svg"),z=b("path"),M=y(),R=m("div"),j=$(t[3]),w(i,"class","text-xl font-semibold"),w(s,"class","flex place-content-center py-2 text-slate-300"),u.autofocus=!0,u.required=!0,u.disabled=t[4],w(u,"class","border rounded w-full py-2 px-3"),w(u,"type","text"),w(u,"placeholder","User Name"),w(l,"class","pb-4"),w(h,"class","border border-red rounded w-full py-2 px-3"),w(h,"type","password"),h.required=!0,h.disabled=t[4],w(h,"placeholder","Password"),w(p,"class","mb-2"),w(o,"class","px-8"),w(C,"fill-rule","evenodd"),w(C,"d","M3 3a1 1 0 011 1v12a1 1 0 11-2 0V4a1 1 0 011-1zm7.707 3.293a1 1 0 010 1.414L9.414 9H17a1 1 0 110 2H9.414l1.293 1.293a1 1 0 01-1.414 1.414l-3-3a1 1 0 010-1.414l3-3a1 1 0 011.414 0z"),w(C,"clip-rule","evenodd"),w(S,"xmlns","http://www.w3.org/2000/svg"),w(S,"class","h-5 w-5"),w(S,"viewBox","0 0 20 20"),w(S,"fill","currentColor"),w(N,"class","transform active:scale-95 hover:ring py-2 px-4 bg-green-100"),w(N,"type","submit"),N.disabled=t[4],w(z,"fill-rule","evenodd"),w(z,"d","M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"),w(z,"clip-rule","evenodd"),w(A,"xmlns","http://www.w3.org/2000/svg"),w(A,"class","h-5 w-5"),w(A,"viewBox","0 0 20 20"),w(A,"fill","currentColor"),w(P,"class","transform active:scale-95 hover:ring py-2 px-4 bg-red-100"),w(P,"type","button"),P.disabled=t[4],w(k,"class","flex items-center place-content-between"),w(E,"class","px-8 pt-4"),w(R,"class","px-8 py-2 text-sm text-center text-red-400"),w(e,"class","bg-opacity-70 bg-slate-900 z-40 fixed top-0 h-full w-full")},m(r,g){var m;!function(t,e,n){t.insertBefore(e,n||null)}(r,e,g),f(e,n),f(n,o),f(o,s),f(s,i),f(i,c),f(o,a),f(o,l),f(l,u),_(u,t[0]),f(o,d),f(o,p),f(p,h),_(h,t[1]),f(n,O),f(n,E),f(E,k),f(k,N),f(N,S),f(S,C),f(k,L),f(k,P),f(P,A),f(A,z),f(n,M),f(n,R),f(R,j),J=!0,u.focus(),B||(H=[v(u,"input",t[7]),v(h,"input",t[8]),v(P,"click",t[9]),v(n,"submit",(m=t[10],function(t){return t.preventDefault(),m.call(this,t)}))],B=!0)},p(t,[e]){(!J||4&e)&&x(c,t[2]),(!J||16&e)&&(u.disabled=t[4]),1&e&&u.value!==t[0]&&_(u,t[0]),(!J||16&e)&&(h.disabled=t[4]),2&e&&h.value!==t[1]&&_(h,t[1]),(!J||16&e)&&(N.disabled=t[4]),(!J||16&e)&&(P.disabled=t[4]),(!J||8&e)&&x(j,t[3])},i(t){J||(t&&D((()=>{J&&(q||(q=G(e,X,{},!0)),q.run(1))})),J=!0)},o(t){t&&(q||(q=G(e,X,{},!1)),q.run(0)),J=!1},d(t){t&&g(e),t&&q&&q.end(),B=!1,r(H)}}}function Z(t,e,n){let o="",r="",s="Login",i="",c=!1;Office.onReady((t=>{Office.context.ui.addHandlerAsync(Office.EventType.DialogParentMessageReceived,(t=>{const e=JSON.parse(t.message);switch(!0){case!!e.title:n(2,s=e.title);break;case!!e.username:n(0,o=e.username);break;case!!e.notice:n(4,c=!1),n(3,i=e.notice)}})),Office.context.ui.messageParent(JSON.stringify({loaded:!0}))}));const a=()=>{n(3,i=""),n(4,c=!0),Office.context.ui.messageParent(JSON.stringify({ok:!0,username:o,password:r}))},l=()=>{Office.context.ui.messageParent(JSON.stringify({cancel:!0}))};return[o,r,s,i,c,a,l,function(){o=this.value,n(0,o)},function(){r=this.value,n(1,r)},()=>l(),()=>a()]}"undefined"!=typeof window&&(window.__svelte||(window.__svelte={v:new Set})).v.add("4");return new class extends W{constructor(t){super(),Q(this,t,Z,Y,i,{})}}({target:document.body,props:{}})}();
//# sourceMappingURL=login.js.map
