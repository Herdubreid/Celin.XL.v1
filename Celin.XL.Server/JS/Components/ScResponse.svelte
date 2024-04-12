<script lang="ts">
  import type { ICslResponse } from "../types";
  import { pasteGrid, pasteSpecs } from "../excel";

  export let detail: ICslResponse;

  const paste = async (option) => {
    const t = await global.blazorLib.invokeMethodAsync("GetScriptOption", detail.id, option);
    switch (option) {
      case 0:
        pasteSpecs(t);
        break;
      case 1:
        pasteGrid(t);
        break;
    }
  };
</script>

<div class="flex flex-row items-center justify-between ml-2 mr-0.5 text-sm">
  <div class="flex flex-row px-2">
    {#if detail.error}
      <div class="bg-red-400 px-2 text-stone-300">
        <pre>{detail.error}</pre>
      </div>
    {:else}
      <div class="text-green-600 mx-2">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-5 w-5"
          viewBox="0 0 20 20"
          fill="currentColor"
        >
          <path
            fill-rule="evenodd"
            d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
            clip-rule="evenodd"
          />
        </svg>
      </div>
    {/if}
    {detail.msg ?? ""}
  </div>
  {#if detail !== null}
    <div class="has-tooltip">
      <button
        class="p-1.5rounded-md hover:bg-gray-300 mx-2"
        on:click={() => paste(detail)}
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
      <span class="tooltip text-teal-800 mt-2">Paste results</span>
    </div>
  {/if}
</div>
