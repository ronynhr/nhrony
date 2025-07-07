using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearingAndForwarding.Migrations
{
    /// <inheritdoc />
    public partial class addedRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillTypeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequisitionDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionId = table.Column<int>(type: "int", nullable: false),
                    ExpenseHeadId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequisitionId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitionDetails_ExpenseHead_ExpenseHeadId",
                        column: x => x.ExpenseHeadId,
                        principalTable: "ExpenseHead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequisitionDetails_Requisition_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Requisition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequisitionDetails_Requisition_RequisitionId1",
                        column: x => x.RequisitionId1,
                        principalTable: "Requisition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionDetails_ExpenseHeadId",
                table: "RequisitionDetails",
                column: "ExpenseHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionDetails_RequisitionId",
                table: "RequisitionDetails",
                column: "RequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionDetails_RequisitionId1",
                table: "RequisitionDetails",
                column: "RequisitionId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillTypes");

            migrationBuilder.DropTable(
                name: "RequisitionDetails");
        }
    }
}
