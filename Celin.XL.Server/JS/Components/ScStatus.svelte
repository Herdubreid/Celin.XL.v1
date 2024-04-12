<script lang="ts">
  import { onDestroy } from "svelte";
  import { fade } from "svelte/transition";
  import { cslProgressStore, cslResponseStore } from "../stores";

  let history = null;
  let progress = history
  ? 0
  : `${Math.round(((history.row + 1) * 100.0) / history.of)}%`;

  const unsubscribe = cslProgressStore.subscribe((s) => {
  });

  const cancel = () => {
    global.blazorLib.invokeMethodAsync("CancelScript");
  };
  const clear = () => {
    cslResponseStore.clear();
    global.blazorLib.invokeMethodAsync("ClearScriptOutput");
  };

  onDestroy(() => unsubscribe());
</script>

<!--
  
-->

<div class="sticky bg-gray-400 mx-4 rounded-bl-md">
  <div class="flex flex-row w-full fixed h-9">
    {#if history}
      <div
        class="h-9 bg-gray-600 rounded-bl opacity-30"
        style:width={progress}
      />
      <div class="ml-2 mt-5 text-xs font-extralight fixed right-5">
        {progress}
      </div>
    {/if}
  </div>
  <div class="flex flex-row w-full items-center h-9">
    <div class="has-tooltip z-10">
      <button
        class="p-1.5 rounded-bl hover:bg-gray-300 active:animate-pulse"
        on:click={() => clear()}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-5 w-5 text-yellow-600"
          viewBox="0 0 20 20"
          fill="currentColor"
        >
          <path
            fill-rule="evenodd"
            d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
            clip-rule="evenodd"
          />
        </svg>
      </button>
      <span class="tooltip ml-4 mt-1">Clear Output</span>
    </div>
    {#if history}
      <div class="has-tooltip">
        <button
          class="p-1.5 hover:bg-gray-300 active:animate-pulse"
          on:click={() => cancel()}
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-5 w-5 text-red-600 animate-spin"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fill-rule="evenodd"
              d="M11.49 3.17c-.38-1.56-2.6-1.56-2.98 0a1.532 1.532 0 01-2.286.948c-1.372-.836-2.942.734-2.106 2.106.54.886.061 2.042-.947 2.287-1.561.379-1.561 2.6 0 2.978a1.532 1.532 0 01.947 2.287c-.836 1.372.734 2.942 2.106 2.106a1.532 1.532 0 012.287.947c.379 1.561 2.6 1.561 2.978 0a1.533 1.533 0 012.287-.947c1.372.836 2.942-.734 2.106-2.106a1.533 1.533 0 01.947-2.287c1.561-.379 1.561-2.6 0-2.978a1.532 1.532 0 01-.947-2.287c.836-1.372-.734-2.942-2.106-2.106a1.532 1.532 0 01-2.287-.947zM10 13a3 3 0 100-6 3 3 0 000 6z"
              clip-rule="evenodd"
            />
          </svg>
        </button>
        <span class="tooltip ml-4 mt-1">Cancel Script</span>
      </div>
      <div class="flex flex-row items-center" in:fade>
        {#each history.msgs as status, i (i)}
          <span class="font-light text-sm mx-2">{status}</span>
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-5 w-5"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fill-rule="evenodd"
              d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z"
              clip-rule="evenodd"
            />
          </svg>
        {/each}
        <span class="font-light text-sm">...</span>
      </div>
    {:else}
      <span class="font-light italic mx-2" in:fade>Ready...</span>
    {/if}
  </div>
</div>
