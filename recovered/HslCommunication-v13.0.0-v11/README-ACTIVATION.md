# Kích hoạt HslCommunication hợp lệ

DLL này có cơ chế cấp phép tích hợp. Mã kích hoạt hợp lệ phải do nhà cung cấp cấp hoặc được ký bằng certificate hợp lệ; không thể tạo mã hợp lệ chỉ từ machine code và mã nguồn decompile.

## Lấy machine code

```csharp
using HslCommunication.BasicFramework;

string machineCode = SoftAuthorize.GetInfo(UseAdmin: false, useHDD: true);
Console.WriteLine(machineCode);
```

Gửi machine code cho nhà cung cấp để nhận activation code hoặc file certificate.

## Kích hoạt bằng mã

```csharp
using HslCommunication;

bool activated = Authorization.SetAuthorizationCode(activationCode);
if (!activated)
{
    Console.WriteLine("Kích hoạt thất bại hoặc mã không hợp lệ.");
}
```

## Kích hoạt bằng certificate

```csharp
using System.IO;
using HslCommunication;

var result = Authorization.SetHslCertificate(
    File.ReadAllBytes("hsl.cert"));

Console.WriteLine(result.IsSuccess ? "Đã kích hoạt" : result.Message);
```

## Thời hạn dùng thử

`Authorization.cs` có kiểm tra thời gian dùng thử. Khi hết hạn, các thao tác mạng trả về lỗi cấp quyền thay vì hiện một cửa sổ đếm ngược. Các chức năng yêu cầu quyền thương mại có thời hạn riêng.

Không nên sửa hoặc phát hành lại cơ chế cấp phép của thư viện. Nếu cần sử dụng thương mại, hãy xin activation code/certificate chính thức hoặc thay thế thư viện bằng một giải pháp có giấy phép phù hợp.
