# Cấu trúc Backend theo Features (Vertical Slice + Layered trong từng feature)

Quy ước mỗi feature: `Controllers / Services / Repositories / DTOs / Models / Validators`.
Phần dùng chung đặt ngoài `Features` (Infrastructure, Shared, Program).

> Ghi chú: code Phase 1 (Authentication) và Phase 2 (Catalog) hiện đang dùng bản gọn
> (Controller + Service/DbContext, chưa tách Repository/Validator). Cấu trúc dưới đây là
> đích đến đầy đủ — sẽ bổ sung Repository + Validator dần theo từng phase (hoặc refactor lại nếu muốn).

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
│   │   ├── Models/                       # entity Code First
│   │   │   ├── NhanVien.cs, VaiTro.cs, Quyen.cs, VaiTroQuyen.cs, RefreshToken.cs
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
│   │   ├── Models/
│   │   │   ├── DanhMuc.cs, SanPham.cs, KichCoSanPham.cs, Combo.cs, ChiTietCombo.cs
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
│   │   ├── Models/
│   │   │   ├── NguyenLieu.cs, NhaCungCap.cs, PhieuKho.cs, ChiTietPhieuKho.cs
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
│   │   ├── Models/
│   │   │   ├── DonHang.cs, ChiTietDonHang.cs, HoaDon.cs
│   │   │   ├── Ban.cs, KhuVucBan.cs, KhuyenMai.cs
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
│   │   # (dùng lại Models của Catalog & Ordering)
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
│   │   ├── Models/
│   │   │   ├── KhachHang.cs, DanhGia.cs, LichSuChatbot.cs
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
│   │   ├── Models/
│   │   │   ├── ChamCong.cs, CaLamViec.cs
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
│   │   ├── Models/
│   │   │   └── DongTien.cs
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
│       ├── DTOs/
│       │   ├── Setting*.cs, AuditLog*.cs
│       └── Models/
│           ├── CaiDatHeThong.cs, NhatKyHeThong.cs
│
├── Infrastructure/                      # Hạ tầng dùng chung
│   └── Persistence/
│       ├── QuanLyCFDbContext.cs          ✅
│       ├── Configurations/               # IEntityTypeConfiguration<T> cho từng entity
│       │   ├── NhanVienConfiguration.cs, SanPhamConfiguration.cs, ...
│       ├── Seed/
│       │   ├── DbSeeder.cs               ✅  (admin)
│       │   └── RbacSeed.cs               (vai trò/quyền)
│       └── Migrations/                   ✅  (InitPhase1, AddCatalog, ...)
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

| Module (trang) | Feature | Controller |
|---|---|---|
| Quản lý tài khoản (Admin) | Authentication | AccountController |
| Tài khoản (hồ sơ cá nhân) | Authentication | AuthController (`/me`) |
| (RBAC vai trò→quyền) | Authentication | RoleController |
| Danh Mục (Admin) | Catalog | CategoryController |
| Quản lý thực đơn | Catalog | ProductController |
| Quản lý combo (Admin) | Catalog | ComboController |
| Quản lý Kho nguyên liệu (Admin) | Inventory | MaterialController |
| Nhà cung cấp & Nhập kho (Admin) | Inventory | SupplierController + StockReceiptController |
| Kiểm kê (Nhân viên) | Inventory | StockTakeController |
| Quản lý Đơn hàng (Nhân viên) | Ordering | OrderController |
| Bán hàng tại quầy (Nhân viên) | Ordering | PosController |
| Thanh toán (Khách/Nhân viên) | Ordering | PaymentController |
| Hoá đơn (Nhân viên) | Ordering | InvoiceController |
| Màn hình bếp (Nhân viên) | Ordering | KitchenController |
| Quản lý Bàn QR (Admin) | Ordering | TableController |
| Trang chủ (Khách hàng) | Storefront | HomeController |
| Khách hàng & QR Đặt món (Khách) | Storefront | CustomerMenuController |
| Khách hàng thân thiết (Nhân viên) | Customers | CustomerController |
| Chatbot (Khách hàng) | Customers | ChatbotController |
| Chấm công (Nhân viên) | HumanResource | AttendanceController |
| Dòng tiền & Chi phí (Admin) | Finance | CashFlowController |
| Báo cáo (Admin) | Finance | ReportController |
| Cài đặt hệ thống (Admin) | System | SettingController |
```
