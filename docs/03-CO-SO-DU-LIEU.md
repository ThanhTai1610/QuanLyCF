# CSDL QuanLyCF — SQL HOÀN CHỈNH CHO TẤT CẢ MODULE

> Bản thiết kế đầy đủ, dựa trên SQL gốc + các cải tiến trong quá trình code:
> - RBAC chuẩn hoá (`Quyen` + `VaiTro_Quyen`) thay cho cột JSON.
> - `NhanVien` thêm bảo mật đăng nhập + PIN; có `RefreshToken`.
> - Bổ sung các bảng còn thiếu: xếp ca, đơn từ, bảng lương, lịch sử điểm, phần thưởng,
>   thanh toán nhiều lần (partial), phần thưởng đổi điểm.
>
> ✅ = đã code & chạy (Phase 1–3) · 🕒 = thiết kế sẵn, sẽ tạo ở phase sau.
> Dự án dùng **EF Code First** — file này để đọc/đối chiếu; DB thật sinh bằng migration.
>
> Quy ước kiểu: `VARCHAR` cho mã/enum/email/sđt/SKU · `NVARCHAR` cho chữ tiếng Việt ·
> `DATETIME2` cho thời gian · `DECIMAL` cho tiền/số lượng · `BIT` cho cờ.

```sql
CREATE DATABASE QuanLyCF;
GO
USE QuanLyCF;
GO
```

---

## 1. XÁC THỰC & PHÂN QUYỀN  ✅

```sql
-- Quyền đơn lẻ (permission)
CREATE TABLE Quyen (
    MaQuyen  INT IDENTITY PRIMARY KEY,
    MaCode   VARCHAR(50)   NOT NULL UNIQUE,   -- TAIKHOAN_QUANLY, DONHANG_XULY...
    TenQuyen NVARCHAR(150) NOT NULL,
    Nhom     VARCHAR(50)   NOT NULL
);

-- Vai trò (role)
CREATE TABLE VaiTro (
    MaVaiTro        INT IDENTITY PRIMARY KEY,
    TenVaiTro       NVARCHAR(50)  NOT NULL UNIQUE,
    MoTa            NVARCHAR(500) NULL,
    LaVaiTroHeThong BIT NOT NULL DEFAULT 0     -- vai trò mặc định, không cho xoá
);

-- Gán quyền cho vai trò (n-n)
CREATE TABLE VaiTro_Quyen (
    MaVaiTro INT NOT NULL,
    MaQuyen  INT NOT NULL,
    CONSTRAINT PK_VaiTro_Quyen PRIMARY KEY (MaVaiTro, MaQuyen),
    CONSTRAINT FK_VTQ_VaiTro FOREIGN KEY (MaVaiTro) REFERENCES VaiTro(MaVaiTro) ON DELETE CASCADE,
    CONSTRAINT FK_VTQ_Quyen  FOREIGN KEY (MaQuyen)  REFERENCES Quyen(MaQuyen)  ON DELETE CASCADE
);

-- Tài khoản nhân viên
CREATE TABLE NhanVien (
    MaNhanVien           INT IDENTITY PRIMARY KEY,
    MaVaiTro             INT NOT NULL,
    HoTen                NVARCHAR(100) NOT NULL,
    Email                VARCHAR(100)  NOT NULL UNIQUE,   -- tên đăng nhập
    MatKhauHash          VARCHAR(255)  NOT NULL,          -- BCrypt
    MaPinHash            VARCHAR(255)  NULL,              -- PIN đăng nhập ca (BCrypt)
    TokenKhoiPhucMatKhau VARCHAR(255)  NULL,
    HanTokenKhoiPhuc     DATETIME2     NULL,
    SoDienThoai          VARCHAR(20)   NULL,
    NgaySinh             DATE          NULL,
    DiaChi               NVARCHAR(255) NULL,
    SoCCCD               VARCHAR(20)   NULL UNIQUE,
    SoTaiKhoanNganHang   VARCHAR(50)   NULL,
    TenNganHang          NVARCHAR(100) NULL,
    LuongCoBan           DECIMAL(12,2) NULL,              -- lương/giờ hoặc lương cứng
    AnhDaiDien           VARCHAR(255)  NULL,
    TrangThaiHoatDong    BIT NOT NULL DEFAULT 1,          -- 1=hoạt động, 0=đã khoá
    LanDangNhapCuoi      DATETIME2     NULL,
    SoLanDangNhapSai     INT NOT NULL DEFAULT 0,
    KhoaDenKhi           DATETIME2     NULL,              -- khoá tạm do sai nhiều lần
    ThoiGianTao          DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ThoiGianCapNhat      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_NhanVien_VaiTro FOREIGN KEY (MaVaiTro) REFERENCES VaiTro(MaVaiTro)
);

-- Refresh token cho JWT
CREATE TABLE RefreshToken (
    MaRefreshToken INT IDENTITY PRIMARY KEY,
    MaNhanVien     INT NOT NULL,
    Token          VARCHAR(255) NOT NULL,
    HetHan         DATETIME2 NOT NULL,
    ThoiGianTao    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ThoiGianThuHoi DATETIME2 NULL,
    CONSTRAINT FK_RefreshToken_NhanVien FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien) ON DELETE CASCADE
);
CREATE INDEX IX_RefreshToken_Token ON RefreshToken(Token);
```

