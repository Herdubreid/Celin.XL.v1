<script>
    import { onMount } from "svelte";
    import { EditorView, basicSetup } from "codemirror";
    import { EditorState } from "@codemirror/state";
    import { StreamLanguage } from "@codemirror/language";
    import { csharp } from "@codemirror/legacy-modes/mode/clike";
    import { oneDark } from "@codemirror/theme-one-dark";
    import ErrorMessage from "./ErrorMessage.svelte";

    let title;
    let editor;
    let notice = null;
    let isErrorVisible = false;
    let errorMessageComponent;

    Office.onReady((info) => {
        Office.context.ui.addHandlerAsync(
            Office.EventType.DialogParentMessageReceived,
            (result) => {
                const msg = JSON.parse(result.message);
                switch (true) {
                    case !!msg.title:
                        title = msg.title;
                        break;
                    case !!msg.doc:
                        editor.state.doc = msg.doc;
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

    $: isErrorVisible = notice != null;

    function saveContent() {
        const doc = editor.state.doc.toString();
        Office.context.ui.messageParent(
            JSON.stringify({
                save: true,
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
    function handleMessage(event) {
        document.querySelector("#editor").style.height =
            `calc(100% - 102px - ${event.detail.height}px)`;
    }
    onMount(() => {
        const state = EditorState.create({
            extensions: [basicSetup, StreamLanguage.define(csharp), oneDark],
        });
        editor = new EditorView({
            state,
            parent: document.querySelector("#editor"),
        });
    });
</script>

<div class="bg-slate-900 h-full w-full p-2 overflow-hidden">
    <div
        class="text-xl font-bold text-gray-400 text-center py-1 overflow-hidden"
    >
        {title}
    </div>
    <div id="editor" class="overflow-auto"></div>
    <ErrorMessage
        bind:message={notice}
        bind:this={errorMessageComponent}
        on:updated={handleMessage}
    />
    <div class="fixed bottom-2 left-0 right-0 flex justify-between px-8 py-1">
        <button
            class="transform active:scale-95 hover:ring py-2 px-4 bg-green-100"
            on:click={saveContent}>Save</button
        >
        <button
            class="transform active:scale-95 hover:ring py-2 px-4 bg-red-100"
            on:click={cancel}>Cancel</button
        >
    </div>
</div>

<style>
    #editor {
        transition: height 0.3s ease;
        height: calc(100% - 102px);
    }
</style>
