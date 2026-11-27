const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');

const sourceRoot = process.env.BACKEND_SOURCE_ROOT || path.resolve(__dirname, '../..');
function clock() {
    let now = 1700000000000, next = 1;
    const jobs = new Map();
    return {
        get now() { return now; },
        setTimeout(fn, delay) { const id = next++; jobs.set(id, { fn, at: now + delay }); return id; },
        clearTimeout(id) { jobs.delete(id); },
        setInterval() { return 0; },
        advance(ms) {
            now += ms;
            for (const [id, job] of [...jobs]) if (job.at <= now) { jobs.delete(id); job.fn(); }
        }
    };
}
function load(relative, options = {}) {
    const routes = new Map();
    const app = { use() {}, listen() { return { setTimeout() {}, close() {} }; } };
    for (const method of ['get', 'post', 'delete']) app[method] = (url, ...handlers) => routes.set(`${method} ${url}`, handlers.at(-1));
    const express = Object.assign(() => app, { json() {}, urlencoded() {}, static() {} });
    const timers = options.clock || globalThis;
    const RealDate = Date;
    class FakeDate extends RealDate {
        constructor(...args) { super(...(args.length ? args : [timers.now])); }
        static now() { return timers.now; }
    }
    const module = { exports: {} };
    const builtins = { fs, path, crypto: require('node:crypto'), express, cors: () => () => {},
        'body-parser': {}, 'swagger-ui-express': { serve: () => {}, setup: () => () => {} }, 'swagger-jsdoc': () => ({}),
        './Mid/sanitizeBodyMiddleware': () => {}, ...options.modules };
    const context = vm.createContext({ module, exports: module.exports, __dirname: options.directory || path.dirname(path.join(sourceRoot, relative)),
        __filename: path.join(sourceRoot, relative), require(name) {
            if (!Object.hasOwn(builtins, name)) throw new Error(`Unmocked dependency blocked: ${name}`);
            return builtins[name];
        }, process: { pid: process.pid, platform: process.platform, env: {}, on() {}, uptime: () => 0, memoryUsage: () => ({}) },
        console: { log() {}, error() {} }, Buffer, URLSearchParams,
        Date: options.clock ? FakeDate : Date,
        setTimeout: timers.setTimeout.bind(timers), clearTimeout: timers.clearTimeout.bind(timers),
        setInterval: options.clock ? timers.setInterval.bind(timers) : () => 0 });
    vm.runInContext(fs.readFileSync(path.join(sourceRoot, relative), 'utf8'), context, { filename: relative });
    return { context, routes, exports: module.exports, evaluate: code => vm.runInContext(code, context) };
}
async function route(sandbox, method, url, request = {}) {
    const response = { statusCode: 200, status(code) { this.statusCode = code; return this; }, json(body) { this.body = body; return this; }, ...request.response };
    const handler = sandbox.routes.get(`${method} ${url}`);
    if (!handler) throw new Error(`Route missing: ${method} ${url}`);
    await handler({ params: {}, query: {}, body: {}, ...request }, response);
    return response;
}
const flush = () => new Promise(resolve => setImmediate(resolve));
module.exports = { sourceRoot, load, route, clock, flush };
