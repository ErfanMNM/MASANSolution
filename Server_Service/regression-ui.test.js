const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const { sourceRoot, load, clock, flush } = require('./test-support/sandbox');

function client(relative, fetch, time = clock()) {
    const elements = new Map();
    function element(tag = 'div') {
        return { tagName: tag, children: [], style: {}, innerHTML: '', className: '', dataset: {}, textContent: '',
            set id(value) { this._id = value; elements.set(value, this); }, get id() { return this._id; },
            addEventListener() {}, appendChild(child) { this.children.push(child); },
            querySelectorAll() { return this.children.filter(child => child.tagName === 'tr'); },
            getElementsByTagName(name) { return this.children.filter(child => child.tagName === name); },
            remove() {}, querySelector(selector) { return this.children.find(child => `.${child.className}` === selector); }
        };
    }
    const document = { createElement: element, getElementById(id) {
        if (!elements.has(id) && !/^(key-row-|value-|status-|expires-)/.test(id)) { const item = element(); item.id = id; }
        return elements.get(id) || null;
    } };
    const context = vm.createContext({ document, window: {}, fetch, alert() {}, console: { error() {} },
        setInterval() {}, setTimeout: time.setTimeout.bind(time), clearTimeout: time.clearTimeout.bind(time) });
    const html = fs.readFileSync(path.join(sourceRoot, relative), 'utf8');
    for (const match of html.matchAll(/<script\b[^>]*>([\s\S]*?)<\/script>/gi)) if (match[1].trim()) vm.runInContext(match[1], context);
    return { context, elements, evaluate: code => vm.runInContext(code, context), time };
}

test('storage UIs display literal keys/values and only read while refreshing', async () => {
    for (const relative of ['Server_Service/public/index.html', 'Tranfer_Service/public/index.html']) {
        const calls = [], keys = ['__proto__', 'A/B'];
        const ui = client(relative, async (url, options = {}) => {
            calls.push({ url, method: options.method || 'GET' });
            return { ok: true, json: async () => url.endsWith('/keys') ? { keys } : { value: '<img src=x>', expiresAt: null } };
        });
        await flush();
        if (relative.startsWith('Server_')) await ui.evaluate('fetchActiveKeys(true)');
        else await ui.evaluate('loadKeys()');
        assert.ok(calls.every(call => call.method === 'GET'));
        assert.ok(calls.some(call => call.url.endsWith('A%2FB')));
        const body = ui.elements.get(relative.startsWith('Server_') ? 'activeKeysBody' : 'keyTable');
        assert.ok(body.children.some(row => row.children[0].textContent === '__proto__'));
        assert.ok(body.children.every(row => row.innerHTML === ''));
        assert.ok(body.children.some(row => row.children[1].textContent === '<img src=x>'));
    }
});

test('printer controls unlock after a failed command request', async () => {
    for (const relative of ['Server_Service/public/mayin.html', 'Tranfer_Service/public/mayin.html']) {
        const ui = client(relative, async url => {
            if (url === '/printer/toggle') throw new Error('synthetic network failure');
            return { ok: true, json: async () => ({ status: 'STOPPED' }) };
        });
        await flush(); await ui.evaluate('togglePrinter()');
        ui.time.advance(2000);
        assert.equal(ui.evaluate('isWaiting'), false);
        assert.equal(ui.elements.get('toggleBtn').disabled, false);
    }
});

test('both Electron tray applications permit Quit after ordinary close hides the window', async () => {
    for (const relative of ['Server_Service/main.js', 'Tranfer_Service/main.js']) {
        const events = {}, windows = [];
        let prevented = false, hidden = false;
        const app = { on(name, callback) { events[name] = callback; }, whenReady: () => Promise.resolve(),
            quit() { events['before-quit']?.(); windows[0].events.close({ preventDefault() { prevented = true; } }); } };
        class Window {
            constructor() { this.events = {}; windows.push(this); }
            on(name, callback) { this.events[name] = callback; } loadURL() {} loadFile() {} show() {} hide() { hidden = true; }
        }
        class Tray { setToolTip() {} setContextMenu() {} on() {} destroy() {} }
        load(relative, { modules: { electron: { app, BrowserWindow: Window, Tray, Menu: { buildFromTemplate: value => value } }, './server': {}, './PO_Service_JSON': {} } });
        await flush(); windows[0].events.close({ preventDefault() { prevented = true; } });
        assert.ok(prevented); assert.ok(hidden);
        prevented = false; app.quit(); assert.equal(prevented, false);
    }
});
