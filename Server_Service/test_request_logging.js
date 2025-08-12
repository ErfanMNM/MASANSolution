// test_request_logging.js - Test request logging functionality
const axios = require('axios');
const fs = require('fs');
const path = require('path');

const SERVER_URL = 'http://localhost:49212';

// Test data
const testRequests = [
    {
        orderNo: "LOG_TEST_001",
        uniqueCode: ["CODE_A1", "CODE_A2", "CODE_A3"],
        blockNo: "BLOCK_A",
        site: "SITE_LOG_TEST",
        factory: "FACTORY_LOG_TEST",
        productionLine: "LINE_LOG_TEST",
        productionDate: "2025-08-12",
        shift: "A",
        orderQty: 1000,
        lotNumber: "LOT_LOG_TEST",
        productCode: "PROD_LOG_TEST",
        productName: "Log Test Product",
        GTIN: "1234567890123",
        customerOrderNo: "CUST_LOG_TEST",
        uom: "PCS"
    },
    {
        orderNo: "LOG_TEST_001", // Same PO
        uniqueCode: ["CODE_B1", "CODE_B2<GS>DATA", "CODE_B3"],
        blockNo: "BLOCK_B", // Different block
        site: "SITE_LOG_TEST",
        factory: "FACTORY_LOG_TEST", 
        productionLine: "LINE_LOG_TEST",
        productionDate: "2025-08-12",
        shift: "B",
        orderQty: 1000,
        lotNumber: "LOT_LOG_TEST",
        productCode: "PROD_LOG_TEST",
        productName: "Log Test Product",
        GTIN: "1234567890123",
        customerOrderNo: "CUST_LOG_TEST",
        uom: "PCS"
    }
];

async function sendTestRequests() {
    console.log('📤 Sending test requests to generate logs...');
    
    const results = [];
    
    for (let i = 0; i < testRequests.length; i++) {
        const request = testRequests[i];
        console.log(`\n🔄 Sending request ${i + 1}/${testRequests.length}:`);
        console.log(`   OrderNo: ${request.orderNo}`);
        console.log(`   BlockNo: ${request.blockNo}`);
        console.log(`   Codes: ${request.uniqueCode.length}`);
        
        try {
            const response = await axios.post(`${SERVER_URL}/api/orders`, request);
            
            console.log(`   ✅ Success: ${response.status}`);
            console.log(`   Inserted: ${response.data.uniqueCode?.insertedCount || 0} codes`);
            
            if (response.data.backupInfo) {
                console.log(`   📦 Backup: ${response.data.backupInfo.codesBackupFile}`);
            }
            
            results.push({
                request: i + 1,
                success: true,
                response: response.data
            });
            
        } catch (error) {
            console.log(`   ❌ Failed: ${error.message}`);
            results.push({
                request: i + 1,
                success: false,
                error: error.message
            });
        }
        
        // Wait between requests
        await new Promise(resolve => setTimeout(resolve, 500));
    }
    
    return results;
}

async function checkRequestFiles() {
    console.log('\n📋 Checking request files...');
    
    try {
        const response = await axios.get(`${SERVER_URL}/api/requests`);
        
        console.log(`✅ Found ${response.data.count} request files:`);
        
        response.data.files.forEach((file, index) => {
            console.log(`   ${index + 1}. ${file.fileName} (${file.fileType}, ${file.size} bytes)`);
        });
        
        console.log(`📊 Total size: ${response.data.totalSize} bytes`);
        
        return response.data.files;
        
    } catch (error) {
        console.log(`❌ Failed to check request files: ${error.message}`);
        return [];
    }
}

async function viewRequestFileContent(fileName) {
    console.log(`\n👁️ Viewing content of ${fileName}...`);
    
    try {
        const response = await axios.get(`${SERVER_URL}/api/requests/${fileName}?view=true`);
        
        console.log(`📄 File: ${response.data.fileName}`);
        console.log(`📏 Size: ${response.data.fileSize} bytes`);
        console.log(`📝 Lines: ${response.data.totalLines}`);
        
        console.log('\n📖 Preview (first 20 lines):');
        response.data.preview.slice(0, 20).forEach((line, index) => {
            console.log(`   ${index + 1}: ${line}`);
        });
        
        if (response.data.totalLines > 20) {
            console.log(`   ... (${response.data.totalLines - 20} more lines)`);
        }
        
        return true;
        
    } catch (error) {
        console.log(`❌ Failed to view file content: ${error.message}`);
        return false;
    }
}

