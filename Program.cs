using aspp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OfficeOpenXml; // 🔥 thiếu cái này

var builder = WebApplication.CreateBuilder(args);

// 🔥 EPPlus license

ExcelPackage.License.SetNonCommercialPersonal("Thi Do");

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// 🔥 CORS (dev thì ok, production không nên để all)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DoThiMyThi_2123110490",
        Version = "v1",
        Description = "API quản lý ký túc xá"
    });
});

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Middleware
app.UseHttpsRedirection();

// 🔥 CORS phải trước MapControllers
app.UseCors("AllowReact");

app.MapControllers();

app.MapGet("/", () => "API is running...");

app.Run();