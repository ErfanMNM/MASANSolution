// Parse first-party sources without executing service code or reading configuration/data files.
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const root = path.resolve(__dirname, '../..');
let files = 0, scripts = 0;
function check(directory) {
    for (const entry of fs.readdirSync(directory, { withFileTypes: true })) {
        if (entry.isDirectory()) {
            if (!['node_modules','obj','bin','dist','data','codes','codes_json','logs','requests','exports','temp','.vscode'].includes(entry.name)) check(path.join(directory, entry.name));
        } else if (/\.(js|html)$/.test(entry.name)) {
            const filename = path.join(directory, entry.name), source = fs.readFileSync(filename, 'utf8');
            if (entry.name.endsWith('.js')) { new vm.Script(source, { filename }); scripts++; }
            else for (const match of source.matchAll(/<script\b[^>]*>([\s\S]*?)<\/script>/gi)) {
                if (match[1].trim()) { new vm.Script(match[1], { filename }); scripts++; }
            }
            files++;
        }
    }
}
for (const directory of ['Server_Service','TimeService','Tranfer_Service','sqlite-explorer']) check(path.join(root, directory));
console.log(JSON.stringify({ filesParsed: files, scriptsParsed: scripts, failures: 0 }));
