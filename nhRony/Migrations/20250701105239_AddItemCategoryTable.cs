using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearingAndForwarding.Migrations
{
    /// <inheritdoc />
    public partial class AddItemCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requisition_ExpenseHead_ExpenseHeadID",
                table: "Requisition");

            migrationBuilder.DropForeignKey(
                name: "FK_RequisitionDetails_Requisition_RequisitionId1",
                table: "RequisitionDetails");

            migrationBuilder.DropIndex(
                name: "IX_RequisitionDetails_RequisitionId1",
                table: "RequisitionDetails");

            migrationBuilder.DropIndex(
                name: "IX_Requisition_ExpenseHeadID",
                table: "Requisition");

            migrationBuilder.DropColumn(
                name: "RequisitionId1",
                table: "RequisitionDetails");

            migrationBuilder.RenameColumn(
                name: "ExpenseHeadID",
                table: "Requisition",
                newName: "ExpenseID");

            migrationBuilder.AddColumn<decimal>(
                name: "AdjustedAmount",
                table: "Requisition",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Requisition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RequisitionID",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_RequisitionID",
                table: "Expenses",
                column: "RequisitionID",
                unique: true,
                filter: "[RequisitionID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Requisition_RequisitionID",
                table: "Expenses",
                column: "RequisitionID",
                principalTable: "Requisition",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Requisition_RequisitionID",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "ItemCategories");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_RequisitionID",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "AdjustedAmount",
                table: "Requisition");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Requisition");

            migrationBuilder.DropColumn(
                name: "RequisitionID",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "ExpenseID",
                table: "Requisition",
                newName: "ExpenseHeadID");

            migrationBuilder.AddColumn<int>(
                name: "RequisitionId1",
                table: "RequisitionDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionDetails_RequisitionId1",
                table: "RequisitionDetails",
                column: "RequisitionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Requisition_ExpenseHeadID",
                table: "Requisition",
                column: "ExpenseHeadID");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisition_ExpenseHead_ExpenseHeadID",
                table: "Requisition",
                column: "ExpenseHeadID",
                principalTable: "ExpenseHead",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequisitionDetails_Requisition_RequisitionId1",
                table: "RequisitionDetails",
                column: "RequisitionId1",
                principalTable: "Requisition",
                principalColumn: "Id");
        }
    }
}
