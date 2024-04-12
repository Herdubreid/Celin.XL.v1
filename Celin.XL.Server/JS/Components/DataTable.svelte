<script>
  import { onDestroy } from "svelte";
  import { fade } from "svelte/transition";
  import { createTable, deleteTable, insertData } from "../excel";
  import { stateStore, cqlStore } from "../stores";

  const insertOptions = ["Insert", "Append", "Replace"];
  const transposeOptions = ["None", "Delta"];
  let autoUpdate;
  let withMenu;
  let insertOption;
  let transposeOption;
  let detail;

  $: detail
    ? cqlStore.edit({
        ...detail,
        autoUpdate,
        withMenu,
        insertOption,
        transposeOption,
      })
    : null;
  $: detail = detail
    ? { ...detail, autoUpdate, withMenu, insertOption, transposeOption }
    : null;

  const unsubscribe = stateStore.subscribe((state) => {
    detail = state.table ? cqlStore.get(state.table) : null;
    autoUpdate = detail?.autoUpdate ?? false;
    withMenu = detail?.withMenu ?? false;
    insertOption = detail?.insertOption ?? 0;
    transposeOption = detail?.transposeOption ?? 0;
  });

  onDestroy(() => unsubscribe());
</script>

{#if detail}
  <div
    transition:fade
    class="absolute z-20 left-10 top-0.5 pb-4 mr-4 rounded bg-gray-400"
  >
    <div class="absolute -left-1 top-11 bg-gray-400 h-4 w-4 rotate-45" />
    <div class="flex flex-row">
      <div class="has-tooltip">
        <button
          class="p-1.5 z-10 rounded-md hover:bg-gray-300 active:scale-90"
          on:click={() => insertData(detail)}
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-5 w-5"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fill-rule="evenodd"
              d="M11.3 1.046A1 1 0 0112 2v5h4a1 1 0 01.82 1.573l-7 10A1 1 0 018 18v-5H4a1 1 0 01-.82-1.573l7-10a1 1 0 011.12-.38z"
              clip-rule="evenodd"
            />
          </svg>
        </button>
        <span class="tooltip ml-10 -mt-4">Go</span>
      </div>
      <div class="has-tooltip">
        <button
          class="p-1.5 rounded-md hover:bg-gray-300 active:scale-90"
          on:click={() => createTable(detail)}
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-5 w-5"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fill-rule="evenodd"
              d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-11a1 1 0 10-2 0v2H7a1 1 0 100 2h2v2a1 1 0 102 0v-2h2a1 1 0 100-2h-2V7z"
              clip-rule="evenodd"
            />
          </svg>
        </button>
        <span class="tooltip ml-10 -mt-4">Create Table</span>
      </div>
      <div class="has-tooltip">
        <button
          class="p-1.5 rounded-md hover:bg-gray-300 active:scale-90"
          on:click={() => deleteTable(detail)}
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-5 w-5"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fill-rule="evenodd"
              d="M10 18a8 8 0 100-16 8 8 0 000 16zM7 9a1 1 0 000 2h6a1 1 0 100-2H7z"
              clip-rule="evenodd"
            />
          </svg>
        </button>
        <span class="tooltip ml-10 -mt-4">Delete Table</span>
      </div>
    </div>
    <div class="text-sm text-teal-900">
      <div class="flex flex-row px-2 py-2">
        <label class="mx-2">
          <input type="checkbox" class="mx-1" bind:checked={autoUpdate} />
          Auto Update
        </label>
        <label class="mx-2">
          <input type="checkbox" class="mx-1" bind:checked={withMenu} />
          With Menu
        </label>
      </div>
      <div class="px-4 py-2 font-semibold">Update Options:</div>
      <div class="flex flex-row px-2">
        {#each insertOptions as option, i}
          <label class="mx-2">
            <input
              type="radio"
              class="mx-1"
              bind:group={insertOption}
              value={i}
            />
            {option}
          </label>
        {/each}
      </div>
      <div class="px-4 py-2 font-semibold">Transpose Options:</div>
      <div class="flex flex-row px-2">
        {#each transposeOptions as option, i}
          <label class="mx-2">
            <input
              type="radio"
              class="mx-1"
              bind:group={transposeOption}
              value={i}
            />
            {option}
          </label>
        {/each}
      </div>
    </div>
  </div>
{/if}
