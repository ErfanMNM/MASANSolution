// test_concurrent.js - Test concurrent requests performance
const axios = require('axios');

const SERVER_URL = 'http://localhost:49212';

// Test data template
const createTestData = (orderIndex, codeIndex) => ({
    orderNo: `TEST_PO_${orderIndex}`,
    uniqueCode: Array.from({ length: 50 }, (_, i) => `CODE_${orderIndex}_${codeIndex}_${i}_${Date.now()}`),
    blockNo: `BLOCK_${orderIndex}_${codeIndex}`,
    site: "TEST_SITE",
    factory: "TEST_FACTORY", 
    productionLine: "LINE_1",
    productionDate: "2025-08-12",
    shift: "A",
    orderQty: 1000,
    lotNumber: `LOT_${orderIndex}`,
    productCode: "PROD_TEST",
    productName: "Test Product",
    GTIN: "1234567890123",
    customerOrderNo: `CUST_${orderIndex}`,
    uom: "PCS"
});

// Test single request
async function testSingleRequest() {
    console.log('Testing single request...');
    const startTime = Date.now();
    
    try {
        const response = await axios.post(`${SERVER_URL}/api/orders`, createTestData(1, 1));
        const duration = Date.now() - startTime;
        
        console.log(`✅ Single request completed in ${duration}ms`);
        console.log(`   Status: ${response.status}`);
        console.log(`   Inserted: ${response.data.uniqueCode.insertedCount} codes`);
        
        return true;
    } catch (error) {
        console.log(`❌ Single request failed: ${error.message}`);
        return false;
    }
}

// Test concurrent requests - same PO
async function testConcurrentSamePO() {
    console.log('\nTesting concurrent requests to SAME PO...');
    const concurrentCount = 10;
    const startTime = Date.now();
    
    const promises = Array.from({ length: concurrentCount }, (_, i) =>
        axios.post(`${SERVER_URL}/api/orders`, createTestData(2, i + 1))
            .then(response => ({ success: true, duration: Date.now() - startTime, data: response.data }))
            .catch(error => ({ success: false, error: error.message }))
    );
    
    const results = await Promise.all(promises);
    const totalDuration = Date.now() - startTime;
    
    const successful = results.filter(r => r.success).length;
    const failed = results.filter(r => !r.success).length;
    
    console.log(`📊 Concurrent Same PO Results:`);
    console.log(`   Total time: ${totalDuration}ms`);
    console.log(`   Successful: ${successful}/${concurrentCount}`);
    console.log(`   Failed: ${failed}/${concurrentCount}`);
    
    if (failed > 0) {
        console.log(`   Errors:`, results.filter(r => !r.success).map(r => r.error));
    }
    
    return successful === concurrentCount;
}

// Test concurrent requests - different POs
async function testConcurrentDifferentPO() {
    console.log('\nTesting concurrent requests to DIFFERENT POs...');
    const concurrentCount = 10;
    const startTime = Date.now();
    
    const promises = Array.from({ length: concurrentCount }, (_, i) =>
        axios.post(`${SERVER_URL}/api/orders`, createTestData(100 + i, 1))
            .then(response => ({ success: true, duration: Date.now() - startTime, data: response.data }))
            .catch(error => ({ success: false, error: error.message }))
    );
    
    const results = await Promise.all(promises);
    const totalDuration = Date.now() - startTime;
    
    const successful = results.filter(r => r.success).length;
    const failed = results.filter(r => !r.success).length;
    
    console.log(`📊 Concurrent Different PO Results:`);
    console.log(`   Total time: ${totalDuration}ms`);
    console.log(`   Successful: ${successful}/${concurrentCount}`);
    console.log(`   Failed: ${failed}/${concurrentCount}`);
    
    if (failed > 0) {
        console.log(`   Errors:`, results.filter(r => !r.success).map(r => r.error));
    }
    
    return successful === concurrentCount;
}