---

## 2. NHÂN SỰ · CA LÀM · LƯƠNG  🕒

```sql
-- Định nghĩa ca làm
CREATE TABLE CaLamViec (
    MaCa              INT IDENTITY PRIMARY KEY,
    TenCa             NVARCHAR(50) NOT NULL,    -- Ca Sáng, Ca Tối, Full-time
    GioBatDau         TIME NOT NULL,
    GioKetThuc        TIME NOT NULL,
    TrangThaiHoatDong BIT NOT NULL DEFAULT 1
);

-- Xếp ca cho nhân viên theo ngày
CREATE TABLE PhanCaLamViec (
    MaPhanCa    INT IDENTITY PRIMARY KEY,
    MaNhanVien  INT NOT NULL,
    MaCa        INT NOT NULL,
    NgayLamViec DATE NOT NULL,
    GhiChu      NVARCHAR(255) NULL,            -- OT, Nghỉ...
    CONSTRAINT FK_PhanCa_NhanVien FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    CONSTRAINT FK_PhanCa_Ca       FOREIGN KEY (MaCa)       REFERENCES CaLamViec(MaCa)
);

-- Chấm công
CREATE TABLE ChamCong (
    MaChamCong     INT IDENTITY PRIMARY KEY,
    MaNhanVien     INT NOT NULL,
    MaCa           INT NULL,
    ThoiGianVao    DATETIME2 NOT NULL,
    ThoiGianRa     DATETIME2 NULL,
    AnhVao         VARCHAR(255) NULL,          -- ảnh xác thực check-in
    AnhRa          VARCHAR(255) NULL,
    SoPhutDiTre    INT DEFAULT 0,
    SoPhutVeSom    INT DEFAULT 0,
    LamThemGioPhut INT DEFAULT 0,
    TrangThai      VARCHAR(50) NULL,           -- HopLe, KhongHopLe, XinPhep
    GhiChu         NVARCHAR(MAX) NULL,
    CONSTRAINT FK_ChamCong_NhanVien FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    CONSTRAINT FK_ChamCong_Ca       FOREIGN KEY (MaCa)       REFERENCES CaLamViec(MaCa)
);

-- Đơn từ nhân viên (nghỉ phép, tăng ca, nghỉ bù...)
CREATE TABLE DonTuNhanVien (
    MaDon            INT IDENTITY PRIMARY KEY,
    MaNhanVien       INT NOT NULL,
    LoaiDon          VARCHAR(50) NOT NULL,     -- PhepNam, TangCa, NghiKhongLuong, NghiBu
    ThoiGianLienQuan NVARCHAR(100) NULL,       -- "T5 20/04 - Ca Sáng"
    LyDo             NVARCHAR(MAX) NULL,
    TrangThai        VARCHAR(50) NOT NULL DEFAULT 'ChoDuyet', -- ChoDuyet, DaDuyet, TuChoi
    MaNguoiDuyet     INT NULL,
    ThoiGianTao      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_DonTu_NhanVien FOREIGN KEY (MaNhanVien)   REFERENCES NhanVien(MaNhanVien),
    CONSTRAINT FK_DonTu_NguoiDuyet FOREIGN KEY (MaNguoiDuyet) REFERENCES NhanVien(MaNhanVien)
);

-- Bảng lương theo kỳ
CREATE TABLE BangLuong (
    MaBangLuong  INT IDENTITY PRIMARY KEY,
    MaNhanVien   INT NOT NULL,
    Ky           VARCHAR(7) NOT NULL,          -- 2026-06
    LuongTheoGio DECIMAL(12,2) NOT NULL,
    SoGioThuong  DECIMAL(8,2)  NOT NULL DEFAULT 0,
    SoGioOT      DECIMAL(8,2)  NOT NULL DEFAULT 0,
    SoNgayPhep   DECIMAL(5,1)  NOT NULL DEFAULT 0,
    PhuCap       DECIMAL(12,2) NOT NULL DEFAULT 0,
    Thuong       DECIMAL(12,2) NOT NULL DEFAULT 0,
    Phat         DECIMAL(12,2) NOT NULL DEFAULT 0,
    ThucLanh     DECIMAL(12,2) NOT NULL DEFAULT 0,
    TrangThai    VARCHAR(50) NOT NULL DEFAULT 'Nhap', -- Nhap, DaChot, DaTra
    ThoiGianTao  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_BangLuong_NhanVien FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien)
);
```

