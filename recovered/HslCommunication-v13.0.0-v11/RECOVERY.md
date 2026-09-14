# HslCommunication v13.0.0 – mã nguồn phục hồi

Thư mục này được decompile từ:

`C:\Users\THUC\Downloads\HslCommunicationDemo-v13.0.0\HslCommunication.dll`

Thông tin assembly:

- Tên: `HslCommunication`
- Version: `13.0.0.0`
- Public key token: `3d72ad3b6b5ec0e3`
- Công cụ: ILSpy 11.0.0.9375

Mã nguồn phục hồi giữ lại namespace, class, public API, XML documentation nhúng trong assembly và phần lớn logic C#. DLL gốc vẫn được giữ nguyên ở vị trí ban đầu; nếu cần đối chiếu, hãy dùng file DLL gốc thay vì sửa trực tiếp nó.

## Mở và build

Mở `HslCommunication.csproj` bằng Visual Studio có .NET Framework 4.5.1 Developer Pack/Targeting Pack, sau đó chọn **Build > Build Project**. Mình đã kiểm tra build thành công bằng MSBuild của Visual Studio 18.4. Project gốc dùng Windows Forms và tham chiếu `Newtonsoft.Json.dll`.

Không dùng `dotnet build` SDK mới cho project này nếu không cần; SDK mới có thể báo `MSB3822/MSB3823` với các resource Windows Forms `.resx`, dù MSBuild của Visual Studio build được bình thường.

Decompile không thể khôi phục comment riêng của người viết, tên biến cục bộ đã bị compiler loại bỏ, file solution gốc, hoặc PDB nếu DLL không kèm PDB. Một vài đoạn có thể cần chỉnh thủ công trước khi build do giới hạn của decompiler hoặc khác biệt dependency.

Đây là source của assembly HslCommunication trong DLL; nó không chứa source của các ứng dụng MASAN/SpT đã tham chiếu thư viện này. Nếu cần phục hồi phần ứng dụng, cần có EXE/DLL của chính ứng dụng đó.
