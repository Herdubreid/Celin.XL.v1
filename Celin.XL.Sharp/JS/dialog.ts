import { blazorLib } from "./main";

export let dialog: Office.Dialog;

export function closeDlg() {
    dialog?.close();
}

export function messageDlg(notice: string) {
    dialog?.messageChild(JSON.stringify({ notice }))
}

export function openLoginDlg(title: string, username: string) {
    const url =
        location.protocol +
        "//" +
        location.hostname +
        (location.port ? ":" + location.port : "") +
        "/assets/login.html";
    console.log(url);
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
                    async (ev: any) => {
                        const msg = JSON.parse(ev.message);
                        switch (true) {
                            case msg.loaded:
                                dialog.messageChild(
                                    JSON.stringify({
                                        username,
                                        title,
                                    }),
                                );
                                break;
                            case msg.ok:
                                await blazorLib.invokeMethodAsync(
                                    "Authenticate",
                                    msg.username,
                                    msg.password,
                                );
                                break;
                            case msg.cancel:
                                blazorLib.invokeMethodAsync("DialogCancelled");
                                dialog.close();
                                break;
                        }
                    },
                );
                dialog.addEventHandler(
                    Office.EventType.DialogEventReceived,
                    (ev: any) => {
                        if (ev.error === 12006) {
                            blazorLib.invokeMethodAsync("DialogCancelled");
                            dialog.close();
                        }
                    },
                );
            }
        },
    );
}