---

## 3. THỰC ĐƠN (CATALOG)  ✅

```sql
-- Danh mục (đa cấp)
CREATE TABLE DanhMuc (
    MaDanhMuc         INT IDENTITY PRIMARY KEY,
    MaDanhMucCha      INT NULL,
    TenDanhMuc        NVARCHAR(100) NOT NULL,
    HinhAnh           VARCHAR(255) NULL,
    ThuTuHienThi      INT NOT NULL DEFAULT 0,
    MoTa              NVARCHAR(MAX) NULL,
    TrangThaiHoatDong BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_DanhMuc_Cha FOREIGN KEY (MaDanhMucCha) REFERENCES DanhMuc(MaDanhMuc)
);

-- Sản phẩm
CREATE TABLE SanPham (
    MaSanPham           INT IDENTITY PRIMARY KEY,
    MaDanhMuc           INT NULL,
    TenSanPham          NVARCHAR(150) NOT NULL,
    MaVach_SKU          VARCHAR(50) NULL UNIQUE,
    GiaVonDuKien        DECIMAL(10,2) NULL,       -- nhập tay
    GiaBan              DECIMAL(10,2) NOT NULL,
    HinhAnh             VARCHAR(255) NULL,
    MoTa                NVARCHAR(MAX) NULL,
    LuongCalo           INT NULL,
    ThoiGianChuanBiPhut INT NULL,                 -- KDS ước tính giờ ra món
    LaMonNoiBat         BIT NOT NULL DEFAULT 0,
    KieuMon             VARCHAR(50) NOT NULL DEFAULT 'MonChinh', -- MonChinh, Topping, MonKem
    TrangThaiBan        BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_SanPham_DanhMuc FOREIGN KEY (MaDanhMuc) REFERENCES DanhMuc(MaDanhMuc) ON DELETE SET NULL
);

-- Kích cỡ (size)
CREATE TABLE KichCoSanPham (
    MaKichCo          INT IDENTITY PRIMARY KEY,
    MaSanPham         INT NOT NULL,
    TenKichCo         NVARCHAR(50) NOT NULL,      -- M, L
    GiaCongThem       DECIMAL(10,2) NOT NULL DEFAULT 0,
    TrangThaiHoatDong BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_KichCo_SanPham FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham) ON DELETE CASCADE
);

-- Combo
CREATE TABLE Combo (
    MaCombo           INT IDENTITY PRIMARY KEY,
    TenCombo          NVARCHAR(150) NOT NULL,
    GiaCombo          DECIMAL(10,2) NOT NULL,
    HinhAnh           VARCHAR(255) NULL,
    MoTa              NVARCHAR(MAX) NULL,
    TrangThaiHoatDong BIT NOT NULL DEFAULT 1
);

CREATE TABLE ChiTietCombo (
    MaChiTietCombo INT IDENTITY PRIMARY KEY,
    MaCombo        INT NOT NULL,
    MaSanPham      INT NOT NULL,
    SoLuong        INT NOT NULL DEFAULT 1,
    CONSTRAINT FK_CTCombo_Combo   FOREIGN KEY (MaCombo)   REFERENCES Combo(MaCombo) ON DELETE CASCADE,
    CONSTRAINT FK_CTCombo_SanPham FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);
```

---

## 4. KHUYẾN MÃI  🕒

```sql
CREATE TABLE KhuyenMai (
    MaKhuyenMai       INT IDENTITY PRIMARY KEY,
    MaGiamGia         VARCHAR(50) NULL UNIQUE,    -- mã nhập tay
    TenChuongTrinh    NVARCHAR(150) NOT NULL,
    LoaiGiamGia       VARCHAR(50) NOT NULL,       -- PhanTram, TienMat, TangMon
    GiaTriGiam        DECIMAL(10,2) NOT NULL,
    GiamToiDa         DECIMAL(10,2) NULL,         -- giảm 20% tối đa 50k
    DonToiThieu       DECIMAL(10,2) NULL,
    SoLuongGioiHan    INT NULL,
    SoLuongDaDung     INT NOT NULL DEFAULT 0,
    NgayBatDau        DATETIME2 NULL,
    NgayKetThuc       DATETIME2 NULL,
    MoTa              NVARCHAR(MAX) NULL,
    HinhAnh           VARCHAR(255) NULL,
    TrangThaiHoatDong BIT NOT NULL DEFAULT 1
);
```

