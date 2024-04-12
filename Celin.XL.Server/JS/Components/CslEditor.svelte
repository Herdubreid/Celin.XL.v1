<script>
  import { onMount } from "svelte/internal";
  import { fade } from "svelte/transition";
  import Prism from "prismjs/prism";
  import { celinsl } from "../prism-celinsl";
  import { scriptStore } from "../stores";
  import { selectedCode } from "../selected";

  export let textareaEl = null;
  export let selected;
  let height;
  let pos;

  $: code =
    Prism.highlight($scriptStore, celinsl, "CSL") ||
    "<em style='color: lightgray; opacity: .6'>open.<i>Form.Version</i>.action[...</em>";

  $: selected = selectedCode(
    $scriptStore,
    textareaEl?.selectionStart ?? 0,
    textareaEl?.selectionEnd ?? 0
  );

  const handlePos = () => {
    selected = selectedCode(
      $scriptStore,
      textareaEl?.selectionStart ?? 0,
      textareaEl?.selectionEnd ?? 0
    );
  };

  const resizeObserver = new ResizeObserver((entries) => {
    for (let entry of entries) {
      if (entry.borderBoxSize) {
        height = `${24 + entry.borderBoxSize[0].blockSize}px`;
      }
    }
  });

  onMount(() => {
    resizeObserver.observe(document.getElementById("csl-display"));
  });
</script>

<div class="relative font-mono h-full w-full overflow-auto">
  <textarea
    bind:value={$scriptStore}
    on:keyup={(ev) => {
      if (/^(?:Arrow|Page|Home|End)/.test(ev.key)) handlePos();
    }}
    on:click={handlePos}
    bind:this={textareaEl}
    style:height
    spellcheck="false"
  />
  <pre><code id="csl-display" transition:fade>{@html code}</code></pre>
  <code
    >{selected.before}<span
      >{@html Prism.highlight(selected.snippet, celinsl, "CSL")}</span
    ></code
  >
</div>

<style>
  div {
    font-size: 16px;
  }
  div > code > span {
    z-index: 6;
  }
  textarea,
  code {
    margin: 0px;
    padding: 10px;
    border: 0;
    left: 0;
    word-break: break-all;
    white-space: break-spaces;
    overflow: visible;
    position: absolute;
    font-family: inherit;
    opacity: 0.7;
  }
  textarea {
    width: 100%;
    min-height: 100%;
    overflow: hidden;
    background: transparent !important;
    z-index: 2;
    resize: none;
    -webkit-text-fill-color: transparent;
  }
  textarea:focus {
    outline: 0;
    border: 0;
    box-shadow: none;
  }
  code {
    z-index: 1;
  }
  pre {
    margin: 0px;
    white-space: pre-wrap;
    word-wrap: break-word;
    font-family: inherit;
  }
</style>
