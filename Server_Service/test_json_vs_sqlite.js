// test_json_vs_sqlite.js - So sánh performance JSON vs SQLite
const axios = require('axios');

const SQLITE_URL = 'http://localhost:49212'; // SQLite version
const JSON_URL = 'http://localhost:49213';   // JSON version

// Test data
const createTestData = (orderIndex, blockIndex) => ({
    orderNo: `PERF_TEST_${orderIndex}`,
    uniqueCode: Array.from({ length: 20 }, (_, i) => 
        `CODE_${orderIndex}_${blockIndex}_${i}_${Date.now()}`
    ),
    blockNo: `BLOCK_${blockIndex}`,
    site: "PERF_SITE",
    factory: "PERF_FACTORY",
    productionLine: "LINE_1", 
    productionDate: "2025-08-12",
    shift: "A",
    orderQty: 1000,
    lotNumber: `LOT_${orderIndex}`,
    productCode: "PERF_PROD",
    productName: "Performance Test Product",
    GTIN: "1234567890123",
    customerOrderNo: `PERF_CUST_${orderIndex}`,
    uom: "PCS"
});

async function checkServers() {
    console.log('🔍 Checking servers availability...\n');
    
    let sqliteOK = false;
    let jsonOK = false;
    
    try {
        await axios.get(`${SQLITE_URL}/api/orders`, { timeout: 5000 });
        console.log('✅ SQLite server is running');
        sqliteOK = true;
    } catch (error) {
        console.log('❌ SQLite server is not running or not responding');
    }
    
    try {
        await axios.get(`${JSON_URL}/api/orders`, { timeout: 5000 });
        console.log('✅ JSON server is running');
        jsonOK = true;
    } catch (error) {
        console.log('❌ JSON server is not running or not responding');
    }
    
    console.log('');
    return { sqliteOK, jsonOK };
}

async function testSingleRequest(serverUrl, serverName) {
    console.log(`🧪 Testing single request - ${serverName}...`);
    
    const startTime = Date.now();
    
    try {
        const response = await axios.post(`${serverUrl}/api/orders`, createTestData(1, 1), {
            timeout: 30000
        });
        
        const duration = Date.now() - startTime;
        
        console.log(`  ✅ Success: ${duration}ms`);
        console.log(`  📊 Inserted: ${response.data.uniqueCode?.insertedCount || 0} codes`);
        
        return { success: true, duration, data: response.data };
        
    } catch (error) {
        const duration = Date.now() - startTime;
        console.log(`  ❌ Failed: ${duration}ms - ${error.message}`);
        return { success: false, duration, error: error.message };
    }
}

async function testConcurrentRequests(serverUrl, serverName, concurrency = 10) {
    console.log(`🚀 Testing ${concurrency} concurrent requests - ${serverName}...`);
    
    const startTime = Date.now();
    
    const promises = Array.from({ length: concurrency }, (_, i) =>
        axios.post(`${serverUrl}/api/orders`, createTestData(100 + i, 1), {
            timeout: 60000
        }).then(response => ({ 
            success: true, 
            duration: Date.now() - startTime, 
            insertedCount: response.data.uniqueCode?.insertedCount || 0,
            queueStats: response.data.queueStats
        })).catch(error => ({ 
            success: false, 
            error: error.message,
            duration: Date.now() - startTime
        }))
    );
    
    const results = await Promise.all(promises);
    const totalDuration = Date.now() - startTime;
    
    const successful = results.filter(r => r.success);
    const failed = results.filter(r => !r.success);
    
    console.log(`  📊 Results:`);
    console.log(`     Total time: ${totalDuration}ms`);
    console.log(`     Successful: ${successful.length}/${concurrency}`);
    console.log(`     Failed: ${failed.length}/${concurrency}`);
    console.log(`     Average time: ${Math.round(totalDuration / concurrency)}ms per request`);
    
    if (failed.length > 0) {
        console.log(`  ❌ Sample errors:`);
        failed.slice(0, 3).forEach(f => {
            console.log(`     - ${f.error}`);
        });
    }
    
    // Show queue stats for JSON version
    if (successful.length > 0 && successful[0].queueStats) {
        const qs = successful[0].queueStats;
        console.log(`  ⚡ Queue stats: processed=${qs.processed}, errors=${qs.errors}, queued=${qs.queued}`);
    }
    
    return {
        totalDuration,
        successCount: successful.length,
        failureCount: failed.length,
        successRate: (successful.length / concurrency) * 100
    };
}

