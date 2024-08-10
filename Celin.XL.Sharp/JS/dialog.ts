import { globalState } from "./common";

export function closeDlg() {
    globalState.dialog?.close();
}

export function messageDlg(notice: string) {
    globalState.dialog?.messageChild(JSON.stringify({ notice }))
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
                globalState.dialog = result.value;
                globalState.dialog.addEventHandler(
                    Office.EventType.DialogMessageReceived,
                    async (ev: any) => {
                        const msg = JSON.parse(ev.message);
                        switch (true) {
                            case msg.loaded:
                                globalState.dialog!.messageChild(
                                    JSON.stringify({
                                        username,
                                        title,
                                    }),
                                );
                                break;
                            case msg.ok:
                                await globalState.blazorLib!.invokeMethodAsync(
                                    "Authenticate",
                                    msg.username,
                                    msg.password,
                                );
                                break;
                            case msg.cancel:
                                globalState.blazorLib!.invokeMethodAsync("DialogCancelled");
                                globalState.dialog!.close();
                                break;
                        }
                    },
                );
                globalState.dialog.addEventHandler(
                    Office.EventType.DialogEventReceived,
                    (ev: any) => {
                        if (ev.error === 12006) {
                            globalState.blazorLib!.invokeMethodAsync("DialogCancelled");
                            globalState.dialog!.close();
                        }
                    },
                );
            }
        },
    );
}
