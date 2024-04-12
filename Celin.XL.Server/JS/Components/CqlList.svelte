<script>
  import { onDestroy } from "svelte";
  import { flip } from "svelte/animate";
  import { stateStore, cqlStore, queryStore } from "../stores";
  import { pasteData } from "../excel";
  import { submitQuery } from "../submit";
  import CqlDetail from "./CqlDetail.svelte";
  import DataInfo from "./DataInfo.svelte";
  import DataTable from "./DataTable.svelte";

  let selected;
  let info;
  let table;

  let enabled;
  let disabled;

  const unsubscribe = stateStore.subscribe((state) => {
    selected = state.selected;
    info = state.info;
    table = state.table;
    disabled = state.selected === null;
    enabled = !disabled;
  });

  onDestroy(() => unsubscribe());
</script>

<div class="relative h-full flex flex-row">
  <DataInfo />
  <DataTable />
  <div class="sticky flex flex-col h-fit w-8 bg-gray-400 rounded-r-md">
    <div class="has-tooltip">
      <button
        class="p-1.5 rounded-tr-md hover:bg-gray-300"
        {disabled}
        class:enabled
        class:disabled
        on:click={async () => pasteData(await cqlStore.get(selected))}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-5 w-5"
          viewBox="0 0 20 20"
          fill="currentColor"
        >
          <path d="M8 2a1 1 0 000 2h2a1 1 0 100-2H8z" />
          <path
            d="M3 5a2 2 0 012-2 3 3 0 003 3h2a3 3 0 003-3 2 2 0 012 2v6h-4.586l1.293-1.293a1 1 0 00-1.414-1.414l-3 3a1 1 0 000 1.414l3 3a1 1 0 001.414-1.414L10.414 13H15v3a2 2 0 01-2 2H5a2 2 0 01-2-2V5zM15 11h2a1 1 0 110 2h-2v-2z"
          />
        </svg>
      </button>
      <span class="tooltip ml-10 -mt-6" class:disabled>Paste results</span>
    </div>
    <div class="has-tooltip h-9">
      <button
        class="p-1.5 my-1 hover:bg-gray-300"
        {disabled}
        class:enabled
        class:disabled
        class:selected={table !== null}
        on:click={() => stateStore.table()}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-5 w-5"
          viewBox="0 0 20 20"
          fill="currentColor"
        >
          <path
            fill-rule="evenodd"
            d="M5 4a3 3 0 00-3 3v6a3 3 0 003 3h10a3 3 0 003-3V7a3 3 0 00-3-3H5zm-1 9v-1h5v2H5a1 1 0 01-1-1zm7 1h4a1 1 0 001-1v-1h-5v2zm0-4h5V8h-5v2zM9 8H4v2h5V8z"
            clip-rule="evenodd"
          />
        </svg>
      </button>
      <span class="tooltip ml-9 -mt-6" class:disabled>Table Options</span>
    </div>
    <div class="has-tooltip h-10">
      <button
        class="p-1.5 my-1 hover:bg-gray-300"
        {disabled}
        class:enabled
        class:disabled
        class:selected={info !== null}
        on:click={() => stateStore.info()}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-5 w-5"
          viewBox="0 0 20 20"
          fill="currentColor"
        >
          <path
            fill-rule="evenodd"
            d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
            clip-rule="evenodd"
          />
        </svg>
      </button>
      <span class="tooltip ml-10 -mt-6" class:disabled>Details</span>
    </div>
    <hr class="border-slate-500 ml-2 mr-2" />
    <div class="has-tooltip">
      <button
        class="p-1.5 my-1 hover:bg-gray-300 active:animate-pulse"
        {disabled}
        class:enabled
        class:disabled
        on:click={() => {
          const d = cqlStore.get(selected);
          submitQuery(d.template ?? d.query, d.id);
        }}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-5 w-5"
          viewBox="0 0 20 20"
          fill="currentColor"
        >
          <path
            fill-rule="evenodd"
            d="M4 2a1 1 0 011 1v2.101a7.002 7.002 0 0111.601 2.566 1 1 0 11-1.885.666A5.002 5.002 0 005.999 7H9a1 1 0 010 2H4a1 1 0 01-1-1V3a1 1 0 011-1zm.008 9.057a1 1 0 011.276.61A5.002 5.002 0 0014.001 13H11a1 1 0 110-2h5a1 1 0 011 1v5a1 1 0 11-2 0v-2.101a7.002 7.002 0 01-11.601-2.566 1 1 0 01.61-1.276z"
            clip-rule="evenodd"
          />
        </svg>
      </button>
      <span class="tooltip ml-10 -mt-6" class:disabled>Re-submit query</span>
    </div>
    <div class="has-tooltip">
      <button
        class="p-1.5 my-1 hover:bg-gray-300 active:animate-pulse"
        {disabled}
        class:enabled
        class:disabled
        on:click={() => {
          const d = cqlStore.get(selected);
          queryStore.append(d.template ?? d.query);
        }}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-5 w-5"
          viewBox="0 0 20 20"
          fill="currentColor"
        >
          <path
            d="M17.414 2.586a2 2 0 00-2.828 0L7 10.172V13h2.828l7.586-7.586a2 2 0 000-2.828z"
          />
          <path
            fill-rule="evenodd"
            d="M2 6a2 2 0 012-2h4a1 1 0 010 2H4v10h10v-4a1 1 0 112 0v4a2 2 0 01-2 2H4a2 2 0 01-2-2V6z"
            clip-rule="evenodd"
          />
        </svg>
      </button>
      <span class="tooltip ml-10 -mt-6" class:disabled>Edit query</span>
    </div>
    <hr class="border-slate-500 ml-2 mr-2" />
    <div class="has-tooltip">
      <button
        class="p-1.5 mt-1 rounded-br-md hover:bg-gray-300"
        {disabled}
        class:enabled
        class:disabled
        on:click={() => {
          cqlStore.delete(selected);
          stateStore.selected(null);
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
      <span class="tooltip ml-10 -mt-6" class:disabled>Delete results</span>
    </div>
  </div>
  <div class="flex flex-wrap h-full w-full p-2 overflow-auto">
    {#each $cqlStore as detail, i (i)}
      <div animate:flip>
        <CqlDetail {detail} />
      </div>
    {/each}
  </div>
</div>
