using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeMenu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CATEGORY",
                columns: table => new
                {
                    CATEGORYID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CATEGORYNAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PARENTCATEGORYID = table.Column<int>(type: "int", nullable: true),
                    ISDELETED = table.Column<bool>(type: "bit", nullable: false),
                    CREATEDDATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATORUSERID = table.Column<int>(type: "int", nullable: true),
                    TENANTID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORY", x => x.CATEGORYID);
                    table.ForeignKey(
                        name: "FK_CATEGORY_CATEGORY_PARENTCATEGORYID",
                        column: x => x.PARENTCATEGORYID,
                        principalTable: "CATEGORY",
                        principalColumn: "CATEGORYID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PERMISSION",
                columns: table => new
                {
                    PERMISSIONID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KEY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DISPLAYNAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GROUPNAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CREATEDDATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ISACTIVE = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERMISSION", x => x.PERMISSIONID);
                });

            migrationBuilder.CreateTable(
                name: "PROPERTY",
                columns: table => new
                {
                    PROPERTYID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KEY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VALUE = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<int>(type: "int", nullable: true),
                    TENANTID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROPERTY", x => x.PROPERTYID);
                });

            migrationBuilder.CreateTable(
                name: "TENANT",
                columns: table => new
                {
                    TENANTID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CODE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ISACTIVE = table.Column<bool>(type: "bit", nullable: false),
                    CREATEDDATE = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TENANT", x => x.TENANTID);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT",
                columns: table => new
                {
                    PRODUCTID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PRODUCNAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CATEGORYID = table.Column<int>(type: "int", nullable: false),
                    PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IMAGEPATH = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ISDELETED = table.Column<bool>(type: "bit", nullable: false),
                    CREATEDDATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATORUSERID = table.Column<int>(type: "int", nullable: true),
                    TENANTID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT", x => x.PRODUCTID);
                    table.ForeignKey(
                        name: "FK_PRODUCT_CATEGORY_CATEGORYID",
                        column: x => x.CATEGORYID,
                        principalTable: "CATEGORY",
                        principalColumn: "CATEGORYID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCTPROPERTY",
                columns: table => new
                {
                    PRODUCTPROPERTYID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PRODUCTID = table.Column<int>(type: "int", nullable: false),
                    PROPERTYID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTPROPERTY", x => x.PRODUCTPROPERTYID);
                    table.ForeignKey(
                        name: "FK_PRODUCTPROPERTY_PRODUCT_PRODUCTID",
                        column: x => x.PRODUCTID,
                        principalTable: "PRODUCT",
                        principalColumn: "PRODUCTID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PRODUCTPROPERTY_PROPERTY_PROPERTYID",
                        column: x => x.PROPERTYID,
                        principalTable: "PROPERTY",
                        principalColumn: "PROPERTYID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ROLE",
                columns: table => new
                {
                    ROLEID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ISSYSTEM = table.Column<bool>(type: "bit", nullable: false),
                    ISACTIVE = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ISDELETED = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CREATEDDATE = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CREATEDBYUSERID = table.Column<int>(type: "int", nullable: true),
                    TENANTID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLE", x => x.ROLEID);
                    table.ForeignKey(
                        name: "FK_ROLE_TENANT_TENANTID",
                        column: x => x.TENANTID,
                        principalTable: "TENANT",
                        principalColumn: "TENANTID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ROLEPERMISSION",
                columns: table => new
                {
                    ROLEPERMISSIONID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ROLEID = table.Column<int>(type: "int", nullable: false),
                    PERMISSIONID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLEPERMISSION", x => x.ROLEPERMISSIONID);
                    table.ForeignKey(
                        name: "FK_ROLEPERMISSION_PERMISSION_PERMISSIONID",
                        column: x => x.PERMISSIONID,
                        principalTable: "PERMISSION",
                        principalColumn: "PERMISSIONID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ROLEPERMISSION_ROLE_ROLEID",
                        column: x => x.ROLEID,
                        principalTable: "ROLE",
                        principalColumn: "ROLEID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    USERID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SURNAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    USERNAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HASHPASSWORD = table.Column<byte[]>(type: "varbinary(64)", nullable: false),
                    SALTPASSWORD = table.Column<byte[]>(type: "varbinary(32)", nullable: false),
                    ROLEID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatorUserId = table.Column<int>(type: "int", nullable: true),
                    TENANTID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.USERID);
                    table.ForeignKey(
                        name: "FK_USER_ROLE_ROLEID",
                        column: x => x.ROLEID,
                        principalTable: "ROLE",
                        principalColumn: "ROLEID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CATEGORY_PARENTCATEGORYID",
                table: "CATEGORY",
                column: "PARENTCATEGORYID");

            migrationBuilder.CreateIndex(
                name: "IX_CATEGORY_TENANTID",
                table: "CATEGORY",
                column: "TENANTID");

            migrationBuilder.CreateIndex(
                name: "IX_PERMISSION_KEY",
                table: "PERMISSION",
                column: "KEY",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_CATEGORYID",
                table: "PRODUCT",
                column: "CATEGORYID");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_TENANTID",
                table: "PRODUCT",
                column: "TENANTID");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTPROPERTY_PRODUCTID",
                table: "PRODUCTPROPERTY",
                column: "PRODUCTID");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTPROPERTY_PROPERTYID",
                table: "PRODUCTPROPERTY",
                column: "PROPERTYID");

            migrationBuilder.CreateIndex(
                name: "IX_PROPERTY_TENANTID",
                table: "PROPERTY",
                column: "TENANTID");

            migrationBuilder.CreateIndex(
                name: "IX_ROLE_CREATEDBYUSERID",
                table: "ROLE",
                column: "CREATEDBYUSERID");

            migrationBuilder.CreateIndex(
                name: "IX_ROLE_NAME_TENANTID",
                table: "ROLE",
                columns: new[] { "NAME", "TENANTID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ROLE_TENANTID",
                table: "ROLE",
                column: "TENANTID");

            migrationBuilder.CreateIndex(
                name: "IX_ROLEPERMISSION_PERMISSIONID",
                table: "ROLEPERMISSION",
                column: "PERMISSIONID");

            migrationBuilder.CreateIndex(
                name: "IX_ROLEPERMISSION_ROLEID_PERMISSIONID",
                table: "ROLEPERMISSION",
                columns: new[] { "ROLEID", "PERMISSIONID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TENANT_CODE",
                table: "TENANT",
                column: "CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USER_ROLEID",
                table: "USER",
                column: "ROLEID");

            migrationBuilder.CreateIndex(
                name: "IX_USER_USERNAME_TENANTID",
                table: "USER",
                columns: new[] { "USERNAME", "TENANTID" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ROLE_USER_CREATEDBYUSERID",
                table: "ROLE",
                column: "CREATEDBYUSERID",
                principalTable: "USER",
                principalColumn: "USERID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ROLE_TENANT_TENANTID",
                table: "ROLE");

            migrationBuilder.DropForeignKey(
                name: "FK_ROLE_USER_CREATEDBYUSERID",
                table: "ROLE");

            migrationBuilder.DropTable(
                name: "PRODUCTPROPERTY");

            migrationBuilder.DropTable(
                name: "ROLEPERMISSION");

            migrationBuilder.DropTable(
                name: "PRODUCT");

            migrationBuilder.DropTable(
                name: "PROPERTY");

            migrationBuilder.DropTable(
                name: "PERMISSION");

            migrationBuilder.DropTable(
                name: "CATEGORY");

            migrationBuilder.DropTable(
                name: "TENANT");

            migrationBuilder.DropTable(
                name: "USER");

            migrationBuilder.DropTable(
                name: "ROLE");
        }
    }
}
