using aspp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dormitory Management API",
        Version = "v1",
        Description = "Sinh viên thực hiện: Đỗ Thị Mỹ Thi - 2123110490\n\n" + 
        "API quản lý ký túc xá"

    });
});

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// Middleware
app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/", () => "API is running...");

app.Run();
