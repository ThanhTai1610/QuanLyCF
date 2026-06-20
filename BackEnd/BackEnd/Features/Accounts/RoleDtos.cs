namespace BackEnd.Features.Accounts;

// Một quyền (trang/chức năng) để tick khi cấu hình vai trò
public record PermissionItem(int MaQuyen, string MaCode, string TenQuyen, string Nhom);

// Vai trò kèm danh sách mã quyền + số tài khoản đang dùng
public record RoleDetail(
    int MaVaiTro,
    string TenVaiTro,
    string? MoTa,
    bool LaVaiTroHeThong,
    int SoTaiKhoan,
    List<string> Quyens);   // danh sách MaCode

// Tạo / cập nhật vai trò (Quyens = danh sách MaCode được tick)
public record SaveRoleRequest(string TenVaiTro, string? MoTa, List<string> Quyens);