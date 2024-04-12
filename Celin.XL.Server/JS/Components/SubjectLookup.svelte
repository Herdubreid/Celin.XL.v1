<script>
  import Select from "svelte-select";
  import { subjectDemoStore } from "../stores";
  import loadOptions from "../loadSubjects";
</script>

<div class="style pl-2 w-3/5">
  <Select
    {loadOptions}
    placeholder="Search by Table or View and then Description..."
    on:select={(ev) => {
      subjectDemoStore.subject(ev.detail);
      global.blazorLib.invokeMethodAsync('SubjectDemoLookupRequest', ev.detail.value)
    }}
  />
</div>

<style>
  .style {
    --listMaxHeight: 320px;
    --background: rgb(226 232 240);
    --listBackground: rgb(226 232 240);
    --border: 0px;
    --margin: 2px;
    --height: 34px;
    --clearSelectBottom: 6px;
    --clearSelectTop: 6px;
    --spinnerLeft: 4px;
    color: rgb(15 23 42);
    font-family: monospace;
  }
</style>
