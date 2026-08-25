// Keyboard and mouse state for the game stage. Blazor asks for one snapshot per frame; edge
// detection happens in C#. Adapted from CodeSchool's input.js; key names are CRE132.Game.Key.
const held = new Set();
const heldByCode = new Map();   // physical key -> name, so a modifier pressed mid-hold cannot strand it
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

// Arrows and space scroll the page; swallowed only while a game runs, so a lesson page with
// nothing playing scrolls as normal.
const swallow = new Set(['Space', 'Up', 'Down', 'Left', 'Right']);

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
function onBlur() { held.clear(); heldByCode.clear(); mouse.down = false; }

let listening = false;

// Idempotent: called every time a stage starts. One stage plays at a time, so the newest
// canvas simply takes over.
export function attach(canvas, width, height) {
    world = { width, height };
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
    running = true;
}

export function release() {
    running = false;
    held.clear(); heldByCode.clear(); mouse.down = false;
}

export function snapshot() {
    return { keys: Array.from(held), mouseX: mouse.x, mouseY: mouse.y, mouseDown: mouse.down };
}
