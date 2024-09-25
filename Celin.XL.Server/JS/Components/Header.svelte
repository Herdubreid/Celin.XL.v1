<script>
  import { fade } from "svelte/transition";
  import { serversStore, stateStore } from "../stores";
    import Server from "./Server.svelte";
    import { onDestroy } from "svelte";

  export let version;
  export let menu;
  export let openMenu = false;
  export let currentTab;
  export let setTab;

  let openServers = false;
  let serverOption = 0;
  let options = [];

  fetch("./assets/data/menu.json")
    .then((response) => response.json())
    .then((json) => (options = json));

  const callMenu = (option) => {
    openMenu = false;
    menu(option);
  };

  const unsubscibe = stateStore.subscribe(state => console.log(state));

  onDestroy(() => {
    unsubscibe();
  });
</script>

{#if openMenu}
  <!-- svelte-ignore a11y-no-static-element-interactions -->
  <div
    transition:fade
    class="peer absolute top-10 z-50"
    on:mouseleave={() => (openMenu = false)}
  >
    <div
      class="mt-1 bg-slate-400 rounded-br-md rounded-tr-md border-slate-600 border-l-2 border-solid"
    >
      {#each options[currentTab] as m}
        {#if m.command === "-"}
          <hr class="border-slate-500 my-1 ml-2 mr-2" />
        {:else}
          <button
            class="flex justify-between rounded w-full text-left py-2 px-4 outline-none hover:bg-slate-300 overflow-hidden font-semibold"
            on:click={() => callMenu(m.command)}
          >
            <span>{m.title}</span>
            <em class="pl-4 text-sm">Alt-{m.command}</em>
          </button>
        {/if}
      {/each}
    </div>
  </div>
{/if}

<div
  class="sticky top-0 z-40 flex flex-row items-center text-teal-900 bg-slate-400 overflow-hidden"
>
  <div class="w-32">
    <button
      class="flex flex-row font-semibold peer-hover:bg-slate-300 hover:bg-slate-300 py-2 h-10 rounded-tr-md box-border"
      class:selected={openMenu}
      on:click={() => (openMenu = !openMenu)}
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="h-6 w-6"
        fill="none"
        viewBox="0 0 24 24"
        stroke="currentColor"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M12 5v.01M12 12v.01M12 19v.01M12 6a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2z"
        />
      </svg>
    </button>
  </div>
  <!-- Tabs -->
  <div class="inline-flex">
    <button
      class="px-4 font-semibold py-2 rounded-t"
      class:active-tab={currentTab === 0}
      class:inactive-tab={currentTab !== 0}
      on:click={() => setTab(0)}>CQL</button
    >
    <button
      class="px-4 font-semibold py-2 rounded-t"
      class:active-tab={currentTab === 1}
      class:inactive-tab={currentTab !== 1}
      on:click={() => setTab(1)}>CSL</button
    >
    <button
      class="px-4 font-semibold py-2 rounded-t"
      class:active-tab={currentTab === 2}
      class:inactive-tab={currentTab !== 2}
      on:click={() => setTab(2)}>CMD</button
    >
  </div>
  <!-- svelte-ignore a11y-no-static-element-interactions -->
  <div on:mouseleave={() => (openServers = false)}>
    {#if openServers}
      <button
        transition:fade
        class="peer fixed top-10 w-36"
        on:click={() => (openServers = false)}
      >
        <div
          class="mt-1 bg-slate-400 rounded-b-md rounded-tr-md border-slate-600 border-l-2 border-solid"
        >
          {#each ($serversStore ?? []).filter((e) => e.id !== $stateStore.contextId) as s}
            <button
              on:click={() => {
                global.blazorLib.invokeMethodAsync("SelectContext", s.id);
                stateStore.context(s.id);
              }}
              class="rounded block w-full text-left py-2 pl-4 pr-2 outline-none hover:bg-slate-300 overflow-hidden font-semibold"
              >{s.name}</button
            >
          {/each}
        </div>
      </button>
    {/if}
    <button
      class="flex flex-row font-semibold peer-hover:bg-slate-300 hover:bg-slate-300 w-auto p-2 h-10 rounded-tr-md box-border"
      class:selected={openServers}
      on:click={() => (openServers = !openServers)}
      disabled={$stateStore.lockContext}
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="h-6 w-6"
        fill="none"
        viewBox="0 0 24 24"
        stroke="currentColor"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M5 12h14M5 12a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v4a2 2 0 01-2 2M5 12a2 2 0 00-2 2v4a2 2 0 002 2h14a2 2 0 002-2v-4a2 2 0 00-2-2m-2-4h.01M17 16h.01"
        />
      </svg>
      <span class="text-nowrap ml-1"
        >{($serversStore ?? []).find((s) => s.id === $stateStore.contextId)?.name ??
          ""}</span
      >
    </button>
  </div>
  <div class="w-full font-thin mr-6 text-right">{version}</div>
</div>
