// demo_export_import.js - Simple demo of export/import without server dependency
const fs = require('fs');
const path = require('path');

// Tạo thư mục exports nếu chưa có
if (!fs.existsSync('./exports')) {
    fs.mkdirSync('./exports');
}

// Demo data
const demoData = {
    orderNo: "PO_001",
    blockNo: "BLOCK_001", 
    codes: [
        "CODE_001_<GS>DATA1",
        "CODE_002_<GS>DATA2",
        "CODE_003_<GS>DATA3",
        "CODE_004_<GS>DATA4",
        "CODE_005_<GS>DATA5"
    ]
};

function exportDemo() {
    console.log('📤 Demo Export Function...');
    
    try {
        // Tạo nội dung file
        const timestamp = new Date().toISOString();
        const header = `# Export UniqueCodes
# OrderNo: ${demoData.orderNo}
# BlockNo: ${demoData.blockNo}
# ExportTime: ${timestamp}
# TotalCodes: ${demoData.codes.length}

`;
        
        // Chuyển {GS} thành ký tự thật (ASCII 29) rồi lại chuyển về {GS} cho dễ đọc
        const codes = demoData.codes.map(code => {
            return code.replace(/<GS>/g, '<GS>'); // Giữ nguyên cho demo
        }).join('\n');

        const content = header + codes;

        // Tạo tên file
        const fileName = `${demoData.orderNo}_${demoData.blockNo}_${timestamp.split('T')[0]}.txt`;
        const filePath = path.join('./exports', fileName);

        // Ghi file
        fs.writeFileSync(filePath, content, 'utf8');
        
        console.log(`✅ Exported successfully:`);
        console.log(`   File: ${fileName}`);
        console.log(`   Path: ${filePath}`);
        console.log(`   Total codes: ${demoData.codes.length}`);
        
        return { fileName, filePath, totalCodes: demoData.codes.length };
        
    } catch (error) {
        console.log(`❌ Export failed: ${error.message}`);
        return null;
    }
}

function importDemo(filePath) {
    console.log('\n📥 Demo Import Function...');
    
    try {
        if (!fs.existsSync(filePath)) {
            throw new Error('File không tồn tại');
        }

        const content = fs.readFileSync(filePath, 'utf8');
        const lines = content.split('\n');
        
        // Bỏ qua các dòng comment và dòng trống
        const codes = lines
            .filter(line => !line.startsWith('#') && line.trim() !== '')
            .map(line => {
                // Chuyển {GS} về ký tự GS thật (ASCII 29)
                return line.trim().replace(/<GS>/g, String.fromCharCode(29));
            })
            .filter(code => code.length > 0);

        if (codes.length === 0) {
            throw new Error('File không chứa codes hợp lệ');
        }
        
        console.log(`✅ Import successful:`);
        console.log(`   File: ${path.basename(filePath)}`);
        console.log(`   Imported codes: ${codes.length}`);
        console.log(`   Sample code: ${codes[0].replace(/\x1D/g, '<GS>')}`);
        
        return { importedCodes: codes.length, codes };
        
    } catch (error) {
        console.log(`❌ Import failed: ${error.message}`);
        return null;
    }
}

function listExportsDemo() {
    console.log('\n📋 Demo List Exports...');
    
    try {
        const files = fs.readdirSync('./exports')
            .filter(file => file.endsWith('.txt'))
            .map(file => {
                const filePath = path.join('./exports', file);
                const stats = fs.statSync(filePath);
                return {
                    fileName: file,
                    size: stats.size,
                    createdTime: stats.birthtime.toISOString(),
                    modifiedTime: stats.mtime.toISOString()
                };
            })
            .sort((a, b) => new Date(b.modifiedTime) - new Date(a.modifiedTime));

        console.log(`✅ Found ${files.length} export files:`);
        files.forEach((file, index) => {
            console.log(`   ${index + 1}. ${file.fileName} (${file.size} bytes)`);
        });
        
        return files;
        
    } catch (error) {
        console.log(`❌ List exports failed: ${error.message}`);
        return null;
    }
}

function demonstrateWorkflow() {
    console.log('🚀 Demonstrating Export/Import Workflow...\n');
    
    // 1. Export
    const exportResult = exportDemo();
    if (!exportResult) return;
    
    // 2. List exports
    const files = listExportsDemo();
    if (!files || files.length === 0) return;
    
    // 3. Import từ file vừa tạo
    const importResult = importDemo(exportResult.filePath);
    if (!importResult) return;
    
    // 4. Verify
    console.log('\n🔍 Verification:');
    console.log(`   Original codes: ${demoData.codes.length}`);
    console.log(`   Imported codes: ${importResult.importedCodes}`);
    console.log(`   Match: ${demoData.codes.length === importResult.importedCodes ? '✅' : '❌'}`);
    
    console.log('\n📖 File Content Preview:');
    const content = fs.readFileSync(exportResult.filePath, 'utf8');
    const lines = content.split('\n');
    lines.slice(0, 10).forEach(line => {
        console.log(`   ${line}`);
    });
    if (lines.length > 10) {
        console.log(`   ... (${lines.length - 10} more lines)`);
    }
    
    console.log('\n🎉 Demo completed successfully!');
    console.log('\n📋 Usage Instructions:');
    console.log('1. Để export một block:');
    console.log('   GET /api/orders/{orderNo}/blocks/{blockNo}/export');
    console.log('');
    console.log('2. Để xem danh sách files:');
    console.log('   GET /api/exports');
    console.log('');
    console.log('3. Để download file:');
    console.log('   GET /api/exports/{fileName}');
    console.log('');
    console.log('4. Để import file:');
    console.log('   POST /api/orders/{orderNo}/blocks/{blockNo}/import');
    console.log('   Body: { "filePath": "filename.txt" }');
    console.log('');
    console.log('5. Để import và replace existing:');
    console.log('   POST /api/orders/{orderNo}/blocks/{blockNo}/import?replaceExisting=true');
    console.log('   Body: { "filePath": "filename.txt" }');
}

if (require.main === module) {
    demonstrateWorkflow();
}

module.exports = { exportDemo, importDemo, listExportsDemo };