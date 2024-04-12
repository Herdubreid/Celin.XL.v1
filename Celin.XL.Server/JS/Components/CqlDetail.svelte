<script>
  import { onDestroy } from "svelte";
  import { stateStore } from "../stores";

  export let detail;
  let selected;

  const unsubscribe = stateStore.subscribe(
    (state) => (selected = state.selected === detail.id)
  );

  onDestroy(() => unsubscribe());
</script>

<div>
  <button
    class="flex flex-row ml-2 mt-2 p-2 mr-0.5 mb-0.5 rounded-md"
    class:bg-slate-700={selected}
    class:selected
    class:mr-0.5={!selected}
    class:mb-0.5={!selected}
    on:click={() => stateStore.selected(detail.id)}
  >
    {#if detail.busy}
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="h-5 w-5 animate-spin text-green-600"
        viewBox="0 0 20 20"
        fill="currentColor"
      >
        <path
          fill-rule="evenodd"
          d="M11.49 3.17c-.38-1.56-2.6-1.56-2.98 0a1.532 1.532 0 01-2.286.948c-1.372-.836-2.942.734-2.106 2.106.54.886.061 2.042-.947 2.287-1.561.379-1.561 2.6 0 2.978a1.532 1.532 0 01.947 2.287c-.836 1.372.734 2.942 2.106 2.106a1.532 1.532 0 012.287.947c.379 1.561 2.6 1.561 2.978 0a1.533 1.533 0 012.287-.947c1.372.836 2.942-.734 2.106-2.106a1.533 1.533 0 01.947-2.287c1.561-.379 1.561-2.6 0-2.978a1.532 1.532 0 01-.947-2.287c.836-1.372-.734-2.942-2.106-2.106a1.532 1.532 0 01-2.287-.947zM10 13a3 3 0 100-6 3 3 0 000 6z"
          clip-rule="evenodd"
        />
      </svg>
    {:else if detail.summary.records === 0}
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="h-5 w-5"
        class:error={detail.error != null}
        viewBox="0 0 20 20"
        fill="currentColor"
      >
        <path
          fill-rule="evenodd"
          d="M13.477 14.89A6 6 0 015.11 6.524l8.367 8.368zm1.414-1.414L6.524 5.11a6 6 0 018.367 8.367zM18 10a8 8 0 11-16 0 8 8 0 0116 0z"
          clip-rule="evenodd"
        />
      </svg>
    {:else}
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="h-5 w-5 text-yellow-600"
        viewBox="0 0 20 20"
        fill="currentColor"
      >
        <path
          d="M2 6a2 2 0 012-2h5l2 2h5a2 2 0 012 2v6a2 2 0 01-2 2H4a2 2 0 01-2-2V6z"
        />
      </svg>
    {/if}
    <div class="text-sm text-left px-2 w-36">
      {detail.error ?? detail.id ?? "Working..."}
    </div>
  </button>
</div>
