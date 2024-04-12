<script>
  import { fade } from "svelte/transition";
  import { cslStore } from "../../stores";
  import { submitScript } from "../../submit";
  import CslDetail from "./CslDetail.svelte";
  import CslProgress from "./CslProgress.svelte";

  let detail = null;

  const back = () => {
    detail = null;
  };
</script>

<div class="flex flex-col h-full w-full px-4 pb-2 overflow-auto">
  <CslDetail {detail} {back} />
  <div
    in:fade
    class:grid={!detail}
    class:hidden={detail}
    class="mt-2 gap-2 grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4"
  >
    {#each $cslStore as item, i (i)}
      <div class="rounded-md bg-slate-200 shadow-lg">
        <div class="flex h-full items-center justify-between">
          <div class="flex h-full flex-col justify-between">
            <button
              class="h-full rounded-tl-md rounded-bl-md p-1 bg-teal-700 hover:bg-teal-600 active:scale-90"
              on:click={() => {
                detail = item;
              }}
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                stroke-width="1.5"
                stroke="currentColor"
                class="w-6 h-6 text-slate-200"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M13.5 6H5.25A2.25 2.25 0 003 8.25v10.5A2.25 2.25 0 005.25 21h10.5A2.25 2.25 0 0018 18.75V10.5m-10.5 6L21 3m0 0h-5.25M21 3v5.25"
                />
              </svg>
            </button>
          </div>
          <div class="flex flex-col w-full text-center">
            <div class="font-semibold">{item.id}</div>
            <CslProgress {item} />
            <div class="bg-red-500 text-slate-200">{item.error ?? ""}</div>
          </div>
          <div class="flex h-full flex-col">
            <button
              class="h-full rounded-tr-md rounded-br-md bg-slate-200 p-1 hover:bg-slate-100 active:scale-90"
              data-bs-target="tooltip"
              title="Run"
              on:click={() =>
                item.busy
                  ? global.blazorLib.invokeMethodAsync("CancelScript", item.id)
                  : submitScript(item.template ?? item.source)}
            >
              {#if item.busy}
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
          </div>
        </div>
      </div>
    {/each}
  </div>
</div>