async function testHighLoad(serverUrl, serverName, requestCount = 50) {
    console.log(`🔥 Testing high load (${requestCount} requests) - ${serverName}...`);
    
    const startTime = Date.now();
    const batchSize = 10;
    let totalSuccessful = 0;
    let totalFailed = 0;
    const results = [];
    
    // Process in batches to avoid overwhelming
    for (let batch = 0; batch < requestCount; batch += batchSize) {
        const batchRequests = [];
        const currentBatchSize = Math.min(batchSize, requestCount - batch);
        
        for (let i = 0; i < currentBatchSize; i++) {
            const requestIndex = batch + i;
            const promise = axios.post(`${serverUrl}/api/orders`, 
                createTestData(200 + requestIndex, 1), {
                timeout: 30000
            }).then(response => ({ success: true, insertedCount: response.data.uniqueCode?.insertedCount || 0 }))
              .catch(error => ({ success: false, error: error.message }));
            
            batchRequests.push(promise);
        }
        
        const batchResults = await Promise.all(batchRequests);
        results.push(...batchResults);
        
        const batchSuccessful = batchResults.filter(r => r.success).length;
        totalSuccessful += batchSuccessful;
        totalFailed += (currentBatchSize - batchSuccessful);
        
        console.log(`    Batch ${Math.floor(batch/batchSize) + 1}: ${batchSuccessful}/${currentBatchSize} successful`);
        
        // Small delay between batches
        await new Promise(resolve => setTimeout(resolve, 100));
    }
    
    const totalDuration = Date.now() - startTime;
    
    console.log(`  📊 High Load Results:`);
    console.log(`     Total time: ${totalDuration}ms`);
    console.log(`     Successful: ${totalSuccessful}/${requestCount} (${Math.round(totalSuccessful/requestCount*100)}%)`);
    console.log(`     Failed: ${totalFailed}/${requestCount} (${Math.round(totalFailed/requestCount*100)}%)`);
    console.log(`     Average: ${Math.round(totalDuration/requestCount)}ms per request`);
    
    return {
        totalDuration,
        successCount: totalSuccessful,
        failureCount: totalFailed,
        successRate: (totalSuccessful / requestCount) * 100
    };
}

