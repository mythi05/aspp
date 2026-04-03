using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspp.Migrations
{
    /// <inheritdoc />
    public partial class dichvu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DangKyDichVu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MSSV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SinhVien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DichVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyDichVu", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DangKyDichVu");
        }
    }
}
