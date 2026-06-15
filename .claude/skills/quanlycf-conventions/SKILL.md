---
name: quanlycf-conventions
description: Quy tắc & kiến trúc BẮT BUỘC khi code dự án Quản lý Cafe (QuanLyCF) — backend ASP.NET Core theo Features + EF Core Code First, frontend Vue 3. Dùng MỖI KHI thêm/sửa tính năng, tạo entity/migration, viết controller/service/DTO, đụng phân quyền (RBAC), hoặc khi review/kiểm tra code có đúng chuẩn dự án không.
---

# Quy tắc code dự án QuanLyCF

Khi làm việc trên repo này, LUÔN tuân thủ. Chi tiết đầy đủ ở `docs/01-QUY-TAC-CODE.md`.

## Trước khi bắt đầu
1. Đọc `docs/01-QUY-TAC-CODE.md` (quy tắc), `docs/02-KIEN-TRUC-BACKEND.md` (cấu trúc), `docs/04-LO-TRINH.md` (phase).
2. Xác định tính năng thuộc **Feature** nào → đặt code đúng thư mục.

## 10 nguyên tắc không được vi phạm
1. **Vertical slice theo Feature**: code đặt trong `Features/<Nhóm>/<Feature>/` (Controllers · Services · DTOs · Validators). Không để logic feature này trong feature khác.
2. **Controller mỏng**: chỉ request→Service→response. Business logic + truy vấn EF nằm ở **Service**.
3. **DTO là `record`, KHÔNG trả entity ra API** — luôn map sang DTO.
4. **Entity** ở `Domain/Entities/<Nhóm>/`, namespace luôn `BackEnd.Domain.Entities`.
5. **EF Code First**: KHÔNG sửa DB bằng SQL tay. Đổi schema → `dotnet ef migrations add <Ten>` + `dotnet ef database update`. Tên migration mô tả rõ.
6. **Tiền = `decimal`** (không float). **Thời gian = UTC**.
7. **RBAC**: mọi endpoint nội bộ có `[Authorize]`; cần quyền cụ thể thêm `[Authorize(Policy = Quyens.XXX)]`. Quyền mới → thêm vào `Shared/Quyens.cs` + `Quyens.TatCa` + seed `Quyen`/`VaiTro_Quyen` + migration.
8. **Bảo mật**: mật khẩu/PIN hash BCrypt (`PasswordHasher`); KHÔNG hardcode secret; KHÔNG log thông tin nhạy cảm.
9. **API**: route `api/<tai-nguyen>`; status đúng (201/204/400/404/409); lỗi nghiệp vụ trả `{ "message": "..." }` tiếng Việt.
10. **Frontend**: tổ chức theo `src/features/<feature>/`; gọi API qua client chung (gắn JWT); `BASE_URL` từ `.env`; bỏ mock khi đã nối API thật.

## Mẫu thêm 1 Feature backend (CRUD)
1. Tạo entity ở `Domain/Entities/<Nhóm>/` + thêm `DbSet` & Fluent config trong `QuanLyCFDbContext`.
2. `Features/<Nhóm>/<Feature>/`: `XxxDtos.cs` (record), `XxxController.cs` (`[ApiController]`, `[Route("api/xxx")]`, `[Authorize(Policy=...)]` từng action), service nếu logic phức tạp.
3. Đăng ký service vào DI trong `Program.cs` (nếu có).
4. `dotnet ef migrations add Add<Feature>` → `dotnet ef database update`.
5. Build + test endpoint (login lấy token → gọi thử) trước khi báo xong.

## Definition of Done
Đúng cấu trúc Feature · có migration nếu đổi schema · endpoint có Authorize/policy · build pass + đã test luồng · không hardcode secret · không lộ entity.

## Khi review code
Đối chiếu PR với `docs/01-QUY-TAC-CODE.md` mục 7 (checklist). Nêu rõ điểm vi phạm và cách sửa.