async function comparePerformance() {
    console.log('⚡ PERFORMANCE COMPARISON: JSON vs SQLite\n');
    console.log('🎯 Testing scenarios:');
    console.log('  1. Single Request');
    console.log('  2. Concurrent Requests (10)');
    console.log('  3. High Load (30 requests)');
    console.log('=' .repeat(80));
    
    const { sqliteOK, jsonOK } = await checkServers();
    
    if (!sqliteOK && !jsonOK) {
        console.log('❌ No servers available for testing');
        return;
    }
    
    const results = {
        sqlite: {},
        json: {}
    };
    
    // Test 1: Single Request
    console.log('\n1️⃣ SINGLE REQUEST TEST\n');
    
    if (sqliteOK) {
        results.sqlite.single = await testSingleRequest(SQLITE_URL, 'SQLite');
    }
    
    if (jsonOK) {
        results.json.single = await testSingleRequest(JSON_URL, 'JSON');
    }
    
    // Test 2: Concurrent Requests
    console.log('\n2️⃣ CONCURRENT REQUESTS TEST (10 requests)\n');
    
    if (sqliteOK) {
        results.sqlite.concurrent = await testConcurrentRequests(SQLITE_URL, 'SQLite', 10);
    }
    
    if (jsonOK) {
        results.json.concurrent = await testConcurrentRequests(JSON_URL, 'JSON', 10);
    }
    
    // Test 3: High Load  
    console.log('\n3️⃣ HIGH LOAD TEST (30 requests)\n');
    
    if (sqliteOK) {
        results.sqlite.highLoad = await testHighLoad(SQLITE_URL, 'SQLite', 30);
    }
    
    if (jsonOK) {
        results.json.highLoad = await testHighLoad(JSON_URL, 'JSON', 30);
    }
    
    // Summary
    console.log('\n📋 PERFORMANCE SUMMARY');
    console.log('=' .repeat(80));
    
    if (sqliteOK && jsonOK) {
        console.log('\n📊 COMPARISON RESULTS:');
        
        // Single request comparison
        if (results.sqlite.single && results.json.single) {
            const sqliteTime = results.sqlite.single.duration;
            const jsonTime = results.json.single.duration;
            const winner = sqliteTime < jsonTime ? 'SQLite' : 'JSON';
            const diff = Math.abs(sqliteTime - jsonTime);
            
            console.log(`\n🏁 Single Request:`);
            console.log(`   SQLite: ${sqliteTime}ms`);
            console.log(`   JSON: ${jsonTime}ms`);
            console.log(`   Winner: ${winner} (${diff}ms faster)`);
        }
        
        // Concurrent comparison
        if (results.sqlite.concurrent && results.json.concurrent) {
            const sqliteSuccess = results.sqlite.concurrent.successRate;
            const jsonSuccess = results.json.concurrent.successRate;
            const winner = sqliteSuccess > jsonSuccess ? 'SQLite' : 'JSON';
            
            console.log(`\n🚀 Concurrent (10 requests):`);
            console.log(`   SQLite: ${sqliteSuccess}% success rate`);
            console.log(`   JSON: ${jsonSuccess}% success rate`);
            console.log(`   Winner: ${winner}`);
        }
        
        // High load comparison
        if (results.sqlite.highLoad && results.json.highLoad) {
            const sqliteSuccess = results.sqlite.highLoad.successRate;
            const jsonSuccess = results.json.highLoad.successRate;
            const winner = sqliteSuccess > jsonSuccess ? 'SQLite' : 'JSON';
            
            console.log(`\n🔥 High Load (30 requests):`);
            console.log(`   SQLite: ${sqliteSuccess}% success rate`);
            console.log(`   JSON: ${jsonSuccess}% success rate`);
            console.log(`   Winner: ${winner}`);
        }
    }
    
    console.log('\n🎯 RECOMMENDATIONS:');
    
    if (results.json.concurrent?.successRate >= 90) {
        console.log('✅ JSON version shows good stability for concurrent requests');
    }
    
    if (results.json.highLoad?.successRate >= 80) {
        console.log('✅ JSON version handles high load well');
    }
    
    if (results.sqlite.concurrent?.successRate < 50) {
        console.log('⚠️  SQLite version has database locking issues under load');
        console.log('💡 Consider switching to JSON version for better concurrent performance');
    }
    
    console.log('\n🏆 CONCLUSION:');
    if (jsonOK && results.json.concurrent?.successRate > (results.sqlite.concurrent?.successRate || 0)) {
        console.log('🎉 JSON version provides better concurrent performance!');
        console.log('📈 Benefits: No database locking, sequential queue processing, better scalability');
    } else if (sqliteOK) {
        console.log('📊 SQLite version may be better for single requests, but has concurrency issues');
    }
    
    return results;
}

async function main() {
    try {
        await comparePerformance();
    } catch (error) {
        console.error('Test failed:', error.message);
    }
}

if (require.main === module) {
    main();
}

module.exports = { comparePerformance, checkServers };