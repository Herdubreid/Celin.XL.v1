<script>
  import { stateStore, serversStore } from "../stores";
  import { fade } from "svelte/transition";

  let server;

  const connect = async () => {
    stateStore.busy(true);
    stateStore.clear();
    const baseUrl = `${location.protocol}//${server}/jderest/v2/`;
    try {
      const rs = await fetch(`${baseUrl}defaultconfig`);
      const json = await rs.json();
      serversStore.set([
        {
          id: 0,
          name: json.defaultEnvironment,
          baseUrl,
        },
      ]);
    } catch (ex) {
      stateStore.error("Connection failed!", ex.message);
    }
  };
</script>

{#if $stateStore.server}
  <div
    transition:fade
    class="bg-opacity-70 bg-slate-900 z-30 fixed top-0 h-full w-full"
  >
    <form
      class="relative top-1/4 mx-auto max-w-md border-4 border-l-slate-600 border-t-slate-600 border-r-slate-400 border-b-slate-400"
      on:submit|preventDefault={() => connect()}
    >
      <div class="flex flex-col px-8 text-slate-300">
        <p class="text-lg text-center font-semibold py-2">Welcome to CelinXL Add-Ins</p>
        <p class="py-2">
          To start using the Add-Ins, enter your AIS Server Url.
        </p>
        <p class="font-thin">
          AIS Server:
        </p>
        <input
          required
          disabled={$stateStore.busy}
          class="border rounded w-full py-2 px-3 text-slate-700"
          type="text"
          placeholder="<host><optional :port>"
          bind:value={server}
        />
        <p class="text-sm">Try demo.steltix.com for testing</p>
        <div class="flex flex-row place-content-center py-6">
          <button
            class="transform active:scale-95 hover:ring py-2 px-4 bg-green-100"
            disabled={$stateStore.busy}
            type="submit"
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
          </button>
        </div>
      </div>
    </form>
  </div>
{/if}
