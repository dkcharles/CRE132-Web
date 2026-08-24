// CodeMirror wrapper, adapted from CodeSchool's single-instance editor.js: a lesson page here
// can hold several editors at once (edit samples, challenges), so instances are keyed by a
// caller-supplied id. No language server in a browser - this gives syntax colouring, line
// numbers and error markers, and nothing pretends otherwise.
const editors = new Map();

export function create(id, element, text) {
    element.innerHTML = '';
    const cm = CodeMirror(element, {
        value: text,
        mode: 'text/x-csharp',
        theme: 'material-darker',
        lineNumbers: true,
        matchBrackets: true,
        indentUnit: 4,
        tabSize: 4,
        lineWrapping: false,
        viewportMargin: Infinity,   // with CSS height:auto the editor grows with its content
        gutters: ['CodeMirror-linenumbers', 'errors']
    });
    editors.set(id, cm);
}

export function getText(id) {
    const cm = editors.get(id);
    return cm ? cm.getValue() : '';
}

export function setText(id, text) {
    const cm = editors.get(id);
    if (cm) {
        cm.setValue(text);
        cm.clearGutter('errors');
    }
}

// Two parallel primitive arrays rather than an array of objects: primitives marshal
// unambiguously across Blazor's JS interop. Lines are 1-based, as Roslyn reports them.
export function setErrors(id, lines, messages) {
    const cm = editors.get(id);
    if (!cm) return;
    cm.clearGutter('errors');
    if (!lines) return;
    for (let i = 0; i < lines.length; i++) {
        const marker = document.createElement('span');
        marker.textContent = '●';
        marker.title = messages[i];
        marker.style.color = '#e0492f';
        cm.setGutterMarker(lines[i] - 1, 'errors', marker);
    }
}

export function destroy(id) {
    editors.delete(id);
}
