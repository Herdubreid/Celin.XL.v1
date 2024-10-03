<script>
  import { onDestroy, onMount } from "svelte";
  import { fade } from "svelte/transition";
  import { isExcel } from "../persist";
  import { parseCmd, submitQuery, submitScript } from "../submit";
  import CqlEditor from "./CqlEditor.svelte";
  import CslEditor from "./CslEditor.svelte";
  import CmdEditor from "./CmdEditor.svelte";
  import Server from "./Server.svelte";
  import Login from "./Login.svelte";
  import WebLogin from "./WebLogin.svelte";
  import Header from "./Header.svelte";
  import SubjectLookup from "./SubjectLookup.svelte";
  import AliasLookup from "./AliasLookup.svelte";
  import Notification from "./Notification.svelte";
  import Slider from "./Slider.svelte";
  import CmdList from "./v2/CmdList.svelte";
  import CqlList from "./v2/CqlList.svelte";
  import CslList from "./v2/CslList.svelte";

  export let version = "";

  let ready = false;
  let cqlRatio = 50;
  let cslRatio = 50;
  let cmdRatio = 50;
  let cqlTextElement;
  let cqlSelected;
  let cqlPaste;
  let cslTextElement;
  let cslSelected;
  let cmdTextElement;
  let cmdSelected;
  let openMenu;
  let currentTab = 0;

  const cqlMenu = {
    KeyR: () => submitQuery(cqlSelected.snippet),
  };

  const cslMenu = {
    KeyR: () => submitScript(cslSelected.snippet, false),
    KeyB: () => submitScript(cslSelected.snippet, true),
  };

  const cmdMenu = {
    KeyB: () => parseCmd(cmdSelected.snippet),
  };

  const menu = (option) => {
    switch (currentTab) {
      case 0:
        cqlMenu[option] ? cqlMenu[option]() : null;
        break;
      case 1:
        cslMenu[option] ? cslMenu[option]() : null;
        break;
      case 2:
        cmdMenu[option] ? cmdMenu[option]() : null;
    }
  };

  const handleKeydown = (ev) => {
    if (ev.altKey) {
      switch (ev.code) {
        case "KeyH":
          break;
        default:
          if (menu(ev.code)) ev.preventDefault();
      }
    }
  };

  onMount(async () => {
    ready = true;
    global.ready = true;
  });

  onDestroy(() => unsubscribe());
</script>

<svelte:window on:keydown={handleKeydown} />

<Notification />

{#await isExcel then yes}
  {#if yes}
    <Login />
  {:else}
    <WebLogin />
  {/if}
{/await}
<Server />

{#if ready}
  <Header
    {version}
    bind:openMenu
    {menu}
    {currentTab}
    setTab={(tab) => (currentTab = tab)}
  />
  <main transition:fade class="relative">
    <div class="fixed w-full" class:hidden={currentTab !== 0}>
      <div class="text-green-400" style="height: calc({cqlRatio}vh - 20px);">
        <CqlEditor
          bind:textareaEl={cqlTextElement}
          bind:selected={cqlSelected}
          bind:paste={cqlPaste}
        />
      </div>
      <Slider bind:ratio={cqlRatio} />
      <div
        class="mt-6"
        style="height: calc(100vh * {1 - cqlRatio / 100} - 80px);"
      >
        <div class="flex flex-row items-center">
          <SubjectLookup />
          <AliasLookup on:alias={(ev) => cqlPaste(ev.detail.text)} />
        </div>
        <CqlList />
      </div>
    </div>
    <div class="fixed w-full" class:hidden={currentTab !== 1}>
      <div class="text-green-400" style="height: calc({cslRatio}vh - 20px);">
        <CslEditor
          bind:textareaEl={cslTextElement}
          bind:selected={cslSelected}
        />
      </div>
      <Slider bind:ratio={cslRatio} />
      <div
        class="mt-6"
        style="height: calc(100vh * {1 - cslRatio / 100} - 50px);"
      >
        <CslList />
      </div>
    </div>
    <div class="fixed w-full" class:hidden={currentTab !== 2}>
      <div class="text-green-400" style="height: calc({cmdRatio}vh - 20px);">
        <CmdEditor
          bind:textareaEl={cmdTextElement}
          bind:selected={cmdSelected}
        />
      </div>
      <Slider bind:ratio={cmdRatio} />
      <div
        class="mt-6 overflow-hidden"
        style="height: calc(100vh * {1 - cmdRatio / 100} - 44px);"
      >
        <CmdList />
      </div>
    </div>
  </main>
{/if}
