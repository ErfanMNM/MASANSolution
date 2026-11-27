// No service/network imports. All storage is created under one disposable OS temp directory.
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { load } = require('./test-support/sandbox');

test('JSON persistence regression suite (temporary data only)', async t => {
    const cwd = process.cwd();
    const temp = fs.mkdtempSync(path.join(os.tmpdir(), 'masan-backend-'));
    process.chdir(temp);
    try {
        const manager = load('Server_Service/fileManager.js').exports;
        await t.test('saved PO can be read, listed and included in GTIN allocation', async () => {
            const saved = await manager.savePOInfo({ orderNo: 'FRESH', GTIN: 'GTIN', orderQty: 25 });
            assert.equal((await manager.getPOInfo('FRESH')).orderNo, 'FRESH');
            assert.equal((await manager.getAllPOInfo()).length, 1);
            assert.equal(await manager.getTotalOrderQtyByGTIN('GTIN'), 25);
            const updated = await manager.savePOInfo({ orderNo: 'FRESH', GTIN: 'GTIN', orderQty: 26 });
            assert.equal(updated.createdAt, saved.createdAt);
        });
        await t.test('legacy bare/prefixed PO identities never alias or overwrite', async () => {
            fs.writeFileSync('data/PO_001.json', JSON.stringify({ orderNo: 'PO_001', GTIN: 'LEGACY' }));
            assert.equal(await manager.getPOInfo('001'), null);
            await manager.savePOInfo({ orderNo: '001', GTIN: 'OTHER' });
            assert.equal((await manager.getPOInfo('PO_001')).GTIN, 'LEGACY');
            fs.writeFileSync('data/PO_COMPAT.json', JSON.stringify({ orderNo: 'COMPAT', GTIN: 'LEGACY' }));
            assert.equal((await manager.getPOInfo('COMPAT')).GTIN, 'LEGACY');
            await manager.savePOInfo({ orderNo: 'A/B', GTIN: 'ONE' });
            await assert.rejects(manager.savePOInfo({ orderNo: 'A_B', GTIN: 'TWO' }), /collision/);
        });
        await t.test('duplicates within/across batches and blocks are rejected', async () => {
            const first = await manager.saveUniqueCodes('DEDUP', 'one', ['A', 'A', 'B']);
            assert.equal(first.insertedCount, 2); assert.equal(first.duplicateCount, 1);
            const second = await manager.saveUniqueCodes('DEDUP', 'two', ['A', 'C', 'C']);
            assert.equal(second.insertedCount, 1); assert.equal(second.duplicateCount, 2);
            assert.equal(await manager.getCodesCount('DEDUP'), 3);
        });
        await t.test('parallel code/log writes retain every unique item', async () => {
            const jobs = Array.from({ length: 20 }, (_, i) => manager.saveUniqueCodes('PARALLEL', `block${i}`, [`C${i}`, 'SHARED']));
            const results = await Promise.all(jobs);
            assert.equal(results.reduce((n, r) => n + r.insertedCount, 0), 21);
            assert.equal(await manager.getCodesCount('PARALLEL'), 21);
            await Promise.all(Array.from({ length: 20 }, (_, i) => manager.savePOLog({ orderNo: `LOG${i}` })));
            assert.equal((await manager.getPOLogs({}, 100)).length, 20);
        });
        await t.test('200000-code batch round-trips without argument-spread overflow', async () => {
            const codes = Array.from({ length: 200000 }, (_, i) => `FAKE_${i.toString().padStart(6, '0')}`);
            const result = await manager.saveUniqueCodes('LARGE', 'block', codes);
            assert.equal(result.insertedCount, 200000);
            const saved = await manager.getUniqueCodes('LARGE');
            assert.equal(saved.length, 200000); assert.equal(new Set(saved.map(c => c.code)).size, 200000);
            assert.equal(saved[199999].code, codes[199999]);
        });
        await t.test('corrupt JSON fails closed and is never replaced by empty state', async () => {
            const original = '{broken'; fs.writeFileSync('codes_json/GTIN_BROKEN.json', original);
            await assert.rejects(manager.saveUniqueCodes('BROKEN', 'block', ['A']));
            assert.equal(fs.readFileSync('codes_json/GTIN_BROKEN.json', 'utf8'), original);
            await manager.saveUniqueCodes('A/B', 'block', ['A']);
            await assert.rejects(manager.saveUniqueCodes('A_B', 'block', ['B']), /collision/);
            await assert.rejects(manager.getUniqueCodes('A_B'), /collision/);
            await assert.rejects(manager.getCodesCount('A_B'), /collision/);
            await assert.rejects(manager.deleteBlockCodes('A_B', 'block'), /collision/);
            await assert.rejects(manager.codesFileExists('A_B'), /collision/);
            assert.equal(await manager.getCodesCount('A/B'), 1);
            assert.equal(await manager.codesFileExists('A/B'), true);
        });
        await t.test('prototype-shaped block names remain ordinary persisted data', async () => {
            for (const block of ['__proto__', 'constructor', 'toString']) await manager.saveUniqueCodes('SPECIAL', block, [block]);
            assert.equal(await manager.getCodesCount('SPECIAL'), 3);
            assert.equal((await manager.getUniqueCodes('SPECIAL', '__proto__'))[0].code, '__proto__');
            await manager.deleteBlockCodes('SPECIAL', '__proto__');
            assert.equal(await manager.getCodesCount('SPECIAL'), 2);
        });
        await t.test('serialized delete/add and failed write leave a reusable lock and no temp files', async () => {
            await manager.saveUniqueCodes('DELETE', 'one', ['A']);
            await Promise.all([manager.deleteBlockCodes('DELETE', 'one'), manager.saveUniqueCodes('DELETE', 'two', ['B'])]);
            assert.equal(await manager.getCodesCount('DELETE'), 1);
            const atomic = manager.writeFileAtomic;
            manager.writeFileAtomic = async () => { throw new Error('injected write failure'); };
            await assert.rejects(manager.saveUniqueCodes('RETRY', 'one', ['A']), /injected/);
            manager.writeFileAtomic = atomic;
            await manager.saveUniqueCodes('RETRY', 'one', ['A']);
            for (const directory of ['codes_json', 'data']) assert.ok(fs.readdirSync(directory).every(file => !file.endsWith('.tmp')));
            assert.equal(manager.fileOperations.size, 0);
        });
        await t.test('PO names cannot overwrite the daily log namespace', async () => {
            const name = `logs_${new Date().toISOString().slice(0, 10)}`;
            await manager.savePOLog({ orderNo: 'BEFORE_RESERVED' });
            await manager.savePOInfo({ orderNo: name, GTIN: 'RESERVED' });
            await manager.savePOLog({ orderNo: name });
            assert.equal((await manager.getPOInfo(name)).GTIN, 'RESERVED');
            assert.ok((await manager.getPOLogs({}, 100)).some(log => log.orderNo === 'BEFORE_RESERVED'));
            assert.ok((await manager.getAllPOInfo()).some(po => po.orderNo === name));
            fs.writeFileSync('data/logs_legacy.json', JSON.stringify({ orderNo: 'logs_legacy', orderQty: 25, GTIN: 'LEGACY_LOGS' }));
            await manager.savePOInfo({ orderNo: 'logs_legacy', orderQty: 26, GTIN: 'LEGACY_LOGS' });
            assert.equal((await manager.getPOInfo('logs_legacy')).orderQty, 26);
        });
    } finally { process.chdir(cwd); fs.rmSync(temp, { recursive: true, force: true }); }
});
