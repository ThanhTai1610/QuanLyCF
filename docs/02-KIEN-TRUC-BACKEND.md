# Cấu trúc Backend theo Features (Vertical Slice + Layered trong từng feature)

Quy ước mỗi feature: `Controllers / Services / Repositories / DTOs / Validators`.
**Entity (Code First) KHÔNG nằm trong Features** mà đặt tập trung ở `Domain/Entities/<Nhóm>/`
(namespace luôn là `BackEnd.Domain.Entities`) — xem [01-QUY-TAC-CODE.md](01-QUY-TAC-CODE.md) mục 1.
Phần dùng chung đặt ngoài `Features` (Infrastructure, Shared, Program).

> **Hiện trạng (cập nhật 2026-06):** code đang ở **bản gọn** — file để phẳng trong từng feature
> (vd `CategoriesController.cs` + `CategoryDtos.cs` cùng cấp), CHƯA tách thư mục con
> `Controllers/ Services/ DTOs/`, CHƯA có tầng `Repositories/` và `Validators/`.
> Một số feature (Catalog, Tables) hiện inject thẳng `DbContext` vào Controller, chưa có Service —
> **cần refactor đưa logic về Service** để đạt Definition of Done (Controller phải mỏng).
> Cấu trúc cây dưới đây là **đích đến đầy đủ** — bổ sung dần theo từng phase.
>
> Khác biệt tên nhóm so với code thực tế: `Authentication` (doc) được tách thành 2 feature
> `Auth/` + `Accounts/`; `Ordering` (doc) thực tế là `Sales/`. Feature `Weather/` là template
> mặc định còn sót, nên xoá.

