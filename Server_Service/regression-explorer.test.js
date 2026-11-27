const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const vm = require('node:vm');
const sqlite3 = require('sqlite3').verbose();
const { sourceRoot, load, route, clock, flush } = require('./test-support/sandbox');

test('SQLite explorer uses only a synthetic temporary SQLite DB', async t => {
    const temp = fs.mkdtempSync(path.join(os.tmpdir(), 'masan-explorer-'));
    try {
        const extras = { ...fs, ensureDirSync: name => fs.mkdirSync(name, { recursive: true }), remove: name => fs.promises.rm(name, { force: true }) };
        const paths = [];
        let uploadStorage;
        const service = load('sqlite-explorer/app.js', { directory: temp, clock: clock(), modules: {
            sqlite3, 'fs-extra': extras,
            multer: Object.assign(() => ({ single: () => () => {} }), { diskStorage: value => { uploadStorage = value; return value; } }),
            'csv-writer': { createObjectCsvWriter: ({ path: filename }) => ({ async writeRecords(data) { paths.push(filename); await fs.promises.writeFile(filename, JSON.stringify(data)); } }) }
        } });
        const dbPath = path.join(temp, 'temp', 'fixture.db');
        const db = await new Promise((resolve, reject) => { const connection = new sqlite3.Database(dbPath, err => err ? reject(err) : resolve(connection)); });
        await new Promise((resolve, reject) => db.exec('CREATE TABLE "../part""codes" ("code""value" TEXT); INSERT INTO "../part""codes" VALUES (\'one\'),(\'two\');', err => err ? reject(err) : resolve()));
        await new Promise((resolve, reject) => db.close(err => err ? reject(err) : resolve()));
        const functions = service.evaluate('({ analyzeSQLiteFile, getTableData, getAllTableData, getTableSchema, executeQuery, findFileById })');
        await t.test('quoted table/column analysis, filter, sort and schema retain exact rows', async () => {
            const info = await functions.analyzeSQLiteFile(dbPath);
            assert.equal(info.tables[0].name, '../part"codes'); assert.equal(info.tables[0].rowCount, 2);
            const data = await functions.getTableData(dbPath, '../part"codes', 50, 0, 'o', 'code"value', 'DESC');
            assert.equal(data.total, 2); assert.equal(data.data[0]['code"value'], 'two');
            assert.equal((await functions.getAllTableData(dbPath, '../part"codes', '', 'code"value', 'DESC'))[0]['code"value'], 'two');
            assert.equal((await functions.getTableSchema(dbPath, '../part"codes')).columns[0].name, 'code"value');
            assert.equal(functions.findFileById('fix'), null);
            assert.equal(functions.findFileById('fixture'), dbPath);
            fs.copyFileSync(dbPath, path.join(temp, 'temp', 'mimeaccepted.data'));
            assert.equal(functions.findFileById('mimeaccepted'), path.join(temp, 'temp', 'mimeaccepted.data'));
            for (const originalname of ['../../outside.db', '..\\..\\outside.db', 'bad.']) {
                uploadStorage.filename({}, { originalname }, (err, name) => {
                    assert.equal(err, null); assert.ok(/^[a-f0-9]{32}\.[a-z0-9]+$/i.test(name));
                });
            }
        });
        await t.test('missing/open-failed DB helpers reject without unhandled process errors', async () => {
            const missing = path.join(temp, 'missing.db');
            await assert.rejects(functions.getTableData(missing, 'x'));
            await assert.rejects(functions.getAllTableData(missing, 'x'));
            await assert.rejects(functions.getTableSchema(missing, 'x'));
            await assert.rejects(functions.executeQuery(missing, 'SELECT 1'));
        });
        await t.test('concurrent same-time CSV exports have unique contained files and independent cleanup', async () => {
            const downloads = [];
            const call = filter => route(service, 'get', '/export/:fileId/:tableName/csv', { params: { fileId: 'fixture', tableName: '../part"codes' }, query: { filter }, response: {
                download(filename, label, callback) { downloads.push({ filename, callback }); }
            } });
            await Promise.all([call('one'), call('two')]);
            assert.equal(paths.length, 2); assert.notEqual(paths[0], paths[1]);
            for (const filename of paths) assert.equal(path.dirname(filename), path.join(temp, 'temp'));
            assert.notEqual(fs.readFileSync(paths[0], 'utf8'), fs.readFileSync(paths[1], 'utf8'));
            downloads[0].callback(); await flush();
            assert.ok(fs.existsSync(downloads[1].filename)); downloads[1].callback(); await flush();
        });
    } finally { fs.rmSync(temp, { recursive: true, force: true }); }
});

