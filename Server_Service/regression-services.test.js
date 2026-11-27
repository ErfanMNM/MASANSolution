const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { load, route, clock, flush } = require('./test-support/sandbox');

test('key storage retains replacement values and consistent expiry without a real listener', async () => {
    const time = clock(), service = load('Server_Service/server.js', { clock: time });
    await route(service, 'post', '/set', { body: { key: 'A', value: 1, ttl: 10 } });
    await route(service, 'post', '/set', { body: { key: 'A', value: 2 } });
    time.advance(10);
    assert.equal((await route(service, 'get', '/get/:key', { params: { key: 'A' } })).body.value, 2);
    await route(service, 'post', '/set', { body: { key: '__proto__', value: 3 } });
    assert.equal((await route(service, 'post', '/get', { body: { keys: ['__proto__'] } })).body.values.__proto__, 3);
    await route(service, 'post', '/set', { body: { key: 'B', value: 9, ttl: 10 } });
    time.advance(11);
    assert.equal((await route(service, 'get', '/get/:key', { params: { key: 'B' } })).body.value, 0);
    assert.equal((await route(service, 'post', '/set', { body: [null] })).statusCode, 400);
    assert.equal((await route(service, 'post', '/set', { body: { key: 'C', value: 4, ttl: 2147483648 } })).statusCode, 400);
});

test('printer heartbeat delivers pending command until matching ACK and then toggles Stop', async () => {
    const service = load('Tranfer_Service/server.js', { clock: clock() });
    await route(service, 'get', '/printer/update/:status', { params: { status: 'STOPPED' } });
    await route(service, 'get', '/printer/toggle');
    const command = await route(service, 'get', '/printer/update/:status', { params: { status: 'STOPPED' } });
    assert.equal(command.body.command, 'PRINTTING');
    await route(service, 'get', '/printer/update/:status', { params: { status: 'PRINTTING' } });
    await route(service, 'get', '/printer/toggle');
    assert.equal((await route(service, 'get', '/printer/command')).body.command, 'STOPPED');
});

test('PO validation, storage stats and expired queue work (no socket or service data)', async t => {
    const cwd = process.cwd(), temp = fs.mkdtempSync(path.join(os.tmpdir(), 'masan-po-routes-'));
    process.chdir(temp);
    try {
        const time = clock();
        const manager = { codesFileExists() { return true; }, async getStorageStats() { return { totalFiles: 7 }; } };
        const service = load('Server_Service/PO_Service_JSON.js', { clock: time, modules: { './fileManager': manager } });
        await t.test('missing GTIN/nonarray codes produce client errors', async () => {
            assert.equal((await route(service, 'post', '/api/orders', { body: null })).statusCode, 400);
            const body = Object.fromEntries(['orderNo','blockNo','site','factory','productionLine','productionDate','shift','lotNumber','productCode','productName','GTIN','customerOrderNo','uom'].map(key => [key, 'fake']));
            body.orderQty = 25; body.uniqueCode = 'invalid';
            assert.equal((await route(service, 'post', '/api/orders', { body })).statusCode, 400);
        });
        await t.test('stats awaits async storage stats', async () => {
            assert.equal((await route(service, 'get', '/api/queue/stats')).body.storage.totalFiles, 7);
        });
        await t.test('queued timeout removes work so timed-out requests never mutate data later', async () => {
            const queue = service.evaluate('new RequestQueue()');
            let release, ran = false;
            const first = queue.addRequest(() => new Promise(resolve => { release = resolve; }));
            const second = queue.addRequest(() => { ran = true; });
            const rejection = assert.rejects(second, /timeout/);
            time.advance(60000); await rejection;
            release(); await first; await flush();
            time.advance(10); await flush();
            assert.equal(ran, false); assert.equal(queue.queue.length, 0);
        });
    } finally { process.chdir(cwd); fs.rmSync(temp, { recursive: true, force: true }); }
});