```
BackEnd/BackEnd/                         # thư mục project (src)
│
├── Program.cs                           # cấu hình DI, JWT, CORS, Swagger, migrate+seed
├── appsettings.json                     # ConnectionStrings, Jwt, Cors
│
├── Features/
│   │
│   ├── Authentication/                  # Đăng nhập, tài khoản, phân quyền  ✅ (đã có bản gọn)
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs         # login, pin-login, refresh, logout, me
│   │   │   ├── AccountController.cs      # CRUD tài khoản, khoá/mở, reset mật khẩu, set PIN
│   │   │   └── RoleController.cs         # danh sách vai trò, gán quyền cho vai trò
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   ├── AccountService.cs
│   │   │   ├── RoleService.cs
│   │   │   └── JwtTokenService.cs
│   │   ├── Repositories/
│   │   │   ├── IAccountRepository.cs / AccountRepository.cs
│   │   │   ├── IRoleRepository.cs / RoleRepository.cs
│   │   │   └── IRefreshTokenRepository.cs / RefreshTokenRepository.cs
│   │   ├── DTOs/
│   │   │   ├── LoginRequest.cs, PinLoginRequest.cs, RefreshRequest.cs
│   │   │   ├── TokenResponse.cs, AccountInfo.cs
│   │   │   ├── CreateAccountRequest.cs, UpdateAccountRequest.cs
│   │   │   ├── ResetPasswordRequest.cs, SetPinRequest.cs
│   │   │   └── RoleItem.cs, AssignPermissionsRequest.cs
│   │   │   # Entity → Domain/Entities/Identity/ (NhanVien, VaiTro, Quyen, VaiTroQuyen, RefreshToken)
│   │   └── Validators/
│   │       ├── LoginRequestValidator.cs
│   │       ├── CreateAccountRequestValidator.cs
│   │       └── UpdateAccountRequestValidator.cs
│   │
│   ├── Catalog/                         # Thực đơn  ✅ (đã có bản gọn)
│   │   ├── Controllers/
│   │   │   ├── CategoryController.cs     # Danh mục
│   │   │   ├── ProductController.cs      # Sản phẩm + size
│   │   │   └── ComboController.cs        # Combo
│   │   ├── Services/
│   │   │   ├── CategoryService.cs, ProductService.cs, ComboService.cs
│   │   ├── Repositories/
│   │   │   ├── ICategoryRepository.cs / CategoryRepository.cs
│   │   │   ├── IProductRepository.cs / ProductRepository.cs
│   │   │   └── IComboRepository.cs / ComboRepository.cs
│   │   ├── DTOs/
│   │   │   ├── Category*.cs (List/Create/Update)
│   │   │   ├── Product*.cs (List/Detail/Create/Update + SizeDto)
│   │   │   └── Combo*.cs (List/Detail/Save + ComboLineDto)
│   │   │   # Entity → Domain/Entities/Catalog/ (DanhMuc, SanPham, KichCoSanPham, Combo, ChiTietCombo)
│   │   └── Validators/
│   │       ├── CreateProductRequestValidator.cs
│   │       ├── CreateCategoryRequestValidator.cs
│   │       └── SaveComboRequestValidator.cs
│   │
│   ├── Inventory/                       # Kho, nhà cung cấp, nhập hàng, kiểm kê  (Phase 3)
│   │   ├── Controllers/
│   │   │   ├── MaterialController.cs     # Nguyên liệu (NguyenLieu)
│   │   │   ├── SupplierController.cs     # Nhà cung cấp
│   │   │   ├── StockReceiptController.cs # Phiếu nhập kho
│   │   │   └── StockTakeController.cs    # Kiểm kê
│   │   ├── Services/
│   │   │   ├── MaterialService.cs, SupplierService.cs
│   │   │   ├── StockReceiptService.cs    # tăng tồn + cập nhật giá vốn TB + công nợ
│   │   │   └── StockTakeService.cs       # duyệt → điều chỉnh tồn
│   │   ├── Repositories/
│   │   │   ├── IMaterialRepository.cs / MaterialRepository.cs
│   │   │   ├── ISupplierRepository.cs / SupplierRepository.cs
│   │   │   └── IStockDocumentRepository.cs / StockDocumentRepository.cs
│   │   ├── DTOs/
│   │   │   ├── Material*.cs, Supplier*.cs, StockReceipt*.cs, StockTake*.cs
│   │   │   # Entity → Domain/Entities/Inventory/ (NguyenLieu, NhaCungCap, PhieuKho, ChiTietPhieuKho)
│   │   └── Validators/
│   │       ├── StockReceiptRequestValidator.cs
│   │       └── StockTakeRequestValidator.cs
│   │
│   ├── Ordering/                        # Đơn hàng, POS, thanh toán, hoá đơn, bếp, bàn (Phase 4-5)
│   │   ├── Controllers/
│   │   │   ├── OrderController.cs        # Quản lý đơn hàng
│   │   │   ├── PosController.cs          # Bán hàng tại quầy
│   │   │   ├── PaymentController.cs      # Thanh toán
│   │   │   ├── InvoiceController.cs      # Hoá đơn
│   │   │   ├── KitchenController.cs      # Màn hình bếp (KDS)
│   │   │   ├── TableController.cs        # Quản lý bàn + QR
│   │   │   └── PromotionController.cs    # Khuyến mãi/voucher
│   │   ├── Services/
│   │   │   ├── OrderService.cs, PosService.cs, PaymentService.cs
│   │   │   ├── InvoiceService.cs, KitchenService.cs, TableService.cs, PromotionService.cs
│   │   ├── Repositories/
│   │   │   ├── IOrderRepository.cs / OrderRepository.cs
│   │   │   ├── IInvoiceRepository.cs / InvoiceRepository.cs
│   │   │   ├── ITableRepository.cs / TableRepository.cs
│   │   │   └── IPromotionRepository.cs / PromotionRepository.cs
│   │   ├── DTOs/
│   │   │   ├── Order*.cs, Pos*.cs, Payment*.cs, Invoice*.cs
│   │   │   ├── Kitchen*.cs, Table*.cs, Promotion*.cs
│   │   │   # Entity → Domain/Entities/Sales/ (DonHang, ChiTietDonHang, HoaDon, Ban, KhuVucBan, KhuyenMai)
│   │   └── Validators/
│   │       ├── CreateOrderRequestValidator.cs
│   │       └── PaymentRequestValidator.cs
│   │
│   ├── Storefront/                      # Khách hàng: trang chủ + QR đặt món (public)
│   │   ├── Controllers/
│   │   │   ├── HomeController.cs         # Trang chủ (thông tin quán, menu nổi bật)
│   │   │   └── CustomerMenuController.cs # Xem menu theo bàn + gửi đơn qua QR
│   │   ├── Services/
│   │   │   └── StorefrontService.cs
│   │   ├── DTOs/
│   │   │   ├── PublicMenuDto.cs, PlaceOrderRequest.cs
│   │   └── Validators/
│   │       └── PlaceOrderRequestValidator.cs
│   │   # (dùng lại Entity của Catalog & Sales trong Domain/Entities/)
│   │
│   ├── Customers/                       # Khách thân thiết, đánh giá, chatbot
│   │   ├── Controllers/
│   │   │   ├── CustomerController.cs     # KH thân thiết (hạng, điểm, đổi quà)
│   │   │   ├── ReviewController.cs       # Đánh giá
│   │   │   └── ChatbotController.cs      # Lịch sử chatbot
│   │   ├── Services/
│   │   │   ├── CustomerService.cs, LoyaltyService.cs, ReviewService.cs, ChatbotService.cs
│   │   ├── Repositories/
│   │   │   ├── ICustomerRepository.cs / CustomerRepository.cs
│   │   │   ├── IReviewRepository.cs / ReviewRepository.cs
│   │   │   └── IChatbotRepository.cs / ChatbotRepository.cs
│   │   ├── DTOs/
│   │   │   ├── Customer*.cs, Review*.cs, Chatbot*.cs
│   │   │   # Entity → Domain/Entities/Customers/ (KhachHang, DanhGia, LichSuChatbot)
│   │   └── Validators/
│   │       └── CreateCustomerRequestValidator.cs
│   │
│   ├── HumanResource/                   # Chấm công, ca làm, bảng lương  (Phase 7)
│   │   ├── Controllers/
│   │   │   ├── AttendanceController.cs   # Chấm công
│   │   │   ├── ShiftController.cs        # Ca làm / xếp ca / đơn từ
│   │   │   └── PayrollController.cs      # Bảng lương
│   │   ├── Services/
│   │   │   ├── AttendanceService.cs, ShiftService.cs, PayrollService.cs
│   │   ├── Repositories/
│   │   │   ├── IAttendanceRepository.cs / AttendanceRepository.cs
│   │   │   └── IShiftRepository.cs / ShiftRepository.cs
│   │   ├── DTOs/
│   │   │   ├── Attendance*.cs, Shift*.cs, Payroll*.cs
│   │   │   # Entity → Domain/Entities/Hr/ (ChamCong, CaLamViec)
│   │   └── Validators/
│   │       └── CheckInRequestValidator.cs
│   │
│   ├── Finance/                         # Dòng tiền & báo cáo  (Phase 8)
│   │   ├── Controllers/
│   │   │   ├── CashFlowController.cs     # Dòng tiền & chi phí
│   │   │   └── ReportController.cs       # Báo cáo doanh thu/lợi nhuận (chỉ đọc)
│   │   ├── Services/
│   │   │   ├── CashFlowService.cs, ReportService.cs
│   │   ├── Repositories/
│   │   │   └── ICashFlowRepository.cs / CashFlowRepository.cs
│   │   ├── DTOs/
│   │   │   ├── CashFlow*.cs, Report*.cs (RevenueReport, ProfitReport, ...)
│   │   │   # Entity → Domain/Entities/Finance/ (DongTien)
│   │   └── Validators/
│   │       └── CashFlowEntryValidator.cs
│   │
│   └── System/                          # Cài đặt & nhật ký hệ thống  (Phase 9)
│       ├── Controllers/
│       │   ├── SettingController.cs      # Cài đặt hệ thống
│       │   └── AuditLogController.cs     # Nhật ký hệ thống
│       ├── Services/
│       │   ├── SettingService.cs, AuditLogService.cs
│       ├── Repositories/
│       │   ├── ISettingRepository.cs / SettingRepository.cs
│       │   └── IAuditLogRepository.cs / AuditLogRepository.cs
│       └── DTOs/
│           ├── Setting*.cs, AuditLog*.cs
│       # Entity → Domain/Entities/System/ (CaiDatHeThong, NhatKyHeThong)
│
├── Domain/                              # Tầng miền — KHÔNG phụ thuộc Infrastructure/Features
│   └── Entities/                        # Entity Code First (namespace BackEnd.Domain.Entities)  ✅
│       ├── Identity/   NhanVien, VaiTro, Quyen, VaiTroQuyen, RefreshToken
│       ├── Catalog/    DanhMuc, SanPham, KichCoSanPham, Combo, ChiTietCombo
│       ├── Inventory/  NguyenLieu, NhaCungCap, PhieuKho, ChiTietPhieuKho
│       ├── Sales/      Ban, KhuVucBan (+ DonHang, HoaDon, KhuyenMai... khi tới phase)
│       ├── Customers/  KhachHang, DanhGia, LichSuChatbot
│       ├── Hr/         ChamCong, CaLamViec
│       ├── Finance/    DongTien
│       └── System/     CaiDatHeThong, NhatKyHeThong
│
├── Infrastructure/                      # Hạ tầng dùng chung
│   └── Persistence/
│       ├── QuanLyCFDbContext.cs          ✅
│       ├── DbSeeder.cs                   ✅  (admin)  — Configurations/Seed gộp trong file này (bản gọn)
│       └── (Configurations/ tách riêng khi cần — đích đến)
│
├── Migrations/                          ✅  (nằm ở gốc project, không trong Infrastructure)
│
└── Shared/                              # Cross-cutting
    ├── Quyens.cs                         ✅  (mã quyền RBAC)
    ├── PasswordHasher.cs                 ✅
    ├── Common/
    │   ├── ApiResult.cs                  # bao response chuẩn { success, data, message }
    │   ├── PagedResult.cs                # phân trang
    │   ├── PaginationQuery.cs
    │   └── CurrentUser.cs                # đọc MaNhanVien/quyền từ JWT claims
    ├── Repositories/
    │   ├── IRepository.cs                # generic CRUD
    │   └── BaseRepository.cs             # cài đặt EF Core dùng chung
    ├── Middlewares/
    │   ├── ExceptionHandlingMiddleware.cs
    │   └── AuditLogMiddleware.cs         # tự ghi NhatKyHeThong cho POST/PUT/DELETE
    └── Extensions/
        ├── ServiceCollectionExtensions.cs # gom AddFeatureServices()
        └── ClaimsPrincipalExtensions.cs
```

