<script lang="ts">
  import { createEventDispatcher } from "svelte";
  import Select from "svelte-select";
  import { subjectDemoStore } from "../stores";
  import "../loadSubjectDemo";

  const dispatch = createEventDispatcher();

  const notify = (text) => dispatch("alias", { text });
</script>

<div class="style pr-2 w-2/5">
  <Select
    items={$subjectDemoStore?.list}
    placeholder={$subjectDemoStore?.subject
      ? $subjectDemoStore?.list
        ? `Specs for ${$subjectDemoStore.subject.value}...`
        : "Loading specs..."
      : "Select subject..."}
    placeholderAlwaysShow={true}
    on:select={(ev) => {
      notify(ev.detail.value);
    }}
  />
</div>

<style>
  .style {
    --listMaxHeight: 320px;
    --background: rgb(226 232 240);
    --listBackground: rgb(226 232 240);
    --border: 0px;
    --margin: 0px;
    --height: 34px;
    --errorBorder: 0px;
    --errorBackground: rgb(245, 178, 178);
    color: rgb(15 23 42);
    font-family: monospace;
    font-size: 12px;
  }
</style>
