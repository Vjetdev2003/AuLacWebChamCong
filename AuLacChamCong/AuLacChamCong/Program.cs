using AuLacChamCong.Client.Pages;
using AuLacChamCong.Components;
using AuLacChamCong.Controllers;
using AuLacChamCong.DataApi;
using AuLacChamCong.Models.Shared;
using AuLacChamCong.Services;
using AuLacChamCong.Services.Modules.BenhVien.Service;
using AuLacChamCong.Services.Modules.ChamCong.Services;
using AuLacChamCong.Services.Modules.ChamCong.Services.FaceIdSerivces;
using AuLacChamCong.Services.Modules.DonCongTac.Service;
using AuLacChamCong.Services.Modules.Email;
using AuLacChamCong.Services.Modules.LichLamViec.Service;
using AuLacChamCong.Services.Modules.LichNghi.Service;
using AuLacChamCong.Services.Modules.NhanVien;
using AuLacChamCong.Services.Modules.Notification.Service;
using AuLacChamCong.Services.Modules.Profile.Service;
using AuLacChamCong.Services.Modules.ThongKe.Service;
using AuLacChamCong.Services.Modules.User.Service;
using AuLacChamCong.Services.TinhCong;
using Blazored.Toast;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Ensure that we use the correct connection string from appsettings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register the ApiContext (for normal application database operations)
builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDistributedMemoryCache();

// Register Identity services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));  // Ensure correct DbContext for Identity

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()  // Ensure we use the correct DbContext for Identity
    .AddDefaultTokenProviders();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
});
// Register application services
//builder.Services.AddScoped(sp =>
//{
//    var navigationManager = sp.GetRequiredService<NavigationManager>();
//    return new HttpClient
//    {
//        BaseAddress = new Uri(navigationManager.BaseUri)
//    };
//});
builder.Services.AddServerSideBlazor(options =>
{
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
});

builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 1024 * 1024 * 10;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<Login_Services>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<PersonnelsDb>();
builder.Services.AddScoped<IHospitalServices, HospitalServices>();
builder.Services.AddScoped<IChamCongService, ChamCongService>();
builder.Services.AddScoped<IDonCongTacServices, DonCongTacServices>();
builder.Services.AddScoped<ILichNghiServices, LichNghiServices>();
builder.Services.AddScoped<IStatisticalService, StatisticalService>();
builder.Services.AddScoped<INotificationServices, NotificationServices>();
builder.Services.AddScoped<INhanVien, NhanVien>();
builder.Services.AddScoped<SendMessage>();
builder.Services.AddScoped<ApplicationUser>();
builder.Services.AddScoped<ILichLamViecServices, LichLamViecService>();
builder.Services.AddScoped<IFaceIdService, FaceIdService>();
builder.Services.AddScoped<ITinhCongService, TinhCongServices>();
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44320"); // Địa chỉ API của bạn
});

builder.Services.AddBlazoredToast();
builder.Services.AddSwaggerGen(options => {
    //options.SwaggerDoc("V1", new OpenApiInfo
    //{
    //    Version = "V1",
    //    Title = "WebAPI",
    //    Description = "Product WebAPI"
    //});
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                }
            },
            new List < string > ()
        }
    });
});
// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(AuLacChamCong.Client._Imports).Assembly);


app.Run();
//test123