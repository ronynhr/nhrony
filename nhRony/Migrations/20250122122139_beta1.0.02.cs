using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearingAndForwarding.Migrations
{
    /// <inheritdoc />
    public partial class beta1002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BEdetailsID",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuotationID",
                table: "ExpenseDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Quantity",
                table: "BEdetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ODRecDate",
                table: "BEdetails",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "LCDate",
                table: "BEdetails",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "InvoiceDate",
                table: "BEdetails",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DeliveryDate",
                table: "BEdetails",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DODate",
                table: "BEdetails",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BEDate",
                table: "BEdetails",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "AssessmentDate",
                table: "BEdetails",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ArrivalDate",
                table: "BEdetails",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Quotation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseHeadID = table.Column<int>(type: "int", nullable: false),
                    itemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotation_ExpenseHead_ExpenseHeadID",
                        column: x => x.ExpenseHeadID,
                        principalTable: "ExpenseHead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requisition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseHeadID = table.Column<int>(type: "int", nullable: false),
                    BEdetailsID = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    RequisitionNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequisitionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RequisitionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requisition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requisition_BEdetails_BEdetailsID",
                        column: x => x.BEdetailsID,
                        principalTable: "BEdetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Requisition_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Requisition_ExpenseHead_ExpenseHeadID",
                        column: x => x.ExpenseHeadID,
                        principalTable: "ExpenseHead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BEdetailsID",
                table: "Expenses",
                column: "BEdetailsID");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseDetails_QuotationID",
                table: "ExpenseDetails",
                column: "QuotationID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_ExpenseHeadID",
                table: "Quotation",
                column: "ExpenseHeadID");

            migrationBuilder.CreateIndex(
                name: "IX_Requisition_BEdetailsID",
                table: "Requisition",
                column: "BEdetailsID");

            migrationBuilder.CreateIndex(
                name: "IX_Requisition_EmployeeId",
                table: "Requisition",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Requisition_ExpenseHeadID",
                table: "Requisition",
                column: "ExpenseHeadID");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseDetails_Quotation_QuotationID",
                table: "ExpenseDetails",
                column: "QuotationID",
                principalTable: "Quotation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_BEdetails_BEdetailsID",
                table: "Expenses",
                column: "BEdetailsID",
                principalTable: "BEdetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseDetails_Quotation_QuotationID",
                table: "ExpenseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_BEdetails_BEdetailsID",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "Quotation");

            migrationBuilder.DropTable(
                name: "Requisition");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_BEdetailsID",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseDetails_QuotationID",
                table: "ExpenseDetails");

            migrationBuilder.DropColumn(
                name: "BEdetailsID",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "QuotationID",
                table: "ExpenseDetails");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "BEdetails",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ODRecDate",
                table: "BEdetails",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LCDate",
                table: "BEdetails",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                table: "BEdetails",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDate",
                table: "BEdetails",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DODate",
                table: "BEdetails",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "BEDate",
                table: "BEdetails",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AssessmentDate",
                table: "BEdetails",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ArrivalDate",
                table: "BEdetails",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
