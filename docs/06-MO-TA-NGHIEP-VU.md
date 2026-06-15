# Mô tả & Nghiệp vụ hệ thống QuanLyCF

- **Mô hình**: SaaS Multi-tenant (hệ thống lớn cho nhiều chủ quán thuê; mỗi tài khoản chủ quán quản lý **01 quán** duy nhất).
- **Hình thức vận hành**: Kết hợp **đặt món tại quầy (POS)** và **quét mã QR đặt món + thanh toán trước tại bàn**.

---

## PHẦN 1 — Đối tượng sử dụng (Actors)

Phân quyền chặt cho 4 nhóm, đảm bảo an toàn & cách ly dữ liệu giữa các Tenant (các quán độc lập):

| Actor | Vai trò |
|---|---|
| **Super Admin** (Admin hệ thống SaaS) | Quản lý toàn hệ thống, quản lý danh sách tài khoản Chủ quán (Tenant); kích hoạt / tạm khoá / gia hạn tài khoản các quán. |
| **Admin** (Chủ quán) | Quản lý toàn bộ cấu hình nội bộ quán: menu, sơ đồ bàn/QR, nhân viên, nhà cung cấp, xem báo cáo dòng tiền, duyệt ca chấm công lỗi. |
| **Staff** (Quầy / Bếp / Chạy bàn) | Order tại quầy, tiếp nhận đơn, đổi bàn, nhận món tại bếp, kiểm kê kho cuối ngày, chấm công bằng FaceID. |
| **Customer** (Khách hàng) | Quét QR tại bàn để xem menu, đặt món, áp mã tích điểm/xác thực OTP, thanh toán trước qua VietQR động; tương tác Chatbot để được tư vấn/gọi hỗ trợ. |

---

## PHẦN 2 — 22 module chức năng & nghiệp vụ bắt buộc

### NHÓM 1 — Trải nghiệm khách hàng (Customer Interface)

#### 1. Trang chủ (Khách hàng)
**Mô tả**: Giao diện khi khách truy cập hệ thống của quán qua liên kết chung hoặc tìm kiếm.
**Nghiệp vụ**: Hiển thị thông tin tổng quan quán (tên, địa chỉ, hotline, giờ mở cửa), banner khuyến mãi hiện hành, danh mục/món nổi bật để kích thích nhu cầu.

#### 2. Khách hàng & QR Đặt món (Khách hàng)
**Mô tả**: Giao diện khi khách quét mã QR vật lý dán tại bàn.
**Nghiệp vụ**:
- Tự động nhận diện **Số bàn** từ tham số trên mã QR (vd `/?table=05`).
- Khách xem thực đơn, chọn món, chọn tùy chọn (topping, lượng đường, lượng đá).
- Bấm **"Đặt món"** → bắt buộc chuyển sang màn hình **Thanh toán trước**.
- Món đã được nhân viên cập nhật **hết hàng** → nút chọn bị khoá, hiển thị "Hết hàng".

#### 3. Thanh toán (Khách hàng / Nhân viên)
**Mô tả**: Xử lý luồng tiền ra/vào cho từng đơn tại bàn hoặc tại quầy.
**Nghiệp vụ**:
- **Tại bàn**: Sinh **mã VietQR động** chứa đúng số tiền + nội dung CK định danh đơn. Khách quét trả tiền; khi ngân hàng báo nhận tiền (Webhook/API), hệ thống tự xác nhận đơn và **bắn dữ liệu xuống Bếp**.
- **Tại quầy**: Thu ngân chọn "Tiền mặt" hoặc "Quét QR tại quầy" rồi **in hoá đơn**.

#### 4. Chatbot (Khách hàng)
**Mô tả**: Trợ lý ảo AI tích hợp trên giao diện đặt món, hỗ trợ real-time.
**Nghiệp vụ**:
- **Tư vấn**: trả lời tự động về menu, gợi ý món theo sở thích, tư vấn combo đang giảm giá.
- **Điều phối vận hành**: khách gõ lệnh nhanh (vd "Cho bàn 3 xin thêm đá", "Bàn 5 cần gọi nhân viên") → Chatbot nhận diện và **bắn thông báo khẩn xuống Màn hình bếp** để xử lý ngay.

### NHÓM 2 — Tiền sảnh & vận hành tại quán (Operation & POS)

