// test_export_import.js - Test export/import functionality
const axios = require('axios');
const fs = require('fs');
const path = require('path');

const SERVER_URL = 'http://localhost:49212';

// Test data
const testPO = {
    orderNo: "TEST_EXPORT_PO",
    uniqueCode: [
        "CODE_001_<GS>DATA1",
        "CODE_002_<GS>DATA2", 
        "CODE_003_<GS>DATA3",
        "CODE_004_<GS>DATA4",
        "CODE_005_<GS>DATA5"
    ],
    blockNo: "BLOCK_EXPORT_001",
    site: "TEST_SITE",
    factory: "TEST_FACTORY",
    productionLine: "LINE_1",
    productionDate: "2025-08-12",
    shift: "A",
    orderQty: 1000,
    lotNumber: "LOT_EXPORT",
    productCode: "PROD_EXPORT",
    productName: "Export Test Product",
    GTIN: "1234567890123",
    customerOrderNo: "CUST_EXPORT",
    uom: "PCS"
};

async function setupTestData() {
    console.log('📋 Setting up test data...');
    
    try {
        const response = await axios.post(`${SERVER_URL}/api/orders`, testPO);
        console.log(`✅ Created test PO: ${testPO.orderNo} with ${testPO.uniqueCode.length} codes`);
        return true;
    } catch (error) {
        console.log(`❌ Failed to create test PO: ${error.message}`);
        return false;
    }
}

async function testExport() {
    console.log('\n📤 Testing Export Function...');
    
    try {
        const response = await axios.get(
            `${SERVER_URL}/api/orders/${testPO.orderNo}/blocks/${testPO.blockNo}/export`
        );
        
        console.log(`✅ Export successful:`);
        console.log(`   File: ${response.data.fileName}`);
        console.log(`   Total codes: ${response.data.totalCodes}`);
        console.log(`   Download URL: ${response.data.downloadUrl}`);
        
        return response.data;
        
    } catch (error) {
        console.log(`❌ Export failed: ${error.response?.data?.message || error.message}`);
        return null;
    }
}

async function testDownload(fileName) {
    console.log('\n📥 Testing Download Function...');
    
    try {
        const response = await axios.get(`${SERVER_URL}/api/exports/${fileName}`, {
            responseType: 'text'
        });
        
        console.log(`✅ Download successful:`);
        console.log(`   File size: ${response.data.length} characters`);
        
        // Check file content
        const lines = response.data.split('\n');
        const headerLines = lines.filter(line => line.startsWith('#'));
        const codeLines = lines.filter(line => !line.startsWith('#') && line.trim() !== '');
        
        console.log(`   Header lines: ${headerLines.length}`);
        console.log(`   Code lines: ${codeLines.length}`);
        console.log(`   Sample code: ${codeLines[0]}`);
        
        // Save to temp file for import test
        const tempFilePath = path.join('./exports', `temp_${fileName}`);
        fs.writeFileSync(tempFilePath, response.data);
        
        return tempFilePath;
        
    } catch (error) {
        console.log(`❌ Download failed: ${error.response?.data?.message || error.message}`);
        return null;
    }
}

async function testImport(filePath) {
    console.log('\n📥 Testing Import Function...');
    
    const newBlockNo = "BLOCK_IMPORT_002";
    
    try {
        const response = await axios.post(
            `${SERVER_URL}/api/orders/${testPO.orderNo}/blocks/${newBlockNo}/import`,
            {
                filePath: path.basename(filePath)
            }
        );
        
        console.log(`✅ Import successful:`);
        console.log(`   Imported codes: ${response.data.importedCodes}`);
        console.log(`   Total codes in file: ${response.data.totalCodesInFile}`);
        console.log(`   Duplicate count: ${response.data.duplicateCount}`);
        console.log(`   Block No: ${response.data.blockNo}`);
        
        return true;
        
    } catch (error) {
        console.log(`❌ Import failed: ${error.response?.data?.message || error.message}`);
        return false;
    }
}

async function testImportReplace(filePath) {
    console.log('\n🔄 Testing Import with Replace...');
    
    try {
        const response = await axios.post(
            `${SERVER_URL}/api/orders/${testPO.orderNo}/blocks/${testPO.blockNo}/import?replaceExisting=true`,
            {
                filePath: path.basename(filePath)
            }
        );
        
        console.log(`✅ Import with replace successful:`);
        console.log(`   Imported codes: ${response.data.importedCodes}`);
        console.log(`   Replaced existing: ${response.data.replaceExisting}`);
        
        return true;
        
    } catch (error) {
        console.log(`❌ Import with replace failed: ${error.response?.data?.message || error.message}`);
        return false;
    }
}

