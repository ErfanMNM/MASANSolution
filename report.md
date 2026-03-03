## Báo cáo sự cố “Mã chai bị đóng vào 2 thùng khác nhau”

### 1. Hiện tượng

- Một chai đã **được đóng thùng thành công** (Camera Sub đọc `Pass`, có `cartonID` trong lịch sử).
- Sau đó công nhân **lấy chai đó ra, đưa lại lên line nhiều lần**:
  - Lần 2: hệ thống **báo trùng (Duplicate)** và **đẩy chai ra khỏi line** (đúng hành vi mong muốn).
  - Lần 3: chai **không còn bị báo trùng**, mà lại **được đóng vào một thùng khác**.
- Trong cơ sở dữ liệu lịch sử Camera Sub (`Records_CameraSub`) xuất hiện **cùng một mã chai nằm ở nhiều thùng khác nhau**
  (ví dụ `cartonID = 445` và `cartonID = 577`).

---

### 2. Bối cảnh vận hành dẫn đến lỗi

- Camera Sub đọc mã chai, gửi kết quả về PLC để quyết định:
  - **Cho chai vào thùng** (`Pass`).
  - **Đẩy chai ra** (`Reject` / `Duplicate` / `Error`).

- **Trong ca sản xuất có sự kiện:**
  Nhà cung cấp vào **sửa máy**, phải **tắt máy rồi bật lại**, trong đó có ảnh hưởng đến hệ thống máy quét (camera).

- Sau khi thiết bị được bật lại:
  - Một số tín hiệu từ camera gửi về PLC có thể **đến trễ (timeout)** hoặc **không còn đồng bộ** với vị trí thực tế của chai trên băng tải.
  - Hậu quả là một số chai có thể:
    - Đã được hệ thống **ghi nhận là `Pass` và gán vào thùng** trong cơ sở dữ liệu,
    - Nhưng do tín hiệu trễ, PLC **vẫn đẩy chai ra khỏi line**.

- Về phía công nhân:
  - Thấy chai bị đẩy ra, tưởng là chai lỗi → **nhặt chai lên và thả lại nhiều lần**:
    - Có lần hệ thống nhận là `Duplicate` và đẩy ra,
    - Có lần (sau khi máy khởi động lại / hệ thống nạp lại dữ liệu) chai lại được coi là **“chưa dùng”** và **được đóng vào một thùng mới**.

---

### 3. Nguyên nhân kỹ thuật (giải thích đơn giản)

#### 3.1. Vấn đề 1 – Bộ nhớ “mã đã quét” của Camera Sub bị reset

- Hệ thống có hai “bộ nhớ” về mã chai:

  - **Dictionary trong RAM**: dùng để check nhanh *“mã này đã quét ở Camera Sub chưa?”*.
  - **Database lịch sử (`Records_CameraSub`)**: ghi lại **toàn bộ nhật ký chai – thùng**.

- Trước khi sửa:

  - Khi **khởi động lại ứng dụng** hoặc **chọn lại PO (đơn hàng)**, chương trình đọc lại dữ liệu từ database.
  - Tuy nhiên, trạng thái *“mã này đã từng được Camera Sub quét”* trong dictionary **luôn bị đặt lại về 0 (chưa quét)**, không dựa trên dữ liệu thực tế trong DB.

- Vì vậy:

  - Một chai đã từng được Camera Sub quét `Pass` trước đó, sau khi restart, bị coi là **mới 100%**.
  - Nếu công nhân thả lại chai này, hệ thống **không phát hiện `Duplicate`**, mà xử lý như chai mới và **có thể đóng vào một thùng khác**.

#### 3.2. Vấn đề 2 – Không có lớp “kiểm tra lịch sử” trước khi gửi dữ liệu ra ngoài

- Khi kết thúc PO, dữ liệu chai – thùng được **gửi lên AWS/MES** để đối soát.

- Trước khi sửa:

  - Hệ thống **không kiểm tra chéo lịch sử** trong bảng `Records_CameraSub`.
  - Nếu vì bất kỳ lý do gì một mã đã nằm trong **2 thùng khác nhau**, **cả 2 bản ghi vẫn có thể được gửi ra ngoài**, gây sai lệch số liệu và khó truy vết.

#### 3.3. Vai trò của việc “camera xử lý trễ, PLC đẩy chai ra”

- Khi camera hoặc PLC bị **restart / lag** (do tắt/bật máy khi sửa):

  - Có trường hợp **camera vẫn gửi `Pass`**, nhưng PLC đã **hết thời gian chờ**, nên **đẩy chai ra**.
  - Về mặt dữ liệu:
    - Database **ghi nhận chai đã `Pass` và đã gán vào thùng**.
  - Về thực tế:
    - Chai **đang nằm ngoài line** → công nhân nhặt lên, thả lại.

