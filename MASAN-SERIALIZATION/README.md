# MASAN-SERIALIZATION

## Tài liệu Hoàn Chỉnh Dự Án

---

## Mục Lục

1. [Tổng Quan Dự Án](#1-tổng-quan-dự-án)
2. [Kiến Trúc Hệ Thống](#2-kiến-trúc-hệ-thống)
3. [Cấu Trúc Dự Án](#3-cấu-trúc-dự-án)
4. [Chi Tiết Các Module](#4-chi-tiết-các-module)
5. [Luồng Dữ Liệu Sản Xuất](#5-luồng-dữ-liệu-sản-xuất)
6. [Sơ Đồ Trạng Thái](#6-sơ-đồ-trạng-thái)
7. [Kết Nối Thiết Bị](#7-kết-nối-thiết-bị)
8. [Giao Tiếp AWS IoT Core](#8-giao-tiếp-aws-iot-core)
9. [Xử Lý Ngoại Lệ - Thiếu Sản Phẩm](#9-xử-lý-ngoại-lệ---thiếu-sản-phẩm)
10. [Cấu Hình Hệ Thống](#10-cấu-hình-hệ-thống)
11. [Hướng Dẫn Cài Đặt](#11-hướng-dẫn-cài-đặt)
12. [Xử Lý Sự Cố](#12-xử-lý-sự-cố)

---

## 1. Tổng Quan Dự Án

### 1.1 Mô Tả

**MASAN-SERIALIZATION** là ứng dụng Windows Forms (.NET Framework 4.8) được phát triển bằng C#, dùng để quản lý và theo dõi quá trình sản xuất, đặc biệt trong ngành thực phẩm và dược phẩm.

### 1.2 Mục Tiêu Chính

| Mục tiêu | Mô tả |
|-----------|--------|
| **Quản lý đơn hàng** | Tích hợp với hệ thống MES |
| **Đọc mã vạch** | Thông qua camera Hikvision |
| **Giao tiếp PLC** | Đồng bộ với thiết bị Omron |
| **Đồng bộ Cloud** | Gửi/nhận dữ liệu qua AWS IoT Core |
| **Quản lý thùng** | 24 sản phẩm mỗi thùng |

### 1.3 Công Nghệ Sử Dụng

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         CÔNG NGHỆ SỬ DỤNG                                │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐    │
│  │   .NET 4.8     │    │   C# 7.0+      │    │  Windows Forms   │    │
│  │   Framework     │    │   Ngôn ngữ      │    │  (UI)           │    │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘    │
│                                                                          │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐    │
│  │   Sunny UI      │    │  HslCommunication│   │   SQLite        │    │
│  │   (Thư viện UI) │    │   (Omron PLC)   │    │   (Database)   │    │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘    │
│                                                                          │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐    │
│  │  Hikvision SDK   │    │   AWS IoT Core   │    │   SpT.Logs      │    │
│  │   (Camera)      │    │   (MQTT)        │    │   (Logging)    │    │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘    │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### 1.4 Cấu Trúc Lưu Trữ Dữ Liệu

```
C:\MasanSerialization_v2\
├── Server_Service\
│   ├── po1.db                    # Database chính của PO
│   ├── data\                     # File JSON của orders
│   │   ├── PO001.json
│   │   └── PO002.json
│   └── codes_json\               # Mã unique theo GTIN
│       └── GTIN_8931234567890.json
│
├── PODatabases\                   # Database cục bộ theo PO
│   └── yyyy-MM\
│       └── GTIN\
│           ├── PO001.db
│           └── PO002.db
│
└── Databases\
    └── POLog.db                   # Nhật ký đơn hàng sản xuất
```

### 1.5 Kiến Trúc Lưu Trữ Mới (Theo GTIN)

```
Cấu trúc mới lưu trữ mã theo GTIN thay vì theo orderNo:

yyyy-MM/
└── GTIN_8931234567890/
    ├── PO001.db          # Database PO với codes từ GTIN
    ├── PO002.db          # Database PO khác cũng dùng GTIN
    ├── Records_PO001.db  # Bản ghi sản xuất
    ├── Cartons_PO001.db  # Thông tin cartons
    ├── Send_AWS_Record_PO001.db
    └── Recive_AWS_Record_PO001.db

Ưu điểm:
- Nhiều PO cùng GTIN dùng chung bộ mã
- Tiết kiệm không gian lưu trữ
- Dễ dàng quản lý theo sản phẩm
```

---

## 2. Kiến Trúc Hệ Thống

### 2.1 Kiến Trúc Tổng Quan

```
╔═══════════════════════════════════════════════════════════════════════════╗
║                     MASAN-SERIALIZATION - KIẾN TRÚC                     ║
╠═══════════════════════════════════════════════════════════════════════════╣
║                                                                            ║
║  ┌────────────────────────────────────────────────────────────────────┐ ║
║  │                        LỚP TRÌNH BÀY (PRESENTATION)                 │ ║
║  │  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐          │ ║
║  │  │Dashboard │  │ Thông tin│  │  AWS IoT │  │ Cài đặt │          │ ║
║  │  │          │  │ Sản xuất │  │          │  │          │          │ ║
║  │  │ • FDash  │  │ • PPOInfo│  │ • PAwsIot│  │ • PSett  │          │ ║
║  │  │ • PCarton│  │          │  │          │  │ • PLCSet │          │ ║
║  │  └──────────┘  └──────────┘  └──────────┘  └──────────┘          │ ║
║  └────────────────────────────────────────────────────────────────────┘ ║
║                                    │                                      ║
║                                    ▼                                      ║
║  ┌────────────────────────────────────────────────────────────────────┐ ║
║  │                      LỚP NGHIỆP VỤ (BUSINESS LOGIC)                │ ║
║  │  ┌────────────────┐  ┌────────────────┐  ┌────────────────────┐  │ ║
║  │  │ProductionHelper │  │CameraSubPlcSync │  │ ThieuSanPhamHelper  │  │ ║
║  │  │                │  │   V2Helper      │  │                     │  │ ║
║  │  │ • Load PO     │  │                 │  │ • Trigger Warning   │  │ ║
║  │  │ • Save PO     │  │ • Sync Camera   │  │ • Write PLC Alarm  │  │ ║
║  │  │ • Process     │  │   & PLC         │  │ • Blink Text       │  │ ║
║  │  └────────────────┘  └────────────────┘  └────────────────────┘  │ ║
║  │  ┌────────────────────────────────────────────────────────────────┐  │ ║
║  │  │                        Globals                                  │  │ ║
║  │  │  • CurrentUser    • ProductionData   • Camera States          │  │ ║
║  │  │  • AppState        • PLC Connections  • AWS Status           │  │ ║
║  │  └────────────────────────────────────────────────────────────────┘  │ ║
║  └────────────────────────────────────────────────────────────────────┘ ║
║                                    │                                      ║
║                                    ▼                                      ║
║  ┌────────────────────────────────────────────────────────────────────┐ ║
║  │                       LỚP TRUY CẬP DỮ LIỆU (DATA ACCESS)           │ ║
║  │  ┌────────────┐  ┌────────────┐  ┌────────────┐  ┌────────────┐  │ ║
║  │  │   MES      │  │    PLC     │  │   Camera   │  │    AWS     │  │ ║
║  │  │  Server   │  │  (Omron)  │  │ (Hikvision)│  │  IoT Core  │  │ ║
║  │  │            │  │            │  │            │  │            │  │ ║
║  │  │ • SQLite   │  │ • FINS/TCP│  │ • SDK      │  │ • MQTT     │  │ ║
║  │  │ • JSON     │  │ • Port 9600│ │ • Ethernet │  │ • TLS 1.2  │  │ ║
║  │  └────────────┘  └────────────┘  └────────────┘  └────────────┘  │ ║
║  │  ┌─────────────────────────────────────────────────────────────┐  │ ║
║  │  │                        SQLite DB                              │  │ ║
║  │  │  • ProductionCodeData      • ProductionCartonData          │  │ ║
║  │  │  • AWS_Send_Data           • AWS_Recive_Data                │  │ ║
║  │  └─────────────────────────────────────────────────────────────┘  │ ║
║  └────────────────────────────────────────────────────────────────────┘ ║
║                                                                            ║
╚═══════════════════════════════════════════════════════════════════════════╝
```

### 2.2 Kiến Trúc Đa Tầng Chi Tiết

```
┌────────────────────────────────────────────────────────────────────────────────┐
│                              LỚP ỨNG DỤNG (APPLICATION)                               │
│                              ════════════════════════                              │
│                                                                                │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │  FMain.cs - Form Chính của Ứng Dụng                                     │  │
│  │  ┌──────────────────────────────────────────────────────────────────┐ │  │
│  │  │  • InitializeComponent()        - Khởi tạo UI components         │ │  │
│  │  │  • InitializeUI()              - Setup Sunny UI styles           │ │  │
│  │  │  • InitializeConfigs()          - Load configuration files        │ │  │
│  │  │  • RenderControlForm()          - Tạo navigation menu            │ │  │
│  │  │  • Start_Main_Process_Task()    - Start background workers       │ │  │
│  │  │  • Main_Process_Async()         - Main application loop         │ │  │
│  │  │  • Login_Process()              - Handle login state machine     │ │  │
│  │  │  • App_State_Process()          - Process application states     │ │  │
│  │  └──────────────────────────────────────────────────────────────────┘ │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
│                                                                                │
└────────────────────────────────────────────────────────────────────────────────┘
                                         │
                                         ▼
┌────────────────────────────────────────────────────────────────────────────────┐
│                              LỚP GIAO DIỆN (VIEW)                                   │
│                              ══════════════════                                    │
│                                                                                │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │  ┌────────────┐  ┌────────────┐  ┌────────────┐  ┌────────────┐     │  │
│  │  │ Dashboards │  │ Production │  │   AWS     │  │  Settings  │     │  │
│  │  │            │  │   Info     │  │           │  │            │     │  │
│  │  ├────────────┤  ├────────────┤  ├────────────┤  ├────────────┤     │  │
│  │  │ FDashboard │  │  PPOInfo   │  │  PAwsIot  │  │  PSettings │     │  │
│  │  │ PCartonDash│  │            │  │            │  │  PLCSetting│     │  │
│  │  └────────────┘  └────────────┘  └────────────┘  └────────────┘     │  │
│  │                                                                      │  │
│  │  ┌────────────┐  ┌────────────┐  ┌────────────┐                     │  │
│  │  │  SCADA     │  │   Login    │  │ Database  │                     │  │
│  │  │            │  │            │  │           │                     │  │
│  │  │PStatictis  │  │   PLogin   │  │  DBBrowser│                     │  │
│  │  │            │  │            │  │ CheckVIP  │                     │  │
│  │  └────────────┘  └────────────┘  └────────────┘                     │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
│                                                                                │
└────────────────────────────────────────────────────────────────────────────────┘
                                         │
                                         ▼
┌────────────────────────────────────────────────────────────────────────────────┐
│                           LỚP NGHIỆP VỤ (BUSINESS LOGIC)                           │
│                           ════════════════════════════                             │
│                                                                                │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │  ProductionOrder - Nghiệp Vụ Sản Xuất Chính                          │  │
│  │  ┌──────────────────────────────────────────────────────────────────┐ │  │
│  │  │  public class ProductionOrder                                       │ │  │
│  │  │  ├── orderNo, orderQty, productName, gtin, ...  (Properties)    │ │  │
│  │  │  ├── getfromMES: GetfromMES              (Data Access)           │ │  │
│  │  │  ├── getDataPO: GetDataPO                (Data Access)           │ │  │
│  │  │  └── setDB: PostDB                        (Data Operations)     │ │  │
│  │  └──────────────────────────────────────────────────────────────────┘ │  │
│  │                                                                        │  │
│  │  ┌──────────────────────────────────────────────────────────────────┐ │  │
│  │  │  Inner Classes:                                                    │ │  │
│  │  │  ├── GetfromMES       - Đọc dữ liệu từ MES/JSON                  │ │  │
│  │  │  ├── GetDataPO        - Đọc dữ liệu PO cục bộ                  │ │  │
│  │  │  ├── PostDB           - Ghi dữ liệu                              │ │  │
│  │  │  ├── Product_Counter  - Bộ đếm sản lượng                         │ │  │
│  │  │  └── AWS_*_Counter   - Bộ đếm AWS                               │ │  │
│  │  └──────────────────────────────────────────────────────────────────┘ │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
│                                                                                │
└────────────────────────────────────────────────────────────────────────────────┘
                                         │
                                         ▼
┌────────────────────────────────────────────────────────────────────────────────┐
│                           LỚP DỮ LIỆU (DATA/INTEGRATION)                          │
│                           ════════════════════════════                           │
│                                                                                │
│  ┌─────────────────────┐  ┌─────────────────────┐  ┌─────────────────────┐  │
│  │   MES SERVER        │  │   PLC OMRON         │  │   AWS IOT CORE      │  │
│  │   (Local SQLite)    │  │   (HslCommunication)│  │   (MQTT)            │  │
│  │                      │  │                     │  │                      │  │
│  │ • po1.db           │  │ • FINS/TCP         │  │ • Publish           │  │
│  │ • codes/*.db       │  │ • Port 9600        │  │ • Subscribe          │  │
│  │ • data/*.json      │  │ • Read/Write      │  │ • TLS 1.2          │  │
│  │ • codes_json/*.json │  │ • 2 instances     │  │ • Topics           │  │
│  │                      │  │                     │  │                      │  │
│  └─────────────────────┘  └─────────────────────┘  └─────────────────────┘  │
│                                                                                │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │                         HỆ THỐNG CAMERA (Hikvision)                       │  │
│  │  ┌─────────────────────┐              ┌─────────────────────┐           │  │
│  │  │   Camera Chính      │              │   Camera Phụ         │           │  │
│  │  │                     │              │                     │           │  │
│  │  │ • Product Barcode   │              │ • Carton Barcode    │           │  │
│  │  │ • PLC Sync         │              │ • PLC Sync          │           │  │
│  │  │ • Duplicate Check │              │ • Carton Activation │           │  │
│  │  └─────────────────────┘              └─────────────────────┘           │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
│                                                                                │
└────────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Cấu Trúc Dự Án

### 3.1 Cấu Trúc Thư Mục

```
MASAN-SERIALIZATION/
│
├── 📄 MASAN-SERIALIZATION.csproj         # File dự án (.NET 4.8)
├── 📄 packages.config                      # Dependencies NuGet
│
├── 📄 FMain.cs                            # Form chính - Điều phối ứng dụng
├── 📄 FMain.Designer.cs                   # Designer UI
├── 📄 FMain.resx                          # Resources
│
├── 📂 Views/                              # Giao diện người dùng (Pages)
│   │
│   ├── 📂 Dashboards/
│   │   ├── FDashboard.cs                  # Dashboard chính
│   │   ├── FDashboard.Designer.cs
│   │   ├── FDashboard.resx
│   │   ├── PCartonDashboard.cs            # Dashboard thùng
│   │   ├── PCartonDashboard.Designer.cs
│   │   └── PCartonDashboard.resx
│   │
│   ├── 📂 ProductionInfo/
│   │   ├── PPOInfo.cs                    # Thông tin đơn hàng sản xuất
│   │   ├── PPOInfo.Designer.cs
│   │   └── PPOInfo.resx
│   │
│   ├── 📂 SCADA/
│   │   ├── PStatictis.cs                 # Thống kê sản xuất
│   │   ├── PStatictis.Designer.cs
│   │   └── PStatictis.resx
│   │
│   ├── 📂 AWS/
│   │   ├── PAwsIot.cs                   # Kết nối AWS IoT Core
│   │   ├── PAwsIot.Designer.cs
│   │   ├── PAwsIot.resx
│   │   ├── PAws.cs                      # Phiên bản thay thế
│   │   └── PAws.Designer.cs
│   │
│   ├── 📂 Settings/
│   │   ├── PSettings.cs                  # Cài đặt ứng dụng
│   │   ├── PSettings.Designer.cs
│   │   ├── PLCSetting.cs                 # Cài đặt PLC
│   │   ├── PLCSetting.Designer.cs
│   │   └── *.resx
│   │
│   ├── 📂 Login/
│   │   ├── PLogin.cs                     # Trang đăng nhập
│   │   ├── PLogin.Designer.cs
│   │   └── *.resx
│   │
│   ├── 📂 Database/
│   │   ├── DBBrowser.cs                  # Trình duyệt Database
│   │   ├── DBBrowser.Designer.cs
│   │   ├── PScaner.cs                   # Scanner
│   │   ├── PCodeSearch.cs               # Tìm kiếm mã
│   │   ├── POrderNoViewer.cs            # Xem PO
│   │   ├── CheckVIP.cs                   # Kiểm tra VIP
│   │   └── *.Designer.cs, *.resx
│   │
│   ├── 📂 Reports/
│   │   ├── ProductionReportForm.cs       # Báo cáo sản xuất
│   │   └── ProductionReportForm.Designer.cs
│   │
│   └── 📂 Test/
│       ├── FormTest.cs
│       ├── FormTest.Designer.cs
│       ├── DemoPage.cs
│       └── DemoPage.Designer.cs
│
├── 📂 Infrastructure/
│   └── Globals.cs                        # Biến toàn cục & Dữ liệu chia sẻ
│
├── 📂 Production/
│   └── ProductionHelper.cs                # Nghiệp vụ sản xuất
│
├── 📂 Helpers/                            # Các class helper
│   ├── CameraSubPlcSyncV2Helper.cs       # Đồng bộ Camera & PLC v2
│   └── ThieuSanPhamHelper.cs             # Xử lý thiếu sản phẩm
│
├── 📂 Configs/
│   └── IniConfigs.cs                      # Cấu hình INI/JSON
│
├── 📂 Utils/
│   ├── Extension.cs                       # Phương thức mở rộng
│   ├── ErrorCodes.cs                      # Mã lỗi
│   ├── ErrorMigrationHelper.cs             # Di chuyển lỗi
│   └── LogExtensions.cs                   # Extensions logging
│
├── 📂 Enums/
│   ├── eState.cs                          # Trạng thái ứng dụng/camera
│   ├── eResult.cs                         # Loại kết quả
│   ├── eData.cs                           # Loại dữ liệu
│   └── SystemLog.cs                        # Loại log hệ thống
│
├── 📂 Properties/
│   └── Settings.Designer.cs               # Settings ứng dụng
│
└── 📂 obj/, bin/                          # Build output (bỏ qua)
```

### 3.2 File Chính và Trách Nhiệm

| File | Class Chính | Trách nhiệm |
|------|-------------|-------------|
| `FMain.cs` | `FMain : UIForm` | Điều phối ứng dụng, state machine |
| `PPOInfo.cs` | `PPOInfo : UIPage` | Quản lý đơn hàng sản xuất, workflow |
| `FDashboard.cs` | `FDashboard : UIForm` | Dashboard chính, monitoring |
| `PAwsIot.cs` | `PAwsIot : UIPage` | Giao tiếp AWS IoT Core |
| `ProductionHelper.cs` | `ProductionOrder` | Nghiệp vụ sản xuất |
| `Globals.cs` | `Globals` | Biến toàn cục dùng chung |
| `IniConfigs.cs` | `AppConfigs` | Cấu hình hệ thống |
| `ThieuSanPhamHelper.cs` | `ThieuSanPhamHelper` | Xử lý ngoại lệ thiếu sản phẩm |

---

## 4. Chi Tiết Các Module

### 4.1 FMain - Bộ Điều Phối Chính

```csharp
// FMain.cs - Form Chính
public partial class FMain : UIForm
{
    // Các trang của ứng dụng
    private PLogin _pLogin = new PLogin();
    private FDashboard _pDashboard = new FDashboard();
    private PPOInfo _pProduction = new PPOInfo();
    private PStatictis _pStatictis = new PStatictis();
    private PCartonDashboard _pCartonDashboard = new PCartonDashboard();
    private PAwsIot _pAws = new PAwsIot();
    private PSettings _pSettings = new PSettings();
    private PLCSetting _pPLCSetting = new PLCSetting();
    private DBBrowser _pDBBrowser = new DBBrowser();
    
    // Background Worker cho tiến trình chính
    private BackgroundWorker WK_Main_Proccess = new BackgroundWorker();
    
    // Luồng khởi tạo:
    // 1. InitializeComponent()
    // 2. InitializeUI() - Styles Sunny UI
    // 3. InitializeConfigs() - Load cấu hình
    // 4. RenderControlForm() - Tạo menu navigation
    // 5. Start_Main_Process_Task() - Khởi động vòng lặp chính
    // 6. InitializePage() - Khởi tạo từng trang
}
```

**Trách nhiệm:**
- Quản lý trạng thái ứng dụng (Login → Active → Deactive)
- Điều hướng giữa các trang
- Vòng lặp xử lý chính
- Quản lý vòng đời ứng dụng

### 4.2 PPOInfo - Quản Lý Sản Xuất

```csharp
// PPOInfo.cs - Trang Thông Tin Sản Xuất
public partial class PPOInfo : UIPage
{
    // Trạng thái sản xuất
    public enum e_Production_State
    {
        NoSelectedPO,      // Chưa chọn PO
        Start,             // Khởi động
        Loading,           // Đang tải PO
        Saving,            // Đang lưu
        Ready,             // Sẵn sàng sản xuất
        Running,           // Đang sản xuất
        Completed,         // Hoàn thành
        Editing,           // Chế độ chỉnh sửa
        Error,             // Lỗi
        ThieuSanPham,     // Phát hiện thiếu sản phẩm
        KiemTraThieu,      // Kiểm tra thiếu sản phẩm
        Camera_Processing, // Xử lý camera
        MaBiTrung,         // Phát hiện mã trùng
        Pause,             // Tạm dừng
        Waiting_Stop       // Chờ dừng
    }
    
    // Luồng xử lý chính:
    // 1. Load PO từ MES (ComboBox)
    // 2. Save PO vào database cục bộ
    // 3. Monitor trạng thái liên tục
    // 4. Xử lý khi user nhấn RUN/STOP
}
```

**Trách nhiệm:**
- Tải đơn hàng sản xuất từ MES
- Lưu thông tin PO
- Khởi động/dừng sản xuất
- Giám sát trạng thái sản xuất
- Xử lý ngoại lệ (thiếu sản phẩm)

### 4.3 ProductionOrder - Nghiệp Vụ Sản Xuất

```csharp
// ProductionOrder - Class chính của sản xuất
public class ProductionOrder
{
    // Properties PO
    public string orderNo { get; set; }
    public string orderQty { get; set; }
    public string gtin { get; set; }
    public string productName { get; set; }
    // ... các properties khác
    
    // Bộ đếm
    public Product_Counter counter { get; set; }
    public AWS_Send_Counter awsSendCounter { get; set; }
    public AWS_Recived_Counter awsRecivedCounter { get; set; }
    
    // Truy cập dữ liệu
    public GetfromMES getfromMES { get; }     // Đọc MES/JSON
    public GetDataPO getDataPO { get; }       // Đọc DB cục bộ
    public PostDB setDB { get; set; }         // Ghi
    
    // Classes nội bộ
    public class GetfromMES {
        public TResult ProductionOrder_Detail(string orderNo);
        public TResult Get_Unique_Code_MES_Count(string orderNo);
        public TResult Get_Unique_Codes_MES(string orderNo);
    }
    
    public class GetDataPO {
        public TResult Get_Records_CameraSub(string orderNo);
        public TResult Get_Record_Count(string orderNo);
        public TResult Get_Carton_Info(string orderNo);
    }
}
```

### 4.4 PAwsIot - Giao Tiếp AWS IoT

```csharp
// PAwsIot.cs - Giao Tiếp AWS IoT Core
public partial class PAwsIot : UIPage
{
    public AwsIotClientHelper awsClient;
    
    // Cấu hình AWS
    string host = "a22qv9bgjnbsae-ats.iot.ap-southeast-1.amazonaws.com";
    string clientId = "MIPWP501";
    string rootCAPath = @"C:\MIPWP501\AmazonRootCA1.pem";
    string pfxPath = @"C:\MIPWP501\client-certificate.pfx";
    
    // Topics MQTT
    string topicPub = "CZ/data";           // Publish
    string topicSub = "CZ/MIPWP501/response"; // Subscribe
    
    // Cấu trúc Payload
    public class AWSSendPayload {
        public string message_id;
        public string orderNo;
        public string uniqueCode;
        public string gtin;
        public string cartonCode;
        public int status;
        public string activate_datetime;
        public string production_date;
        public string thing_name;
    }
}
```

### 4.5 ThieuSanPhamHelper - Xử Lý Ngoại Lệ

```csharp
// ThieuSanPhamHelper.cs
public static class ThieuSanPhamHelper
{
    // Kích hoạt cảnh báo thiếu sản phẩm
    public static void TriggerThieuSanPhamWarning(Control opTer);
    
    // Dừng cảnh báo
    public static void StopThieuSanPhamWarning();
    
    // Ghi alarm PLC
    private static void WriteAlarmToPLC();
    
    // Xóa alarm PLC
    public static void ClearPLCAlarm();
}
```

### 4.6 Globals - Biến Toàn Cục

```csharp
// Globals.cs
public static class Globals
{
    // User hiện tại
    public static UserData CurrentUser { get; set; }
    
    // Trạng thái ứng dụng
    public static e_App_State AppState { get; set; }
    public static e_App_Render_State AppRenderState { get; set; }
    public static bool ACTIVE_State { get; set; }
    
    // Trạng thái sản xuất
    public static e_Production_State Production_State { get; set; }
    public static ProductionOrder ProductionData { get; set; }
    
    // Trạng thái thiết bị
    public static bool PLC_Connected { get; set; }
    public static bool PLC_Connected_02 { get; set; }
    public static e_Camera_State CameraMain_State { get; set; }
    public static e_Camera_State CameraSub_State { get; set; }
    public static e_awsIot_status AWS_IoT_Status { get; set; }
    
    // Instances PLC
    public static OmronPLC_Hsl PLC_Instance { get; set; }
    public static OmronPLC_Hsl PLC_Instance_02 { get; set; }
}

// Database toàn cục
public static class Globals_Database
{
    public static Dictionary<string, ProductionCodeData> Dictionary_ProductionCode_Data;
    public static Dictionary<int, ProductionCartonData> Dictionary_ProductionCarton_Data;
    
    // Hàng đợi xử lý
    public static Queue<...> Update_Product_To_SQLite_Queue;
    public static Queue<...> Insert_Product_To_Record_Queue;
    public static Queue<...> aWS_Recive_Datas;
    public static Queue<...> aWS_Send_Datas;
    public static Queue<string> Activate_Carton;
}
```

### 4.7 AppConfigs - Cấu Hình

```csharp
// IniConfigs.cs
[ConfigFile("Configs\\MSC.ini")]
public class AppConfigs : IniConfig<AppConfigs>
{
    [ConfigSection("APP")]
    
    // Camera
    public string Camera_Main_IP { get; set; }
    public int Camera_Main_Port { get; set; }
    public bool CameraMain_DuplicateReject_Enabled { get; set; }
    
    // AWS
    public bool AWS_ENA { get; set; }
    public string host { get; set; }
    public string clientId { get; set; }
    public string rootCAPath { get; set; }
    public string pfxPath { get; set; }
    public bool Auto_Send_AWS { get; set; }
    public bool AWS_Dev_Mode { get; set; }
    
    // Carton
    public int cartonPack { get; set; }     // 24 sản phẩm/thùng
    public int cartonOfset { get; set; }
    public int cartonWarning { get; set; }
    public bool cartonAutoStart { get; set; }
    public bool cartonScaner_Only_Once { get; set; }
    
    // PLC
    public bool PLC_Test_Mode { get; set; }
    public bool PLC_Duo_Mode { get; set; }
    public string PLC_Address_Sheet_Name { get; set; }
    
    // CameraSub Timeout
    public bool CameraSub_Timeout_Enabled { get; set; }
    public int CameraSub_Timeout_Ms { get; set; }
    public int CameraSub_Polling_Interval_Ms { get; set; }
    
    // Các chế độ
    public bool TestMode { get; set; }
    public bool Check_Duplica_Enabled { get; set; }
    public bool Check_Db_Old_Active { get; set; }
    public bool Check_Db_Old_Bypass { get; set; }
}
```

---

## 5. Luồng Dữ Liệu Sản Xuất

### 5.1 Luồng Toàn Cục

```
┌────────────────────────────────────────────────────────────────────────────────┐
│                         LUỒNG DỮ LIỆU SẢN XUẤT                                   │
│                         ════════════════════                                   │
└────────────────────────────────────────────────────────────────────────────────┘

     ┌─────────────────┐
     │       MES       │  📡 HTTP/REST (Local Files)
     │     Server      │──────────────────┐
     └────────┬────────┘                  │
              │                           │
              │ • orderNo                 │   ┌──────────────────────────┐
              │ • orderQty                 │◄──│ 1. Load Order from MES   │
              │ • productInfo             │   └──────────────────────────┘
              │ • gtin                   │                │
              │ • shift                  │                ▼
              │ • factory                │        ┌─────────────────┐
              ▼                          │        │  Local Storage   │
     ┌─────────────────┐          │        │  (JSON Files)   │
     │  Production     │          │        └────────┬────────┘
     │    Order       │          │                 │
     └────────┬────────┘          │                 ▼
              │                  │        ┌─────────────────┐
              ▼                  │        │    SQLite       │
     ┌─────────────────┐          │        │   Database      │
     │   Validation    │          │        └────────┬────────┘
     │    & Setup      │          │                 │
     └────────┬────────┘          │                 ▼
              │                  │        ┌─────────────────┐
              ▼                  │        │   Dictionary    │
     ┌─────────────────┐          │        │   (Code Cache) │
     │    PLC Device    │◄─────────┼────────│ Camera Main     │
     │    (Omron)     │  Sync   │        └────────┬────────┘
     └────────┬────────┘          │                 │
              │                  │                 │ Product Code
              │ PLC Counter      │                 │
              │ Alarm Signal     │                 ▼
              ▼                  │        ┌─────────────────┐
     ┌─────────────────┐          │        │   Camera Sub    │
     │   Production     │◄─────────┼────────│  (Carton Scan)  │
     │    Processing    │  Sync   │        └────────┬────────┘
     └────────┬────────┘          │                 │
              │                  │                 │ Carton Code
              │                  │                 ▼
              ▼                  │        ┌─────────────────────────────────┐
     ┌─────────────────┐          │        │      XÁC MINH MÃ               │        │
     │       │        │          │        ├───────────────────────────────────┤        │
     │       │        │          │        │                                   │        │
     │       ▼        ▼        │        │  ┌─────────────────────────────┐  │        │
     │  ┌─────────┐  ┌─────────┐ │        │  │ 1. Kiểm tra Dictionary     │  │        │
     │  │  PASS   │  │  FAIL   │ │        │  │    - Có tồn tại?         │  │        │
     │  │   ✓     │  │   ✗     │ │        │  │    - Đã được sử dụng?    │  │        │
     │  └───┬─────┘  └───┬─────┘ │        │  └─────────────┬─────────────┘  │        │
     │      │            │        │        │                │                │        │
     │      │            │        │        │      ┌─────────┴─────────┐      │        │
     │      ▼            │        │        │      ▼                   ▼      │        │
     │  ┌─────────┐      │        │        │  ┌─────────┐       ┌─────────┐ │        │
     │  │ CARTON  │      │        │        │  │  PASS   │       │  FAIL   │ │        │
     │  │MANAGEMENT│     │        │        │  │   ✓     │       │   ✗     │ │        │
     │  └────┬────┘      │        │        │  └───┬─────┘       └────┬─────┘ │        │
     │       │           │        │        │      │                   │      │        │
     │       │           │        │        │      ▼                   │      │        │
     │       ▼           │        │        │  ┌─────────┐              │      │        │
     │  ┌─────────┐      │        │        │  │ CARTON  │              │      │        │
     │  │  AWS    │      │        │        │  │MANAGEMENT│             │      │        │
     │  │ Publish │      │        │        │  └────┬────┘              │      │        │
     │  └─────────┘      │        │        │       │                   │      │        │
     │                   │        │        │       ▼                   │      │        │
     │                   │        │        │  ┌─────────┐              │      │        │
     │                   │        │        │  │   AWS   │              │      │        │
     │                   │        │        │  │ Publish │              │      │        │
     │                   │        │        │  └────┬────┘              │      │        │
     │                   │        │        │       │                   │      │        │
     └───────────────────┼────────┼────────┼───────┼───────────────────┼──────┘        │
                         │        │        │       │                   │               │
                         ▼        ▼        ▼       ▼                   ▼               ▼
     ┌─────────────────────────────────────────────────────────────────────────────┐
     │                          LỚP DATABASE                                        │
     ├─────────────────────────────────────────────────────────────────────────────┤
     │  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────────────┐  │
     │  │  Records.db       │  │  Cartons.db      │  │  AWS_Send_Record.db     │  │
     │  │                   │  │                   │  │                          │  │
     │  │  • ID (PK)       │  │  • cartonID (PK) │  │  • message_id           │  │
     │  │  • orderNo       │  │  • orderNo       │  │  • orderNo              │  │
     │  │  • code          │  │  • cartonCode    │  │  • uniqueCode           │  │
     │  │  • cartonCode    │  │  • Start_Date    │  │  • status               │  │
     │  │  • cartonID (FK) │  │  • Activate_Date │  │  • activate_datetime    │  │
     │  │  • Status        │  │  • Products      │  │  • send_datetime        │  │
     │  │  • PLCStatus     │  │                  │  │                          │  │
     │  └──────────────────┘  └──────────────────┘  └──────────────────────────┘  │
     └─────────────────────────────────────────────────────────────────────────────┘
                                   │
                                   ▼
     ┌─────────────────────────────────────────────────────────────────────────────┐
     │                          AWS IOT CORE                                         │
     ├─────────────────────────────────────────────────────────────────────────────┤
     │                                                                             │
     │   Topic: CZ/data (Production)                                               │
     │   ◄── Publish: {message_id, orderNo, uniqueCode, gtin, cartonCode, ...}   │
     │                                                                             │
     │   Topic: CZ/{clientId}/response (Reply)                                     │
     │   ──► Subscribe: {status, message_id, error_message}                        │
     │                                                                             │
     └─────────────────────────────────────────────────────────────────────────────┘
```

### 5.2 Cấu Trúc Thùng Carton

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           CẤU TRÚC THÙNG CARTON                              │
│                           ════════════════                                   │
│                                                                             │
│   ┌─────────────────────────────────────────────────────────────────────┐   │
│   │                        THÙNG CARTON (24 sản phẩm)                       │   │
│   │                                                                        │   │
│   │   ┌────┐ ┌────┐ ┌────┐ ┌────┐                                        │   │
│   │   │ P1 │ │ P2 │ │ P3 │ │ P4 │   ← Hàng 1 (4 sản phẩm)               │   │
│   │   └────┘ └────┘ └────┘ └────┘                                        │   │
│   │   ┌────┐ ┌────┐ ┌────┐ ┌────┐                                        │   │
│   │   │ P5 │ │ P6 │ │ P7 │ │ P8 │   ← Hàng 2                             │   │
│   │   └────┘ └────┘ └────┘ └────┘                                        │   │
│   │   ┌────┐ ┌────┐ ┌────┐ ┌────┐                                        │   │
│   │   │ P9 │ │P10 │ │P11 │ │P12 │   ← Hàng 3                             │   │
│   │   └────┘ └────┘ └────┘ └────┘                                        │   │
│   │   ┌────┐ ┌────┐ ┌────┐ ┌────┐                                        │   │
│   │   │P13 │ │P14 │ │P15 │ │P16 │   ← Hàng 4                             │   │
│   │   └────┘ └────┘ └────┘ └────┘                                        │   │
│   │   ┌────┐ ┌────┐ ┌────┐ ┌────┐                                        │   │
│   │   │P17 │ │P18 │ │P19 │ │P20 │   ← Hàng 5                             │   │
│   │   └────┘ └────┘ └────┘ └────┘                                        │   │
│   │   ┌────┐ ┌────┐ ┌────┐ ┌────┐                                        │   │
│   │   │P21 │ │P22 │ │P23 │ │P24 │   ← Hàng 6 (sản phẩm cuối)            │   │
│   │   └────┘ └────┘ └────┘ └────┘                                        │   │
│   │                                                                        │   │
│   │   Mã Thùng: [CTN-2026-08-03-001]  ← Được quét bởi Camera Sub          │   │
│   │                                                                        │   │
│   │   ─────────────────────────────────────────────────────────────────   │   │
│   │   CartonID: 1                                                         │   │
│   │   Người kích hoạt: "Operator01"                                       │   │
│   │   Thời gian bắt đầu: "2026-08-03 14:30:00"                         │   │
│   │   Thời gian kích hoạt: "2026-08-03 14:35:00"                        │   │
│   └─────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 6. Sơ Đồ Trạng Thái

### 6.1 Máy Trạng Thái Ứng Dụng

```
┌────────────────────────────────────────────────────────────────────────────────┐
│                     MÁY TRẠNG THÁI ỨNG DỤNG                                       │
│                     ════════════════════════════                                  │
└────────────────────────────────────────────────────────────────────────────────┘

                              ┌───────────────────┐
                              │                   │
                              │      START        │
                              │   (Khởi động)    │
                              │                   │
                              └─────────┬─────────┘
                                        │
                                        ▼
                              ┌───────────────────┐
                              │                   │
                              │      LOGIN        │
                              │ (Chưa đăng nhập)  │
                              │                   │
                              │ lblAllStatus:     │
                              │ "Chưa đăng nhập" │
                              │ FillColor: 🔴    │
                              └─────────┬─────────┘
                                        │
                              Đăng nhập thành công
                              CurrentUser ≠ ""
                                        │
                                        ▼
                              ┌───────────────────┐
                              │                   │
                              │     DEACTIVE      │
                              │ (ACTIVE_State =   │
                              │      false)       │
                              │                   │
                              │ Hiển thị login    │
                              │ nhưng không cho   │
                              │ thao tác          │
                              └─────────┬─────────┘
                                        │
                              ACTIVE_State = true
                                        │
                                        ▼
                              ┌───────────────────┐
                              │                   │
                              │      ACTIVE       │
                              │  (Hoạt động)     │
                              │                   │
                              │ NavMenu.Enabled   │
                              └─────────┬─────────┘
                                        │
              ┌─────────────────────────┼─────────────────────────┐
              │                         │                         │
              ▼                         ▼                         ▼
    ┌──────────────────┐      ┌──────────────────┐      ┌──────────────────┐
    │                  │      │                  │      │                  │
    │      READY       │      │     RUNNING      │      │    COMPLETED     │
    │   (Sẵn sàng)    │─────►│   (Đang sản xuất) │      │  (Hoàn thành)    │
    │                  │ RUN  │                  │ STOP │                  │
    └──────────────────┘      └──────────────────┘      └──────────────────┘
              ▲                         │
              │                         │
              │         ┌───────────────┘
              │         │
              │         ▼
              │   ┌──────────────────┐
              └───│   CHECKING       │
                  │     QUEUE        │
                  │ (Kiểm tra đợi)  │
                  └──────────────────┘


    ════════════════════════════════════════════════════════════════════════════
    CÁC TRẠNG THÁI SẢN XUẤT:
    ════════════════════════════════════════════════════════════════════════════

    ┌──────────────────────┬────────────────────────────────────────────┐
    │   Production_State    │                    Mô tả                     │
    ├──────────────────────┼────────────────────────────────────────────┤
    │   NoSelectedPO        │ Chưa chọn PO                             │
    │   Start               │ Khởi động ứng dụng                       │
    │   Loading             │ Đang tải PO                              │
    │   Saving             │ Đang lưu                                 │
    │   Ready              │ Sẵn sàng sản xuất                        │
    │   Running            │ Đang sản xuất                            │
    │   Completed          │ PO hoàn thành                            │
    │   Editing            │ Chế độ chỉnh sửa                         │
    │   Error              │ Lỗi sản xuất                             │
    │   ThieuSanPham       │ ⚠️ Phát hiện thiếu sản phẩm             │
    │   KiemTraThieu        │ Kiểm tra thiếu sản phẩm                  │
    │   Camera_Processing   │ Đang xử lý camera                        │
    │   MaBiTrung          │ ⚠️ Phát hiện mã trùng                   │
    │   Pause              │ Tạm dừng sản xuất                       │
    │   Waiting_Stop       │ Chờ dừng                                │
    └──────────────────────┴────────────────────────────────────────────┘


    ════════════════════════════════════════════════════════════════════════════
    MÀU TRẠNG THÁI:
    ════════════════════════════════════════════════════════════════════════════

    ┌──────────────────────┬────────────────────┬────────────────────┐
    │   Production_State   │   Text Color      │   Background       │
    ├──────────────────────┼────────────────────┼────────────────────┤
    │   NoSelectedPO       │ Đen              │ 🟡 Vàng          │
    │   Ready              │ Đen              │ 🟡 Vàng          │
    │   Running            │ Trắng            │ 🟢 Xanh lá       │
    │   Completed          │ Trắng            │ 🔵 Xanh dương    │
    │   Editing            │ Đen              │ 🟡 Vàng          │
    │   Saving             │ Đen              │ 🟡 Vàng          │
    │   Error              │ 🟡 Vàng        │ 🔴 Đỏ           │
    └──────────────────────┴────────────────────┴────────────────────┘
```

### 6.2 Luồng Trạng Thái Sản Xuất Chi Tiết

```
┌────────────────────────────────────────────────────────────────────────────────┐
│                    LUỒNG TRẠNG THÁI SẢN XUẤT (CHI TIẾT)                          │
└────────────────────────────────────────────────────────────────────────────────┘

                    ┌─────────────────────────┐
                    │       START             │
                    │   (App khởi động)      │
                    └───────────┬─────────────┘
                                │
                                ▼
              ┌───────────────────────────────────────┐
              │                                       │
              │            START                       │
              │   • Lấy PO cuối cùng từ Database      │
              │   • Kiểm tra PO đang chạy dở          │
              │   • Bổ sung mã thiếu vào Dictionary    │
              │                                       │
              └───────────────────┬───────────────────┘
                                │
                                ▼
                    ┌─────────────────────────┐
                    │       LOADING           │
                    │   (Đang tải PO)         │
                    │   • Chọn PO trong UI     │
                    │   • Lưu thông tin PO     │
                    └───────────┬─────────────┘
                                │
                                ▼
                    ┌─────────────────────────┐
                    │       SAVING           │
                    │   (Đang lưu vào DB)     │
                    │   • Kiểm tra database    │
                    │   • Load records         │
                    │   • Reset counters       │
                    └───────────┬─────────────┘
                                │
                                ▼
                    ┌─────────────────────────┐
                    │       READY            │◄────────────────┐
                    │   (Sẵn sàng SX)       │                 │
                    │   btnRUN.Enabled = true│                 │
                    └───────────┬─────────────┘                 │
                                │                               │
                    Người dùng nhấn nút RUN                    │
                                │                               │
                                ▼                               │
┌───────────────────┐  ┌─────────────────────────┐             │
│  PUSHING_NEW_PO   │  │                         │             │
│   to PLC          │  │       RUNNING          │             │
│  (PO mới)         │  │   (Đang sản xuất)    │─────────────┘
└───────────────────┘  │                         │
                        │  • Camera đọc mã      │
┌───────────────────┐  │  • PLC đếm sản phẩm    │
│PUSHING_CONTINUE_PO│  │  • Kiểm tra trùng lặp  │
│   to PLC          │  │  • Đóng thùng (24sp)   │
│  (Tiếp tục PO)   │  │  • Gửi AWS IoT         │
└───────────────────┘  │                         │
                        └───────────┬─────────────┘
                                    │
                        Đủ số lượng (passCount >= orderQty)
                                    │
                                    ▼
                        ┌─────────────────────────┐
                        │    WAITING_STOP        │
                        │   (Chờ dừng)           │
                        │   • Kiểm tra queue      │
                        │   • Chờ hoàn tất ghi DB  │
                        └───────────┬─────────────┘
                                    │
                        Queue empty
                                    │
                                    ▼
                        ┌─────────────────────────┐
                        │     COMPLETED          │
                        │   (Đơn hàng hoàn thành)│
                        │   • Mở khóa btnPO       │
                        │   • Tắt btnRUN          │
                        │   • Thông báo user      │
                        └─────────────────────────┘
```

---

## 7. Kết Nối Thiết Bị

### 7.1 Kiến Trúc Kết Nối

```
╔═══════════════════════════════════════════════════════════════════════════╗
║                     KẾT NỐI THIẾT BỊ                                    ║
╠═══════════════════════════════════════════════════════════════════════════╣
║                                                                        ║
║    ┌────────────────────────────────────────────────────────────────┐   ║
║    │                      ỨNG DỤNG                                    │   ║
║    │  ┌──────────────────────────────────────────────────────────┐ │   ║
║    │  │                    GIAO TIẾP PLC                           │ │   ║
║    │  │  ┌─────────────────┐      ┌─────────────────┐            │ │   ║
║    │  │  │ OmronPLC_Hsl    │      │ OmronPLC_Hsl    │            │ │   ║
║    │  │  │ (PLC_Instance)  │      │(PLC_Instance_02)│            │ │   ║
║    │  │  │                 │      │                 │            │ │   ║
║    │  │  │ • Đọc Counter │      │ • Đọc Counter │            │ │   ║
║    │  │  │ • Ghi Alarm   │      │ • Ghi Alarm   │            │ │   ║
║    │  │  │ • Sync Signal  │      │ • Sync Signal │            │ │   ║
║    │  │  └────────┬────────┘      └────────┬────────┘            │ │   ║
║    │  └───────────┼────────────────────────┼────────────────────┘ │   ║
║    │              │                        │                        │   ║
║    │  ┌───────────┴────────────────────────┴────────────────────┐ │   ║
║    │  │                    HỆ THỐNG CAMERA                         │ │   ║
║    │  │  ┌─────────────────┐      ┌─────────────────┐            │ │   ║
║    │  │  │   Camera Chính  │      │   Camera Phụ    │            │ │   ║
║    │  │  │  (Hikvision)  │      │  (Hikvision)  │            │ │   ║
║    │  │  │                 │      │                 │            │ │   ║
║    │  │  │ • Mã sản phẩm │      │ • Mã thùng     │            │ │   ║
║    │  │  │ • PLC Sync    │      │ • PLC Sync    │            │ │   ║
║    │  │  │ • Kiểm tra  │      │ • Kích hoạt  │            │ │   ║
║    │  │  │   trùng lặp  │      │   thùng       │            │ │   ║
║    │  │  └────────┬────────┘      └────────┬────────┘            │ │   ║
║    │  └───────────┼────────────────────────┼────────────────────┘ │   ║
║    │              │                        │                        │   ║
║    │  ┌───────────┴────────────────────────┴────────────────────┐ │   ║
║    │  │                    LỚP AWS IOT                          │ │   ║
║    │  │  ┌─────────────────────────────────────────────────────┐ │ │   ║
║    │  │  │                AwsIotClientHelper                    │ │ │   ║
║    │  │  │                                                      │ │ │   ║
║    │  │  │  • Connect()          • Publish()                    │ │ │   ║
║    │  │  │  • Disconnect()        • Subscribe()                  │ │ │   ║
║    │  │  │  • AWSStatus_OnChange  • AWSStatus_OnReceive       │ │ │   ║
║    │  │  └─────────────────────────────────────────────────────┘ │ │   ║
║    │  └──────────────────────────────────────────────────────────┘ │   ║
║    └────────────────────────────────────────────────────────────────┘   ║
║                                                                        ║
╚═══════════════════════════════════════════════════════════════════════════╝
                │                        │                        │
                │    TCP/IP             │    SDK               │    MQTT/TLS
                │    (HslCommunication) │    (Hikvision)      │
                ▼                        ▼                        ▼
    ┌───────────────────┐      ┌───────────────────┐      ┌───────────────────┐
    │                   │      │                   │      │                   │
    │     PLC OMRON     │      │   HỆ THỐNG CAMERA │      │   AWS IOT CORE    │
    │                   │      │                   │      │                   │
    │  ┌─────────────┐  │      │ ┌─────────────┐  │      │ ┌─────────────┐  │
    │  │ PLC_01      │  │      │ │ Camera Chính│  │      │ │  Endpoints  │  │
    │  │ • Counter   │  │      │ │ • Sản phẩm  │  │      │ │  • /CZ/data│  │
    │  │ • Alarm     │  │      │ │ • Barcode   │  │      │ │  • response │  │
    │  │ • Trigger   │  │      │ └─────────────┘  │      │ └─────────────┘  │
    │  └─────────────┘  │      │ ┌─────────────┐  │      │                   │
    │  ┌─────────────┐  │      │ │ Camera Phụ  │  │      │ ┌─────────────┐  │
    │  │ PLC_02      │  │      │ │ • Thùng     │  │      │ │  Things     │  │
    │  │ • Counter   │  │      │ │ • Barcode   │  │      │ │  MIPWP501   │  │
    │  │ • Alarm     │  │      │ └─────────────┘  │      │ └─────────────┘  │
    │  │ • Trigger   │  │      │                   │      │                   │
    │  └─────────────┘  │      └───────────────────┘      └───────────────────┘
    │                   │
    │  ════════════════════════════════════════════════════════════════════
    │  CHI TIẾT GIAO THỨC:
    │  ════════════════════════════════════════════════════════════════════
    │  • PLC: FINS/TCP (Omron) - Port 9600
    │  • Camera: SDK HTTP/RTSP (Hikvision) - Port 8000, 554
    │  • AWS: MQTT over TLS 1.2 - Port 8883
    └───────────────────────────────────────────────────────────────────────
```

### 7.2 Giao Thức Kết Nối

| Thiết bị | Giao thức | Port | Thư viện |
|----------|-----------|------|----------|
| PLC Omron 1 | FINS/TCP | 9600 | HslCommunication |
| PLC Omron 2 | FINS/TCP | 9600 | HslCommunication |
| Camera Chính | SDK Ethernet | 8000/554 | Hikvision SDK |
| Camera Phụ | SDK Ethernet | 8000/554 | Hikvision SDK |
| AWS IoT | MQTT/TLS 1.2 | 8883 | AwsIotClientHelper |

---

## 8. Giao Tiếp AWS IoT Core

### 8.1 Kiến Trúc MQTT

```
┌────────────────────────────────────────────────────────────────────────────────┐
│                          GIAO TIẾP AWS IOT CORE                                 │
│                          ═════════════════════════                               │
└────────────────────────────────────────────────────────────────────────────────┘

    ┌─────────────────────────────────────────────────────────────────────────┐
    │                    ỨNG DỤNG MASAN-SERIALIZATION                            │
    │  ┌─────────────────────────────────────────────────────────────────┐  │
    │  │                    AwsIotClientHelper                              │  │
    │  │  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐        │  │
    │  │  │ MQTT Client   │  │   TLS/SSL     │  │  JSON Parser  │        │  │
    │  │  │               │  │  Connection   │  │               │        │  │
    │  │  └───────┬───────┘  └───────┬───────┘  └───────────────┘        │  │
    │  │          │                  │                                   │  │
    │  └──────────┼──────────────────┼───────────────────────────────────┘  │
    │             │                  │                                      │
    └─────────────┼──────────────────┼──────────────────────────────────────┘
                  │    Port 8883     │
                  │   (MQTT/TLS)     │
                  ▼                  ▼
    ┌─────────────────────────────────────────────────────────────────────────┐
    │                        AWS IOT CORE                                      │
    │                                                                          │
    │  ┌─────────────────────────────────────────────────────────────────────┐ │
    │  │                         ENDPOINT                                      │ │
    │  │                    (TLS Connection)                                 │ │
    │  │       a22qv9bgjnbsae-ats.iot.ap-southeast-1.amazonaws.com        │ │
    │  └─────────────────────────────────────────────────────────────────────┘ │
    │                                    │                                      │
    │              ┌─────────────────────┼─────────────────────┐                │
    │              │                     │                     │                │
    │              ▼                     ▼                     ▼                │
    │  ┌───────────────────┐ ┌───────────────────┐ ┌───────────────────┐     │
    │  │   Topic: CZ/data  │ │ Topic: CZ/dataDev │ │ Topic: CZ/{id}/  │     │
    │  │                   │ │                   │ │    response       │     │
    │  │  ═══════════════ │ │  ═══════════════ │ │                   │     │
    │  │  Publish (Out)   │ │  Publish (Dev)   │ │  Subscribe (In)    │     │
    │  │                   │ │                   │ │                   │     │
    │  │  Payload:        │ │  Payload:        │ │  Payload:        │     │
    │  │  {               │ │  {               │ │  {               │     │
    │  │    message_id,   │ │    (giống       │ │    status,        │     │
    │  │    orderNo,      │ │     prod)        │ │    message_id,    │     │
    │  │    uniqueCode,   │ │                  │ │    error_message  │     │
    │  │    gtin,         │ │                  │ │  }               │     │
    │  │    cartonCode,   │ │                  │ │                   │     │
    │  │    status,       │ │                  │ │  Hành động:      │     │
    │  │    activate_dt,  │ │                  │ │  - Update DB     │     │
    │  │    production_dt │ │                  │ │  - Handle error  │     │
    │  │  }               │ │                  │ │                   │     │
    │  └───────────────────┘ └───────────────────┘ └───────────────────┘     │
    │                                                                          │
    │  ┌─────────────────────────────────────────────────────────────────────┐ │
    │  │                         AWS IoT Rules                                │ │
    │  │  • Route CZ/data to DynamoDB                                       │ │
    │  │  • Route CZ/{id}/response to SQS                                   │ │
    │  └─────────────────────────────────────────────────────────────────────┘ │
    │                                                                          │
    └─────────────────────────────────────────────────────────────────────────┘
```

### 8.2 Định Dạng Payload

```
┌─────────────────────────────────────────────────────────────────────────┐
│ SEND PAYLOAD (Publish to CZ/data):                                      │
│                                                                          │
│  {                                                                       │
│    "message_id": "123-PO001-2026-08-03T14:30:00.000Z",                │
│    "orderNo": "PO001",                                                   │
│    "uniqueCode": "8901234567890",                                        │
│    "gtin": "012345678901",                                              │
│    "cartonCode": "CTN20260803001",                                      │
│    "status": 1,                                                         │
│    "activate_datetime": "2026-08-03T14:30:00.000Z",                   │
│    "production_date": "2026-08-03T08:00:00.000Z",                     │
│    "thing_name": "MIPWP501"                                             │
│  }                                                                       │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│ RECEIVE PAYLOAD (Subscribe from CZ/{id}/response):                        │
│                                                                          │
│  {                                                                       │
│    "status": "OK",                                                      │
│    "message_id": "123-PO001-2026-08-03T14:30:00.000Z",                │
│    "error_message": ""                                                  │
│  }                                                                       │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 9. Xử Lý Ngoại Lệ - Thiếu Sản Phẩm

### 9.1 Luồng Phát Hiện và Xử Lý

```
┌────────────────────────────────────────────────────────────────────────────────┐
│                    XỬ LÝ THIẾU SẢN PHẨM                                         │
│                    ═══════════════════════                                     │
└────────────────────────────────────────────────────────────────────────────────┘

    ┌─────────────────────────────────────────────────────────────────────────┐
    │                         GIAI ĐOẠN PHÁT HIỆN                               │
    │                                                                          │
    │   Trong quá trình sản xuất, hệ thống phát hiện:                         │
    │                                                                          │
    │   1. Số lượng sản phẩm trong thùng hiện tại < 24                       │
    │   2. PLC trigger không đầy đủ (thiếu sản phẩm trên băng chuyền)        │
    │   3. Camera Sub đọc thùng nhưng không đủ sản phẩm                      │
    │                                                                          │
    │   ════════════════════════════════════════════════════════════════════  │
    │                                                                          │
    │   ┌─────────────────────────────────────────────────────────────────┐    │
    │   │                    ĐIỀU KIỆN KÍCH HOẠT                           │    │
    │   │                                                                  │    │
    │   │  if (thùng hiện tại đã đóng) AND (số sản phẩm < cartonPack)   │    │
    │   │                                                                  │    │
    │   │  → Chuyển sang trạng thái: ThieuSanPham                        │    │
    │   │                                                                  │    │
    │   └─────────────────────────────────────────────────────────────────┘    │
    │                                                                          │
    └─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
    ┌─────────────────────────────────────────────────────────────────────────┐
    │                         GIAI ĐOẠN CẢNH BÁO                                │
    │                                                                          │
    │   ┌─────────────────────────────────────────────────────────────────┐    │
    │   │                      HÀNH ĐỘNG UI                                  │    │
    │   │  ┌─────────────────────────────────────────────────────────┐    │    │
    │   │  │  1. Kích hoạt btnResetPO (cho phép reset)              │    │    │
    │   │  │  2. Nhấp nháy chữ đỏ trên opTer                         │    │    │
    │   │  │  3. Ghi alarm vào PLC (bật đèn cảnh báo)                │    │    │
    │   │  │  4. Hiển thị dialog lỗi chi tiết                        │    │    │
    │   │  └─────────────────────────────────────────────────────────┘    │    │
    │   │                                                                  │    │
    │   │  PLC Alarm Signal:                                               │    │
    │   │  ──────────────────                                              │    │
    │   │  • Ghi D100 = 1 (bật alarm)                                    │    │
    │   │  • Ghi D101 = mã lỗi (01: thiếu sp)                          │    │
    │   │  • Băng chuyền dừng hoặc chậm lại                             │    │
    │   │                                                                  │    │
    │   └─────────────────────────────────────────────────────────────────┘    │
    │                                                                          │
    └─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
    ┌─────────────────────────────────────────────────────────────────────────┐
    │                         GIAI ĐOẠN ĐIỀU TRA                                │
    │                                                                          │
    │   Khi người dùng nhấn btnResetPO:                                       │
    │                                                                          │
    │   ┌─────────────────────────────────────────────────────────────────┐    │
    │   │              Trạng thái KiemTraThieu                              │    │
    │   │                                                                  │    │
    │   │  1. Lấy thông tin thùng hiện tại (cartonID lớn nhất)           │    │
    │   │  2. Đếm số sản phẩm trong thùng hiện tại                       │    │
    │   │  3. Lấy thông tin thùng trước đó                                │    │
    │   │  4. Đếm số sản phẩm trong thùng trước                         │    │
    │   │                                                                  │    │
    │   └─────────────────────────────────────────────────────────────────┘    │
    │                                                                          │
    │   ════════════════════════════════════════════════════════════════════  │
    │                                                                          │
    │   ┌─────────────────────────────────────────────────────────────────┐    │
    │   │                    CÂY QUYẾT ĐỊNH                                 │    │
    │   │                                                                  │    │
    │   │                         ▼                                         │    │
    │   │              ┌───────────────────────┐                          │    │
    │   │              │  Thùng hiện tại       │                          │    │
    │   │              │  có 0 sản phẩm?       │                          │    │
    │   │              └───────────┬───────────┘                          │    │
    │   │                   Có / \ Không                                   │    │
    │   │                   /     \                                        │    │
    │   │                  ▼       ▼                                       │    │
    │   │       ┌─────────────┐   ┌─────────────┐                         │    │
    │   │       │ THÙNG TRƯỚC  │   │  THIẾU SP  │                         │    │
    │   │       │  < 24 sp?    │   │  NGHIÊM     │                         │    │
    │   │       └──────┬──────┘   │  TRỌNG      │                         │    │
    │   │          Có/ \Không      └──────┬──────┘                         │    │
    │   │            /    \              │                                 │    │
    │   │           ▼      ▼             │                                 │    │
    │   │     ┌────────┐  ┌──────────┐   │                                 │    │
    │   │     │ TH1:   │  │ TH2:     │   │                                 │    │
    │   │     │ RESET  │  │ CẢNH BÁO │   │                                 │    │
    │   │     │ CARTON │  │ + DỪNG   │   │                                 │    │
    │   │     └────────┘  └──────────┘   │                                 │    │
    │   │                                                                  │    │
    │   └─────────────────────────────────────────────────────────────────┘    │
    │                                                                          │
    └─────────────────────────────────────────────────────────────────────────┘
                                    │
                    ┌───────────────┴───────────────┐
                    │                               │
                    ▼                               ▼
    ┌─────────────────────────────┐   ┌─────────────────────────────┐
    │          TH1 (Tự động)      │   │      TH2 (Thủ công)        │
    │  ─────────────────────────  │   │  ─────────────────────────  │
    │                             │   │                             │
    │  Điều kiện:                │   │  Điều kiện:                 │
    │  • Thùng hiện tại = 0 sp  │   │  • Tất cả trường hợp khác  │
    │  • Thùng trước < 24 sp    │   │                             │
    │                             │   │  Hành động:                 │
    │  Hành động:                │   │  1. Hiển thị dialog cảnh   │
    │  1. Reset thùng hiện tại   │   │     báo nghiêm trọng        │
    │     (cartonCode = 0)        │   │  2. Yêu cầu dừng sản xuất │
    │  2. Xóa PLC alarm          │   │  3. Liên hệ nhà cung cấp  │
    │  3. Ghi log chi tiết        │   │  4. Không reset - chờ      │
    │  4. Đóng ứng dụng           │   │     xử lý thủ công         │
    │                             │   │                             │
    └─────────────────────────────┘   └─────────────────────────────┘
```

---

## 10. Cấu Hình Hệ Thống

### 10.1 File Cấu Hình MSC.ini

```ini
[APP]
# Xác thực hai yếu tố
TwoFA_Enabled=false

# Cài đặt Camera Chính
Camera_Main_IP=127.0.0.1
Camera_Main_Port=51236
CameraMain_DuplicateReject_Enabled=false

# Cổng COM Scanner
HandScanCOM01=COM2
HandScanCOM02=COM3
HandScanCOMMain=COM4

# Cài đặt Carton
cartonPack=24
cartonOfset=2
cartonWarning=5
cartonAutoStart=false
cartonScanerMode=0
cartonScaner_Only_Once=false
cartonCode_Line01=0
cartonCode_Line02=1
cartonScanerTCP_IP=192.168.250.14
cartonScanerTCP_Port=5566

# Cài đặt AWS IoT
AWS_ENA=true
host=a22qv9bgjnbsae-ats.iot.ap-southeast-1.amazonaws.com
clientId=MIPWP501
rootCAPath=C:\MIPWP501\AmazonRootCA1.pem
pfxPath=C:\MIPWP501\client-certificate.pfx
pfxPassword=thuc
Auto_Send_AWS=false
AWS_Dev_Mode=false

# Cài đặt PLC
PLC_Test_Mode=true
PLC_Duo_Mode=false
PLC_Address_Sheet_Name=PLC

# Camera Sub Timeout Settings
CameraSub_Timeout_Enabled=true
CameraSub_Timeout_Ms=500
CameraSub_Polling_Interval_Ms=10
CameraSub_V2_Polling_Exit_Ms=500
CameraSub_V2_Polling_Delay_Ms=10
CameraSub_Timeout_Log_Enabled=true
CameraSub_Timeout_Mode_2=false

# Các chế độ ứng dụng
TestMode=false
Check_Duplica_Enabled=false
Check_Db_Old_Active=false
Check_Db_Old_Bypass=false
```

### 10.2 Bảng Tổng Hợp Tham Số

| Tham số | Loại | Mặc định | Mô tả |
|---------|------|-----------|--------|
| `cartonPack` | int | 24 | Số sản phẩm mỗi thùng |
| `cartonWarning` | int | 5 | Ngưỡng cảnh báo |
| `CameraSub_Timeout_Ms` | int | 500 | Timeout Camera Sub (ms) |
| `PLC_Duo_Mode` | bool | false | Chế độ kép PLC |
| `TestMode` | bool | false | Chế độ kiểm tra |
| `AWS_ENA` | bool | true | Kích hoạt AWS |
| `Auto_Send_AWS` | bool | false | Tự động gửi AWS |

---

## 11. Hướng Dẫn Cài Đặt

### 11.1 Yêu Cầu

| Thành phần | Phiên bản | Mô tả |
|------------|-----------|--------|
| .NET Framework | 4.8 | Runtime .NET |
| Windows | 10/11 | Hệ điều hành |
| Visual Studio | 2019+ | Để biên dịch |
| SQLite | 3.x | Database cục bộ |

### 11.2 Cấu Trúc Thư Mục Dữ Liệu

```
C:\
├── MasanSerialization_v2\
│   ├── Server_Service\
│   │   ├── po1.db                    # Database PO
│   │   ├── data\                     # File JSON PO
│   │   │   └── PO001.json
│   │   └── codes_json\               # Mã theo GTIN
│   │       └── GTIN_*.json
│   ├── PODatabases\                  # Database cục bộ
│   └── Databases\
│       └── POLog.db                  # Nhật ký PO
│
├── MIPWP501\                         # Chứng chỉ AWS
│   ├── AmazonRootCA1.pem
│   └── client-certificate.pfx
│
└── MASAN-SERIALIZATION\
    └── Logs\                        # Logs ứng dụng
        └── applog.tl
```

### 11.3 Khởi Động Ứng Dụng

```powershell
# 1. Clone dự án
git clone <repository_url>

# 2. Mở trong Visual Studio
MASAN-SERIALIZATION.sln

# 3. Restore packages NuGet
nuget restore

# 4. Biên dịch
Build > Build Solution

# 5. Chạy (F5)
```

---

## 12. Xử Lý Sự Cố

### 12.1 Mã Lỗi Thường Gặp

| Mã | Module | Mô tả | Giải pháp |
|----|--------|--------|-----------|
| P01 | Database | Database PO không tìm thấy | Kiểm tra đường dẫn po1.db |
| P02 | Database | Lỗi đọc chi tiết PO | Kiểm tra quyền truy cập |
| P03 | Database | Lỗi đọc mã CZ | Kiểm tra file mã |
| PP05 | Production | Lỗi tải dữ liệu | Kiểm tra kết nối DB |
| PP07 | Production | Lỗi đếm records | Kiểm tra DB cục bộ |
| EA001 | Database | File database không tìm thấy | Liên hệ hỗ trợ |
| PP_TS01 | Exception | Không tìm thấy thùng hiện tại | Kiểm tra trạng thái sản xuất |

### 12.2 Vấn Đề Thường Gặp và Giải Pháp

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         HƯỚNG DẪN XỬ LÝ SỰ CỐ                             │
└─────────────────────────────────────────────────────────────────────────┘

VẤN ĐỀ: PLC không kết nối
═══════════════════════════════
Triệu chứng:
  • lblAllStatus hiển thị "Lỗi thiết bị"
  • PLC_Connected = false

Giải pháp:
  1. Kiểm tra kết nối mạng PLC (IP: 192.168.x.x)
  2. Kiểm tra port FINS/TCP (9600)
  3. Khởi động lại service HslCommunication
  4. Kiểm tra cấu hình IP trong MSC.ini


VẤN ĐỀ: Camera không kết nối
═════════════════════════════════
Triệu chứng:
  • CameraMain_State = DISCONNECTED
  • CameraSub_State = DISCONNECTED

Giải pháp:
  1. Kiểm tra IP camera (Camera_Main_IP)
  2. Kiểm tra cáp mạng
  3. Kiểm tra credentials camera
  4. Khởi động lại services camera


VẤN ĐỀ: AWS IoT không kết nối
══════════════════════════════════
Triệu chứng:
  • AWS_IoT_Status = Disconnected
  • Không có dữ liệu được gửi

Giải pháp:
  1. Kiểm tra chứng chỉ AWS (rootCA, pfx)
  2. Kiểm tra endpoint host
  3. Kiểm tra clientId
  4. Kiểm tra kết nối internet


VẤN ĐỀ: Lỗi "Thiếu sản phẩm"
═══════════════════════════════════════
Triệu chứng:
  • Chữ đỏ nhấp nháy "THIẾU SẢN PHẨM"
  • Alarm PLC được kích hoạt

Giải pháp:
  1. Kiểm tra băng chuyền (sản phẩm bị kẹt?)
  2. Kiểm tra scanner (tất cả mã được đọc?)
  3. Liên hệ nhà cung cấp nếu lỗi lặp lại
  4. Xem mục 9 để biết logic xử lý


VẤN ĐỀ: Mã CZ không đủ
═══════════════════════════════════
Triệu chứng:
  • "Số lượng mã MES gửi xuống chưa đủ"
  • Lỗi PP04

Giải pháp:
  1. Kiểm tra API MES
  2. Kiểm tra file JSON PO
  3. Kiểm tra GTIN tương ứng
```

---

## Phụ Lục A: Enum

### A.1 Trạng Thái Ứng Dụng

```csharp
public enum e_App_State
{
    LOGIN,     // Chưa đăng nhập
    ACTIVE,    // Hoạt động
    DEACTIVE   // Bị vô hiệu hóa
}

public enum e_App_Render_State
{
    LOGIN,     // Hiển thị trang login
    ACTIVE,    // Hiển thị giao diện active
    DEACTIVE   // Hiển thị bị vô hiệu hóa
}

public enum e_Camera_State
{
    DISCONNECTED,  // Chưa kết nối
    CONNECTED,    // Đã kết nối
    RECONNECTING   // Đang kết nối lại
}
```

### A.2 Trạng Thái Sản Xuất

```csharp
public enum e_Production_State
{
    NoSelectedPO,       // Chưa chọn PO
    Start,              // Khởi động
    Loading,            // Đang tải
    Saving,             // Đang lưu
    Ready,              // Sẵn sàng
    Running,            // Đang chạy
    Completed,          // Hoàn thành
    Editing,            // Chỉnh sửa
    Error,              // Lỗi
    ThieuSanPham,       // ⚠️ Thiếu sản phẩm
    KiemTraThieu,        // Kiểm tra thiếu
    Camera_Processing,   // Xử lý camera
    Pushing_new_PO_to_PLC,
    Pushing_continue_PO_to_PLC,
    Pushing_to_Dic,
    Check_After_Completed,
    Check_Carton_Full,
    Pause,
    Waiting_Stop,
    MaBiTrung           // ⚠️ Mã trùng
}
```

### A.3 Trạng Thái Mã Sản Xuất

```csharp
public enum e_Production_Status
{
    Pass,       // Mã hợp lệ
    Fail,       // Mã thất bại
    NotFound,   // Mã không tìm thấy
    ReadFail,   // Đọc camera thất bại
    Duplicate,  // Mã trùng lặp
    Timeout,    // Timeout
    Error       // Lỗi
}
```

---

## Phụ Lục B: Cấu Trúc Dữ Liệu

### B.1 ProductionCodeData

```csharp
public class ProductionCodeData
{
    public string orderNo { get; set; }        // Số đơn hàng
    public string Code { get; set; }            // Mã sản phẩm
    public int codeID { get; set; }            // ID trong DB
    public string cartonCode { get; set; }     // Mã thùng
    public string Activate_User { get; set; } // Người kích hoạt
    public string Camera_Status { get; set; } // Trạng thái camera
    public string Activate_Datetime { get; set; } // Thời gian kích hoạt
    public string Production_Datetime { get; set; } // Thời gian sản xuất
}
```

### B.2 ProductionCartonData

```csharp
public class ProductionCartonData
{
    public int cartonID { get; set; }          // ID thùng
    public string orderNo { get; set; }        // Số đơn hàng
    public string cartonCode { get; set; }     // Mã thùng
    public string Activate_User { get; set; } // Người kích hoạt
    public string Start_Datetime { get; set; } // Thời gian bắt đầu
    public string Activate_Datetime { get; set; } // Thời gian kích hoạt
    public string Production_Datetime { get; set; } // Thời gian sản xuất
}
```

### B.3 PLCCounter

```csharp
public class PLCCounter
{
    public int total { get; set; }             // Tổng sản phẩm
    public int total_pass { get; set; }       // Tổng đạt
    public int total_failed { get; set; }     // Tổng thất bại
    public int camera_read_fail { get; set; } // Lỗi đọc camera
    public int timeout { get; set; }          // Timeout
}
```

---

## Phụ Lục C: Định Dạng JSON

### C.1 Format PO JSON

```json
{
  "orderNo": "PO001",
  "orderQty": 1000,
  "customerOrderNo": "CUST001",
  "productionLine": "LINE01",
  "productName": "Sữa Hòa Tan",
  "productCode": "PROD001",
  "lotNumber": "LOT20260803",
  "gtin": "8931234567890",
  "shift": "DAY",
  "factory": "MASAN",
  "site": "BINH DUONG",
  "uom": "PCS",
  "productionDate": "2026-08-03 08:00:00"
}
```

### C.2 Format GTIN Codes JSON

```json
{
  "gtin": "8931234567890",
  "blocks": {
    "0": {
      "codes": [
        {"code": "8931234567890001", "createdAt": "2026-08-01T00:00:00Z"},
        {"code": "8931234567890002", "createdAt": "2026-08-01T00:00:00Z"}
      ]
    },
    "1": {
      "codes": [
        {"code": "8931234567890011", "createdAt": "2026-08-01T00:00:00Z"}
      ]
    }
  }
}
```

---

## Thuật Ngữ

| Thuật ngữ | Định nghĩa |
|-----------|------------|
| **PO** | Production Order - Đơn hàng sản xuất |
| **GTIN** | Global Trade Item Number - Mã sản phẩm quốc tế |
| **MES** | Manufacturing Execution System - Hệ thống thực thi sản xuất |
| **PLC** | Programmable Logic Controller - Bộ điều khiển logic khả trình |
| **Carton** | Đơn vị đóng gói 24 sản phẩm |
| **CZ Code** | Mã duy nhất để truy xuất nguồn gốc |
| **AWS IoT** | Amazon Web Services Internet of Things |
| **FINS** | Factory Interface Network Service - Giao thức Omron |

---

*Tài liệu được tạo: 2026-08-03*
*Phiên bản: 1.0*
*Dự án: MASAN-SERIALIZATION*