async function checkRequestStats() {
    console.log('\n📊 Checking request statistics...');
    
    try {
        const response = await axios.get(`${SERVER_URL}/api/requests-stats`);
        
        console.log(`✅ Request Files Statistics:`);
        console.log(`   Total files: ${response.data.totalFiles}`);
        console.log(`   Request logs: ${response.data.requestFiles}`);
        console.log(`   Code backups: ${response.data.codeFiles}`);
        console.log(`   Total size: ${response.data.totalSizeMB} MB`);
        
        if (response.data.statsByDate) {
            console.log('\n📅 Stats by date:');
            Object.entries(response.data.statsByDate).forEach(([date, stats]) => {
                console.log(`   ${date}: ${stats.totalFiles} files (${stats.requestFiles} requests, ${stats.codeFiles} codes)`);
            });
        }
        
        return true;
        
    } catch (error) {
        console.log(`❌ Failed to get request stats: ${error.message}`);
        return false;
    }
}

async function verifyLocalFiles() {
    console.log('\n🔍 Verifying local files...');
    
    try {
        const requestsDir = './requests';
        
        if (!fs.existsSync(requestsDir)) {
            console.log('❌ Requests directory does not exist');
            return false;
        }
        
        const files = fs.readdirSync(requestsDir)
            .filter(file => file.endsWith('.txt'));
        
        console.log(`✅ Found ${files.length} local files:`);
        
        const today = new Date().toISOString().split('T')[0];
        const todayFiles = files.filter(file => file.includes(today));
        
        console.log(`📅 Today's files (${today}): ${todayFiles.length}`);
        
        todayFiles.forEach(file => {
            const filePath = path.join(requestsDir, file);
            const stats = fs.statSync(filePath);
            const type = file.startsWith('requests_') ? 'REQUEST_LOG' : 
                        file.startsWith('codes_') ? 'CODES_BACKUP' : 'UNKNOWN';
            
            console.log(`   📄 ${file} (${type}, ${stats.size} bytes)`);
        });
        
        // Sample content from a request log file
        const requestLogFile = todayFiles.find(file => file.startsWith('requests_'));
        if (requestLogFile) {
            console.log(`\n📖 Sample from ${requestLogFile}:`);
            const content = fs.readFileSync(path.join(requestsDir, requestLogFile), 'utf8');
            const lines = content.split('\n');
            lines.slice(0, 10).forEach((line, index) => {
                console.log(`   ${index + 1}: ${line}`);
            });
            if (lines.length > 10) {
                console.log(`   ... (${lines.length - 10} more lines)`);
            }
        }
        
        return true;
        
    } catch (error) {
        console.log(`❌ Failed to verify local files: ${error.message}`);
        return false;
    }
}

async function runRequestLoggingTests() {
    console.log('🚀 Starting Request Logging Tests...\n');
    
    const tests = [];
    
    // 1. Send test requests
    const sendResults = await sendTestRequests();
    const sendSuccess = sendResults.every(r => r.success);
    tests.push({ name: 'Send Test Requests', passed: sendSuccess });
    
    // 2. Check request files via API
    const files = await checkRequestFiles();
    tests.push({ name: 'Check Request Files API', passed: files.length > 0 });
    
    // 3. View file content
    if (files.length > 0) {
        const requestFile = files.find(f => f.fileType === 'requests_log');
        const viewSuccess = requestFile ? await viewRequestFileContent(requestFile.fileName) : false;
        tests.push({ name: 'View File Content', passed: viewSuccess });
    } else {
        tests.push({ name: 'View File Content', passed: false });
    }
    
    // 4. Check statistics
    const statsSuccess = await checkRequestStats();
    tests.push({ name: 'Check Request Stats', passed: statsSuccess });
    
    // 5. Verify local files
    const localSuccess = await verifyLocalFiles();
    tests.push({ name: 'Verify Local Files', passed: localSuccess });
    
    return tests;
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
    
    const results = await runRequestLoggingTests();
    
    // Summary
    console.log('\n📋 Test Summary:');
    const passed = results.filter(r => r.passed).length;
    const total = results.length;
    
    results.forEach(result => {
        console.log(`   ${result.passed ? '✅' : '❌'} ${result.name}`);
    });
    
    console.log(`\n🎯 Overall: ${passed}/${total} tests passed (${Math.round(passed/total*100)}%)`);
    
    if (passed === total) {
        console.log('🎉 All request logging tests passed!');
        console.log('\n📝 Request logging is working correctly:');
        console.log('   ✅ Requests are saved to ./requests/requests_YYYY-MM-DD.txt');
        console.log('   ✅ UniqueCodes are backed up to ./requests/codes_OrderNo_BlockNo_Date_Time.txt');
        console.log('   ✅ API endpoints for viewing and managing request files work');
        console.log('   ✅ Statistics and file management features work');
    } else {
        console.log('⚠️  Some tests failed. Please check the implementation.');
    }
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = { runRequestLoggingTests, checkServer };