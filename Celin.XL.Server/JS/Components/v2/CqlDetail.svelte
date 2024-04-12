<script>
  import { fade } from "svelte/transition";
  import Prism from "prismjs/prism";
  import { celinql } from "../../prism-celinql";
  import { cqlStore, cqlStateStore } from "../../stores";
  import { createTable, deleteTable, insertData } from "../../excel";
  import { submitQuery } from "../../submit";
  import { onDestroy } from "svelte";

  const insertOptions = ["Insert", "Append", "Replace"];
  export let detail = false;
  export let back;

  const unsubscribe = cqlStateStore.subscribe((data) => {
    if (data && detail && data.id === detail.id) {
      detail = { ...detail, ...data };
    }
  });

  const since = (dt) => {
    const m = (Date.now() - dt) / (1000 * 60);
    if (m > 60) {
      const h = m / 60;
      if (h > 24) {
        const d = h / 24;
        if (d > 30) return dt.toDateString();
        return `${Math.round(d, 0)} days ago`;
      }
      return `${Math.round(h, 0)} hours ago`;
    }
    if (m < 1) return "Just now";
    return `${Math.round(m, 0)} minute${m < 2 ? "" : "s"} ago`;
  };
  let timeSince;
  const timer = setInterval(
    () => (timeSince = since(new Date(detail.submitted))),
    1000,
  );

  const summary = (records) =>
    records > 0 ? (records > 1 ? `${records} rows` : "1 row") : "No data";

  onDestroy(() => {
    clearInterval(timer);
    unsubscribe();
  });
</script>

