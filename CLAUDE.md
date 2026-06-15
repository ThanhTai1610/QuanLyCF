# QuanLyCF — Claude Code

> **Toàn bộ tài liệu & quy tắc nằm trong [`docs/`](docs/00-README.md).** File này chỉ là con trỏ.
> Skill `quanlycf-conventions` cũng tự nhắc các quy tắc này.

TRƯỚC KHI CODE, đọc [docs/01-QUY-TAC-CODE.md](docs/01-QUY-TAC-CODE.md) (quy tắc), [docs/02-KIEN-TRUC-BACKEND.md](docs/02-KIEN-TRUC-BACKEND.md) (cấu trúc), [docs/05-HUONG-DAN-DEV.md](docs/05-HUONG-DAN-DEV.md) (chạy FE/BE).

**Cốt lõi:** Vertical-slice theo Feature · Controller mỏng (logic ở Service) · DTO `record`, không trả entity · EF **Code First** (migration, KHÔNG sửa DB tay) · RBAC `[Authorize(Policy = Quyens.XXX)]` · tiền `decimal`, UTC · hash BCrypt, không hardcode secret · build + test trước khi xong.
