// The browser's hashchange event fires for EVERY fragment change - anchor clicks (including
// ones made before Blazor finished booting, which native-jump straight past component event
// handlers), Back/Forward, and hand-edited URLs. One listener feeding .NET replaces per-anchor
// click handlers and cannot be raced by the boot window.
export function listen(dotnet) {
    window.addEventListener('hashchange', () =>
        dotnet.invokeMethodAsync('OnHashChanged', window.location.hash));
}
