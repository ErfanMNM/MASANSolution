# BẢNG THÔNG TIN SẢN XUẤT (PPOInfo)

> **Trang quản lý đơn hàng và vận hành sản xuất** — Phần mềm MASAN-SERIALIZATION

---

## 1. Tổng quan

Trang **Bảng Thông Tin Sản Xuất** (tên mã: `PPOInfo`) là giao diện trung tâm của hệ thống, nơi người vận hành:

- Chọn và nạp thông tin **đơn hàng sản xuất** (Production Order - PO)
- Xem toàn bộ thông tin chi tiết của đơn hàng: sản phẩm, số lượng, dây chuyền, ca làm...
- **Bắt đầu / Dừng** quá trình sản xuất
- Theo dõi **thống kê thời gian thực**: số sản phẩm đạt/lỗi, tình trạng gửi MES
- Chỉnh sửa **ngày sản xuất** (khi cần)
- Xử lý tình huống **thiếu sản phẩm** nghiêm trọng

---

## 2. Giao diện

```
┌──────────────────────────────────────────────────────────────────────┐
│                     🏭 BẢNG THÔNG TIN SẢN XUẤT                      │
├─────────────────────────────────┬────────────────────────────────────┤
│  Số yêu cầu (OrderNo)    [▼]   │  Nhà máy (Factory)      [...]     │
│  Ngày SX (ProductionDate)[📅]   │  Nhà xưởng (Site)       [...]     │
│  Số lô (LotNumber)       [...] │  Dây chuyền (Line)      [...]     │
│  Mã vạch (GTIN)          [...] │  Ca làm (Shift)          [...]     │
│  Sản lượng (OrderQty)     [...] │  Mã đơn (CustomerOrderNo)[...]    │
│  Tổng mã CZ đã nhận     [...] │  Số lượng đóng gói      [...]     │
│  Mã sản phẩm (ProductCode)[...] │                                │
│  Đơn vị sản phẩm (UOM)   [...] │                                │
├─────────────────────────────────┴────────────────────────────────────┤
│  Tên sản phẩm (ProductName): [..................................]   │
├──────────────────────────────────────────────────────────────────────┤
│  Thống kê cơ bản                                                 │
│  ┌──────────────┬──────────────────┬─────────────────┬──────────┐  │
│  │ Đã đóng thùng│ Sản phẩm loại   │ Gửi nhận MES OK │ [nhãn]   │  │
│  │    [...]      │    [...]         │      [...]       │          │  │
│  ├──────────────┼──────────────────┼─────────────────┼──────────┤  │
│  │ Đang chờ gửi │ Đang chờ gửi lại│ Đang chờ trả về │ [nhãn]   │  │
│  │    [...]      │    [...]         │      [...]       │          │  │
│  ├──────────────┼──────────────────┴─────────────────┴──────────┤  │
│  │ Thùng đang đóng / Dự kiến │ Thùng: X / Y (CS)             │  │
│  └───────────────────────────┴───────────────────────────────┘  │
├──────────────────────────────────────────────────────────────────────┤
│  [BẮT ĐẦU SẢN XUẤT] │ [Chọn PO] │ [Sửa Ngày SX] │ [RESET LỖI]     │
├──────────────────────────────────────────────────────────────────────┤
│  Tên sản phẩm: [...]                                    │ [Báo Cáo] │
│  ──────────────────────────────── trạng thái ─────────────────────── │
└──────────────────────────────────────────────────────────────────────┘
```

---

## 3. Thông tin hiển thị

### 3.1. Thông tin đơn hàng (Input - chỉnh sửa được khi chọn PO)

| Trường | Mô tả | Nguồn |
|--------|-------|-------|
| **Số yêu cầu (OrderNo)** | Mã đơn hàng sản xuất | MES - danh sách PO |
| **Ngày sản xuất** | Ngày giờ bắt đầu sản xuất | Người dùng chọn / lịch sử |

### 3.2. Thông tin đơn hàng (Output - chỉ đọc)

| Trường | Mô tả |
|--------|-------|
| **Nhà máy (Factory)** | Tên nhà máy thực hiện |
| **Nhà xưởng (Site)** | Vị trí / khu vực sản xuất |
| **Dây chuyền (Production Line)** | Dây chuyền sản xuất |
| **Ca làm (Shift)** | Ca sản xuất (sáng / chiều / đêm) |
| **Mã đơn (CustomerOrderNo)** | Mã đơn hàng từ khách hàng |
| **Sản lượng (OrderQty)** | Tổng số sản phẩm cần sản xuất |
| **Mã vạch (GTIN)** | Mã vạch sản phẩm |
| **Số lô (LotNumber)** | Số lô sản xuất |
| **Tổng mã CZ đã nhận** | Số mã serial đã nhận từ MES |
| **Mã sản phẩm (ProductCode)** | Mã SKU sản phẩm |
| **Tên sản phẩm (ProductName)** | Tên sản phẩm |
| **Đơn vị (UOM)** | Đơn vị tính (PCS, KG...) |
| **Số lượng đóng gói** | Số sản phẩm mỗi thùng (mặc định 24) |

