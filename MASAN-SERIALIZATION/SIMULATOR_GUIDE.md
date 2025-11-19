# 📚 Hướng Dẫn Sử Dụng Code Simulator

## 🎯 Mục Đích
Chức năng Simulator giúp bạn test hệ thống bằng cách giả lập gửi mã từ database, không cần camera thật.

---

## 🚀 Các Chức Năng Chính

### 1. **LoadCodesFromDatabase(int count = 100)**
Lấy mã từ database để test.

```csharp
// Lấy 100 mã đầu tiên từ DB
FDashboard.LoadCodesFromDatabase(100);

// Lấy 50 mã
FDashboard.LoadCodesFromDatabase(50);
```

---

### 2. **StartSimulator(int mode = 0, int intervalMs = 500)**
Bắt đầu giả lập gửi mã tự động.

**Tham số:**
- `mode`:
  - `0` = Gửi cho cả Camera Main và Camera Sub (mặc định)
  - `1` = Chỉ gửi cho Camera Main
  - `2` = Chỉ gửi cho Camera Sub
- `intervalMs`: Thời gian giữa mỗi lần gửi (milliseconds, mặc định 500ms)

**Ví dụ:**
```csharp
// Gửi cả 2 camera, mỗi 500ms
FDashboard.StartSimulator(0, 500);

// Chỉ gửi Camera Main, mỗi 1 giây
FDashboard.StartSimulator(1, 1000);

// Chỉ gửi Camera Sub, mỗi 300ms
FDashboard.StartSimulator(2, 300);
```

---

### 3. **StopSimulator()**
Dừng simulator.

```csharp
FDashboard.StopSimulator();
```

---

### 4. **SendTestCode(string code, bool toCameraMain = true)**
Gửi một mã test ngay lập tức.

```csharp
// Gửi mã cho Camera Main
FDashboard.SendTestCode("01234567890123", true);

// Gửi mã cho Camera Sub
FDashboard.SendTestCode("01234567890123", false);
```

---

### 5. **GetSimulatorStatus()**
Xem trạng thái simulator.

```csharp
string status = FDashboard.GetSimulatorStatus();
Console.WriteLine(status);
```

Output:
```
Simulator Status:
- Running: True
- Mode: Both
- Interval: 500ms
- Queue CMain: 45 codes
- Queue CSub: 45 codes
```

---

## 📋 Quy Trình Test Cơ Bản

### **Bước 1: Load mã từ DB**
```csharp
FDashboard.LoadCodesFromDatabase(50);
```

### **Bước 2: Bắt đầu sản xuất**
Đảm bảo hệ thống ở trạng thái `Running` trước khi test.

### **Bước 3: Chạy simulator**
```csharp
// Test Camera Main trước (interval nhanh hơn)
FDashboard.StartSimulator(1, 300);

// Sau khi xong, test Camera Sub
// FDashboard.StartSimulator(2, 500);
```

### **Bước 4: Theo dõi kết quả**
Xem log ở tab "Thông báo" và "Lịch sử" trong dashboard.

### **Bước 5: Dừng khi cần**
```csharp
FDashboard.StopSimulator();
```

---

## 🔍 Cách Gọi Từ Code Khác

### **Từ Form Main:**
```csharp
// Giả sử FDashboard là instance của trang Dashboard
var dashboardPage = GetDashboardPageInstance();
dashboardPage.StartSimulator(0, 500);
```

### **Từ Button Click:**
```csharp
private void btnStartSim_Click(object sender, EventArgs e)
{
    FDashboard.StartSimulator(0, 500);
}

private void btnStopSim_Click(object sender, EventArgs e)
{
    FDashboard.StopSimulator();
}
```

---

## ⚠️ Lưu Ý Quan Trọng

