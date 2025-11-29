using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CafeMenu.Infrastructure.Services;

public class MenuPdfService : IMenuPdfService
{

    public Task<byte[]> GenerateMenuPdfAsync(MenuViewModel menuData, CancellationToken cancellationToken = default)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var primaryColorHex = "#d4a574";
        var primaryDarkHex = "#b8935f";
        var secondaryColorHex = "#8b6f47";
        var textColorHex = "#3a3a3a";
        var bgColorHex = "#f8f6f3";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginVertical(50);
                page.MarginHorizontal(40);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header()
                    .PaddingBottom(15)
                    .BorderBottom(3)
                    .BorderColor(primaryColorHex)
                    .AlignCenter()
                    .Text(menuData.TenantName)
                    .FontSize(28)
                    .Bold()
                    .FontColor(primaryDarkHex);

                page.Content()
                    .PaddingVertical(15)
                    .Column(column =>
                    {
                        column.Spacing(25);

                        foreach (var category in menuData.Categories)
                        {
                            column.Item().Element(c => RenderCategory(c, category, primaryColorHex, primaryDarkHex, secondaryColorHex, textColorHex, bgColorHex));
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .PaddingTop(10)
                    .Text($"Oluşturulma Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Medium);
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    private void RenderCategory(IContainer container, MenuCategoryViewModel category, string primaryColorHex, string primaryDarkHex, string secondaryColorHex, string textColorHex, string bgColorHex)
    {
        container.Column(column =>
        {
            column.Item().PaddingBottom(8).PaddingTop(5).Background(bgColorHex).Padding(8).BorderBottom(2).BorderColor(primaryColorHex).Column(catCol =>
            {
                catCol.Item().Text(category.CategoryName.ToUpperInvariant()).FontSize(20).Bold().FontColor(primaryDarkHex).LetterSpacing(1);
            });

            if (category.SubCategories.Any())
            {
                foreach (var subCategory in category.SubCategories)
                {
                    column.Item().PaddingTop(8).PaddingLeft(15).Column(subCol =>
                    {
                        subCol.Item().PaddingBottom(4).Text(subCategory.CategoryName).FontSize(16).SemiBold().FontColor(secondaryColorHex);

                        if (subCategory.SubCategories.Any())
                        {
                            foreach (var grandSubCategory in subCategory.SubCategories)
                            {
                                column.Item().PaddingTop(4).PaddingLeft(25).Column(grandCol =>
                                {
                                    grandCol.Item().PaddingBottom(3).Text(grandSubCategory.CategoryName).FontSize(13).SemiBold().FontColor(textColorHex).Italic();
                                    RenderProducts(grandCol, grandSubCategory.Products, primaryDarkHex, textColorHex);
                                });
                            }
                        }

                        RenderProducts(subCol, subCategory.Products, primaryDarkHex, textColorHex);
                    });
                }
            }

            RenderProducts(column, category.Products, primaryDarkHex, textColorHex);
        });
    }

    private void RenderProducts(ColumnDescriptor column, List<MenuProductViewModel> products, string priceColorHex, string textColorHex)
    {
        if (!products.Any()) return;

        foreach (var product in products)
        {
            column.Item().PaddingTop(6).PaddingBottom(4).Row(row =>
            {
                row.RelativeItem(4).Column(prodCol =>
                {
                    prodCol.Item().Text(product.ProductName).FontSize(12).Bold().FontColor(textColorHex);
                    if (product.Badges.Any())
                    {
                        prodCol.Item().PaddingTop(2).Text(string.Join(" • ", product.Badges)).FontSize(9).FontColor(Colors.Grey.Medium);
                    }
                });

                row.ConstantItem(90).AlignRight().AlignMiddle().Text(product.Price.ToString("N2") + " ₺").FontSize(12).Bold().FontColor(priceColorHex);
            });
        }
    }

}