// Test high load
async function testHighLoad() {
    console.log('\nTesting HIGH LOAD (50 concurrent requests)...');
    const concurrentCount = 50;
    const startTime = Date.now();
    
    const promises = Array.from({ length: concurrentCount }, (_, i) => {
        const orderNo = Math.floor(i / 5) + 200; // 5 requests per PO
        const blockNo = (i % 5) + 1;
        
        return axios.post(`${SERVER_URL}/api/orders`, createTestData(orderNo, blockNo), {
                timeout: 60000 // 60 second timeout
            })
            .then(response => ({ success: true, duration: Date.now() - startTime, data: response.data }))
            .catch(error => ({ success: false, error: error.message }));
    });
    
    const results = await Promise.all(promises);
    const totalDuration = Date.now() - startTime;
    
    const successful = results.filter(r => r.success).length;
    const failed = results.filter(r => !r.success).length;
    
    console.log(`📊 High Load Results:`);
    console.log(`   Total time: ${totalDuration}ms`);
    console.log(`   Average time per request: ${Math.round(totalDuration / concurrentCount)}ms`);
    console.log(`   Successful: ${successful}/${concurrentCount} (${Math.round(successful/concurrentCount*100)}%)`);
    console.log(`   Failed: ${failed}/${concurrentCount} (${Math.round(failed/concurrentCount*100)}%)`);
    
    if (failed > 0) {
        console.log(`   Sample errors:`, results.filter(r => !r.success).slice(0, 3).map(r => r.error));
    }
    
    return successful / concurrentCount >= 0.9; // 90% success rate acceptable
}

// Test read operations during write load
async function testReadDuringWrite() {
    console.log('\nTesting READ operations during WRITE load...');
    const writeCount = 20;
    const readCount = 10;
    
    const startTime = Date.now();
    
    // Start write operations
    const writePromises = Array.from({ length: writeCount }, (_, i) =>
        axios.post(`${SERVER_URL}/api/orders`, createTestData(300 + i, 1))
            .then(() => ({ type: 'write', success: true }))
            .catch(error => ({ type: 'write', success: false, error: error.message }))
    );
    
    // Start read operations simultaneously
    const readPromises = Array.from({ length: readCount }, (_, i) =>
        axios.get(`${SERVER_URL}/api/orders`)
            .then(() => ({ type: 'read', success: true }))
            .catch(error => ({ type: 'read', success: false, error: error.message }))
    );
    
    const allResults = await Promise.all([...writePromises, ...readPromises]);
    const totalDuration = Date.now() - startTime;
    
    const writeResults = allResults.filter(r => r.type === 'write');
    const readResults = allResults.filter(r => r.type === 'read');
    
    const writeSuccess = writeResults.filter(r => r.success).length;
    const readSuccess = readResults.filter(r => r.success).length;
    
    console.log(`📊 Read During Write Results:`);
    console.log(`   Total time: ${totalDuration}ms`);
    console.log(`   Write successful: ${writeSuccess}/${writeCount}`);
    console.log(`   Read successful: ${readSuccess}/${readCount}`);
    
    return writeSuccess === writeCount && readSuccess === readCount;
}

// Main test runner
async function runAllTests() {
    console.log('🚀 Starting Performance Tests...\n');
    
    const tests = [
        { name: 'Single Request', fn: testSingleRequest },
        { name: 'Concurrent Same PO', fn: testConcurrentSamePO },
        { name: 'Concurrent Different PO', fn: testConcurrentDifferentPO },
        { name: 'High Load', fn: testHighLoad },
        { name: 'Read During Write', fn: testReadDuringWrite }
    ];
    
    const results = [];
    
    for (const test of tests) {
        try {
            const passed = await test.fn();
            results.push({ name: test.name, passed });
            console.log(`${passed ? '✅' : '❌'} ${test.name}: ${passed ? 'PASSED' : 'FAILED'}\n`);
        } catch (error) {
            console.log(`❌ ${test.name}: ERROR - ${error.message}\n`);
            results.push({ name: test.name, passed: false });
        }
        
        // Wait between tests
        await new Promise(resolve => setTimeout(resolve, 1000));
    }
    
    // Summary
    console.log('📋 Test Summary:');
    const passed = results.filter(r => r.passed).length;
    const total = results.length;
    
    results.forEach(result => {
        console.log(`   ${result.passed ? '✅' : '❌'} ${result.name}`);
    });
    
    console.log(`\n🎯 Overall: ${passed}/${total} tests passed (${Math.round(passed/total*100)}%)`);
    
    if (passed === total) {
        console.log('🎉 All tests passed! Database locking issues have been resolved.');
    } else {
        console.log('⚠️  Some tests failed. Please check the implementation.');
    }
}

// Check if server is running first
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

// Run tests
async function main() {
    const serverRunning = await checkServer();
    if (serverRunning) {
        await runAllTests();
    }
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = { runAllTests, checkServer };