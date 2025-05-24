using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quangcao.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhGias_AspNetUsers_UserId",
                table: "DanhGias");

            migrationBuilder.DropForeignKey(
                name: "FK_LienHes_AspNetUsers_UserId",
                table: "LienHes");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamYeuThiches_SanPhams_SanPhamIdSanPham",
                table: "SanPhamYeuThiches");

            migrationBuilder.DropForeignKey(
                name: "FK_TinTucs_AspNetUsers_UserId",
                table: "TinTucs");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamYeuThiches_SanPhamIdSanPham",
                table: "SanPhamYeuThiches");

            migrationBuilder.DropIndex(
                name: "IX_LienHes_UserId",
                table: "LienHes");

            migrationBuilder.DropIndex(
                name: "IX_DanhGias_UserId",
                table: "DanhGias");

            migrationBuilder.DropColumn(
                name: "SanPhamIdSanPham",
                table: "SanPhamYeuThiches");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LienHes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DanhGias");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TinTucs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "TieuDe",
                table: "TinTucs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "TinTucs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdSanPham",
                table: "SanPhamYeuThiches",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SanPhamYeuThiches",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<string>(
                name: "ChiTiet",
                table: "SanPhams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoTen",
                table: "LienHes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoai",
                table: "LienHes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiaChi",
                table: "HoaDons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DaBaoCao",
                table: "DanhGias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "DanhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LuotHuuIch",
                table: "DanhGias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenNguoiDung",
                table: "DanhGias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TenSanPham",
                table: "ChiTietHoaDons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoTen",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgaySinh",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnhBiaTrangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTrang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuongDanAnh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnhBiaTrangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnhBiaTrangs_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AnhBiaTrangs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "baogia",
                columns: table => new
                {
                    IdBaoGia = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_baogia", x => x.IdBaoGia);
                    table.ForeignKey(
                        name: "FK_baogia_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gioiThieus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnhGioiThieu = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gioiThieus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gioiThieus_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_gioiThieus_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "thanhVienDoiNgus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TamHiet = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AnhUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_thanhVienDoiNgus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_thanhVienDoiNgus_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_thanhVienDoiNgus_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamYeuThiches_IdSanPham",
                table: "SanPhamYeuThiches",
                column: "IdSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_AnhBiaTrangs_ApplicationUserId",
                table: "AnhBiaTrangs",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnhBiaTrangs_UserId",
                table: "AnhBiaTrangs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_baogia_UserId",
                table: "baogia",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_gioiThieus_ApplicationUserId",
                table: "gioiThieus",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_gioiThieus_UserId",
                table: "gioiThieus",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_thanhVienDoiNgus_ApplicationUserId",
                table: "thanhVienDoiNgus",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_thanhVienDoiNgus_UserId",
                table: "thanhVienDoiNgus",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamYeuThiches_SanPhams_IdSanPham",
                table: "SanPhamYeuThiches",
                column: "IdSanPham",
                principalTable: "SanPhams",
                principalColumn: "IdSanPham",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TinTucs_AspNetUsers_UserId",
                table: "TinTucs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamYeuThiches_SanPhams_IdSanPham",
                table: "SanPhamYeuThiches");

            migrationBuilder.DropForeignKey(
                name: "FK_TinTucs_AspNetUsers_UserId",
                table: "TinTucs");

            migrationBuilder.DropTable(
                name: "AnhBiaTrangs");

            migrationBuilder.DropTable(
                name: "baogia");

            migrationBuilder.DropTable(
                name: "gioiThieus");

            migrationBuilder.DropTable(
                name: "thanhVienDoiNgus");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamYeuThiches_IdSanPham",
                table: "SanPhamYeuThiches");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "TinTucs");

            migrationBuilder.DropColumn(
                name: "ChiTiet",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "HoTen",
                table: "LienHes");

            migrationBuilder.DropColumn(
                name: "SoDienThoai",
                table: "LienHes");

            migrationBuilder.DropColumn(
                name: "DiaChi",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "DaBaoCao",
                table: "DanhGias");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "DanhGias");

            migrationBuilder.DropColumn(
                name: "LuotHuuIch",
                table: "DanhGias");

            migrationBuilder.DropColumn(
                name: "TenNguoiDung",
                table: "DanhGias");

            migrationBuilder.DropColumn(
                name: "DiaChi",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HoTen",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NgaySinh",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TinTucs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TieuDe",
                table: "TinTucs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdSanPham",
                table: "SanPhamYeuThiches",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SanPhamYeuThiches",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SanPhamIdSanPham",
                table: "SanPhamYeuThiches",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "LienHes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DanhGias",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TenSanPham",
                table: "ChiTietHoaDons",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamYeuThiches_SanPhamIdSanPham",
                table: "SanPhamYeuThiches",
                column: "SanPhamIdSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_LienHes_UserId",
                table: "LienHes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGias_UserId",
                table: "DanhGias",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGias_AspNetUsers_UserId",
                table: "DanhGias",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LienHes_AspNetUsers_UserId",
                table: "LienHes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamYeuThiches_SanPhams_SanPhamIdSanPham",
                table: "SanPhamYeuThiches",
                column: "SanPhamIdSanPham",
                principalTable: "SanPhams",
                principalColumn: "IdSanPham",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TinTucs_AspNetUsers_UserId",
                table: "TinTucs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
