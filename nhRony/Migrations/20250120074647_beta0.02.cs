using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearingAndForwarding.Migrations
{
    /// <inheritdoc />
    public partial class beta002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitName",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BINno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TINno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitName", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BENo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BEDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LCNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LCDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UOM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssessableValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Shipper = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ODRecDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DODate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OnChassis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainerNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainerSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitNameId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentDetails_UnitName_UnitNameId",
                        column: x => x.UnitNameId,
                        principalTable: "UnitName",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDetails_UnitNameId",
                table: "ShipmentDetails",
                column: "UnitNameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentDetails");

            migrationBuilder.DropTable(
                name: "UnitName");
        }
    }
}
