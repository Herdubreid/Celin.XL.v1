import { globalState } from "./common";

const dialogUrl = (name: string) =>
    location.protocol +
    "//" +
    location.hostname +
    (location.port ? ":" + location.port : "") +
    `/assets/${name}.html`;

export function closeDlg() {
    globalState.dialog?.close();
}

export function messageDlg(notice: string) {
    globalState.dialog?.messageChild(JSON.stringify({ notice }))
}

export function openEditorDlg(title: string, doc: string)
{
    Office.context.ui.displayDialogAsync(
        dialogUrl("editor"),
        {
            height: 40,
            width: 40,
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
                                        title,
                                        doc,
                                    }),
                                );
                                break;
                            case msg.save:
                                await globalState.blazorLib!.invokeMethodAsync(
                                    "UpdateScript",
                                    msg.doc,
                                );
                                break;
                            case msg.cancel:
                                globalState.blazorLib!.invokeMethodAsync("CancelDlg");
                                globalState.dialog!.close();
                                break;
                        }
                    },
                );
                globalState.dialog.addEventHandler(
                    Office.EventType.DialogEventReceived,
                    (ev: any) => {
                        if (ev.error === 12006) {
                            globalState.blazorLib!.invokeMethodAsync("CancelDlg");
                            globalState.dialog!.close();
                        }
                    },
                );
            }
        }
    )
}

export function openLoginDlg(title: string, username: string) {
    Office.context.ui.displayDialogAsync(
        dialogUrl("login"),
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
                                globalState.blazorLib!.invokeMethodAsync("CancelDlg");
                                globalState.dialog!.close();
                                break;
                        }
                    },
                );
                globalState.dialog.addEventHandler(
                    Office.EventType.DialogEventReceived,
                    (ev: any) => {
                        if (ev.error === 12006) {
                            globalState.blazorLib!.invokeMethodAsync("CancelDlg");
                            globalState.dialog!.close();
                        }
                    },
                );
            }
        },
    );
}