{#if detail}
  <div in:fade class="rounded-md bg-slate-200 text-slate-900 shadow-lg">
    <div
      class="grid grid-cols-3 items-center border-b border-solid border-teal-900"
    >
      <div class="flex">
        <button
          class="p-1.5 rounded-tl-md bg-blue-700 hover:bg-blue-600 active:scale-90"
          data-bs-target="tooltip"
          title="Back"
          on:click={() => {
            cqlStore.edit(detail);
            back();
          }}
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 20 20"
            fill="currentColor"
            class="w-5 h-5 text-slate-200"
          >
            <path
              fill-rule="evenodd"
              d="M7.793 2.232a.75.75 0 01-.025 1.06L3.622 7.25h10.003a5.375 5.375 0 010 10.75H10.75a.75.75 0 010-1.5h2.875a3.875 3.875 0 000-7.75H3.622l4.146 3.957a.75.75 0 01-1.036 1.085l-5.5-5.25a.75.75 0 010-1.085l5.5-5.25a.75.75 0 011.06.025z"
              clip-rule="evenodd"
            />
          </svg>
        </button>
        <button
          class="p-1.5 hover:bg-slate-100 active:scale-90"
          data-bs-target="tooltip"
          title="Refresh"
          on:click={() =>
            detail.busy
              ? global.blazorLib.invokeMethodAsync("CancelQuery", detail.id)
              : submitQuery(detail.template ?? detail.query, detail.id)}
        >
          {#if detail.busy}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              class="h-5 w-5 text-red-700 animate-spin"
            >
              <path
                fill-rule="evenodd"
                d="M8.34 1.804A1 1 0 019.32 1h1.36a1 1 0 01.98.804l.295 1.473c.497.144.971.342 1.416.587l1.25-.834a1 1 0 011.262.125l.962.962a1 1 0 01.125 1.262l-.834 1.25c.245.445.443.919.587 1.416l1.473.294a1 1 0 01.804.98v1.361a1 1 0 01-.804.98l-1.473.295a6.95 6.95 0 01-.587 1.416l.834 1.25a1 1 0 01-.125 1.262l-.962.962a1 1 0 01-1.262.125l-1.25-.834a6.953 6.953 0 01-1.416.587l-.294 1.473a1 1 0 01-.98.804H9.32a1 1 0 01-.98-.804l-.295-1.473a6.957 6.957 0 01-1.416-.587l-1.25.834a1 1 0 01-1.262-.125l-.962-.962a1 1 0 01-.125-1.262l.834-1.25a6.957 6.957 0 01-.587-1.416l-1.473-.294A1 1 0 011 10.68V9.32a1 1 0 01.804-.98l1.473-.295c.144-.497.342-.971.587-1.416l-.834-1.25a1 1 0 01.125-1.262l.962-.962A1 1 0 015.38 3.03l1.25.834a6.957 6.957 0 011.416-.587l.294-1.473zM13 10a3 3 0 11-6 0 3 3 0 016 0z"
                clip-rule="evenodd"
              />
            </svg>
          {:else}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              class="h-5 w-5 text-green-700"
            >
              <path
                fill-rule="evenodd"
                d="M15.312 11.424a5.5 5.5 0 01-9.201 2.466l-.312-.311h2.433a.75.75 0 000-1.5H3.989a.75.75 0 00-.75.75v4.242a.75.75 0 001.5 0v-2.43l.31.31a7 7 0 0011.712-3.138.75.75 0 00-1.449-.39zm1.23-3.723a.75.75 0 00.219-.53V2.929a.75.75 0 00-1.5 0V5.36l-.31-.31A7 7 0 003.239 8.188a.75.75 0 101.448.389A5.5 5.5 0 0113.89 6.11l.311.31h-2.432a.75.75 0 000 1.5h4.243a.75.75 0 00.53-.219z"
                clip-rule="evenodd"
              />
            </svg>
          {/if}
        </button>
      </div>
      <div class="text-center font-semibold">{detail.id}</div>
      <button
        class="rounded-tr-md ml-auto p-1.5 hover:bg-slate-100 active:scale-90"
        data-bs-target="tooltip"
        title="Delete"
        on:click={() => {
          cqlStore.delete(detail.id);
          back();
        }}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-5 w-5 text-red-800"
          viewBox="0 0 20 20"
          fill="currentColor"
        >
          <path
            fill-rule="evenodd"
            d="M9 2a1 1 0 00-.894.553L7.382 4H4a1 1 0 000 2v10a2 2 0 002 2h8a2 2 0 002-2V6a1 1 0 100-2h-3.382l-.724-1.447A1 1 0 0011 2H9zM7 8a1 1 0 012 0v6a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v6a1 1 0 102 0V8a1 1 0 00-1-1z"
            clip-rule="evenodd"
          />
        </svg>
      </button>
    </div>
    <div class="text-sm">
      <div class="w-full py-2 text-center text-lg">Table Options</div>
      <div class="border-b border-solid border-teal-900">
        <div class="flex flex-row items-center justify-center pt-2">
          <button
            class="p-1.5 rounded-md hover:bg-slate-100 active:scale-90"
            data-bs-target="tooltip"
            title="Update Table"
            on:click={() => insertData(detail)}
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              class="w-5 h-5"
            >
              <path
                fill-rule="evenodd"
                d="M3 4.25A2.25 2.25 0 015.25 2h5.5A2.25 2.25 0 0113 4.25v2a.75.75 0 01-1.5 0v-2a.75.75 0 00-.75-.75h-5.5a.75.75 0 00-.75.75v11.5c0 .414.336.75.75.75h5.5a.75.75 0 00.75-.75v-2a.75.75 0 011.5 0v2A2.25 2.25 0 0110.75 18h-5.5A2.25 2.25 0 013 15.75V4.25z"
                clip-rule="evenodd"
              />
              <path
                fill-rule="evenodd"
                d="M19 10a.75.75 0 00-.75-.75H8.704l1.048-.943a.75.75 0 10-1.004-1.114l-2.5 2.25a.75.75 0 000 1.114l2.5 2.25a.75.75 0 101.004-1.114l-1.048-.943h9.546A.75.75 0 0019 10z"
                clip-rule="evenodd"
              />
            </svg>
          </button>
          <button
            class="p-1.5 rounded-md hover:bg-slate-100 active:scale-90"
            data-bs-target="tooltip"
            title="Create Table"
            on:click={() => createTable(detail)}
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5"
              viewBox="0 0 512 512"
              ><!--! Font Awesome Pro 6.2.0 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2022 Fonticons, Inc. --><path
                d="M64 32C28.7 32 0 60.7 0 96V416c0 35.3 28.7 64 64 64H448c35.3 0 64-28.7 64-64V96c0-35.3-28.7-64-64-64H64zm88 64v64H64V96h88zm56 0h88v64H208V96zm240 0v64H360V96h88zM64 224h88v64H64V224zm232 0v64H208V224h88zm64 0h88v64H360V224zM152 352v64H64V352h88zm56 0h88v64H208V352zm240 0v64H360V352h88z"
              /></svg
            >
          </button>
          <button
            class="p-1.5 rounded-md hover:bg-slate-100 active:scale-90"
            data-bs-target="tooltip"
            title="Delete Table"
            on:click={() => deleteTable(detail)}
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5 text-red-800"
              viewBox="0 0 448 512"
              fill="currentColor"
              ><!--! Font Awesome Pro 6.2.0 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2022 Fonticons, Inc. --><path
                d="M135.2 17.7L128 32H32C14.3 32 0 46.3 0 64S14.3 96 32 96H416c17.7 0 32-14.3 32-32s-14.3-32-32-32H320l-7.2-14.3C307.4 6.8 296.3 0 284.2 0H163.8c-12.1 0-23.2 6.8-28.6 17.7zM416 128H32L53.2 467c1.6 25.3 22.6 45 47.9 45H346.9c25.3 0 46.3-19.7 47.9-45L416 128z"
              /></svg
            >
          </button>
        </div>
        <div class="flex flex-row justify-center py-2">
          <label class="mx-2">
            <input
              type="checkbox"
              class="mx-1"
              bind:checked={detail.withMenu}
            />
            With Menu Column
          </label>
          <label class="mx-2">
            <input
              type="checkbox"
              class="mx-1"
              bind:checked={detail.aliasHeader}
            />
            Alias Column Header
          </label>
        </div>
        <div class="flex flex-row justify-center py-2">
          <label class="mx-2">
            <input
              type="checkbox"
              class="mx-1"
              bind:checked={detail.autoUpdate}
            />
            Auto Update
          </label>
          {#each insertOptions as option, i}
            <label class="mx-2">
              <input
                type="radio"
                class="mx-1"
                bind:group={detail.insertOption}
                value={i}
              />
              {option}
            </label>
          {/each}
        </div>
      </div>
      <div class="w-full py-2 text-center text-lg">Info</div>
      <div class="flex flex-col mx-6 mb-2">
        <table class="table-auto text-sm">
          <tbody class="p-4">
            {#if detail.error}
              <tr class="bg-red-200">
                <td class="font-semibold text-right px-2">Error</td>
                <td>{detail.error}</td>
              </tr>
            {/if}
            <tr>
              <td class="font-semibold text-right px-2">Title</td>
              <td>{detail.title}</td>
            </tr>
            <tr>
              <td class="font-semibold text-right px-2">User</td>
              <td>{detail.user}</td>
            </tr>
            <tr>
              <td class="font-semibold text-right px-2">Environment</td>
              <td>{detail.environment}</td>
            </tr>
            <tr>
              <td class="font-semibold text-right px-2">Submitted</td>
              <td>{timeSince ?? since(new Date(detail.submitted))}</td>
            </tr>
            <tr>
              <td class="font-semibold text-right px-2">Summary</td>
              <td
                >{detail.summary?.moreRecords ? "first " : ""}{summary(
                  detail.summary?.records,
                )}</td
              >
            </tr>
          </tbody>
        </table>
      </div>
      <div class="w-full py-2 text-center text-lg">Source</div>
      <div class="mx-6 mb-2 p-2 bg-slate-900 text-green-400">
        <pre><code class="whitespace-pre-wrap break-words"
            >{@html Prism.highlight(
              detail.query ?? detail.template,
              celinql,
              "CelinQL",
            )}</code
          ></pre>
      </div>
    </div>
  </div>
{/if}
