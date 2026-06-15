# Quy tắc dự án QuanLyCF (cho mọi AI)

> **Toàn bộ tài liệu & quy tắc nằm trong thư mục [`docs/`](docs/00-README.md).** File này chỉ là con trỏ.

TRƯỚC KHI CODE, đọc:
- [docs/01-QUY-TAC-CODE.md](docs/01-QUY-TAC-CODE.md) — quy tắc BẮT BUỘC
- [docs/02-KIEN-TRUC-BACKEND.md](docs/02-KIEN-TRUC-BACKEND.md) — cấu trúc Features
- [docs/03-CO-SO-DU-LIEU.md](docs/03-CO-SO-DU-LIEU.md) · [docs/04-LO-TRINH.md](docs/04-LO-TRINH.md) · [docs/05-HUONG-DAN-DEV.md](docs/05-HUONG-DAN-DEV.md)

**Cốt lõi:** Vertical-slice theo Feature · Controller mỏng (logic ở Service) · DTO `record`, không trả entity ra API · EF **Code First** (đổi schema bằng migration, KHÔNG sửa DB tay) · RBAC `[Authorize(Policy = Quyens.XXX)]` · tiền `decimal`, thời gian UTC · hash BCrypt, không hardcode secret · build + test trước khi báo xong.
