using aspp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

// 🔥 FIX CORS TRIỆT ĐỂ (quan trọng)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy
                .SetIsOriginAllowed(_ => true) // 🔥 cho phép mọi origin (fix preflight)
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

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// Middleware
app.UseHttpsRedirection();

// 🔥 CORS PHẢI ĐẶT Ở ĐÂY
app.UseCors("AllowReact");

// Routing
app.MapControllers();

// Test root
app.MapGet("/", () => "API is running...");

app.Run();