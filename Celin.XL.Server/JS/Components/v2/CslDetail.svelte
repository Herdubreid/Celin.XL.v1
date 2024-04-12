<script>
  import { onDestroy } from "svelte";
  import { fade } from "svelte/transition";
  import Prism from "prismjs/prism";
  import { celinsl } from "../../prism-celinsl";
  import {
    cslResponseStore,
    cslStore,
    cslStateStore,
    stateStore,
    cslProgressStore,
  } from "../../stores";
  import { submitScript } from "../../submit";
  import { pasteGrid, pasteSpecs } from "../../excel";
  import CslProgress from "./CslProgress.svelte";

  export let detail;
  export let back;

  const unsubscribe = cslStateStore.subscribe((data) => {
    if (data && detail && data.id === detail.id) {
      detail = { ...detail, ...data };
    }
  });
  onDestroy(() => unsubscribe());
</script>

{#if detail}
  <div
    in:fade
    class="rounded-md bg-slate-200 text-slate-900 shadow-lg"
  >
    <div
      class="grid grid-cols-3 items-center border-b border-solid border-teal-900"
    >
      <div class="flex">
        <button
          class="p-1.5 rounded-tl-md bg-teal-700 hover:bg-teal-600 active:scale-90"
          data-bs-target="tooltip"
          title="Back"
          on:click={() => {
            back();
          }}
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 20 20"
            fill="currentColor"
            class="w-5 h-5 text-slate-200"
          >
            <path
              fill-rule="evenodd"
              d="M7.793 2.232a.75.75 0 01-.025 1.06L3.622 7.25h10.003a5.375 5.375 0 010 10.75H10.75a.75.75 0 010-1.5h2.875a3.875 3.875 0 000-7.75H3.622l4.146 3.957a.75.75 0 01-1.036 1.085l-5.5-5.25a.75.75 0 010-1.085l5.5-5.25a.75.75 0 011.06.025z"
              clip-rule="evenodd"
            />
          </svg>
        </button>
        <button
          class="p-1.5 hover:bg-slate-100 active:scale-90"
          data-bs-target="tooltip"
          title="Run"
          on:click={() =>
            detail.busy
              ? global.blazorLib.invokeMethodAsync("CancelScript", detail.id)
              : submitScript(detail.template ?? detail.source)}
        >
          {#if detail.busy}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              class="h-5 w-5 text-red-700 animate-spin"
            >
              <path
                fill-rule="evenodd"
                d="M8.34 1.804A1 1 0 019.32 1h1.36a1 1 0 01.98.804l.295 1.473c.497.144.971.342 1.416.587l1.25-.834a1 1 0 011.262.125l.962.962a1 1 0 01.125 1.262l-.834 1.25c.245.445.443.919.587 1.416l1.473.294a1 1 0 01.804.98v1.361a1 1 0 01-.804.98l-1.473.295a6.95 6.95 0 01-.587 1.416l.834 1.25a1 1 0 01-.125 1.262l-.962.962a1 1 0 01-1.262.125l-1.25-.834a6.953 6.953 0 01-1.416.587l-.294 1.473a1 1 0 01-.98.804H9.32a1 1 0 01-.98-.804l-.295-1.473a6.957 6.957 0 01-1.416-.587l-1.25.834a1 1 0 01-1.262-.125l-.962-.962a1 1 0 01-.125-1.262l.834-1.25a6.957 6.957 0 01-.587-1.416l-1.473-.294A1 1 0 011 10.68V9.32a1 1 0 01.804-.98l1.473-.295c.144-.497.342-.971.587-1.416l-.834-1.25a1 1 0 01.125-1.262l.962-.962A1 1 0 015.38 3.03l1.25.834a6.957 6.957 0 011.416-.587l.294-1.473zM13 10a3 3 0 11-6 0 3 3 0 016 0z"
                clip-rule="evenodd"
              />
            </svg>
          {:else}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              class="h-5 w-5 text-green-700"
            >
              <path
                fill-rule="evenodd"
                d="M2 10a8 8 0 1116 0 8 8 0 01-16 0zm6.39-2.908a.75.75 0 01.766.027l3.5 2.25a.75.75 0 010 1.262l-3.5 2.25A.75.75 0 018 12.25v-4.5a.75.75 0 01.39-.658z"
                clip-rule="evenodd"
              />
            </svg>
          {/if}
        </button>
        <button
          class="p-1.5 hover:bg-slate-100 active:scale-90"
          data-bs-target="tooltip"
          title="Clear Output Trace"
          on:click={() => {
            cslProgressStore.clear(detail.id);
            cslResponseStore.clear(detail.id);
          }}
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            stroke-width="1.5"
            stroke="currentColor"
            class="w-6 h-6 text-red-800"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>
      </div>
      <div class="text-center font-semibold">{detail.id}</div>
      <button
        class="ml-auto rounded-tr-md p-1.5 hover:bg-slate-100 active:scale-90"
        data-bs-target="tooltip"
        title="Delete"
        on:click={() => {
          cslStore.delete(detail.id);
          back();
        }}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-5 w-5 text-red-800"
          viewBox="0 0 20 20"
          fill="currentColor"
        >
          <path
            fill-rule="evenodd"
            d="M9 2a1 1 0 00-.894.553L7.382 4H4a1 1 0 000 2v10a2 2 0 002 2h8a2 2 0 002-2V6a1 1 0 100-2h-3.382l-.724-1.447A1 1 0 0011 2H9zM7 8a1 1 0 012 0v6a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v6a1 1 0 102 0V8a1 1 0 00-1-1z"
            clip-rule="evenodd"
          />
        </svg>
      </button>
    </div>
    <div class="flex flex-col mx-4 my-2">
      <CslProgress item={detail} />
    </div>
    <div class="mx-2 max-h-48 overflow-auto">
      {#each $cslResponseStore as rs, i (i)}
        {#if rs.id === detail.id}
          <div class="flex flex-col text-sm">
            <div class="flex flex-row px-2">
              {rs.msg ?? ""}
            </div>
            {#if rs.error}
              <div class="flex flex-row">
                <pre class="bg-red-400 px-2 text-stone-300">{rs.error}</pre>
              </div>
            {/if}
            {#each rs.data as d, i (i)}
              <div class="flex flex-row items-center pl-14">
                {#if d.type}
                  <div>
                    {d.type}
                  </div>
                  {#if ["dump", "data"].includes(d.type)}
                    <button
                      class="mx-2 p-0.5 hover:bg-slate-100 active:scale-90"
                      data-bs-target="tooltip"
                      title="Copy to Clipboard"
                      on:click={() => {
                        navigator.clipboard
                          .writeText(JSON.stringify(d.content))
                          .then(() => null)
                          .catch((err) =>
                            stateStore.error("Copy failed", err, null)
                          );
                      }}
                    >
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke-width="1.5"
                        stroke="currentColor"
                        class="w-6 h-6"
                      >
                        <path
                          stroke-linecap="round"
                          stroke-linejoin="round"
                          d="M9 12h3.75M9 15h3.75M9 18h3.75m3 .75H18a2.25 2.25 0 002.25-2.25V6.108c0-1.135-.845-2.098-1.976-2.192a48.424 48.424 0 00-1.123-.08m-5.801 0c-.065.21-.1.433-.1.664 0 .414.336.75.75.75h4.5a.75.75 0 00.75-.75 2.25 2.25 0 00-.1-.664m-5.8 0A2.251 2.251 0 0113.5 2.25H15c1.012 0 1.867.668 2.15 1.586m-5.8 0c-.376.023-.75.05-1.124.08C9.095 4.01 8.25 4.973 8.25 6.108V8.25m0 0H4.875c-.621 0-1.125.504-1.125 1.125v11.25c0 .621.504 1.125 1.125 1.125h9.75c.621 0 1.125-.504 1.125-1.125V9.375c0-.621-.504-1.125-1.125-1.125H8.25zM6.75 12h.008v.008H6.75V12zm0 3h.008v.008H6.75V15zm0 3h.008v.008H6.75V18z"
                        />
                      </svg>
                    </button>
                  {:else}
                    <button
                      class="mx-2 p-0.5 hover:bg-slate-100 active:scale-90"
                      data-bs-target="tooltip"
                      title="Paste into Sheet"
                      on:click={() => {
                        switch (d.type) {
                          case "grid":
                            pasteGrid(d.content);
                            break;
                          case "specs":
                            pasteSpecs(d.content);
                            break;
                        }
                      }}
                    >
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke-width="1.5"
                        stroke="currentColor"
                        class="w-6 h-6"
                      >
                        <path
                          stroke-linecap="round"
                          stroke-linejoin="round"
                          d="M8.25 7.5V6.108c0-1.135.845-2.098 1.976-2.192.373-.03.748-.057 1.123-.08M15.75 18H18a2.25 2.25 0 002.25-2.25V6.108c0-1.135-.845-2.098-1.976-2.192a48.424 48.424 0 00-1.123-.08M15.75 18.75v-1.875a3.375 3.375 0 00-3.375-3.375h-1.5a1.125 1.125 0 01-1.125-1.125v-1.5A3.375 3.375 0 006.375 7.5H5.25m11.9-3.664A2.251 2.251 0 0015 2.25h-1.5a2.251 2.251 0 00-2.15 1.586m5.8 0c.065.21.1.433.1.664v.75h-6V4.5c0-.231.035-.454.1-.664M6.75 7.5H4.875c-.621 0-1.125.504-1.125 1.125v12c0 .621.504 1.125 1.125 1.125h9.75c.621 0 1.125-.504 1.125-1.125V16.5a9 9 0 00-9-9z"
                        />
                      </svg>
                    </button>
                  {/if}
                {:else}
                  <div>{d.content}</div>
                {/if}
              </div>
            {/each}
          </div>
        {/if}
      {/each}
    </div>
    <div class="flex flex-col mx-6 my-2 p-2 bg-slate-900 text-green-400">
      <pre class="max-h-36 overflow-auto"><code class="whitespace-pre-wrap"
          >{@html Prism.highlight(detail.source, celinsl, "CelinSL")}</code
        ></pre>
    </div>
  </div>
{/if}
