<script>
  import { flip } from "svelte/animate";
  import { cslResponseStore } from "../stores";
  import ScStatus from "./ScStatus.svelte";
  import ScResponse from "./ScResponse.svelte";
</script>

<ScStatus />
<div class="relative h-full text-teal-500 mx-4 p-2 overflow-auto">
  {#each $cslResponseStore as detail, i (i)}
    <ScResponse {detail} />
  {/each}
</div>
