<script>
  import { onDestroy } from "svelte";
  import { fade } from "svelte/transition";
  import { stateStore, serversStore } from "../stores";

  let username = "";
  let password = "";

  const unsubscibe = serversStore.subscribe((servers) => username =
      servers?.length > 0 ? servers[serversStore.server]?.authResponse?.username : username);

  const ok = () => {
    stateStore.busy(true);
    global.blazorLib.invokeMethodAsync("Authenticate", username, password);
  };
  const cancel = () => stateStore.login(false);

  onDestroy(() => {
    unsubscibe();
  });
</script>

{#if $stateStore.login}
  <div
    transition:fade
    class="bg-opacity-70 bg-slate-900  z-40 fixed top-0 h-full w-full"
  >
    <form
      class="relative top-1/4 mx-auto max-w-xs border-4 border-l-slate-600 border-t-slate-600 border-r-slate-400 border-b-slate-400"
      on:submit|preventDefault={() => ok()}
    >
      <div class="px-8">
        <div class="flex place-content-center py-4 text-slate-300">
          <p class="text-xl font-semibold">{serversStore.server}</p>
        </div>
        <div class="pb-4">
          <!-- svelte-ignore a11y-autofocus -->
          <input
            autofocus
            required
            disabled={$stateStore.busy}
            class="border rounded w-full py-2 px-3"
            type="text"
            placeholder="User Name"
            bind:value={username}
          />
        </div>
        <div class="mb-2">
          <input
            class="border border-red rounded w-full py-2 px-3"
            type="password"
            required
            disabled={$stateStore.busy}
            placeholder="Password"
            bind:value={password}
          />
        </div>
      </div>
      <div class="px-8 py-5">
        <div class="flex items-center place-content-between">
          <button
            class="transform active:scale-95 hover:ring py-2 px-4 bg-green-100"
            type="submit"
            disabled={$stateStore.busy}
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5"
              viewBox="0 0 20 20"
              fill="currentColor"
            >
              <path
                fill-rule="evenodd"
                d="M3 3a1 1 0 011 1v12a1 1 0 11-2 0V4a1 1 0 011-1zm7.707 3.293a1 1 0 010 1.414L9.414 9H17a1 1 0 110 2H9.414l1.293 1.293a1 1 0 01-1.414 1.414l-3-3a1 1 0 010-1.414l3-3a1 1 0 011.414 0z"
                clip-rule="evenodd"
              />
            </svg>
          </button>
          <button
            class="transform active:scale-95 hover:ring py-2 px-4 bg-red-100"
            type="button"
            disable={$stateStore.busy}
            on:click={() => cancel()}
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5"
              viewBox="0 0 20 20"
              fill="currentColor"
            >
              <path
                fill-rule="evenodd"
                d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
                clip-rule="evenodd"
              />
            </svg>
          </button>
        </div>
      </div>
    </form>
  </div>
{/if}
