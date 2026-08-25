// Draws one frame's commands onto a canvas. Commands arrive flat, nine slots each:
// [kind, a, b, c, d, r, g, b, text] with kind 0 Clear, 1 Rect, 2 Circle, 3 Line, 4 Text.
// The canvas persists between frames (only Clear wipes it), exactly like the text renderer.
const CELL = 16;

function prepare(canvas, width, height) {
    const dpr = window.devicePixelRatio || 1;
    const w = Math.round(width * dpr), h = Math.round(height * dpr);
    if (canvas.width !== w || canvas.height !== h) {      // first frame or Screen.Size changed: clears
        canvas.width = w; canvas.height = h;
        canvas.style.aspectRatio = width + ' / ' + height;
        const ctx = canvas.getContext('2d');
        ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
        ctx.fillStyle = '#000'; ctx.fillRect(0, 0, width, height);
    }
    const ctx = canvas.getContext('2d');
    ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
    return ctx;
}

export function reset(canvas, width, height) {
    canvas.width = 0;                      // force prepare() to resize and clear
    prepare(canvas, width, height);
}

export function frame(canvas, width, height, cmds) {
    const ctx = prepare(canvas, width, height);
    ctx.font = CELL + 'px ui-monospace, Consolas, monospace';
    ctx.textBaseline = 'top';
    for (let i = 0; i + 8 < cmds.length; i += 9) {
        const colour = 'rgb(' + cmds[i + 5] + ',' + cmds[i + 6] + ',' + cmds[i + 7] + ')';
        ctx.fillStyle = colour; ctx.strokeStyle = colour;
        const a = cmds[i + 1], b = cmds[i + 2], c = cmds[i + 3], d = cmds[i + 4];
        switch (cmds[i]) {
            case 0: ctx.fillRect(0, 0, width, height); break;
            case 1: ctx.fillRect(a, b, c, d); break;
            case 2: ctx.beginPath(); ctx.arc(a, b, Math.max(0, c), 0, Math.PI * 2); ctx.fill(); break;
            case 3: ctx.lineWidth = 2; ctx.beginPath(); ctx.moveTo(a, b); ctx.lineTo(c, d); ctx.stroke(); break;
            case 4: {
                const text = cmds[i + 8] || '';
                for (let k = 0; k < text.length; k++) ctx.fillText(text[k], a + k * CELL, b);   // one char per cell
                break;
            }
        }
    }
}