---

## 5. KHO · NHÀ CUNG CẤP · NHẬP/KIỂM KÊ  ✅

```sql
CREATE TABLE NguyenLieu (
    MaNguyenLieu    INT IDENTITY PRIMARY KEY,
    TenNguyenLieu   NVARCHAR(150) NOT NULL,
    MaVach_SKU      VARCHAR(50) NULL UNIQUE,
    DonViTinh       NVARCHAR(20) NOT NULL,       -- g, ml, Lon, Chiếc
    SoLuongTon      DECIMAL(10,3) NOT NULL DEFAULT 0,
    MucTonToiThieu  DECIMAL(10,3) NULL,
    MucTonToiDa     DECIMAL(10,3) NULL,
    GiaVonTrungBinh DECIMAL(10,2) NULL,          -- bình quân gia quyền
    HanSuDungNgay   INT NULL,
    HinhAnh         VARCHAR(255) NULL
);

CREATE TABLE NhaCungCap (
    MaNhaCungCap  INT IDENTITY PRIMARY KEY,
    TenNhaCungCap NVARCHAR(150) NOT NULL,
    MaSoThue      VARCHAR(50) NULL,
    NguoiLienHe   NVARCHAR(100) NULL,
    SoDienThoai   VARCHAR(20) NULL,
    Email         VARCHAR(100) NULL,
    DiaChi        NVARCHAR(MAX) NULL,
    SoTaiKhoan    VARCHAR(50) NULL,
    TenNganHang   NVARCHAR(100) NULL,
    CongNoHienTai DECIMAL(12,2) NOT NULL DEFAULT 0
);

-- Phiếu kho: NhapKho / KiemKe / XuatHuy / TraHang
CREATE TABLE PhieuKho (
    MaPhieu            INT IDENTITY PRIMARY KEY,
    LoaiPhieu          VARCHAR(50) NOT NULL,
    MaNhanVien         INT NOT NULL,
    MaNhaCungCap       INT NULL,
    TongTienHang       DECIMAL(12,2) NULL,
    TienDaThanhToan    DECIMAL(12,2) NULL,
    TrangThaiThanhToan VARCHAR(50) NULL,         -- ChuaThanhToan, ThanhToanMotPhan, DaThanhToan
    TrangThai          VARCHAR(50) NOT NULL DEFAULT 'DaHoanTat', -- ChoDuyet, DaDuyet, TuChoi
    GhiChu             NVARCHAR(MAX) NULL,
    ThoiGianTao        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_PhieuKho_NhanVien   FOREIGN KEY (MaNhanVien)   REFERENCES NhanVien(MaNhanVien),
    CONSTRAINT FK_PhieuKho_NhaCungCap FOREIGN KEY (MaNhaCungCap) REFERENCES NhaCungCap(MaNhaCungCap)
);

CREATE TABLE ChiTietPhieuKho (
    MaChiTietPhieu INT IDENTITY PRIMARY KEY,
    MaPhieu        INT NOT NULL,
    MaNguyenLieu   INT NOT NULL,
    SoLuong        DECIMAL(10,3) NOT NULL,       -- SL nhập/xuất (hoặc tồn HT khi kiểm kê)
    DonGia         DECIMAL(10,2) NULL,
    SoLuongThucTe  DECIMAL(10,3) NULL,           -- số đếm thực tế khi kiểm kê
    LyDoLech       NVARCHAR(MAX) NULL,
    CONSTRAINT FK_CTPK_PhieuKho   FOREIGN KEY (MaPhieu)      REFERENCES PhieuKho(MaPhieu) ON DELETE CASCADE,
    CONSTRAINT FK_CTPK_NguyenLieu FOREIGN KEY (MaNguyenLieu) REFERENCES NguyenLieu(MaNguyenLieu)
);
```

---

## 6. BÀN & KHU VỰC  🕒

