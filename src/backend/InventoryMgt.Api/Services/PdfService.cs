using InventoryMgt.Shared.DTOs;
using InventoryMgt.Shared.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InventoryMgt.Api.Services;

public class PdfService : IPdfService
{
    public byte[] GeneratePurchaseReportPdf(
        IEnumerable<PurchaseReadDto> purchases,
        string? productName = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        string? sortColumn = null,
        string? sortDirection = null)
    {
        var purchaseList = purchases.ToList();
        var totalRecords = purchaseList.Count;
        var totalAmount = purchaseList.Sum(p => p.Quantity * p.UnitPrice);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(content => ComposeContent(content, purchaseList, totalRecords, totalAmount));
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Page ");
                    text.CurrentPageNumber();
                    text.Span(" of ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();

        void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Text("Purchase Report")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                column.Item().PaddingTop(5).AlignCenter().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);

                // Display applied filters
                var filters = new List<string>();
                if (!string.IsNullOrEmpty(productName))
                    filters.Add($"Product: {productName}");
                if (dateFrom.HasValue)
                    filters.Add($"From: {dateFrom.Value:yyyy-MM-dd}");
                if (dateTo.HasValue)
                    filters.Add($"To: {dateTo.Value:yyyy-MM-dd}");
                if (!string.IsNullOrEmpty(sortColumn))
                    filters.Add($"Sort: {sortColumn} {sortDirection?.ToUpper() ?? "ASC"}");

                if (filters.Any())
                {
                    column.Item().PaddingTop(3).AlignCenter().Text($"Filters: {string.Join(" | ", filters)}")
                        .FontSize(8)
                        .Italic()
                        .FontColor(Colors.Grey.Darken1);
                }

                column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            });
        }
    }

    private void ComposeContent(IContainer container, List<PurchaseReadDto> purchases, int totalRecords, double totalAmount)
    {
        container.PaddingTop(10).Column(column =>
        {
            column.Item().Table(table =>
            {
                // Define columns
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);   // Product Name
                    columns.RelativeColumn(1.5f); // SKU
                    columns.RelativeColumn(1);   // Quantity
                    columns.RelativeColumn(1);   // Unit Price
                    columns.RelativeColumn(1);   // Total Price
                    columns.RelativeColumn(1.5f); // Purchase Date
                    columns.RelativeColumn(1.5f); // Received Date
                    columns.RelativeColumn(1.5f); // PO Number
                    columns.RelativeColumn(1.5f); // Invoice Number
                    columns.RelativeColumn(2);   // Description
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Product Name").Bold();
                    header.Cell().Element(CellStyle).Text("SKU").Bold();
                    header.Cell().Element(CellStyle).Text("Quantity").Bold();
                    header.Cell().Element(CellStyle).Text("Unit Price").Bold();
                    header.Cell().Element(CellStyle).Text("Total Price").Bold();
                    header.Cell().Element(CellStyle).Text("Purchase Date").Bold();
                    header.Cell().Element(CellStyle).Text("Received Date").Bold();
                    header.Cell().Element(CellStyle).Text("PO Number").Bold();
                    header.Cell().Element(CellStyle).Text("Invoice Number").Bold();
                    header.Cell().Element(CellStyle).Text("Description").Bold();

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .Background(Colors.Blue.Darken2)
                            .Padding(5)
                            .AlignCenter()
                            .AlignMiddle();
                    }
                });

                // Data rows
                int rowIndex = 0;
                foreach (var purchase in purchases)
                {
                    var isEvenRow = rowIndex % 2 == 0;
                    var backgroundColor = isEvenRow ? Colors.Grey.Lighten3 : Colors.White;

                    table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                        .Text(purchase.ProductName);
                    table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                        .Text(purchase.Sku);
                    table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                        .AlignRight().Text($"{purchase.Quantity:N2}");
                    table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                        .AlignRight().Text($"${purchase.UnitPrice:N2}");
                    table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                        .AlignRight().Text($"${(purchase.Quantity * purchase.UnitPrice):N2}");
                    table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                        .Text(purchase.PurchaseDate.ToString("yyyy-MM-dd"));
                    table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                        .Text(purchase.ReceivedDate?.ToString("yyyy-MM-dd") ?? "-");
                    table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                        .Text(purchase.PurchaseOrderNumber ?? "-");
                    table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                        .Text(purchase.InvoiceNumber ?? "-");
                    table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                        .Text(purchase.Description ?? "-");

                    rowIndex++;
                }

                static IContainer DataCellStyle(IContainer container, string backgroundColor)
                {
                    return container
                        .Background(backgroundColor)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(5);
                }
            });

            // Summary section
            column.Item().PaddingTop(15).Row(row =>
            {
                row.RelativeItem().AlignRight().Column(summaryColumn =>
                {
                    summaryColumn.Item().Text(text =>
                    {
                        text.Span("Total Records: ").Bold();
                        text.Span($"{totalRecords:N0}");
                    });

                    summaryColumn.Item().PaddingTop(3).Text(text =>
                    {
                        text.Span("Total Amount: ").Bold();
                        text.Span($"${totalAmount:N2}").FontColor(Colors.Green.Darken2);
                    });
                });
            });
        });
    }

    public byte[] GenerateSaleReportPdf(
        IEnumerable<SaleReadDto> sales,
        string? productName = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        string? sortColumn = null,
        string? sortDirection = null)
    {
        var saleList = sales.ToList();
        var totalRecords = saleList.Count;
        var totalAmount = saleList.Sum(s => s.Quantity * s.Price);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(content => ComposeContent(content, saleList, totalRecords, totalAmount));
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Page ");
                    text.CurrentPageNumber();
                    text.Span(" of ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();

        void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Text("Sales Report")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Green.Darken2);

                column.Item().PaddingTop(5).AlignCenter().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);

                // Display applied filters
                var filters = new List<string>();
                if (!string.IsNullOrEmpty(productName))
                    filters.Add($"Product: {productName}");
                if (dateFrom.HasValue)
                    filters.Add($"From: {dateFrom.Value:yyyy-MM-dd}");
                if (dateTo.HasValue)
                    filters.Add($"To: {dateTo.Value:yyyy-MM-dd}");
                if (!string.IsNullOrEmpty(sortColumn))
                    filters.Add($"Sort: {sortColumn} {sortDirection?.ToUpper() ?? "ASC"}");

                if (filters.Any())
                {
                    column.Item().PaddingTop(3).AlignCenter().Text($"Filters: {string.Join(" | ", filters)}")
                        .FontSize(8)
                        .Italic()
                        .FontColor(Colors.Grey.Darken1);
                }

                column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            });
        }

        void ComposeContent(IContainer container, List<SaleReadDto> sales, int totalRecords, decimal totalAmount)
        {
            container.PaddingTop(10).Column(column =>
            {
                column.Item().Table(table =>
                {
                    // Define columns
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2.5f); // Product Name
                        columns.RelativeColumn(1.5f); // SKU
                        columns.RelativeColumn(1);    // Quantity
                        columns.RelativeColumn(1);    // Price
                        columns.RelativeColumn(1.2f); // Total Price
                        columns.RelativeColumn(1.5f); // Selling Date
                        columns.RelativeColumn(2);    // Description
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Product Name").Bold();
                        header.Cell().Element(CellStyle).Text("SKU").Bold();
                        header.Cell().Element(CellStyle).Text("Quantity").Bold();
                        header.Cell().Element(CellStyle).Text("Price").Bold();
                        header.Cell().Element(CellStyle).Text("Total Price").Bold();
                        header.Cell().Element(CellStyle).Text("Selling Date").Bold();
                        header.Cell().Element(CellStyle).Text("Description").Bold();

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .Background(Colors.Green.Darken2)
                                .Padding(5)
                                .AlignCenter()
                                .AlignMiddle();
                        }
                    });

                    // Data rows
                    int rowIndex = 0;
                    foreach (var sale in sales)
                    {
                        var isEvenRow = rowIndex % 2 == 0;
                        var backgroundColor = isEvenRow ? Colors.Grey.Lighten3 : Colors.White;

                        table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                            .Text(sale.ProductName);
                        table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                            .Text(sale.Sku);
                        table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                            .AlignRight().Text($"{sale.Quantity:N2}");
                        table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                            .AlignRight().Text($"${sale.Price:N2}");
                        table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                            .AlignRight().Text($"${(sale.Quantity * sale.Price):N2}");
                        table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                            .Text(sale.SellingDate.ToString("yyyy-MM-dd"));
                        table.Cell().Element(container => DataCellStyle(container, backgroundColor))
                            .Text(sale.Description ?? "-");

                        rowIndex++;
                    }

                    static IContainer DataCellStyle(IContainer container, string backgroundColor)
                    {
                        return container
                            .Background(backgroundColor)
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(5);
                    }
                });

                // Summary section
                column.Item().PaddingTop(15).Row(row =>
                {
                    row.RelativeItem().AlignRight().Column(summaryColumn =>
                    {
                        summaryColumn.Item().Text(text =>
                        {
                            text.Span("Total Records: ").Bold();
                            text.Span($"{totalRecords:N0}");
                        });

                        summaryColumn.Item().PaddingTop(3).Text(text =>
                        {
                            text.Span("Total Amount: ").Bold();
                            text.Span($"${totalAmount:N2}").FontColor(Colors.Green.Darken2);
                        });
                    });
                });
            });
        }
    }
}
