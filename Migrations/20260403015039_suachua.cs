using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspp.Migrations
{
    /// <inheritdoc />
    public partial class suachua : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaoTriSuaChua",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Phong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NguoiBaoCao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Loai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MucDo = table.Column<int>(type: "int", nullable: false),
                    NgayBaoCao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoTriSuaChua", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaoTriSuaChua");
        }
    }
}