#### 5. Bán hàng tại quầy (Nhân viên)
**Mô tả**: Màn hình POS dành cho thu ngân.
**Nghiệp vụ**: Chọn món cho khách đang chờ, nhập giảm giá / SĐT thành viên, nhận tiền, xác nhận đơn. Bấm **"Hoàn thành"** → máy in tại quầy tự **in phiếu chế biến** gửi vào Bếp.

#### 6. Quản lý Đơn hàng (Nhân viên)
**Mô tả**: Trung tâm tiếp nhận & điều phối mọi hoá đơn trong quán.
**Nghiệp vụ**:
- Hiển thị đơn từ **cả 2 nguồn**: tại quầy và QR tại bàn.
- **Đổi bàn**: khách đã thanh toán Bàn 5 muốn chuyển Bàn 3 → chọn đơn → "Đổi bàn" → cập nhật DB + **bắn thông báo real-time xuống Bếp** (vd "Đơn #102: đổi Bàn 5 → Bàn 3").
- **Hết món / Huỷ đơn**: đơn đã thanh toán nhưng Bếp báo hết nguyên liệu → "Huỷ món/Huỷ đơn" → kích hoạt **hoàn tiền tự động** cho khách.

#### 7. Màn hình bếp (Nhân viên)
**Mô tả**: Danh sách món cần chế biến đặt tại quầy pha chế/bếp.
**Nghiệp vụ**:
- Nhận dữ liệu **real-time** ngay khi đơn thanh toán hợp lệ; sắp xếp theo thời gian gọi (vào trước làm trước).
- Tiếp nhận **thông báo khẩn từ Chatbot** của khách (thêm đá, gọi nhân viên).
- Pha chế xong bấm **"Hoàn thành"** → báo nhân viên chạy bàn mang món ra đúng số bàn.

#### 8. Hoá đơn (Nhân viên)
**Mô tả**: Lưu trữ & quản lý toàn bộ chứng từ giao dịch tài chính.
**Nghiệp vụ**: Tra cứu hoá đơn cũ, in lại bill theo yêu cầu, kiểm tra trạng thái (Thành công, Đã huỷ, Đã hoàn tiền).

#### 9. Khách hàng thân thiết (Nhân viên)
**Mô tả**: Quản lý thành viên, hạng thẻ, cơ chế đổi thưởng.
**Nghiệp vụ**:
- **Tích điểm**: nhập SĐT khi lên đơn → tự cộng điểm theo giá trị hoá đơn.
- **Đổi/tiêu điểm**: khi khách dùng điểm trừ tiền vào hoá đơn → hệ thống **bắt buộc gửi OTP** về SĐT khách; nhập đúng OTP mới được cấn trừ tiền mặt.

### NHÓM 3 — Kho & Nhà cung cấp (Inventory Management)

#### 10. Quản lý Kho nguyên liệu (Admin)
**Mô tả**: Danh mục & theo dõi tồn kho toàn bộ vật tư (hạt cà phê, sữa đặc, đường, ly, ống hút...).
**Nghiệp vụ**: Thiết lập danh mục vật tư, đơn vị tính; đặt **"Ngưỡng cảnh báo tối thiểu"** — tồn chạm ngưỡng → tự **cảnh báo Admin** để nhập hàng.

#### 11. Quản lý Nhà cung cấp & Nhập kho (Admin)
**Mô tả**: Quản lý đối tác cung ứng & quy trình nhập hàng.
**Nghiệp vụ**: Lưu thông tin NCC (tên, SĐT, công nợ). Khi nhập: tạo phiếu Nhập kho → điền SL & đơn giá → duyệt phiếu → tồn **tự tăng**, đồng thời giá trị đơn nhập **tự ghi nhận khoản chi (Cash-out)** ở Module Dòng tiền.

#### 12. Kiểm kê (Nhân viên)
**Mô tả**: Chốt sổ kho định kỳ **thủ công** (không trừ kho tự động theo công thức ly nước).
**Nghiệp vụ**: Cuối ngày/tuần, đếm số lượng thực tế và nhập vào module; hệ thống lấy **thực tế − tồn lý thuyết** để tính **hao hụt/thất thoát** trong kỳ.

### NHÓM 4 — Menu & cấu hình hệ thống (Admin Configuration)

