using System.Text;
using BackEnd.Features.Auth;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}
