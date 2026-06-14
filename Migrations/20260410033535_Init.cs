using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspp.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DangKyDichVu",
                table: "DangKyDichVu");

            migrationBuilder.RenameTable(
                name: "DangKyDichVu",
                newName: "ServiceRegistrations");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "ServiceRegistrations",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "SinhVien",
                table: "ServiceRegistrations",
                newName: "StudentName");

            migrationBuilder.RenameColumn(
                name: "Phong",
                table: "ServiceRegistrations",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "NgayKetThuc",
                table: "ServiceRegistrations",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "NgayBatDau",
                table: "ServiceRegistrations",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "MSSV",
                table: "ServiceRegistrations",
                newName: "Room");

            migrationBuilder.RenameColumn(
                name: "Gia",
                table: "ServiceRegistrations",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "DichVu",
                table: "ServiceRegistrations",
                newName: "ServiceName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceRegistrations",
                table: "ServiceRegistrations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceRegistrations",
                table: "ServiceRegistrations");

            migrationBuilder.RenameTable(
                name: "ServiceRegistrations",
                newName: "DangKyDichVu");

            migrationBuilder.RenameColumn(
                name: "StudentName",
                table: "DangKyDichVu",
                newName: "SinhVien");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "DangKyDichVu",
                newName: "Phong");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "DangKyDichVu",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "DangKyDichVu",
                newName: "NgayKetThuc");

            migrationBuilder.RenameColumn(
                name: "ServiceName",
                table: "DangKyDichVu",
                newName: "DichVu");

            migrationBuilder.RenameColumn(
                name: "Room",
                table: "DangKyDichVu",
                newName: "MSSV");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "DangKyDichVu",
                newName: "Gia");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "DangKyDichVu",
                newName: "NgayBatDau");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DangKyDichVu",
                table: "DangKyDichVu",
                column: "Id");
        }
    }
}
