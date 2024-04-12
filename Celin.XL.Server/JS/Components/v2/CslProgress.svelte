<script lang="ts">
  import { onDestroy } from "svelte";
  import { fade } from "svelte/transition";
  import { tweened } from "svelte/motion";
  import { cubicOut } from "svelte/easing";
  import { cslProgressStore } from "../../stores";
  import type { ICsl } from "../../types";

  export let item: ICsl;
  let progress;

  const rate = tweened(0, {
    duration: 400,
    easing: cubicOut,
  });

  const unsubscribe = cslProgressStore.subscribe((p) => {
    progress = p.find((e) => e.id === item.id);
    rate.set(progress && progress.of ? (progress.row + 1) / progress.of : 0);
  });

  onDestroy(() => unsubscribe());
</script>

<progress class="h-1 w-full" value={$rate} />
<div class="w-0 text-sm font-light whitespace-nowrap">
  {#if progress}
    {#each progress.msgs as msg, i (i)}
      <span in:fade class="mx-2">{msg}</span>
    {/each}
  {/if}
</div>
{#if progress?.errors}
  <div class="text-sm text-center font-bold bg-red-400 text-slate-200" transition:fade>
    {progress.errors} Error(s)
  </div>
{/if}

<style>
  progress {
    background-color: rgb(226 232 240 / var(--tw-bg-opacity));
    color: #0f766e;
  }
  /* background: */
  progress::-webkit-progress-bar {
    background-color: rgb(226 232 240 / var(--tw-bg-opacity));
    width: 100%;
  }
  /* value: */
  progress::-webkit-progress-value {
    background-color: #0f766e !important;
  }
  progress::-moz-progress-bar {
    background-color: #0f766e !important;
  }
</style>
