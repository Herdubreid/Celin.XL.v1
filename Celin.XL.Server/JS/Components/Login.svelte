<script>
  import { onDestroy } from "svelte";
  import { fade } from "svelte/transition";
  import { stateStore, serversStore } from "../stores";

  const url =
    location.protocol +
    "//" +
    location.hostname +
    (location.port ? ":" + location.port : "") +
    "/assets/login.html";

  let username;
  let dialog = false;

  const lastUser = serversStore.subscribe((servers) => {
    username =
      servers?.length > 0 ? servers[0].authResponse?.username : username;
  });
  const prompt = stateStore.subscribe((state) => {
    if (dialog) {
      if (!state.login) {
        dialog.close();
        dialog = false;
      } else if (state.loginMsg) {
        dialog.messageChild(JSON.stringify({ notice: state.loginMsg }));
        stateStore.loginMsg(null);
      }
    } else if (state.login) {
      dialog = true;
      Office.context.ui.displayDialogAsync(
        url,
        {
          height: 28,
          width: 15,
          displayInIframe: true,
        },
        (result) => {
          if (result.status === Office.AsyncResultStatus.Failed) {
            console.error(`${result.error.code} ${result.error.message}`);
          } else {
            dialog = result.value;
            dialog.addEventHandler(
              Office.EventType.DialogMessageReceived,
              (ev) => {
                const msg = JSON.parse(ev.message);
                switch (true) {
                  case msg.loaded:
                    dialog.messageChild(JSON.stringify({ username }));
                    break;
                  case msg.ok:
                    stateStore.busy(true);
                    global.blazorLib.invokeMethodAsync(
                      "Authenticate",
                      msg.username,
                      msg.password
                    );
                    break;
                  case msg.cancel:
                    stateStore.login(false);
                    break;
                }
              }
            );
            dialog.addEventHandler(
              Office.EventType.DialogEventReceived,
              (ev) => {
                if (ev.error === 12006) {
                  stateStore.login(false);
                }
              }
            );
          }
        }
      );
    }
  });

  onDestroy(() => {
    lastUser();
    prompt();
  });
</script>

{#if $stateStore.login}
  <div
    transition:fade
    class="bg-opacity-70 bg-slate-900  z-40 fixed top-0 h-full w-full"
  />
{/if}