```sql
CREATE TABLE KhuVucBan (
    MaKhuVuc  INT IDENTITY PRIMARY KEY,
    TenKhuVuc NVARCHAR(100) NOT NULL,            -- Tầng 1, Sân thượng, VIP
    PhuThu    DECIMAL(10,2) NOT NULL DEFAULT 0
);

CREATE TABLE Ban (
    MaBan     INT IDENTITY PRIMARY KEY,
    MaKhuVuc  INT NOT NULL,
    TenBan    NVARCHAR(20) NOT NULL UNIQUE,
    SucChua   INT NULL,
    MaQRHash  VARCHAR(255) NOT NULL UNIQUE,       -- token QR đặt món
    TrangThai VARCHAR(50) NOT NULL DEFAULT 'Trong', -- Trong, CoKhach, BaoTri
    CONSTRAINT FK_Ban_KhuVuc FOREIGN KEY (MaKhuVuc) REFERENCES KhuVucBan(MaKhuVuc)
);
```

---

## 7. KHÁCH HÀNG · TÍCH ĐIỂM · ĐÁNH GIÁ · CHATBOT  🕒

```sql
CREATE TABLE KhachHang (
    MaKhachHang     INT IDENTITY PRIMARY KEY,
    Email           VARCHAR(100) NULL UNIQUE,
    HoTen           NVARCHAR(100) NULL,
    SoDienThoai     VARCHAR(20) NULL,
    GioiTinh        NVARCHAR(10) NULL,
    NgaySinh        DATE NULL,
    AnhDaiDien      VARCHAR(255) NULL,
    HangThanhVien   NVARCHAR(50) NOT NULL DEFAULT N'Member', -- Member, Silver, Gold, Diamond
    DiemTichLuy     INT NOT NULL DEFAULT 0,
    TongTienDaTieu  DECIMAL(15,2) NOT NULL DEFAULT 0,
    LanGheThamCuoi  DATETIME2 NULL,
    GhiChuKhachHang NVARCHAR(MAX) NULL,
    ThoiGianTao     DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Phần thưởng đổi điểm
CREATE TABLE PhanThuong (
    MaPhanThuong INT IDENTITY PRIMARY KEY,
    TenPhanThuong NVARCHAR(150) NOT NULL,
    DiemCanDoi   INT NOT NULL,
    MoTa         NVARCHAR(MAX) NULL,
    TrangThaiHoatDong BIT NOT NULL DEFAULT 1
);
```

---

## 8. ĐƠN HÀNG · BÁN · BẾP  🕒

```sql
CREATE TABLE DonHang (
    MaDonHang       INT IDENTITY PRIMARY KEY,
    MaBan           INT NULL,
    MaKhachHang     INT NULL,
    MaNhanVien      INT NULL,                    -- người tạo đơn
    MaKhuyenMai     INT NULL,
    LoaiDonHang     VARCHAR(50) NOT NULL DEFAULT 'DineIn', -- DineIn, TakeAway, QR
    TongTienHang    DECIMAL(10,2) NOT NULL,
    TienGiamGia     DECIMAL(10,2) NOT NULL DEFAULT 0,
    PhiDichVu       DECIMAL(10,2) NOT NULL DEFAULT 0,
    ThueVAT         DECIMAL(10,2) NOT NULL DEFAULT 0,
    ThanhTien       DECIMAL(10,2) NOT NULL,
    GhiChuDonHang   NVARCHAR(MAX) NULL,
    TrangThaiDon    VARCHAR(50) NOT NULL DEFAULT 'ChoXacNhan', -- ChoXacNhan, DangPha, HoanThanh, Huy
    LyDoHuy         NVARCHAR(MAX) NULL,
    ThoiGianTao     DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ThoiGianCapNhat DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_DonHang_Ban        FOREIGN KEY (MaBan)       REFERENCES Ban(MaBan),
    CONSTRAINT FK_DonHang_KhachHang  FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    CONSTRAINT FK_DonHang_NhanVien   FOREIGN KEY (MaNhanVien)  REFERENCES NhanVien(MaNhanVien),
    CONSTRAINT FK_DonHang_KhuyenMai  FOREIGN KEY (MaKhuyenMai) REFERENCES KhuyenMai(MaKhuyenMai)
);

CREATE TABLE ChiTietDonHang (
    MaChiTiet         INT IDENTITY PRIMARY KEY,
    MaDonHang         INT NOT NULL,
    MaSanPham         INT NULL,
    MaKichCo          INT NULL,
    MaCombo           INT NULL,
    SoLuong           INT NOT NULL,
    DonGia            DECIMAL(10,2) NOT NULL,    -- đã cộng giá size
    TienGiamGia       DECIMAL(10,2) NOT NULL DEFAULT 0,
    ThanhTien         DECIMAL(10,2) NOT NULL,
    GhiChuMon         NVARCHAR(MAX) NULL,        -- ít đá, 50% đường
    -- Phục vụ màn hình bếp (KDS)
    TrangThaiBep      VARCHAR(50) NOT NULL DEFAULT 'ChoLam', -- ChoLam, DangLam, HoanThanh, DaTraKhach
    MaNhanVienPhaChe  INT NULL,                  -- nhân viên được gán làm món
    BaoHetNguyenLieu  BIT NOT NULL DEFAULT 0,
    ThoiGianBaoBep    DATETIME2 NULL,
    ThoiGianLamXong   DATETIME2 NULL,
    CONSTRAINT FK_CTDH_DonHang  FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang) ON DELETE CASCADE,
    CONSTRAINT FK_CTDH_SanPham  FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
    CONSTRAINT FK_CTDH_KichCo   FOREIGN KEY (MaKichCo)  REFERENCES KichCoSanPham(MaKichCo),
    CONSTRAINT FK_CTDH_Combo    FOREIGN KEY (MaCombo)   REFERENCES Combo(MaCombo),
    CONSTRAINT FK_CTDH_PhaChe   FOREIGN KEY (MaNhanVienPhaChe) REFERENCES NhanVien(MaNhanVien)
);
```

