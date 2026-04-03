using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspp.Migrations
{
    /// <inheritdoc />
    public partial class Adddevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThietBi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenThietBi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Loai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    TinhTrang = table.Column<int>(type: "int", nullable: false),
                    NgayMua = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GiaTri = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThietBi", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThietBi");
        }
    }
}
