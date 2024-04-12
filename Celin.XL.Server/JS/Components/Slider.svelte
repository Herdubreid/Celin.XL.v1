<script>
  export let ratio;
  let dragging = false;

  const drag = (ev) => {
    if (dragging) {
      ratio = Math.min(Math.max((ev.clientY - 40) / window.innerHeight * 100, 2), 95);
    }
  };
</script>

<svelte:window on:mouseup={() => (dragging = false)} on:mousemove={drag} />

<div
  class="absolute flex flex-row justify-center border-b border-b-slate-400"
  style="top: calc({ratio}vh - 10px); width: 94vw; margin-left: 3vw;"
  >
  <button on:mousedown={() => (dragging = true)}
    class:cursor-ns-resize={dragging}
    ><svg
      xmlns="http://www.w3.org/2000/svg"
      class="absolute h-5 w-5 bg-slate-400 rounded-md hover:bg-gray-200 -top-2"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
    >
      <path
        stroke-linecap="round"
        stroke-linejoin="round"
        stroke-width="2"
        d="M8 9l4-4 4 4m0 6l-4 4-4-4"
      />
    </svg></button
  >
</div>
