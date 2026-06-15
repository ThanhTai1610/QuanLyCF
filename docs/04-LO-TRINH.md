# Lộ trình triển khai theo Phase

Thứ tự phụ thuộc: nền tảng → danh mục → kho → giao dịch → dẫn xuất.
✅ đã xong · 🕒 chưa làm.

| Phase | Nội dung | Module/Feature | Trạng thái |
|---|---|---|---|
| 0 | Nền tảng: EF, DB, CORS, cấu hình | Infrastructure | ✅ |
| 1 | Xác thực + RBAC + Tài khoản | Authentication | ✅ |
| 2 | Thực đơn: Danh mục → Sản phẩm → Combo | Catalog | ✅ |
| 3 | Kho + Nhà cung cấp + Nhập/Kiểm kê | Inventory | ✅ |
| 4 | Bàn (QR) + Đơn hàng + Bếp (KDS) | Ordering, Tables, Kitchen | 🕒 |
| 5 | Thanh toán → Hoá đơn + VAT/giảm giá | Payments, Invoices, Promotions | 🕒 |
| 6 | Khách thân thiết (tích/đổi điểm) | Customers, Loyalty | 🕒 |
| 7 | Chấm công + Ca làm + Lương | HumanResource | 🕒 |
| 8 | Dòng tiền + Báo cáo (dẫn xuất) | Finance | 🕒 |
| 9 | Cài đặt + Nhật ký hệ thống (audit) | System | 🕒 song song |

## Quy tắc nghiệp vụ đã chốt
- **Giá vốn kho**: bình quân gia quyền. Nhập thẳng theo đơn vị tồn.
- **Đã bỏ module Công thức (Recipes)** → kho quản lý thủ công (nhập/điều chỉnh/kiểm kê), không trừ kho tự động theo định mức.

## Cần chốt trước khi làm Phase 5
- Thuế VAT (mặc định 8%), phí dịch vụ, quy tắc giảm giá/voucher, mã hoá đơn, có hoá đơn điện tử không.
