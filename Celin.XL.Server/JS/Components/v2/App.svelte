<script>
  import { onMount } from "svelte";
  import { isExcel } from "../../persist";
  import { serversStore } from "../../stores";
  import Server from "../Server.svelte";
  import Login from "../Login.svelte";
  import WebLogin from "../WebLogin.svelte";
  import Notification from "../Notification.svelte";
  import ActionList from "./CqlList.svelte";
  import Cql from "./CqlDetail.svelte";

  onMount(() => {
    serversStore.init();
  });
</script>

<Notification />

{#await isExcel then yes}
  {#if yes}
    <Login />
  {:else}
    <WebLogin />
  {/if}
{/await}
<Server />

<main class="w-screen bg-slate-900">
  <Cql />
  <ActionList />
</main>
