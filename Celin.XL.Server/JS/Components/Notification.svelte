<script lang="ts">
  import { onDestroy } from "svelte";

  import { fade } from "svelte/transition";
  import { stateStore } from "../stores";

  let timeout;

  const unsubscribe = stateStore.subscribe((state) => {
    clearTimeout(timeout);
    if (state.notify) {
      timeout = setTimeout(() => {
        stateStore.clear();
      }, state.timeout ?? 10000);
    }
  });

  onDestroy(() => unsubscribe());
</script>

{#if $stateStore.notify}
  <div
    transition:fade
    class="absolute flex z-40 max-w-md top-1/3 left-auto right-0 mr-4 justify-between bg-white dark:bg-gray-800 shadow-lg rounded"
  >
    <button
      class="absolute top-1 right-1 border-0 text-gray-500 dark:text-gray-100 dark:hover:text-gray-400 hover:text-gray-600 transition duration-150 ease-in-out cursor-pointer focus:ring-2 focus:outline-none focus:ring-gray-500 rounded"
      on:click={() => stateStore.clear()}
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="icon icon-tabler icon-tabler-x"
        width="20"
        height="20"
        viewBox="0 0 24 24"
        stroke-width="2.5"
        stroke="currentColor"
        fill="none"
        stroke-linecap="round"
        stroke-linejoin="round"
      >
        <path stroke="none" d="M0 0h24v24H0z" />
        <line x1="18" y1="6" x2="6" y2="18" />
        <line x1="6" y1="6" x2="18" y2="18" />
      </svg>
    </button>
    <div
      class="p-2 flex items-center justify-center bg-red-600 rounded-tl rounded-bl h-auto w-auto text-white"
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="h-6 w-6"
        viewBox="0 0 20 20"
        fill="currentColor"
      >
        <path
          fill-rule="evenodd"
          d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z"
          clip-rule="evenodd"
        />
      </svg>
    </div>
    <div class="flex-col justify-center p-2 mr-6">
      <h1 class="text-lg text-gray-800 dark:text-gray-100 font-semibold pb-1">
        {$stateStore.notify.title}
      </h1>
      <p class="text-sm text-gray-600 dark:text-gray-400 font-normal">
        {$stateStore.notify.details}
      </p>
    </div>
  </div>
{/if}
