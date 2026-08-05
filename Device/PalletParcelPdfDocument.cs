using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using VTP_Induction.Common;

namespace VTP_Induction.Device
{
    public class PalletParcelPdfDocument : IDocument
    {
        private readonly List<ParcelLabelData> _items;

        private const float LABEL_W_MM = 70; // sửa theo khổ tem thật của bạn
        private const float LABEL_H_MM = 40;  // sửa theo khổ tem thật của bạn

        public PalletParcelPdfDocument(List<ParcelLabelData> items)
        {
            _items = items;
        }

        public DocumentMetadata GetMetadata()
        {
            return DocumentMetadata.Default;
        }

        public void Compose(IDocumentContainer container)
        {
            foreach (var item in _items)
            {
                container.Page(page =>
                {
                    page.Size(LABEL_W_MM, LABEL_H_MM, Unit.Millimetre); // 70 x 40
                    page.Margin(0.5f, Unit.Millimetre);
                    page.PageColor(Colors.White);

                    page.DefaultTextStyle(x => x
                        .FontFamily("Arial")
                        .FontSize(6));

                    page.Content()
                        .Padding(1, Unit.Millimetre)
                        .Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                row.ConstantItem(29, Unit.Millimetre)
                                    .AlignTop()
                                    .Image(CreateQrPng(item.ParcelCode));

                                row.RelativeItem()
                                    .PaddingLeft(2, Unit.Millimetre)
                                    .Column(info =>
                                    {
                                        info.Spacing(1);

                                        info.Item().Text("Tên sản phẩm:")
                                            .FontSize(5.5f);

                                        info.Item().Text(item.ItemName ?? "")
                                            .FontSize(7)
                                            .Bold();

                                        info.Item().Text("Quy cách: " + item.InnerCtn + " chiếc")
                                            .FontSize(5.5f);

                                        info.Item().Text("Mã pallet: " + item.PalletCode)
                                            .FontSize(5.5f);

                                        info.Item().Text("Ngày: " + item.CreatedTime.ToString("HH:mm dd/MM/yyyy"))
                                            .FontSize(5.5f);
                                    });

                                row.ConstantItem(10, Unit.Millimetre)
                                    .AlignTop()
                                    .AlignRight()
                                    .Text(item.CartonNo.ToString() ?? "")
                                    .FontSize(24)
                                    .Bold();
                            });

                            column.Item()
                                .PaddingTop(1, Unit.Millimetre)
                                .Text(text =>
                                {
                                    text.Span("Mã định danh: ").FontSize(5.5f);
                                    text.Span(item.ParcelCode).FontSize(6.5f).Bold();
                                });
                        });
                });
            }
        }

        private static byte[] CreateQrPng(string content)
        {
            return PngByteQRCodeHelper.GetQRCode(
                content,
                QRCodeGenerator.ECCLevel.L,
                8
            );
        }

        public DocumentSettings GetSettings()
        {
            return DocumentSettings.Default;
        }
    }
}
