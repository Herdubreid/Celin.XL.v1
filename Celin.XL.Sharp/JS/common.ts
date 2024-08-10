import { DotNet } from "@microsoft/dotnet-js-interop";

export const globalState = {
    blazorLib: null as DotNet.DotNetObject | null,
    dialog: null as Office.Dialog | null,
}