---

## 9. THANH TOÁN · HOÁ ĐƠN  🕒

```sql
CREATE TABLE HoaDon (
    MaHoaDon          INT IDENTITY PRIMARY KEY,
    MaDonHang         INT NOT NULL UNIQUE,
    MaNhanVienThuNgan INT NULL,
    MaSoThueXuatHD    VARCHAR(50) NULL,          -- nếu xuất hoá đơn đỏ
    TongThanhTien     DECIMAL(10,2) NOT NULL,
    SoTienKhachTra    DECIMAL(10,2) NOT NULL DEFAULT 0,
    TienThoiLai       DECIMAL(10,2) NOT NULL DEFAULT 0,
    TrangThai         VARCHAR(50) NOT NULL DEFAULT 'DaThanhToan', -- ChuaTT, ThanhToanMotPhan, DaThanhToan
    ThoiGianThanhToan DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_HoaDon_DonHang  FOREIGN KEY (MaDonHang)         REFERENCES DonHang(MaDonHang),
    CONSTRAINT FK_HoaDon_NhanVien FOREIGN KEY (MaNhanVienThuNgan) REFERENCES NhanVien(MaNhanVien)
);

-- Chi tiết thanh toán (hỗ trợ nhiều phương thức / trả nhiều lần)
CREATE TABLE ThanhToanChiTiet (
    MaThanhToan       INT IDENTITY PRIMARY KEY,
    MaHoaDon          INT NOT NULL,
    PhuongThuc        VARCHAR(50) NOT NULL,      -- TienMat, ChuyenKhoan, Momo, VNPay, ZaloPay, The
    SoTien            DECIMAL(10,2) NOT NULL,
    MaGiaoDichCong    VARCHAR(100) NULL,         -- mã giao dịch cổng thanh toán
    ThoiGianThanhToan DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_TTCT_HoaDon FOREIGN KEY (MaHoaDon) REFERENCES HoaDon(MaHoaDon) ON DELETE CASCADE
);
```

---

## 10. KHÁCH HÀNG – HOẠT ĐỘNG (cần Đơn hàng)  🕒

```sql
-- Lịch sử biến động điểm
CREATE TABLE LichSuDiem (
    MaLichSuDiem  INT IDENTITY PRIMARY KEY,
    MaKhachHang   INT NOT NULL,
    MaDonHang     INT NULL,
    LoaiBienDong  VARCHAR(20) NOT NULL,          -- Tich, Doi, DieuChinh
    SoDiem        INT NOT NULL,                  -- + tích / - đổi
    GhiChu        NVARCHAR(255) NULL,
    ThoiGianTao   DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_LSDiem_KhachHang FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    CONSTRAINT FK_LSDiem_DonHang   FOREIGN KEY (MaDonHang)   REFERENCES DonHang(MaDonHang)
);

-- Đánh giá
CREATE TABLE DanhGia (
    MaDanhGia        INT IDENTITY PRIMARY KEY,
    MaKhachHang      INT NOT NULL,
    MaDonHang        INT NULL,
    DiemSo           INT NOT NULL CHECK (DiemSo BETWEEN 1 AND 5),
    BinhLuan         NVARCHAR(MAX) NULL,
    HinhAnhDinhKem   VARCHAR(500) NULL,
    PhanHoiTuQuanLy  NVARCHAR(MAX) NULL,
    TrangThaiHienThi BIT NOT NULL DEFAULT 1,
    ThoiGianTao      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_DanhGia_KhachHang FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    CONSTRAINT FK_DanhGia_DonHang   FOREIGN KEY (MaDonHang)   REFERENCES DonHang(MaDonHang)
);

-- Lịch sử chatbot
CREATE TABLE LichSuChatbot (
    MaLichSu      INT IDENTITY PRIMARY KEY,
    MaKhachHang   INT NULL,
    PhienChat     VARCHAR(100) NULL,
    TinNhanKhach  NVARCHAR(MAX) NOT NULL,
    PhanHoiBot    NVARCHAR(MAX) NOT NULL,
    IntentContext VARCHAR(100) NULL,             -- HoiMenu, KhieuNai, HoiKhuyenMai
    ThoiGianTao   DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Chatbot_KhachHang FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang)
);
```

