<script>
  import { fade } from "svelte/transition";

  let username = "";
  let password = "";
  let title = "Login";
  let notice = "";
  let busy = false;

  Office.onReady((info) => {
    Office.context.ui.addHandlerAsync(
      Office.EventType.DialogParentMessageReceived,
      (result) => {
        const msg = JSON.parse(result.message);
        switch (true) {
          case !!msg.title:
            title = msg.title;
            break;
          case !!msg.username:
            username = msg.username;
            break;
          case !!msg.notice:
            busy = false;
            notice = msg.notice;
            break;
        }
      },
    );
    Office.context.ui.messageParent(JSON.stringify({ loaded: true }));
  });

  const ok = () => {
    notice = "";
    busy = true;
    Office.context.ui.messageParent(
      JSON.stringify({
        ok: true,
        username,
        password,
      }),
    );
  };
  const cancel = () => {
    Office.context.ui.messageParent(
      JSON.stringify({
        cancel: true,
      }),
    );
  };
</script>

<div
  transition:fade
  class="bg-opacity-70 bg-slate-900 z-40 fixed top-0 h-full w-full"
>
  <form on:submit|preventDefault={() => ok()}>
    <div class="px-8">
      <div class="flex place-content-center py-2 text-slate-300">
        <p class="text-xl font-semibold">
          {title}
        </p>
      </div>
      <div class="pb-4">
        <!-- svelte-ignore a11y-autofocus -->
        <input
          autofocus
          required
          disabled={busy}
          class="border rounded w-full py-2 px-3"
          type="text"
          placeholder="User Name"
          bind:value={username}
        />
      </div>
      <div class="mb-2">
        <input
          class="border border-red rounded w-full py-2 px-3"
          type="password"
          required
          disabled={busy}
          placeholder="Password"
          bind:value={password}
        />
      </div>
    </div>
    <div class="px-8 pt-4">
      <div class="flex items-center place-content-between">
        <button
          class="transform active:scale-95 hover:ring py-2 px-4 bg-green-100"
          type="submit"
          disabled={busy}
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-5 w-5"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fill-rule="evenodd"
              d="M3 3a1 1 0 011 1v12a1 1 0 11-2 0V4a1 1 0 011-1zm7.707 3.293a1 1 0 010 1.414L9.414 9H17a1 1 0 110 2H9.414l1.293 1.293a1 1 0 01-1.414 1.414l-3-3a1 1 0 010-1.414l3-3a1 1 0 011.414 0z"
              clip-rule="evenodd"
            />
          </svg>
        </button>
        <button
          class="transform active:scale-95 hover:ring py-2 px-4 bg-red-100"
          type="button"
          disabled={busy}
          on:click={() => cancel()}
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-5 w-5"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fill-rule="evenodd"
              d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
              clip-rule="evenodd"
            />
          </svg>
        </button>
      </div>
    </div>
    <div class="px-8 py-2 text-sm text-center text-red-400">{notice}</div>
  </form>
</div>
