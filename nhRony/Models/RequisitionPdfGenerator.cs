using ClearingAndForwarding.Helpers;
using ClearingAndForwarding.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class RequisitionPdfGenerator
{
    public static byte[] CreatePdf(Requisition requisition, Dictionary<int, decimal>? quotationAmounts = null)
    {
        return Document.Create(container =>
        {
            _ = container.Page(page =>
            {
                page.MarginHorizontal(30);
                page.MarginVertical(20);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(header =>
                {
                    header.Item().Text("DULAL BROTHERS LIMITED").FontSize(14).Bold().AlignCenter();
                    header.Item().Text("(A unit of DBL Group)").FontSize(11).AlignCenter();
                    header.Item().Text("BGMEA Bhaban (Level- 7), 669/E, Jhautala Road, South Khulshi, Chittagong").FontSize(10).AlignCenter();
                    header.Item().PaddingBottom(2).Text("ADVANCE REQUISITION").FontSize(14).Bold().AlignCenter();
                    header.Item().PaddingBottom(2).Text("");
                    header.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    header.Item().PaddingBottom(2).Text("");
                });

                page.Content().Column(col =>
                {
                    col.Spacing(5);

                    col.Item().Row(row =>
                    {
                        // Left block
                        row.ConstantItem(300).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(77);
                                c.RelativeColumn();
                            });

                            table.Cell().Element(CellStyle).Text("Employee Name").SemiBold();
                            table.Cell().Element(CellStyle).Text($":  {requisition.Employee?.Name}" ?? "N/A");

                            table.Cell().Element(CellStyle).Text("Designation").SemiBold();
                            table.Cell().Element(CellStyle).Text($":  {requisition.Employee?.Designation ?? requisition.Employee?.Department}" ?? "N/A");

                            table.Cell().Element(CellStyle).Text("Item Name").SemiBold();
                            table.Cell().Element(CellStyle).Text($":  {requisition.Bedetails.ItemName}" ?? "N/A");

                            table.Cell().Element(CellStyle).Text("Unit Name").SemiBold();
                            table.Cell().Element(CellStyle).Text($":  {requisition.Bedetails.UnitCode}" ?? "N/A");

                            table.Cell().Element(CellStyle).Text("LC No").SemiBold();
                            table.Cell().Element(CellStyle).Text($":  {requisition.Bedetails.LCNo}" ?? "N/A");
                        });

                        // Right block
                        row.RelativeItem().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(60);
                                c.RelativeColumn();
                            });

                            table.Cell().Element(CellStyle).Text("Date").SemiBold();
                            table.Cell().Element(CellStyle).Text($": {requisition.RequisitionDate:dd-MMM-yyyy}");

                            table.Cell().Element(CellStyle).Text("BE No").SemiBold();
                            table.Cell().Element(CellStyle).Text($": {requisition.Bedetails?.BENo}" ?? "N/A");

                            table.Cell().Element(CellStyle).Text("Cont. No").SemiBold();
                            table.Cell().Element(CellStyle).Text("Cont. No").SemiBold();
                            var containerInfo = requisition.Bedetails?.ContainerNo > 0 &&
                                                !string.IsNullOrWhiteSpace(requisition.Bedetails?.ContainerSize)
                                ? $": {requisition.Bedetails.ContainerNo} x {requisition.Bedetails.ContainerSize}″"
                                : ": N/A";
                            table.Cell().Element(CellStyle).Text(containerInfo);


                            table.Cell().Element(CellStyle).Text("Quantity").SemiBold();
                            table.Cell().Element(CellStyle).Text($": {requisition.Bedetails.Quantity} {requisition.Bedetails.UOM}" ?? "N/A");

                            table.Cell().Element(CellStyle).Text("Req. No").SemiBold();
                            table.Cell().Element(CellStyle).Text($": {requisition.Id}" ?? "N/A");
                        });

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.PaddingVertical(1);
                        }
                    });

                    // Expense Table
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                            columns.ConstantColumn(100);
                        });

                        var rows = requisition.RequisitionDetails.ToList();
                        int rowCount = rows.Count;
                        string indentation = "\u00A0\u00A0\u00A0";

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Element(c => ExcelCell(c, true, true, false, true, false)).Text(indentation + "Expense Head").Bold();
                            header.Cell().Element(c => ExcelCell(c, true, true, false, false, false)).AlignCenter().Text("Quotation Amount").Bold();
                            header.Cell().Element(c => ExcelCell(c, true, true, false, false, true)).AlignCenter().Text("Requisition Amount").Bold();
                        });

                        // Rows
                        for (int i = 0; i < rowCount; i++)
                        {
                            var detail = rows[i];
                            bool isFirst = i == 0;
                            bool isLast = i == rowCount - 1;

                            table.Cell().Element(c => ExcelCell(c, false, isFirst, isLast, true, false))
                                .Text(indentation + (detail.ExpenseHead?.ExpenseName ?? ""));

                            table.Cell().Element(c => ExcelCell(c, false, isFirst, isLast, false, false))
                                .AlignCenter().Text(
                                    quotationAmounts?.GetValueOrDefault(detail.ExpenseHead.Id).ToString("N2") ?? "N/A"
                                );

                            table.Cell().Element(c => ExcelCell(c, false, isFirst, isLast, false, true))
                                .AlignRight().Text($"{detail.Amount:N2}" + indentation);
                        }

                        // Total
                        var totalAmount = requisition.RequisitionDetails.Sum(r => r.Amount);
                        table.Cell().ColumnSpan(2).Element(c => ExcelCell(c, false, false, true, true, false))
                            .Text(indentation + "Total Amount").Bold();

                        table.Cell().Element(c => ExcelCell(c, false, false, true, false, true))
                            .AlignRight().Text($"{totalAmount:N2}" + indentation).Bold();

                        static IContainer ExcelCell(IContainer container, bool isHeader, bool isFirstRow, bool isLastRow, bool isFirstCol, bool isLastCol)
                        {
                            float borderWidth = 0.5f;
                            return container
                                .MinHeight(20)
                                .BorderVertical(borderWidth)
                                .BorderHorizontal(borderWidth)
                                .BorderColor(Colors.Black)
                                .Background(isHeader ? Colors.Grey.Lighten2 : Colors.White)
                                .AlignMiddle();
                        }
                    });

                    // Amount in Words
                    var amountInWords = NumberToWordsConverter.ConvertAmountToWords(requisition.RequisitionDetails.Sum(r => r.Amount));
                    col.Item().Text(text =>
                    {
                        text.Span("Amount in words: ").Bold();
                        text.Span(amountInWords);
                    });

                    // Signatures side by side
                    col.Item().PaddingTop(30).Row(row =>
                    {
                        row.RelativeItem().Text("Signature: _________________________");
                        row.RelativeItem().AlignRight().Text("Approved By: _______________________");
                    });
                    // Horizontal line before footer text
                    col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    // Footer-like text (new line after line)
                    col.Item().PaddingTop(2).AlignCenter().Text(
                        $"Generated by C&F Accounting System | Printed on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                    ).FontSize(9);
                });
            });
        }).GeneratePdf();
    }
}
