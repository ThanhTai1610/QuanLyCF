# Quy tắc Code — Dự án Quản lý Cafe (QuanLyCF)

> Mọi thành viên **bắt buộc** tuân thủ. Mục tiêu: code đồng nhất, dễ đọc, dễ bảo trì.

## 0. Nguyên tắc chung
- Code & comment quan trọng viết **tiếng Việt không dấu cho định danh** (PascalCase/camelCase) nhưng **tên miền nghiệp vụ giữ tiếng Việt** (vd `NhanVien`, `DonHang`) cho khớp DB.
- Một thay đổi nhỏ, dễ review. Không trộn nhiều việc trong 1 commit/PR.
- Không commit thẳng lên `main`. Tạo nhánh, mở PR.
- Trước khi báo "xong": phải **build pass** và **test thử endpoint/luồng** liên quan.

## 1. Kiến trúc Backend — Vertical Slice theo Feature
Cấu trúc chuẩn (xem chi tiết [02-KIEN-TRUC-BACKEND.md](02-KIEN-TRUC-BACKEND.md)):
```
Features/<Nhóm>/<Feature>/   Controllers · Services · (Repositories) · DTOs · Validators
Domain/Entities/<Nhóm>/       entity Code First (namespace: BackEnd.Domain.Entities)
Infrastructure/Persistence/   DbContext · Configurations · Seed · Migrations
Shared/                       Quyens · PasswordHasher · Common · Extensions · Middlewares
```
**Quy tắc:**
- Mỗi feature 1 thư mục riêng. KHÔNG đặt logic của feature này trong feature khác.
- Controller **mỏng**: chỉ nhận request → gọi Service → trả response. Logic nghiệp vụ ở Service.
- Truy vấn EF nằm trong Service (hoặc Repository). KHÔNG viết business logic trong Controller.
- DTO là `record`. **KHÔNG** trả thẳng entity ra API → luôn map sang DTO.
- Entity đặt ở `Domain/Entities/<Nhóm>/`, namespace luôn là `BackEnd.Domain.Entities`.

## 2. EF Core — Code First (BẮT BUỘC)
- **TUYỆT ĐỐI KHÔNG** sửa cấu trúc DB bằng SQL tay. Mọi thay đổi schema qua **migration**.
- Quy trình khi đổi entity:
  ```bash
  dotnet ef migrations add <TenThayDoiRoRang>   # vd: AddOrdering, AddInvoiceColumn
  dotnet ef database update
  ```
- Tên migration viết PascalCase, mô tả đúng việc (không đặt "Update1", "Migration2").
- Cấu hình entity bằng Fluent API trong `QuanLyCFDbContext` (hoặc file `Configurations/`), không dùng Data Annotations rải rác.
- Tiền/khối lượng: `decimal` (tiền `decimal(12,2)` hoặc `(10,2)`, số lượng kho `decimal(10,3)`). KHÔNG dùng `float/double` cho tiền.
- Thời gian lưu **UTC** (`DateTime.UtcNow`), format hiển thị ở tầng FE/DTO.

## 3. Bảo mật & Phân quyền (RBAC) — BẮT BUỘC
- Mọi endpoint nội bộ phải có `[Authorize]`. Endpoint cần quyền cụ thể thêm `[Authorize(Policy = Quyens.XXX)]`.
- Thêm quyền mới: khai báo hằng trong `Shared/Quyens.cs` → thêm vào `Quyens.TatCa` → seed `Quyen` + gán `VaiTro_Quyen` trong DbContext → migration.
- Mật khẩu/PIN: hash bằng **BCrypt** (`PasswordHasher`). KHÔNG lưu plaintext, KHÔNG log.
- KHÔNG hardcode secret. `Jwt:Key`, connection string... để trong `appsettings`/biến môi trường.
- Endpoint công khai (menu QR, trang chủ khách) phải tách rõ và chỉ trả dữ liệu công khai.

## 4. API & quy ước response
- Route: `api/<tai-nguyen-so-nhieu>` (vd `api/products`, `api/stock-receipts`).
- HTTP: `GET` list/detail, `POST` tạo, `PUT` sửa, `DELETE` xoá; trả status đúng (201 khi tạo, 204 khi sửa/xoá, 400/404/409 khi lỗi nghiệp vụ).
- Lỗi nghiệp vụ trả `{ "message": "..." }` tiếng Việt, dễ hiểu cho người dùng cuối.
- Validate đầu vào (bắt buộc, định dạng, trùng lặp) trước khi ghi DB.

## 5. Frontend (Vue 3 + TS)
- Tổ chức theo feature: `src/features/<feature>/` gồm `*.vue`, `api.ts`, `store.ts`, `types.ts`.
- Gọi API qua client chung (gắn JWT tự động), đọc `BASE_URL` từ `.env` — KHÔNG hardcode URL.
- Khi 1 trang đã nối API thật thì **bỏ mock** của trang đó.
- Dùng Pinia cho state dùng chung; component giữ state cục bộ khi đủ.

## 6. Git
- Nhánh: `feature/<mo-ta>`, `fix/<mo-ta>`.
- Commit ngắn gọn, thì hiện tại: "Them API quan ly ton kho", "Sua loi tinh gia von".
- PR mô tả: làm gì, vì sao, test thế nào. Reviewer check theo tài liệu này.

## 7. Definition of Done
- [ ] Tuân thủ cấu trúc Feature + đặt đúng tầng.
- [ ] Có migration nếu đổi schema; `database update` chạy được.
- [ ] Endpoint có `[Authorize]`/policy đúng vai trò.
- [ ] Build pass, đã test luồng chính.
- [ ] Không hardcode secret, không lộ entity ra API.