- Chuỗi sự kiện “tắt/bật máy – tín hiệu trễ – công nhân thả lại” làm lộ rõ hai lỗ hổng:

  - Bộ nhớ “đã quét” trong RAM **bị reset sai** sau restart.
  - **Không có bước kiểm tra lịch sử thùng** trước khi gửi dữ liệu ra ngoài.

---

### 4. Giải pháp đã triển khai

#### 4.1. Sửa cách nạp dữ liệu cho Camera Sub (giữ đúng trạng thái “đã quét”)

- Khi ứng dụng khởi động hoặc load lại PO:

  - Thay vì đặt cứng *“mã này chưa được Camera Sub quét”*,
  - Hệ thống **đọc cột thời gian quét của Camera Sub trong DB**:
    - Nếu **có thời gian quét** → đánh dấu là **“đã quét”** (`Sub_Camera_Status = 1`).
    - Nếu **chưa có** → đánh dấu là **“chưa quét”** (`Sub_Camera_Status = 0`).

- **Tác dụng:**

  - Sau restart, mọi mã từng được Camera Sub quét (kể cả đã Pass hay đã bị đẩy ra) vẫn được coi là **“đã dùng rồi”**.
  - Công nhân có thả lại chai 2–3 lần, hệ thống **luôn báo `Duplicate` và đẩy ra**, **không cho đóng vào thùng mới**.

---

#### 4.2. Thêm cơ chế kiểm tra đặc biệt trước khi gửi AWS – chế độ **MaBiTrung**

- Thêm một **trạng thái mới trong hệ thống**: `MaBiTrung` (mã bị trùng thùng).

- Trước khi gửi **từng bản ghi** lên AWS:

  - Hệ thống mở database `Record_CameraSub_{PO}` và kiểm tra:
    - **Mã này đã từng được ghi nhận `Pass` ở bao nhiêu `cartonID` khác nhau?**

  - Nếu:
    - **Chỉ 0 hoặc 1 thùng** → coi là **an toàn**, gửi bình thường.
    - **Từ 2 thùng trở lên**:
      - Kích hoạt trạng thái **`MaBiTrung`**.
      - Ghi rõ cảnh báo trên màn hình AWS, ví dụ:
        - *“MÃ BỊ TRÙNG THÙNG – BỎ QUA GỬI AWS: Code = …, CartonIDs = 445, 577, …”*
      - **Không gửi bản ghi đó lên AWS/MES**.

- **Tác dụng:**

  - Dù lỗi đã xảy ra trong quá khứ (trước khi có fix), nếu database hiện tại đang có **mã nằm trong 2 thùng**:
    - Hệ thống vẫn **phát hiện được khi gửi dữ liệu**.
    - **Không cho dữ liệu sai ra khỏi nhà máy**, đồng thời **báo động cho vận hành/QA** để xử lý.

---

### 5. Lợi ích đối với vận hành & quản lý

- **An toàn dữ liệu:**

  - Chặn **từ gốc** việc một mã chai hợp lệ xuất hiện trong **2 thùng khác nhau** sau khi restart hoặc sau sự cố thiết bị (tắt/bật máy khi sửa).
  - Bảo vệ dữ liệu gửi ra AWS/MES, **tránh phải xử lý sai lệch và truy vết phức tạp** sau này.

- **Tăng khả năng giám sát lỗi thao tác:**

  - Trường hợp công nhân nhặt chai bị đá ra rồi thả lại nhiều lần sẽ:
    - Hoặc bị báo **`Duplicate` liên tục** (không thể đóng thùng lại),
    - Hoặc nếu trong dữ liệu cũ chai đã lỡ vào nhiều thùng, sẽ bị gắn nhãn **`MaBiTrung`** và **dừng ngay ở bước gửi AWS**.

- **Dễ hiểu với người không rành IT:**

  - Có thể coi đây như việc:
    - **“Hệ thống nhớ lâu hơn”**: sau khi **tắt/mở máy để sửa**, hệ thống vẫn **nhớ chính xác chai nào đã đi qua Camera Sub**.
    - **“Có bảo vệ 2 lớp”**:
      - **Lớp 1**: Không cho cùng một chai đi vào 2 thùng khác nhau trong quá trình chạy.
      - **Lớp 2**: Nếu lỡ có rồi (do sự cố đặc biệt), hệ thống **không gửi thông tin đó ra ngoài**, và báo lỗi rõ ràng để quản lý quyết định hướng xử lý.

