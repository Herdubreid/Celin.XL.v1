<script>
  import { fade } from "svelte/transition";
  import { CommandType } from "../../types"
  import { cmdStore } from "../../stores";
  import { runCmd, toggleCmd } from "../../submit";
  import CmdDetail from "./CmdDetail.svelte";

  let detail = null;
  const back = () => (detail = null);
</script>

{#if detail}
  <div in:fade class="flex flex-col h-full w-full px-4 pb-2 overflow-auto">
    <CmdDetail {detail} {back} />
  </div>
{:else}
  <div in:fade class="flex flex-col h-full w-full px-4 pb-2 overflow-auto">
    <div
      class="mt-2 grid gap-2 grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4"
    >
      {#each $cmdStore as cmd, i (i)}
        <div class="rounded-md bg-slate-200 shadow-lg">
          <div class="flex h-full items-center justify-between">
            <div class="flex h-full flex-col justify-between">
              <button
                class="h-full rounded-tl-md rounded-bl-md p-1 bg-amber-700 hover:bg-amber-600 active:scale-90"
                on:click={() => (detail = cmd)}
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke-width="1.5"
                  stroke="currentColor"
                  class="w-6 h-6 text-slate-200"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M13.5 6H5.25A2.25 2.25 0 003 8.25v10.5A2.25 2.25 0 005.25 21h10.5A2.25 2.25 0 0018 18.75V10.5m-10.5 6L21 3m0 0h-5.25M21 3v5.25"
                  />
                </svg>
              </button>
            </div>
            <div class="flex flex-col w-full text-center">
              <div class="font-semibold">{cmd.id}</div>
              <div class="bg-red-500 text-slate-200">{cmd.error ?? ""}</div>
            </div>
            <div class="flex h-full flex-col">
              <button
                class="h-full rounded-tr-md rounded-br-md bg-slate-200 p-1 hover:bg-slate-100 active:scale-90"
                data-bs-target="tooltip"
                title="Run"
                on:click={async () =>
                  cmd.type === CommandType.func
                    ? await runCmd(cmd, null)
                    : await toggleCmd(cmd)}
              >
                {#if cmd.unsub}
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke-width="1.5"
                    stroke="currentColor"
                    class="w-6 h-6 text-red-600"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M9 9.563C9 9.252 9.252 9 9.563 9h4.874c.311 0 .563.252.563.563v4.874c0 .311-.252.563-.563.563H9.564A.562.562 0 019 14.437V9.564z"
                    />
                  </svg>
                {:else}
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke-width="1.5"
                    stroke="currentColor"
                    class="w-6 h-6 text-green-600"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M15.91 11.672a.375.375 0 010 .656l-5.603 3.113a.375.375 0 01-.557-.328V8.887c0-.286.307-.466.557-.327l5.603 3.112z"
                    />
                  </svg>
                {/if}
              </button>
            </div>
          </div>
        </div>
      {/each}
    </div>
  </div>
{/if}