#### 13 & 14. Quản lý thực đơn & Trang Danh Mục (Admin)
**Mô tả**: Thiết lập cấu trúc Menu hiển thị cho khách.
**Nghiệp vụ**: Tạo danh mục (Cà phê, Trà, Bánh ngọt). Mỗi danh mục thêm món chi tiết: tên, ảnh, giá, mô tả, nhóm tùy chọn (topping, kích cỡ ly S/M/L). Có **nút bật/tắt nhanh "Còn hàng / Hết hàng"** cập nhật trực tiếp lên trang đặt món.

#### 15. Quản lý Combo (Admin)
**Mô tả**: Gói sản phẩm bán chung giá ưu đãi (vd Combo Sáng Suốt = 1 Cà phê + 1 Bánh mì).
**Nghiệp vụ**: Lưu combo là gói có giá đặc biệt. Khi khách đặt combo → **tách dữ liệu**: hiển thị tên combo tổng trên Màn hình bếp, đồng thời liệt kê chi tiết từng món lẻ để bếp biết cần chuẩn bị gì.

#### 16. Quản lý Bàn bằng QR (Admin)
**Mô tả**: Số hoá không gian vật lý của quán lên phần mềm.
**Nghiệp vụ**: Nhập sơ đồ bàn (Bàn 1, Bàn 2, khu Ban công...). Mỗi bàn mới → hệ thống tự sinh **mã QR độc nhất** mã hoá ID bàn; Admin tải QR về để in & dán tại bàn.

### NHÓM 5 — Nhân sự, tài chính & quản trị (Admin & System)

#### 17. Chấm công (Nhân viên)
**Mô tả**: Quản lý ngày công, áp dụng công nghệ chống gian lận.
**Nghiệp vụ**:
- Nhân viên dùng điện thoại cá nhân truy cập hệ thống để chấm công ca.
- **Xác thực**: quét **khuôn mặt** (FaceID/ảnh đối chiếu) đồng thời kiểm tra **IP & Wifi** có trùng Wifi cố định của quán.
- **Ngoại lệ**: FaceID mờ hoặc rớt Wifi (phải dùng 4G) → trạng thái **"Chờ duyệt"** gửi về Admin; Admin xem ảnh/lý do để **"Duyệt công"** hoặc **"Từ chối công"** thủ công.

#### 18. Dòng tiền & Chi phí vận hành (Admin)
**Mô tả**: Sổ cái thu chi nội bộ của quán.
**Nghiệp vụ**:
- **Dòng thu (Cash-in)**: tự đồng bộ & phân loại theo hình thức thanh toán (tiền mặt, VietQR) từ mọi hoá đơn thành công → phục vụ đối soát ngân hàng cuối ngày.
- **Dòng chi (Cash-out)**: tự ghi nhận từ phiếu Nhập kho; cho phép Admin **nhập tay** chi phí phát sinh (điện, nước, mặt bằng, sửa máy pha, lương NV) để tính chuẩn **Lợi nhuận gộp & ròng**.

#### 19. Báo cáo (Admin)
**Mô tả**: Trung tâm phân tích dữ liệu kinh doanh bằng biểu đồ trực quan.
**Nghiệp vụ**: Doanh thu theo ngày/tuần/tháng; tỷ trọng đơn (tại quầy vs tại bàn); món bán chạy nhất; chi phí hao hụt kho sau mỗi lần kiểm kê.

#### 20 & 21. Quản lý tài khoản & Cài đặt hệ thống (Admin)
**Mô tả**: Phân quyền nhân sự nội bộ & cấu hình hạ tầng quán.
**Nghiệp vụ**:
- Phân quyền tài khoản nhân viên (Thu ngân chỉ thấy POS/Đơn hàng; Bếp chỉ thấy Màn hình bếp; Kho chỉ được tạo phiếu kiểm kê).
- Cài đặt thông tin hoá đơn, cấu hình **kết nối IP máy in** hoá đơn vật lý tại quầy/bếp.

#### 22. Tài khoản
**Mô tả**: Trang cá nhân dùng chung cho mọi User khi đăng nhập.
**Nghiệp vụ**: Xem profile cá nhân, đổi mật khẩu, cập nhật **ảnh đại diện** làm mẫu đối chiếu FaceID khi chấm công.
