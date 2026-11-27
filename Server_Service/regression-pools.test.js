const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const sqlite3 = require('sqlite3').verbose();
const { load, clock, flush } = require('./test-support/sandbox');

test('SQLite pool regressions in disposable storage', async t => {
    const cwd = process.cwd(), temp = fs.mkdtempSync(path.join(os.tmpdir(), 'masan-pools-'));
    process.chdir(temp);
    const instances = [];
    try {
        await t.test('PO pool waits for schema and operation timeouts retain the native lease', async () => {
            const time = clock();
            const pool = load('Server_Service/db_po.js', { clock: time, modules: { sqlite3 } }).exports;
            instances.push(pool);
            await Promise.all(Array.from({ length: 5 }, () => pool.get('SELECT COUNT(*) AS count FROM POInfo')));
            const held = [];
            for (let i = 0; i < 4; i++) held.push(await pool.getConnection());
            const connection = pool.connections.find(conn => !pool.busyConnections.has(conn));
            const originalGet = connection.get;
            let finish;
            connection.get = (sql, params, callback) => { finish = callback; };
            const operation = pool.get('SELECT 1');
            const rejected = assert.rejects(operation, /timeout/);
            await flush();
            let nextAcquired = false;
            const next = pool.getConnection().then(db => { nextAcquired = true; return db; });
            time.advance(30000); await rejected; await flush();
            assert.equal(nextAcquired, false); assert.ok(pool.busyConnections.has(connection));
            finish(null, { value: 1 }); await next;
            connection.get = originalGet;
            pool.releaseConnection(connection);
            for (const conn of held) pool.releaseConnection(conn);
            for (const method of ['get', 'all', 'run']) await assert.rejects(pool[method](null));
            assert.equal(pool.busyConnections.size, 0);
            assert.equal((await pool.get('SELECT 1 AS value')).value, 1);
        });
        await t.test('code batch holds lease through empty COMMIT and rolls back failed COMMIT', async () => {
            const time = clock();
            const manager = load('Server_Service/codeDbManager.js', { clock: time, modules: { sqlite3 } }).exports;
            instances.push(manager);
            manager.maxConnectionsPerDB = 1;
            const connection = await manager.getConnection('FAKE'); manager.releaseConnection('FAKE', connection);
            const originalRun = connection.run;
            let finishCommit;
            connection.run = function(sql, ...args) {
                if (sql === 'COMMIT') { finishCommit = () => originalRun.call(this, sql, ...args); return this; }
                return originalRun.call(this, sql, ...args);
            };
            let done = false;
            const empty = manager.batchInsertCodes('FAKE', [], 'block', 'fake').then(value => { done = true; return value; });
            while (!finishCommit) await flush();
            assert.equal(done, false); assert.equal(manager.dbPools.get('FAKE').busyConnections.size, 1);
            finishCommit(); await empty; await flush();
            let failCommit = true, rollback = false;
            connection.run = function(sql, ...args) {
                if (sql === 'COMMIT' && failCommit) { failCommit = false; args.at(-1)(new Error('forced COMMIT failure')); return this; }
                if (sql === 'ROLLBACK') rollback = true;
                return originalRun.call(this, sql, ...args);
            };
            await assert.rejects(manager.batchInsertCodes('FAKE', ['A'], 'block', 'fake'), /forced COMMIT/); await flush();
            assert.ok(rollback); assert.equal(await manager.getCodesCount('FAKE'), 0);
            connection.run = originalRun;
            assert.equal((await manager.batchInsertCodes('FAKE', ['A', 'A', 'B'], 'block', 'fake')).insertedCount, 2);
            assert.equal(await manager.getCodesCount('FAKE'), 2);
        });
        await t.test('code count timeout cannot lend a still-running connection twice', async () => {
            const time = clock();
            const manager = load('Server_Service/codeDbManager.js', { clock: time, modules: { sqlite3 } }).exports;
            instances.push(manager); manager.maxConnectionsPerDB = 1;
            const connection = await manager.getConnection('LEASE'); manager.releaseConnection('LEASE', connection);
            let finish;
            const originalGet = connection.get;
            connection.get = (sql, params, callback) => { finish = callback; };
            const operation = manager.getCodesCount('LEASE'); const rejected = assert.rejects(operation, /timeout/);
            await flush();
            let nextAcquired = false;
            const next = manager.getConnection('LEASE').then(db => { nextAcquired = true; return db; });
            time.advance(15000); await rejected; await flush();
            assert.equal(nextAcquired, false);
            manager.cleanup(); assert.equal(manager.dbPools.size, 1);
            finish(null, { count: 0 }); await next;
            connection.get = originalGet; manager.releaseConnection('LEASE', connection);
        });
        await t.test('initialization errors reject acquisition instead of escaping as error events', async () => {
            class FailedDatabase extends require('node:events').EventEmitter {
                constructor(filename, callback) {
                    super();
                    setImmediate(() => callback ? callback(new Error('forced open failure')) : this.emit('error', new Error('forced open failure')));
                }
            }
            const driver = { verbose() { return this; }, Database: FailedDatabase };
            const pool = load('Server_Service/db_po.js', { modules: { sqlite3: driver } }).exports;
            await assert.rejects(pool.get('SELECT 1'), /forced open/);
            const codes = load('Server_Service/codeDbManager.js', { modules: { sqlite3: driver } }).exports;
            await assert.rejects(codes.getConnection('FAILED'), /forced open/);
            assert.equal(codes.dbPools.size, 0);
        });
        await t.test('cleanup cannot close a pool while initialization is paused', async () => {
            let finishOpen, closed = false;
            class PausedDatabase {
                constructor(filename, callback) { finishOpen = () => callback(null); }
                run(sql, callback) { setImmediate(() => callback(null)); }
                close(callback) { closed = true; if (callback) callback(); }
            }
            const driver = { verbose() { return this; }, Database: PausedDatabase };
            const manager = load('Server_Service/codeDbManager.js', { modules: { sqlite3: driver } }).exports;
            manager.maxConnectionsPerDB = 1;
            const pending = manager.getConnection('PAUSED');
            manager.cleanup(); assert.equal(closed, false); assert.equal(manager.dbPools.size, 1);
            finishOpen(); const connection = await pending;
            manager.releaseConnection('PAUSED', connection); manager.cleanup(); assert.equal(closed, true);
        });
        await t.test('failed rollback quarantines the old connection and permits a fresh pool', async () => {
            const manager = load('Server_Service/codeDbManager.js', { modules: { sqlite3 } }).exports;
            instances.push(manager); manager.maxConnectionsPerDB = 1;
            const connection = await manager.getConnection('RECOVERY');
            manager.releaseConnection('RECOVERY', connection);
            const originalRun = connection.run;
            connection.run = function(sql, ...args) {
                if (sql === 'COMMIT' || sql === 'ROLLBACK') { setImmediate(() => args.at(-1)(new Error(`forced ${sql}`))); return this; }
                return originalRun.call(this, sql, ...args);
            };
            await assert.rejects(manager.batchInsertCodes('RECOVERY', ['A'], 'block', 'fake'), /forced COMMIT/);
            await flush(); assert.equal(manager.dbPools.size, 0);
            const fresh = await manager.getConnection('RECOVERY'); assert.notEqual(fresh, connection);
            manager.releaseConnection('RECOVERY', fresh);
            assert.equal((await manager.batchInsertCodes('RECOVERY', ['B'], 'block', 'fake')).insertedCount, 1);
        });
        await t.test('batch deadline retains the native lease and rolls back before subsequent reads', async () => {
            const time = clock();
            const manager = load('Server_Service/codeDbManager.js', { clock: time, modules: { sqlite3 } }).exports;
            instances.push(manager); manager.maxConnectionsPerDB = 1;
            const connection = await manager.getConnection('DEADLINE'); manager.releaseConnection('DEADLINE', connection);
            const prepare = connection.prepare;
            let finishRow;
            connection.prepare = function(...args) {
                const statement = prepare.apply(this, args), run = statement.run;
                statement.run = function(...runArgs) { finishRow = () => run.apply(statement, runArgs); return statement; };
                return statement;
            };
            const operation = manager.batchInsertCodes('DEADLINE', ['A'], 'block', 'fake');
            const rejected = assert.rejects(operation, /timeout/);
            while (!finishRow) await flush();
            time.advance(60000); await rejected;
            assert.equal(manager.dbPools.get('DEADLINE').busyConnections.size, 1);
            finishRow();
            assert.equal(await manager.getCodesCount('DEADLINE'), 0);
            connection.prepare = prepare;
            await new Promise((resolve, reject) => connection.run('DROP TABLE UniqueCodes', err => err ? reject(err) : resolve()));
            await assert.rejects(manager.batchInsertCodes('DEADLINE', ['B'], 'block', 'fake'), /no such table/);
            await flush(); assert.equal(manager.dbPools.get('DEADLINE').busyConnections.size, 0);
        });
    } finally {
        for (const instance of instances) {
            const connections = instance.connections || [...instance.dbPools.values()].flatMap(pool => pool.connections);
            await Promise.all(connections.map(db => new Promise(resolve => db.close(() => resolve()))));
        }
        process.chdir(cwd); fs.rmSync(temp, { recursive: true, force: true });
    }
});
