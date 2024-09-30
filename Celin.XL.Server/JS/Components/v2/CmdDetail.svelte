<script>
  import Prism from "prismjs/prism";
  import { celincm } from "../../prism-celincm";
  import { CommandType } from "../../types";
  import { cmdStore, cmdStateStore } from "../../stores";
  import { runCmd, toggleCmd } from "../../submit";
  import { onDestroy } from "svelte";

  export let detail = null;
  export let back;

  const unsubscribe = cmdStateStore.subscribe((data) => {
    if (data && detail && data.id === detail.id) {
      detail = { ...detail, ...data };
    }
  });

  onDestroy(() => unsubscribe);
</script>

{#if detail}
  <div class="rounded-md bg-slate-200 text-slate-900 shadow-lg">
    <div
      class="grid grid-cols-3 items-center border-b border-solid border-amber-900"
    >
      <div class="flex">
        <button
          class="p-1.5 rounded-tl-md bg-amber-700 hover:bg-amber-600 active:scale-90"
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
          on:click={async () =>
            detail.type === CommandType.func
              ? await runCmd(detail, null)
              : await toggleCmd(detail)}
        >
          {#if detail.unsub}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              stroke-width="1.5"
              stroke="currentColor"
              class="w-6 h-6 text-red-600"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
              />
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M9 9.563C9 9.252 9.252 9 9.563 9h4.874c.311 0 .563.252.563.563v4.874c0 .311-.252.563-.563.563H9.564A.562.562 0 019 14.437V9.564z"
              />
            </svg>
          {:else}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              stroke-width="1.5"
              stroke="currentColor"
              class="w-6 h-6 text-green-600"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
              />
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M15.91 11.672a.375.375 0 010 .656l-5.603 3.113a.375.375 0 01-.557-.328V8.887c0-.286.307-.466.557-.327l5.603 3.112z"
              />
            </svg>
          {/if}
        </button>
      </div>
      <div class="text-center font-semibold">{detail.id}</div>
      <div class="has-tooltip ml-auto">
        <button
          class="rounded-tr-md p-1.5 hover:bg-slate-100 active:scale-90"
          data-bs-target="tooltip"
          title="Delete"
          on:click={() => {
            cmdStore.delete(detail.id);
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
    </div>
    <div class="flex flex-col mx-6 p-2">
      <div class="bg-red-500 text-slate-200">{detail.error ?? ""}</div>
    </div>
    <div class="flex flex-col mx-6 my-2 p-2 bg-slate-900 text-green-400">
      <pre class="max-h-36 overflow-auto"><code class="whitespace-pre-wrap"
          >{@html Prism.highlight(detail.source, celincm, "CelinSL")}</code
        ></pre>
    </div>
  </div>
{/if}
