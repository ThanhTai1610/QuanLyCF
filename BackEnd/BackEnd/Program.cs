using System.Text;
using BackEnd.Features.Auth;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace BackEnd
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── EF Core ─────────────────────────────────────────
            builder.Services.AddDbContext<QuanLyCFDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("QuanLyCF")));

            // ── JWT ─────────────────────────────────────────────
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                        ClockSkew = TimeSpan.FromSeconds(30),
                    };
                });

            // Mỗi mã quyền → 1 policy yêu cầu claim "perm" tương ứng
            builder.Services.AddAuthorization(options =>
            {
                foreach (var code in Quyens.TatCa)
                    options.AddPolicy(code, p => p.RequireClaim(JwtTokenService.PermissionClaim, code));
            });

            // ── DI ──────────────────────────────────────────────
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddSingleton<JwtTokenService>();
            builder.Services.AddScoped<Features.Inventory.StockReceipts.StockReceiptService>();
            builder.Services.AddScoped<Features.Inventory.StockTakes.StockTakeService>();
            builder.Services.AddScoped<Features.Sales.Orders.OrderService>();
            builder.Services.AddScoped<Features.Sales.Promotions.PromotionService>();
            // System
            builder.Services.AddScoped<Features.System.SettingService>();
            builder.Services.AddScoped<Features.System.AuditLogService>();
            builder.Services.AddMemoryCache();

            // ── CORS cho frontend Vite ──────────────────────────
            var feOrigin = builder.Configuration["Cors:FrontendOrigin"] ?? "http://localhost:5173";
            builder.Services.AddCors(o => o.AddPolicy("frontend", p =>
                p.WithOrigins(feOrigin).AllowAnyHeader().AllowAnyMethod()));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "QuanLyCF API", Version = "v1" });
                var scheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập: Bearer {access token}",
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };
                c.AddSecurityDefinition("Bearer", scheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = Array.Empty<string>() });
            });

            var app = builder.Build();

            // ── Tự migrate + seed khi khởi động ─────────────────
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<QuanLyCFDbContext>();
                await db.Database.MigrateAsync();
                await DbSeeder.SeedAsync(db);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("frontend");
            app.UseAuthentication();

            // ── Maintenance Mode Middleware (cache 30s để tránh query DB mỗi request) ───
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value ?? "";

                // Các đường dẫn luôn được phép đi qua
                bool isExceptionPath =
                    path.StartsWith("/api/settings/maintenance", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/api/auth/login",          StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/api/auth/staff-login",    StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/swagger",                 StringComparison.OrdinalIgnoreCase);

                if (!isExceptionPath)
                {
                    var cache = context.RequestServices.GetRequiredService<IMemoryCache>();

                    // Đọc từ cache; nếu miss mới query DB (TTL = 30 giây)
                    if (!cache.TryGetValue("__maint_on", out bool isMaint))
                    {
                        var db = context.RequestServices.GetRequiredService<QuanLyCFDbContext>();
                        var row = await db.CaiDatHeThongs
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.KhoaCaiDat == "CHE_DO_BAO_TRI");
                        isMaint = row?.GiaTriCaiDat?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
                        cache.Set("__maint_on", isMaint, TimeSpan.FromSeconds(30));
                    }

                    if (isMaint)
                    {
                        bool isAdmin = context.User.Identity?.IsAuthenticated == true
                                    && context.User.HasClaim("perm", Quyens.CaiDatQuanLy);

                        if (!isAdmin)
                        {
                            if (!cache.TryGetValue("__maint_msg", out string? message) || message is null)
                            {
                                var db = context.RequestServices.GetRequiredService<QuanLyCFDbContext>();
                                var msgRow = await db.CaiDatHeThongs
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.KhoaCaiDat == "THONG_DIEP_BAO_TRI");
                                message = msgRow?.GiaTriCaiDat ?? "Hệ thống đang bảo trì. Vui lòng quay lại sau.";
                                cache.Set("__maint_msg", message, TimeSpan.FromSeconds(30));
                            }

                            context.Response.StatusCode  = StatusCodes.Status503ServiceUnavailable;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsJsonAsync(new { message, isMaintenance = true });
                            return;
                        }
                    }
                }

                await next(context);
            });

            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}
