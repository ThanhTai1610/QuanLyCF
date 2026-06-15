# Tài liệu dự án Quản lý Cafe (QuanLyCF)

Toàn bộ tài liệu dự án gom tại thư mục `docs/` này.

| # | Tài liệu | Nội dung |
|---|---|---|
| 01 | [Quy tắc Code](01-QUY-TAC-CODE.md) | Nguyên tắc & chuẩn code BẮT BUỘC cho cả nhóm |
| 02 | [Kiến trúc Backend](02-KIEN-TRUC-BACKEND.md) | Cấu trúc thư mục Features đầy đủ + đối chiếu module |
| 03 | [Cơ sở dữ liệu](03-CO-SO-DU-LIEU.md) | Schema SQL đầy đủ cho tất cả module |
| 04 | [Lộ trình](04-LO-TRINH.md) | Tiến độ theo phase + quy tắc nghiệp vụ đã chốt |
| 05 | [Hướng dẫn Dev](05-HUONG-DAN-DEV.md) | Cài đặt, chạy FE/BE, vòng đời migration |

## Công nghệ
- **Backend**: ASP.NET Core 8 Web API · EF Core (Code First) · SQL Server · JWT + RBAC.
- **Frontend**: Vue 3 + TypeScript + Vite + Pinia + Tailwind.

## Bắt đầu nhanh
1. Đọc [01-QUY-TAC-CODE.md](01-QUY-TAC-CODE.md) trước khi code.
2. Cài đặt theo [05-HUONG-DAN-DEV.md](05-HUONG-DAN-DEV.md).

## Quy tắc tự áp dụng cho MỌI trợ lý AI
**Toàn bộ nội dung chỉ nằm trong `docs/`.** Các file dưới đây ở gốc repo chỉ là **con trỏ** (bắt buộc
đặt đúng vị trí để từng công cụ AI nhận diện) — chúng trỏ về `docs/01-QUY-TAC-CODE.md`:

| Công cụ AI | File con trỏ |
|---|---|
| Chuẩn chung (nhiều agent) | `AGENTS.md` |
| Claude Code | `CLAUDE.md` + skill `.claude/skills/quanlycf-conventions/` |
| GitHub Copilot | `.github/copilot-instructions.md` |
| Cursor | `.cursor/rules/quanlycf.mdc` (+ `.cursorrules` bản cũ) |
| Windsurf | `.windsurfrules` |
| Cline | `.clinerules` |

> ✅ **Chỉ cần sửa quy tắc ở `docs/01-QUY-TAC-CODE.md`** — các file con trỏ không chứa nội dung
> riêng nên không lo lệch. Tài liệu mới đặt trong `docs/`, đánh số thứ tự, cập nhật bảng đầu file.