---

## 11. TÀI CHÍNH (DÒNG TIỀN)  🕒

```sql
CREATE TABLE DongTien (
    MaDongTien         INT IDENTITY PRIMARY KEY,
    LoaiGiaoDich       VARCHAR(20) NOT NULL,     -- Thu, Chi
    NhomGiaoDich       VARCHAR(100) NOT NULL,    -- DoanhThuPOS, NhapHang, TraLuong, DienNuoc, MatBang
    MaThamChieu        INT NULL,                 -- ID HoaDon / PhieuKho liên quan
    PhuongThucThanhToan VARCHAR(50) NOT NULL,
    SoTien             DECIMAL(12,2) NOT NULL,
    NguoiNopNhan       NVARCHAR(100) NULL,
    GhiChu             NVARCHAR(MAX) NULL,
    ChungTuDinhKem     VARCHAR(500) NULL,
    MaNhanVienGhiNhan  INT NOT NULL,
    ThoiGianTao        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_DongTien_NhanVien FOREIGN KEY (MaNhanVienGhiNhan) REFERENCES NhanVien(MaNhanVien)
);
```

---

## 12. HỆ THỐNG (NHẬT KÝ · CÀI ĐẶT)  🕒

```sql
CREATE TABLE NhatKyHeThong (
    MaNhatKy    INT IDENTITY PRIMARY KEY,
    MaNhanVien  INT NULL,
    HanhDong    VARCHAR(255) NOT NULL,           -- CREATE, UPDATE, DELETE, LOGIN
    Module      VARCHAR(100) NOT NULL,
    DuLieuCu    NVARCHAR(MAX) NULL,              -- JSON trước khi sửa
    DuLieuMoi   NVARCHAR(MAX) NULL,              -- JSON sau khi sửa
    DiaChiIP    VARCHAR(50) NULL,
    ThietBi     VARCHAR(255) NULL,
    ThoiGianTao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_NhatKy_NhanVien FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien)
);

CREATE TABLE CaiDatHeThong (
    MaCaiDat        INT IDENTITY PRIMARY KEY,
    NhomCaiDat      VARCHAR(50) NOT NULL,        -- CHUNG, THANH_TOAN, TICH_DIEM
    KhoaCaiDat      VARCHAR(100) NOT NULL UNIQUE,-- THUE_VAT_MAC_DINH, TY_LE_TICH_DIEM
    GiaTriCaiDat    NVARCHAR(MAX) NULL,
    MoTa            NVARCHAR(MAX) NULL,
    ThoiGianCapNhat DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
```

---

## 13. DỮ LIỆU SEED

