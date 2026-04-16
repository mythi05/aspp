using aspp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

// 🔥 ADD CORS (QUAN TRỌNG)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // React
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

// 🔥 BẬT CORS (PHẢI ĐẶT TRƯỚC MapControllers)
app.UseCors("AllowReact");

app.MapControllers();

app.MapGet("/", () => "API is running...");

app.Run();