### 3.3. Thống kê thời gian thực

| Trường | Màu | Mô tả |
|--------|-----|-------|
| **Đã đóng thùng** | Xanh lá | Tổng số sản phẩm đạt (Pass) |
| **Sản phẩm lỗi** | Đỏ | Tổng số sản phẩm lỗi (Fail/NotFound/ReadFail/Duplicate/Timeout) |
| **Gửi nhận hoàn thành MES** | Trắng | AWS: gửi thành công + nhận phản hồi |
| **Đang chờ gửi / Gửi lại** | Trắng | AWS: đang chờ gửi hoặc gửi thất bại |
| **Đang chờ trả về MES** | Trắng | AWS: đã gửi, chờ phản hồi |
| **Thùng đang đóng / Dự kiến** | Trắng | Thùng hiện tại và tổng số thùng |
| **Tổng mã CZ đã quét** | Trắng | Số mã đã quét vào hệ thống |

---

## 4. Các nút điều khiển

### 4.1. BẮT ĐẦU SẢN XUẤT / DỪNG

- **Trạng thái Ready**: Khởi động sản xuất — kiểm tra thiết bị, gửi PO xuống PLC
- **Trạng thái Running**: Dừng sản xuất — chờ queue xử lý xong
- **Kiểm tra trước khi chạy**:
  - `Globals.APP_Ready`: Ứng dụng đã sẵn sàng
  - `Globals.Device_Ready`: Thiết bị đã sẵn sàng

### 4.2. Chọn PO / Đổi PO

- Mở danh sách đơn hàng từ **MES**
- Chỉ cho phép đổi PO khi chưa có sản phẩm nào được quét
- **Auto Test Mode**: Nếu bật Test Mode, tự động tạo và chọn `PO Test`

### 4.3. Sửa Ngày SX

- Cho phép chỉnh ngày sản xuất **khi đơn hàng đang chạy**
- Lưu vào bảng `PO_Run_History` trong database

### 4.4. RESET LỖI

- Kích hoạt khi hệ thống phát hiện **thiếu sản phẩm nghiêm trọng** (`ThieuSanPham`)
- Chuyển sang trạng thái kiểm tra chi tiết (`KiemTraThieu`)

### 4.5. Báo Cáo

- Mở form báo cáo chi tiết cho đơn hàng đang chọn
- Hiển thị: danh sách sản phẩm, trạng thái AWS, lịch sử...

---

## 5. Trạng thái sản xuất

Hệ thống quản lý bởi `Globals.Production_State` — một FSM (Finite State Machine):

```
┌─────────────┐
│ NoSelectedPO│ ◄──────────────────┐
└──────┬──────┘                    │
       │ chọn PO                   │
       ▼                           │
    ┌───────┐                       │
    │ Start │                      │
    └───┬───┘                      │
        │ tự động / chọn PO        │
        ▼                          │
   ┌──────────┐                    │
   │ Loading  │                    │
   └────┬─────┘                    │
        │ delay 2s                 │
        ▼                          │
   ┌──────────┐                    │
   │  Saving  │──── lỗi ──────────┴──► (NoSelectedPO)
   └────┬─────┘
        │ thành công
        ▼
   ┌──────────┐
   │  Ready   │ ◄──────────────┐
   └──┬───────┘               │
      │ nhấn RUN              │
      ▼                       │
  ┌──────────┐               │
  │ Running  │─── đủ SL ───► │
  └────┬─────┘               │ đổi PO
       │ nhấn Dừng           │
       ▼                     │
 ┌────────────┐              │
 │CheckingQueue│──────────────┘
 └────┬───────┘
      │ queue rỗng
      ▼
   (Ready)
```

### 5.1. Các trạng thái chi tiết

| Trạng thái | Mô tả |
|------------|-------|
| `NoSelectedPO` | Chưa chọn đơn hàng |
| `Start` | Khởi tạo, tìm PO cuối cùng |
| `Loading` | Đang nạp thông tin PO (delay 2s) |
| `Saving` | Đang lưu dữ liệu PO vào database |
| `Ready` | Sẵn sàng sản xuất |
| `Running` | Đang chạy sản xuất |
| `Checking_Queue` | Đang chờ queue xử lý xong |
| `Completed` | Đơn hàng hoàn thành |
| `ThieuSanPham` | Phát hiện thiếu sản phẩm - cảnh báo nhấp nháy |
| `KiemTraThieu` | Kiểm tra chi tiết thiếu sản phẩm |
| `Pushing_new_PO_to_PLC` | Gửi PO mới xuống PLC |
| `Pushing_continue_PO_to_PLC` | Gửi PO tiếp tục xuống PLC |
| `Error` | Có lỗi xảy ra |

