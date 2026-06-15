# Hướng dẫn Dev — Cài đặt & Chạy

## Yêu cầu
- .NET SDK 8 (project target `net8.0`)
- SQL Server (instance mặc định `localhost`) + SSMS
- Node.js ≥ 20 (frontend Vue + Vite)
- Công cụ EF: `dotnet tool install --global dotnet-ef`

## Backend (`BackEnd/BackEnd`)
```bash
cd BackEnd/BackEnd
dotnet restore
dotnet ef database update      # tạo/migrate DB QuanLyCF
dotnet run                     # API: http://localhost:5124  | Swagger: /swagger
```
- Chuỗi kết nối & JWT: `appsettings.json` (`ConnectionStrings:QuanLyCF`, `Jwt:Key`, `Cors:FrontendOrigin`).
- Lúc khởi động: tự `MigrateAsync` + seed (RBAC, danh mục, tài khoản **admin@brew.vn / demo1234**, PIN `2006`).
- Đổi `Jwt:Key` thành chuỗi bí mật riêng trước khi dùng thật.

### Vòng đời migration
```bash
dotnet ef migrations add <Ten>     # sau khi đổi entity
dotnet ef database update
dotnet ef migrations remove        # gỡ migration cuối (nếu chưa update)
```

## Frontend (`FontEnd`)
```bash
cd FontEnd
npm install
npm run dev                    # http://localhost:5173
```

## Tài khoản mẫu
| Vai trò | Email | Mật khẩu |
|---|---|---|
| Quản lý | admin@brew.vn | demo1234 |

## Lỗi hay gặp
- **Build báo file bị khoá (`BackEnd.exe ... used by another process`)**: tắt tiến trình đang chạy
  `Get-Process BackEnd | Stop-Process -Force` rồi build lại.
- **Không kết nối SQL**: kiểm tra server name trong connection string (mặc định `localhost`).
- **FE gọi API bị CORS**: chỉnh `Cors:FrontendOrigin` cho khớp cổng Vite đang chạy.
