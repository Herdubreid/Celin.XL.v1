<script>
  import { createEventDispatcher } from "svelte";
  import { fade } from "svelte/transition";
  import Select from "svelte-select";
  import { stateStore } from "../stores";

  const dispatch = createEventDispatcher();

  const notify = (text) => dispatch("operator", { text });

  let items = [];
  fetch("./assets/data/operators.json")
    .then(async (response) => items = await response.json());

  const getOptionLabel = (item) => {
    const examples = item.examples.map(
      (e) => `
      <div class="flex text-sm place-items-center">
        <div class="font-mono bg-slate-600 text-green-300 mr-2 pl-1 pr-1">${e.code}</div>
        <div class="max-w-xs whitespace-normal">${e.description}</div>
      </div>`
    );
    return `
    <div class="container m-1">
      <div class="flex font-semibold mb-1">
        <div class="mr-6">${item.label}</div>
        <div class="font-mono bg-slate-600 text-green-300 pr-1 pl-1">${
          item.value
        }</div>
      </div>
      ${examples.join("")}
      <hr class="border-slate-500 my-1 ml-2 mr-2" />
    </div>`;
  };
</script>

{#if $stateStore.opPrompt}
  <div
    transition:fade
    class="absolute flex justify-end top-0 left-0 z-20 h-full w-full pr-1"
    on:click={() => stateStore.opPrompt(false)}
  >
    <div class="style w-2/3 lg:w-1/4 mt-10 mr-2">
      <Select
        {items}
        {getOptionLabel}
        listOpen={true}
        placeholder=""
        on:select={(ev) => notify(ev.detail.value)}
      />
    </div>
  </div>
{/if}

<style>
  .style {
    --listMaxHeight: 380px;
    --background: rgb(30, 41, 59);
    --listBackground: rgb(156, 163, 175);
    --border: 0px;
    --height: auto;
    font-size: 0.9em;
    border-top-width: 0px;
    color: rgb(19, 78, 74);
  }
</style>
