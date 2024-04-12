<script>
  import { onMount } from "svelte";
  import { fade } from "svelte/transition";
  import Prism from "prismjs/prism";
  import { celinql } from "../prism-celinql";
  import { selectedCode } from "../selected";
  import { queryStore } from "../stores";

  export let textareaEl = null;
  export let selected;
  export const paste = (text) => {
    const last =
      textareaEl.selectionStart > 0
        ? textareaEl.value[textareaEl.selectionStart - 1]
        : " ";
    const alias = /\w/.test(last) ? `,${text}` : text;
    textareaEl.setRangeText(
      alias,
      textareaEl.selectionStart,
      textareaEl.selectionEnd,
      "end"
    );
    queryStore.set(textareaEl.value);
  };
  let height;

  $: code =
    Prism.highlight($queryStore, celinql, "CelinQL") ||
    "<em style='color: lightgray; opacity: .6'>Subject (alias list...) all|any(alias filters...)</em>";

  $: selected = selectedCode(
    $queryStore,
    textareaEl?.selectionStart ?? 0,
    textareaEl?.selectionEnd ?? 0
  );

  const handlePos = () => {
    selected = selectedCode(
      $queryStore,
      textareaEl?.selectionStart,
      textareaEl?.selectionEnd
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
    resizeObserver.observe(document.getElementById("cql-display"));
  });
</script>

<div class="relative font-mono h-full w-full overflow-auto">
  <textarea
    bind:value={$queryStore}
    on:keyup={(ev) => {
      if (/^(?:Arrow|Page|Home|End)/.test(ev.key)) handlePos();
    }}
    on:click={handlePos}
    bind:this={textareaEl}
    style:height
    spellcheck="false"
  />
  <pre><code id="cql-display" transition:fade>{@html code}</code></pre>
  <code
    >{selected.before}<span
      >{@html Prism.highlight(selected.snippet, celinql, "CelinQL")}</span
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
    opacity: 0.6;
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