## Đối chiếu 22 module ↔ Feature/Controller

Cột **TT** (Tình trạng): ✅ đã có code · ⏳ chưa làm. Cột **Feature** ghi tên thư mục thực tế.

| Module (trang) | Feature (thư mục) | Controller | TT |
|---|---|---|---|
| Quản lý tài khoản (Admin) | Accounts | AccountsController | ✅ |
| Tài khoản (hồ sơ cá nhân) | Auth | AuthController (`/me`) | ✅ |
| (RBAC vai trò→quyền) | Auth | (đang gộp, chưa tách RoleController) | ⏳ |
| Danh Mục (Admin) | Catalog/Categories | CategoriesController | ✅ |
| Quản lý thực đơn | Catalog/Products | ProductsController | ✅ |
| Quản lý combo (Admin) | Catalog/Combos | CombosController | ✅ |
| Quản lý Kho nguyên liệu (Admin) | Inventory/Materials | MaterialsController | ✅ |
| Nhà cung cấp & Nhập kho (Admin) | Inventory/Suppliers + /StockReceipts | SuppliersController + StockReceiptsController | ✅ |
| Kiểm kê (Nhân viên) | Inventory/StockTakes | StockTakesController | ✅ |
| Quản lý Bàn QR (Admin) | Sales/Tables | TablesController + ZonesController | ✅ |
| Quản lý Đơn hàng (Nhân viên) | Sales (Ordering) | OrderController | ⏳ |
| Bán hàng tại quầy (Nhân viên) | Sales (Ordering) | PosController | ⏳ |
| Thanh toán (Khách/Nhân viên) | Sales (Ordering) | PaymentController | ⏳ |
| Hoá đơn (Nhân viên) | Sales (Ordering) | InvoiceController | ⏳ |
| Màn hình bếp (Nhân viên) | Sales (Ordering) | KitchenController | ⏳ |
| Trang chủ (Khách hàng) | Storefront | HomeController | ⏳ |
| Khách hàng & QR Đặt món (Khách) | Storefront | CustomerMenuController | ⏳ |
| Khách hàng thân thiết (Nhân viên) | Customers | CustomerController | ⏳ |
| Chatbot (Khách hàng) | Customers | ChatbotController | ⏳ |
| Chấm công (Nhân viên) | HumanResource | AttendanceController | ⏳ |
| Dòng tiền & Chi phí (Admin) | Finance | CashFlowController | ⏳ |
| Báo cáo (Admin) | Finance | ReportController | ⏳ |
| Cài đặt hệ thống (Admin) | System | SettingController | ⏳ |
