<!-- ErrorMessage.svelte -->
<script lang="ts">
    import { fade } from "svelte/transition";
    import { afterUpdate, createEventDispatcher } from "svelte";

    const dispatch = createEventDispatcher();

    export let message: string | null = null;
    let errorMessageElement: HTMLElement;
    let isVisible = false;

    $: isVisible = message !== null;

    function close() {
        message = null;
    }

    afterUpdate(() => {
        dispatch("updated", {
            height: errorMessageElement
                ? errorMessageElement.offsetHeight + 20
                : 0,
        });
    });
    export function getHeight() {
        return errorMessageElement ? errorMessageElement.offsetHeight : 0;
    }
</script>

{#if isVisible}
    <div
        transition:fade
        class="bg-red-200 text-red-900 text-sm p-2.5 rounded-md my-2 flex justify-between items-center"
        bind:this={errorMessageElement}
    >
        <pre class="w-full max-h-[240px] whitespace-pre-wrap overflow-y-auto">{message}</pre>
        <button
            class="self-baseline bg-none border-none text-base cursor-pointer pl-2"
            on:click={close}>✖</button
        >
    </div>
{/if}
