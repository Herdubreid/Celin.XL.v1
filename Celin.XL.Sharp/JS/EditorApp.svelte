<script lang="ts">
    import { onMount } from "svelte";
    import { EditorView, basicSetup } from "codemirror";
    import { EditorState } from "@codemirror/state";
    import { StreamLanguage } from "@codemirror/language";
    import { csharp } from "@codemirror/legacy-modes/mode/clike";
    import { oneDark } from "@codemirror/theme-one-dark";
    import ErrorMessage from "./ErrorMessage.svelte";

    let busy = false;
    let loaded = false;
    let key: string;
    let title: string;
    let editor: EditorView;
    let notice: string | null = null;

    Office.onReady(() => {
        Office.context.ui.addHandlerAsync(
            Office.EventType.DialogParentMessageReceived,
            (result) => {
                const msg = JSON.parse(result.message);
                switch (true) {
                    case msg.update:
                        key = msg.key;
                        title = msg.title;
                        const update = editor.state.update({
                            changes: {
                                from: 0,
                                to: editor.state.doc.length,
                                insert: msg.doc,
                            },
                        });
                        editor.update([update]);
                        loaded = true;
                        break;
                    case !!msg.notice:
                        busy = false;
                        notice = msg.notice;
                        busy = false;
                        break;
                }
            },
        );
        Office.context.ui.messageParent(JSON.stringify({ loaded: true }));
    });

    function playDoc() {
        busy = true;
        const doc = editor.state.doc.toString();
        Office.context.ui.messageParent(
            JSON.stringify({
                play: true,
                key,
                doc,
            }),
        );
    }
    const cancel = () => {
        Office.context.ui.messageParent(
            JSON.stringify({
                cancel: true,
            }),
        );
    };
    function handleMessage(event: any) {
        let el: HTMLElement = document.querySelector("#editor")!;
        el.style.height = `calc(100vh - 62px - ${event.detail.height}px)`;
    }

    onMount(() => {
        const updateListenter = EditorView.updateListener.of(async (update) => {
            if (loaded && update.docChanged) {
                const doc = update.state.doc.toString();
                Office.context.ui.messageParent(
                    JSON.stringify({
                        save: true,
                        key,
                        doc,
                    }),
                );
            }
        });
        const state = EditorState.create({
            extensions: [
                basicSetup,
                StreamLanguage.define(csharp),
                oneDark,
                updateListenter,
            ],
        });
        editor = new EditorView({
            state,
            parent: document.querySelector("#editor")!,
        });
    });
</script>

<div class="bg-slate-900 h-full w-full p-2 overflow-hidden">
    <div id="editor" class="overflow-auto"></div>
    <ErrorMessage bind:message={notice} on:updated={handleMessage} />
    <div class="fixed bottom-2 left-0 right-0 py-2 flex justify-between px-8">
        <button
            class="bg-transparent hover:bg-blue-500 text-blue-300 hover:text-white border border-blue-500 p-1 hover:border-transparent rounded"
            disabled={busy}
            on:click={playDoc}
        >
            <svg
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                stroke-width="1.5"
                stroke="currentColor"
                class="size-6"
            >
                <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z"
                />
                <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M15.91 11.672a.375.375 0 0 1 0 .656l-5.603 3.113a.375.375 0 0 1-.557-.328V8.887c0-.286.307-.466.557-.327l5.603 3.112Z"
                />
            </svg>
        </button>
        <button
            class="bg-transparent hover:bg-red-500 text-red-700 hover:text-white border p-1 border-red-500 hover:border-transparent rounded"
            on:click={cancel}
            ><svg
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                stroke-width="1.5"
                stroke="currentColor"
                class="size-6"
            >
                <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M6 18 18 6M6 6l12 12"
                />
            </svg>
        </button>
    </div>
</div>

<style>
    #editor {
        transition: height 0.3s ease;
        height: calc(100vh - 62px);
    }
</style>
