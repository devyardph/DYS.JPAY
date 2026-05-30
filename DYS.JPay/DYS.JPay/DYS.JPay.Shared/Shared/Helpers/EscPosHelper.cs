// Services/EscPosHelper.cs
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;      // Needed for SelectMany
using System.Text;      // Needed for Encoding

namespace DYS.JPay.Shared.Shared.Helpers
{

    public static class EscPosHelper
    {
        // ESC/POS Commands
        public static byte[] Initialize() => new byte[] { 0x1B, 0x40 };
        public static byte[] FeedLines(int n) => new byte[] { 0x1B, 0x64, (byte)n };
        public static byte[] CutPaper() => new byte[] { 0x1D, 0x56, 0x41, 0x00 };
        public static byte[] AlignLeft() => new byte[] { 0x1B, 0x61, 0x00 };
        public static byte[] AlignCenter() => new byte[] { 0x1B, 0x61, 0x01 };
        public static byte[] AlignRight() => new byte[] { 0x1B, 0x61, 0x02 };
        public static byte[] BoldOn() => new byte[] { 0x1B, 0x45, 0x01 };
        public static byte[] BoldOff() => new byte[] { 0x1B, 0x45, 0x00 };
        public static byte[] DoubleSizeOn() => new byte[] { 0x1D, 0x21, 0x11 };
        public static byte[] DoubleSizeOff() => new byte[] { 0x1D, 0x21, 0x00 };

        public static byte[] Text(string text)
            => Encoding.GetEncoding(850).GetBytes(text + "\n"); // safer than "IBM850"

        public static byte[] BuildReceipt(string title, Dictionary<string, string> items, string total)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            try
            {
                var doc = new List<byte[]>
            {
            Initialize(),
            AlignCenter(),
            DoubleSizeOn(),
            BoldOn(),
            Text(title),
            BoldOff(),
            DoubleSizeOff(),
            Text("--------------------------------"),
            AlignLeft()
        };

                foreach (var item in items)
                {
                    string line = item.Key.PadRight(20) + item.Value.PadLeft(12);
                    doc.Add(Text(line));
                }

                doc.Add(Text("--------------------------------"));
                doc.Add(AlignRight());
                doc.Add(BoldOn());
                doc.Add(Text($"TOTAL: {total}"));
                doc.Add(BoldOff());
                doc.Add(AlignCenter());
                doc.Add(Text("Thank you!"));
                doc.Add(FeedLines(4));
                doc.Add(CutPaper());

                return doc.SelectMany(b => b).ToArray();
            }
            catch (Exception ex)
            { 

                throw;
            }
           
        }
        public static byte[] BuildReceipt(AppSetting setting, CartDto cart)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            try
            {
                var doc = new List<byte[]>
                {
                Initialize(),
                AlignCenter(),
                DoubleSizeOn(),
                BoldOn(),
                Text(setting.StoreName),
                BoldOff(),
                DoubleSizeOff(),
                Text($"{cart.Transaction?.DateCreated.FormatDate("MMMM dd, yyyy hh:mmtt")}"),
                Text($"Processed by: {cart.Transaction?.Cashier}"),
                Text("--------------------------------"),
                AlignLeft()
                };

                foreach (var item in cart.Orders)
                {
                    var name = $"{item.Name.Truncate(10)}:{item.Price?.ToString("N2")}x{item.Quantity}";
                    var total = item.Price * item.Quantity;
                    string line = name.PadRight(20) + total?.ToString("N2").PadLeft(12);
                    doc.Add(Text(line));
                }

                doc.Add(Text("--------------------------------"));
                doc.Add(AlignRight());
               
                doc.Add(Text($"Discount: {cart.Transaction.DiscountAmount?.ToString("N2")}"));
                doc.Add(Text($"Sub Total: {cart.Transaction.Total?.ToString("N2")}"));
                doc.Add(Text($"Tax({setting.Tax}%): {cart.Transaction.TotalTax?.ToString("N2")}"));
                doc.Add(BoldOn());
                doc.Add(Text($"TOTAL: {cart.Transaction.GrandTotal?.ToString("N2")}"));
                doc.Add(BoldOff());
                doc.Add(AlignCenter());
                doc.Add(Text("Thank you & God Bless!"));
                doc.Add(FeedLines(4));
                doc.Add(CutPaper());

                return doc.SelectMany(b => b).ToArray();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public static byte[] BuildReceipt(AppSetting setting,string test)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            try
            {
                var doc = new List<byte[]>
                {
                Initialize(),
                AlignCenter(),
                DoubleSizeOn(),
                BoldOn(),
                Text(setting?.StoreName ?? string.Empty),
                BoldOff(),
                DoubleSizeOff(),
                Text("--------------------------------"),
                AlignLeft()
                };

                doc.Add(Text(test));
                doc.Add(Text("--------------------------------"));
                doc.Add(AlignRight());
                doc.Add(BoldOff());
                doc.Add(AlignCenter());
                doc.Add(FeedLines(4));
                doc.Add(CutPaper());

                return doc.SelectMany(b => b).ToArray();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }

}
