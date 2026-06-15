using BackEnd.Domain.Entities;
using BackEnd.Shared;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Infrastructure.Persistence;

public static class DbSeeder
{
    /// <summary>Tạo tài khoản Quản lý mặc định nếu chưa có nhân viên nào.</summary>
    public static async Task SeedAsync(QuanLyCFDbContext db)
    {
        if (await db.NhanViens.AnyAsync()) return;

        db.NhanViens.Add(new NhanVien
        {
            HoTen = "Quản trị viên",
            Email = "admin@brew.vn",
            MaVaiTro = 1, // Quản lý
            MatKhauHash = PasswordHasher.Hash("demo1234"),
            MaPinHash = PasswordHasher.Hash("2006"),
            TrangThaiHoatDong = true,
        });
        await db.SaveChangesAsync();
    }
}
