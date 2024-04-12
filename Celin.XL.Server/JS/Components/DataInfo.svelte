<script>
  import { onDestroy } from "svelte";
  import { fade } from "svelte/transition";
  import { stateStore, cqlStore } from "../stores";

  let detail;

  const unsubscribe = stateStore.subscribe(
    (state) => (detail = state.info ? cqlStore.get(state.info) : null)
  );

  const summary = (records) =>
    records > 0 ? (records > 1 ? `${records} rows` : "1 row") : "No data";

  onDestroy(() => unsubscribe());
</script>

{#if detail}
  <div
    transition:fade
    class="absolute z-20 left-10 p-4 mr-4 rounded bg-gray-400 text-teal-900"
  >
    <div class="absolute z-10 bg-gray-400 -left-1 top-20 mt-1 h-4 w-4 rotate-45" />
    <table class="table-auto text-sm">
      <tbody>
        {#if detail.error}
          <tr class="bg-red-200">
            <td class="font-semibold">Error</td>
            <td>{detail.error}</td>
          </tr>
        {/if}
        <tr>
          <td class="font-semibold">Title</td>
          <td>{detail.title}</td>
        </tr>
        <tr>
          <td class="font-semibold">User</td>
          <td>{detail.user}</td>
        </tr>
        <tr>
          <td class="font-semibold">Query</td>
          <td>{detail.query}</td>
        </tr>
        <tr>
          <td class="font-semibold">Environment</td>
          <td>{detail.environment}</td>
        </tr>
        <tr>
          <td class="font-semibold">Submitted</td>
          <td>{detail.submitted}</td>
        </tr>
        <tr>
          <td class="font-semibold">Summary</td>
          <td
            >{detail.summary?.moreRecords ? "first " : ""}{summary(
              detail.summary?.records
            )}</td
          >
        </tr>
      </tbody>
    </table>
  </div>
{/if}

<style>
  td {
    padding: 2px;
  }
</style>