```sql
-- 16 quyền
SET IDENTITY_INSERT Quyen ON;
INSERT INTO Quyen (MaQuyen, MaCode, Nhom, TenQuyen) VALUES
 (1,'TAIKHOAN_XEM','TaiKhoan',N'Xem tài khoản'),(2,'TAIKHOAN_QUANLY','TaiKhoan',N'Quản lý tài khoản'),
 (3,'SANPHAM_XEM','SanPham',N'Xem sản phẩm'),(4,'SANPHAM_QUANLY','SanPham',N'Quản lý sản phẩm'),
 (5,'KHO_XEM','Kho',N'Xem kho'),(6,'KHO_QUANLY','Kho',N'Quản lý kho'),
 (7,'DONHANG_XEM','DonHang',N'Xem đơn hàng'),(8,'DONHANG_XULY','DonHang',N'Xử lý đơn hàng'),
 (9,'BEP_XEM','Bep',N'Màn hình bếp'),(10,'THANHTOAN','ThanhToan',N'Thanh toán'),
 (11,'KHACHHANG_XEM','KhachHang',N'Xem khách hàng'),(12,'KHACHHANG_QUANLY','KhachHang',N'Quản lý khách hàng'),
 (13,'NHANSU_XEM','NhanSu',N'Xem nhân sự'),(14,'NHANSU_QUANLY','NhanSu',N'Quản lý nhân sự'),
 (15,'BAOCAO_XEM','BaoCao',N'Xem báo cáo'),(16,'CAIDAT_QUANLY','CaiDat',N'Quản lý cài đặt');
SET IDENTITY_INSERT Quyen OFF;

-- 5 vai trò
SET IDENTITY_INSERT VaiTro ON;
INSERT INTO VaiTro (MaVaiTro, LaVaiTroHeThong, MoTa, TenVaiTro) VALUES
 (1,1,N'Toàn quyền hệ thống',N'Quản lý'),(2,1,N'Bếp, đơn hàng, xem kho',N'Pha chế'),
 (3,1,N'Bán hàng, thu ngân, hoá đơn',N'Thu ngân'),(4,1,N'Đơn hàng, bàn, phục vụ',N'Phục vụ'),
 (5,1,N'Màn hình bếp',N'Bếp');
SET IDENTITY_INSERT VaiTro OFF;

-- Gán quyền
INSERT INTO VaiTro_Quyen (MaVaiTro, MaQuyen) VALUES
 (1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7),(1,8),(1,9),(1,10),(1,11),(1,12),(1,13),(1,14),(1,15),(1,16),
 (2,3),(2,5),(2,7),(2,8),(2,9),
 (3,3),(3,7),(3,8),(3,10),(3,11),
 (4,3),(4,7),(4,8),
 (5,7),(5,9);

-- 5 danh mục
SET IDENTITY_INSERT DanhMuc ON;
INSERT INTO DanhMuc (MaDanhMuc, TenDanhMuc, ThuTuHienThi, TrangThaiHoatDong) VALUES
 (1,N'Cà phê',1,1),(2,N'Trà',2,1),(3,N'Đá xay',3,1),(4,N'Bánh',4,1),(5,N'Khác',5,1);
SET IDENTITY_INSERT DanhMuc OFF;

-- Cài đặt mặc định
INSERT INTO CaiDatHeThong (NhomCaiDat, KhoaCaiDat, GiaTriCaiDat, MoTa) VALUES
 ('CHUNG','TEN_QUAN',N'BrewManager Coffee',N'Tên quán'),
 ('CHUNG','DIA_CHI',N'',N'Địa chỉ quán'),
 ('THANH_TOAN','THUE_VAT_MAC_DINH','8',N'Thuế VAT mặc định (%)'),
 ('THANH_TOAN','PHI_DICH_VU','0',N'Phí dịch vụ mặc định (đ)'),
 ('TICH_DIEM','TY_LE_TICH_DIEM','1',N'Số điểm cho mỗi 1.000đ');

-- Tài khoản Quản lý mặc định (BE tự tạo lúc khởi động, mật khẩu lưu BCrypt):
--   Email: admin@brew.vn | Mật khẩu: demo1234 | PIN: 2006
```

---

## Tình trạng triển khai

| Module | Bảng | Trạng thái |
|---|---|---|
| Xác thực & phân quyền | Quyen, VaiTro, VaiTro_Quyen, NhanVien, RefreshToken | ✅ Phase 1 |
| Thực đơn | DanhMuc, SanPham, KichCoSanPham, Combo, ChiTietCombo | ✅ Phase 2 |
| Kho | NguyenLieu, NhaCungCap, PhieuKho, ChiTietPhieuKho | ✅ Phase 3 |
| Bàn & Đơn hàng & Bếp | KhuVucBan, Ban, DonHang, ChiTietDonHang | 🕒 Phase 4 |
| Thanh toán & Hoá đơn | HoaDon, ThanhToanChiTiet, KhuyenMai | 🕒 Phase 5 |
| Khách thân thiết | KhachHang, PhanThuong, LichSuDiem, DanhGia, LichSuChatbot | 🕒 Phase 6 |
| Nhân sự & Lương | CaLamViec, PhanCaLamViec, ChamCong, DonTuNhanVien, BangLuong | 🕒 Phase 7 |
| Tài chính | DongTien | 🕒 Phase 8 |
| Hệ thống | NhatKyHeThong, CaiDatHeThong | 🕒 Phase 9 |