---

## 6. Xử lý thiếu sản phẩm

Khi hệ thống phát hiện số sản phẩm trong thùng ít hơn quy định (`ThieuSanPham`):

### TH1: Thùng hiện tại = 0 sản phẩm VÀ thùng trước chưa đầy
```
→ Reset thùng hiện tại về trạng thái "chưa đóng"
→ Xóa cảnh báo PLC
→ Đóng phần mềm
→ Ghi log cảnh báo
```

### TH2: Các trường hợp còn lại
```
→ Hiển thị cảnh báo nghiêm trọng
→ Yêu cầu dừng sản xuất
→ Liên hệ nhà cung cấp kiểm tra
```

---

## 7. Tương tác MES / Database

### 7.1. MES Integration

| Chức năng | Mô tả |
|-----------|-------|
| `MES_Load_OrderNo_ToComboBox` | Lấy danh sách PO từ MES |
| `ProductionOrder_Detail` | Lấy chi tiết đơn hàng |
| `Get_Unique_Code_MES_Count` | Đếm số mã serial trong PO |
| `Ensure_TestMode_PO` | Tạo PO Test nếu chưa có |

### 7.2. Local Database

| Chức năng | Mô tả |
|-----------|-------|
| `Check_Database_File` | Kiểm tra / tạo file SQLite cho PO |
| `Get_Records_CameraSub` | Lấy danh sách sản phẩm đã quét |
| `Get_Record_Count` | Đếm tổng sản phẩm |
| `Save_PO` | Lưu thông tin PO vào bảng `PO_Run_History` |

### 7.3. Queues (xử lý bất đồng bộ)

- `Insert_Product_To_Record_Queue` — Thêm sản phẩm mới
- `Update_Product_To_SQLite_Queue` — Cập nhật SQLite
- `Update_Product_To_Record_Carton_Queue` — Cập nhật thông tin thùng
- `AWS_Recive_Datas` — Dữ liệu phản hồi từ AWS
- `AWS_Send_Datas` — Dữ liệu gửi lên AWS

---

## 8. Cấu trúc code

```
PPOInfo.cs
├── Constructor & Initialization
│   ├── InitializeLogger()
│   ├── InitializeOrderNoComboBox()
│   ├── TryAutoSwitchToTestPO()
│   ├── InitializeBackgroundWorkers()
│   └── StartMainProcess()
│
├── Main Process Loop (BackgroundWorker)
│   ├── ProcessMainLoop()
│   └── ProcessProductionState()
│       ├── HandleNoSelectedPOState()
│       ├── HandleStartState()
│       ├── HandleLoadingState()
│       ├── HandleSavingState()
│       ├── HandleReadyState()
│       ├── HandleRunningState()
│       ├── HandleCompletedOrder()
│       ├── HandleThieuSanPhamState()
│       └── HandleKiemTraThieuState()
│
├── Button Event Handlers
│   ├── btnPO_Click()
│   ├── btnRUN_Click()
│   ├── btnProductionDate_Click()
│   └── btnReport_Click()
│
├── Data Operations
│   ├── RenderOrderInfo()
│   ├── LoadCountersAsync()
│   ├── CalculateCounters()
│   ├── ExecuteSavingProcess()
│   └── UpdateAWSCounters()
│
└── UI Helpers
    ├── UpdateStatusMessage()
    ├── SetEditMode()
    ├── ConfigurePreparingMode()
    └── RestoreAfterRunning()
```

---

## 9. Các thông số quan trọng

| Tham số | Giá trị mặc định | Mô tả |
|---------|-------------------|-------|
| `cartonPack` | 24 | Số sản phẩm mỗi thùng |
| `_processCounter` | 10 | Bộ đếm chu kỳ xử lý |
| `TestModePOName` | "PO Test" | Tên PO cho chế độ test |
| Loop delay | 100ms | Thời gian chờ mỗi vòng lặp |

---

## 10. Mã lỗi thường gặp

| Mã lỗi | Mô tả |
|---------|-------|
| `PP02` | Đơn hàng đã bị xóa |
| `PP03` | Đơn hàng đã hoàn thành |
| `PP04` | Số lượng mã MES gửi xuống chưa đủ |
| `PP05` | Lấy dữ liệu đơn hàng thất bại |
| `PP06` | Không lấy được thông tin đơn hàng |
| `EA001` | Lỗi database nghiêm trọng - liên hệ nhà cung cấp |
| `PP_TS01-06` | Lỗi khi xử lý thiếu sản phẩm |