test('explorer client handles large totals, literal data and response ordering', async t => {
    const html = fs.readFileSync(path.join(sourceRoot, 'sqlite-explorer/public/index.html'), 'utf8');
    const elements = new Map();
    const element = id => {
        if (!elements.has(id)) elements.set(id, { innerHTML: '', textContent: '', value: '', style: {}, classList: { add() {}, remove() {} }, querySelectorAll: () => [] });
        return elements.get(id);
    };
    const context = vm.createContext({ document: { getElementById: element, addEventListener() {}, querySelectorAll: () => [], querySelector: () => element('selected') },
        window: { addEventListener() {} }, URLSearchParams, setTimeout, clearTimeout, console });
    for (const match of html.matchAll(/<script\b[^>]*>([\s\S]*?)<\/script>/gi)) if (match[1].trim()) vm.runInContext(match[1], context);
    vm.runInContext('showLoading=()=>{};hideLoading=()=>{};showError=message=>{throw new Error(message);}', context);
    await t.test('pagination offsets remain nonnegative page multiples up to 200000 rows', () => {
        vm.runInContext('loadTableData=()=>{};', context);
        for (const total of [0,1,49,50,51,125,1000,200000]) {
            vm.runInContext(`currentOffset=0;updatePagination({total:${total}});changePage('last');`, context);
            const last = Math.max(0, Math.floor((total - 1) / 50) * 50);
            assert.equal(vm.runInContext('currentOffset', context), last);
            vm.runInContext("currentOffset=0;changePage('next');changePage('next');", context);
            assert.equal(vm.runInContext('currentOffset', context), Math.min(last, 100));
        }
    });
    await t.test('DB data is escaped and identifier sorting uses data attributes', () => {
        context.sample = { data: [{ '<img src=x>': '<img src=x onerror=alert(1)>' }] };
        vm.runInContext('displayTableData(sample);displayQueryResult(sample);displayTableSchema({columns:[],createSql:"<script>alert(1)</script>"});', context);
        assert.ok(!element('dataContent').innerHTML.includes('<img'));
        assert.ok(element('dataContent').innerHTML.includes('&lt;img'));
        assert.ok(!element('dataContent').innerHTML.includes('onclick='));
        assert.ok(!element('queryResult').innerHTML.includes('<img'));
        assert.ok(!element('schemaContent').innerHTML.includes('<script>'));
    });
    await t.test('late old-table response is ignored and URL path segments are encoded', async () => {
        // Restore production loader after the pagination stub above.
        const script = [...html.matchAll(/<script\b[^>]*>([\s\S]*?)<\/script>/gi)].map(m => m[1]).join('\n');
        const start = script.indexOf('async function loadTableData()');
        const end = script.indexOf('function displayTableData(', start);
        vm.runInContext(script.slice(start, end), context);
        const pending = [];
        context.fetch = url => new Promise(resolve => pending.push({ url, resolve }));
        vm.runInContext("currentFileId='id';currentTable='old/table';", context);
        const first = vm.runInContext('loadTableData()', context);
        vm.runInContext("currentTable='new#table';", context);
        const second = vm.runInContext('loadTableData()', context);
        pending[1].resolve({ ok: true, json: async () => ({ data: [{ value: 'NEW' }], total: 1 }) }); await second;
        pending[0].resolve({ ok: true, json: async () => ({ data: [{ value: 'OLD' }], total: 1 }) }); await first;
        assert.ok(pending[0].url.includes('old%2Ftable')); assert.ok(pending[1].url.includes('new%23table'));
        assert.ok(element('dataContent').innerHTML.includes('NEW')); assert.ok(!element('dataContent').innerHTML.includes('OLD'));
    });
});
