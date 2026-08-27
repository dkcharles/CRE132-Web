// Keyboard and mouse state for the game stage. Blazor asks for one snapshot per frame; edge
// detection happens in C#. Adapted from CodeSchool's input.js; key names are CRE132.Game.Key.
const held = new Set();
const heldByCode = new Map();   // physical key -> name, so a modifier pressed mid-hold cannot strand it
// Keys pressed since the last poll, even if already released. The 30 Hz poll runs on the WASM
// main thread after Step + frame + a possible re-render, so the real gap between snapshots is
// often well over 33 ms and a quick tap can fall entirely between two of them. Latching the
// press here and clearing it as it is read makes such a tap show as down for exactly one
// frame, so Keys.WasPressed (computed in C# against the previous snapshot) fires once - which
// is what Lesson 15 promises: the count goes up by exactly one per tap.
const pressedSinceSnapshot = new Set();
let mouse = { x: 0, y: 0, down: false };
let target = null;
let world = { width: 640, height: 360 };
let running = false;

function nameOf(e) {
    const k = e.key;
    if (k === ' ') return 'Space';
    if (k === 'Enter') return 'Enter';
    if (k === 'Escape') return 'Escape';
    if (k === 'ArrowUp') return 'Up';
    if (k === 'ArrowDown') return 'Down';
    if (k === 'ArrowLeft') return 'Left';
    if (k === 'ArrowRight') return 'Right';
    if (k.length === 1 && k >= 'a' && k <= 'z') return k.toUpperCase();
    if (k.length === 1 && k >= 'A' && k <= 'Z') return k;
    if (k.length === 1 && k >= '0' && k <= '9') return 'D' + k;
    return null;
}

// Arrows and space scroll the page; Space and Enter also re-fire whichever button still has
// focus (the Run the student just clicked), restarting the game they are playing. Swallowed
// only while a game runs, so a lesson page with nothing playing scrolls as normal. Escape is
// not in the set: it activates no button and cancels nothing here.
const swallow = new Set(['Space', 'Enter', 'Up', 'Down', 'Left', 'Right']);

function isEditableTarget(e) {
    const t = e.target;
    if (!t) return false;
    if (t.isContentEditable) return true;
    const tag = t.tagName;
    return tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT';
}

function onKeyDown(e) {
    if (isEditableTarget(e)) return;          // typing in CodeMirror never feeds the game
    const name = nameOf(e);
    if (!name) return;
    if (running && swallow.has(name)) e.preventDefault();
    held.add(name);
    pressedSinceSnapshot.add(name);
    if (e.code) heldByCode.set(e.code, name);
}

function onKeyUp(e) {
    const byCode = e.code ? heldByCode.get(e.code) : undefined;
    if (byCode !== undefined) { held.delete(byCode); heldByCode.delete(e.code); return; }
    const name = nameOf(e);
    if (name) held.delete(name);
}

function toScreen(e) {
    if (!target) return { x: 0, y: 0 };
    const rect = target.getBoundingClientRect();
    return {
        x: Math.round((e.clientX - rect.left) / rect.width * world.width),
        y: Math.round((e.clientY - rect.top) / rect.height * world.height)
    };
}

function onMouseMove(e) { const p = toScreen(e); mouse.x = p.x; mouse.y = p.y; }
function onMouseDown(e) { const p = toScreen(e); mouse.x = p.x; mouse.y = p.y; mouse.down = true; }
function onMouseUp() { mouse.down = false; }
function onBlur() { held.clear(); heldByCode.clear(); pressedSinceSnapshot.clear(); mouse.down = false; }

let listening = false;

// The playing stage, so a hidden tab can pause it. ONE document listener for the life of the
// page (idempotent like the key listeners above); the reference is swapped, never the
// listener. release() drops it, so a stage that has stopped is never woken by a tab switch -
// and because release() is also what teardown calls, a disposed component's reference cannot
// be invoked after Blazor has thrown it away.
let visibilityRef = null;
let visibilityListening = false;

function onVisibilityChange() {
    // Fire and forget: a stage disposed between the event and the call rejects, and an
    // unhandled rejection in a listener is noise a student should never see.
    if (visibilityRef) visibilityRef.invokeMethodAsync('OnVisibility', document.hidden).catch(function () {});
}

// Registered by GameStage right after attach, with itself as the reference.
export function onVisibility(dotnetRef) {
    visibilityRef = dotnetRef;
    if (!visibilityListening) {
        document.addEventListener('visibilitychange', onVisibilityChange);
        visibilityListening = true;
    }
}

// Idempotent: called every time a stage starts. One stage plays at a time, so the newest
// canvas simply takes over.
export function attach(canvas, width, height) {
    world = { width, height };
    // A key tapped while NOTHING was playing is still latched in pressedSinceSnapshot, and the
    // new loop's first snapshot would read it as a press belonging to this run. Escape made it
    // obvious - stop a game with it, press Run, and the fresh game stopped on frame 1 - but it
    // was always wrong: a run starts from no input at all.
    clear();
    if (!listening) {
        window.addEventListener('keydown', onKeyDown);
        window.addEventListener('keyup', onKeyUp);
        window.addEventListener('blur', onBlur);
        window.addEventListener('mouseup', onMouseUp);
        listening = true;
    }
    if (target !== canvas) {
        if (target) { target.removeEventListener('mousemove', onMouseMove); target.removeEventListener('mousedown', onMouseDown); }
        target = canvas;
        if (target) { target.addEventListener('mousemove', onMouseMove); target.addEventListener('mousedown', onMouseDown); }
    }
    // The student just clicked Run, so the button holds focus; Space or Enter would fire its
    // click again and restart the game they are trying to play. Dropping focus to the body
    // costs nothing (keys are read from window) and removes the trap before the first frame.
    if (document.activeElement && document.activeElement.blur) document.activeElement.blur();
    running = true;
}

export function release() {
    running = false;
    visibilityRef = null;
    clear();
}

// Everything release() forgets EXCEPT the stage itself: pausing on a hidden tab must not
// leave a key latched down from before the switch (alt-tab holds Alt, and a key released in
// another window sends no keyup here), but the paused stage still has to be told when the
// tab comes back.
export function clear() {
    held.clear(); heldByCode.clear(); pressedSinceSnapshot.clear(); mouse.down = false;
}

export function snapshot() {
    const keys = new Set(held);
    for (const k of pressedSinceSnapshot) keys.add(k);
    pressedSinceSnapshot.clear();          // each latched press is reported to exactly one frame
    return { keys: Array.from(keys), mouseX: mouse.x, mouseY: mouse.y, mouseDown: mouse.down };
}