async function testListExports() {
    console.log('\n📋 Testing List Exports...');
    
    try {
        const response = await axios.get(`${SERVER_URL}/api/exports`);
        
        console.log(`✅ List exports successful:`);
        console.log(`   Total files: ${response.data.count}`);
        
        if (response.data.files.length > 0) {
            console.log(`   Latest file: ${response.data.files[0].fileName}`);
            console.log(`   File size: ${response.data.files[0].size} bytes`);
        }
        
        return true;
        
    } catch (error) {
        console.log(`❌ List exports failed: ${error.response?.data?.message || error.message}`);
        return false;
    }
}

async function verifyImportedData() {
    console.log('\n🔍 Verifying imported data...');
    
    try {
        const response = await axios.get(`${SERVER_URL}/api/orders/${testPO.orderNo}/codes`);
        
        const originalBlock = response.data.data.filter(code => code.blockNo === testPO.blockNo);
        const importedBlock = response.data.data.filter(code => code.blockNo === "BLOCK_IMPORT_002");
        
        console.log(`✅ Data verification:`);
        console.log(`   Original block codes: ${originalBlock.length}`);
        console.log(`   Imported block codes: ${importedBlock.length}`);
        console.log(`   Total codes: ${response.data.count}`);
        
        // Check if codes contain GS character properly
        if (originalBlock.length > 0) {
            const sampleCode = originalBlock[0].code;
            const hasGS = sampleCode.includes(String.fromCharCode(29));
            console.log(`   GS character preserved: ${hasGS ? '✅' : '❌'}`);
        }
        
        return originalBlock.length > 0 && importedBlock.length > 0;
        
    } catch (error) {
        console.log(`❌ Data verification failed: ${error.response?.data?.message || error.message}`);
        return false;
    }
}

async function cleanup() {
    console.log('\n🧹 Cleaning up...');
    
    try {
        // Remove temp files
        const tempFiles = fs.readdirSync('./exports')
            .filter(file => file.startsWith('temp_'))
            .forEach(file => {
                const filePath = path.join('./exports', file);
                fs.unlinkSync(filePath);
                console.log(`   Deleted: ${file}`);
            });
        
        console.log('✅ Cleanup completed');
    } catch (error) {
        console.log(`⚠️ Cleanup warning: ${error.message}`);
    }
}

async function runExportImportTests() {
    console.log('🚀 Starting Export/Import Tests...\n');
    
    const results = [];
    
    // 1. Setup test data
    const setupOk = await setupTestData();
    results.push({ name: 'Setup Test Data', passed: setupOk });
    if (!setupOk) return results;
    
    // 2. Test export
    const exportResult = await testExport();
    results.push({ name: 'Export Block', passed: !!exportResult });
    if (!exportResult) return results;
    
    // 3. Test download
    const downloadPath = await testDownload(exportResult.fileName);
    results.push({ name: 'Download File', passed: !!downloadPath });
    if (!downloadPath) return results;
    
    // 4. Test import to new block
    const importOk = await testImport(downloadPath);
    results.push({ name: 'Import to New Block', passed: importOk });
    
    // 5. Test import with replace
    const replaceOk = await testImportReplace(downloadPath);
    results.push({ name: 'Import with Replace', passed: replaceOk });
    
    // 6. Test list exports
    const listOk = await testListExports();
    results.push({ name: 'List Exports', passed: listOk });
    
    // 7. Verify data integrity
    const verifyOk = await verifyImportedData();
    results.push({ name: 'Verify Data Integrity', passed: verifyOk });
    
    // 8. Cleanup
    await cleanup();
    
    return results;
}

async function checkServer() {
    try {
        await axios.get(`${SERVER_URL}/api/orders`);
        return true;
    } catch (error) {
        console.log('❌ Server is not running. Please start the server first.');
        console.log('   Run: node PO_Service.js');
        return false;
    }
}

async function main() {
    const serverRunning = await checkServer();
    if (!serverRunning) return;
    
    const results = await runExportImportTests();
    
    // Summary
    console.log('\n📋 Test Summary:');
    const passed = results.filter(r => r.passed).length;
    const total = results.length;
    
    results.forEach(result => {
        console.log(`   ${result.passed ? '✅' : '❌'} ${result.name}`);
    });
    
    console.log(`\n🎯 Overall: ${passed}/${total} tests passed (${Math.round(passed/total*100)}%)`);
    
    if (passed === total) {
        console.log('🎉 All export/import tests passed!');
    } else {
        console.log('⚠️  Some tests failed. Please check the implementation.');
    }
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = { runExportImportTests, checkServer };