### 1. **Trạng thái Production**
- Camera Main: Có thể test khi `Production_State != Running` (sẽ báo lỗi nhưng vẫn xử lý)
- Camera Sub: **Chỉ hoạt động khi `Production_State == Running/Waiting_Stop/Check_After_Completed`**

### 2. **Queue Management**
- Simulator tự động dừng khi hết mã trong queue
- Có thể load thêm mã bằng `LoadCodesFromDatabase()` khi đang chạy

### 3. **Performance**
- Interval quá nhỏ (< 100ms) có thể gây quá tải
- Khuyến nghị: 300-500ms cho test bình thường

### 4. **Camera Sub Busy**
- Nếu Camera Sub đang bận (subpr.IsBusy), simulator sẽ skip mã đó
- Log sẽ hiển thị: `⚠️ SIM CSub bận, skip: {code}`

---

## 📊 Log Symbols

Simulator sử dụng các ký hiệu để dễ phân biệt:

| Symbol | Ý Nghĩa |
|--------|---------|
| ✅ | Thành công (load mã, hoàn thành) |
| ▶️ | Bắt đầu simulator |
| ⏹️ | Dừng simulator |
| 🔵 | Gửi mã cho Camera Main |
| 🟢 | Gửi mã cho Camera Sub |
| ⚠️ | Cảnh báo (busy, skip, not running) |
| 📤 | Gửi test code thủ công |

---

## 🧪 Test Scenarios

### **Scenario 1: Test Full Flow**
```csharp
// 1. Load 100 mã
FDashboard.LoadCodesFromDatabase(100);

// 2. Bắt đầu production
// (Thực hiện manually hoặc qua UI)

// 3. Chạy simulator cho cả 2 camera
FDashboard.StartSimulator(0, 500);

// 4. Để simulator chạy tự động cho đến hết mã
// (Nó sẽ tự dừng khi queue empty)
```

### **Scenario 2: Test Riêng Từng Camera**
```csharp
// Test Camera Main trước
FDashboard.StartSimulator(1, 300);
// ... đợi xong ...
FDashboard.StopSimulator();

// Sau đó test Camera Sub
FDashboard.StartSimulator(2, 500);
```

### **Scenario 3: Test Mã Cụ Thể**
```csharp
// Gửi từng mã một để debug
FDashboard.SendTestCode("01234567890123", true);  // CMain
Thread.Sleep(1000);
FDashboard.SendTestCode("01234567890123", false); // CSub
```

---

## 🐛 Troubleshooting

### **Vấn đề: Simulator không gửi mã**
✅ Kiểm tra:
- Queue có mã không? → Gọi `GetSimulatorStatus()`
- Timer đã start chưa? → Xem log
- Production_State có đúng không? (đặc biệt với Camera Sub)

### **Vấn đề: Camera Sub skip tất cả mã**
✅ Nguyên nhân:
- `Production_State` không phải `Running`
- Hoặc `subpr.IsBusy` liên tục

✅ Giải pháp:
- Đảm bảo hệ thống đang `Running`
- Tăng interval để Camera Sub kịp xử lý

### **Vấn đề: Mã bị duplicate**
✅ Điều này là bình thường nếu:
- Bạn gửi cùng 1 mã nhiều lần
- Mã đã được activate trước đó

---

## 💡 Tips & Best Practices

1. **Load ít mã khi test lần đầu** (10-20 mã) để dễ debug
2. **Theo dõi console log** để hiểu flow xử lý
3. **Test từng camera riêng** trước khi test cả 2
4. **Dừng simulator trước khi thay đổi cấu hình**
5. **Backup database trước khi test** để tránh mất dữ liệu

---

## 📞 Hỗ Trợ

Nếu có vấn đề, kiểm tra:
1. Log file tại: `C:\Users\...\MASAN-SERIALIZATION\Logs\Pages\PDAlog.ptl`
2. Console log trong tab "Thông báo"
3. Database records để xác nhận mã đã được xử lý

---

**Chúc bạn test thành công! 🎉**
