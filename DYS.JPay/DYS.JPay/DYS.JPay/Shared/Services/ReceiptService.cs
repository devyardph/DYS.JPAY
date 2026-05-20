using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Services;
using System.Text;

namespace DYS.JPay.Shared.Services
{
    public interface IReceiptService
    {
        string GeneratePlainText(AppSetting setting, CartDto cart);
        //byte[] GenerateReceipt(AppSetting setting, CartDto cart);
    }

    public class ReceiptService: IReceiptService
    {
        public string GeneratePlainText(AppSetting setting, CartDto cart)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"   {setting.StoreName}");
            sb.AppendLine("--------------------------");
            sb.AppendLine($"Date: {cart.Transaction.DateCreated:yyyy-MM-dd HH:mm}");
            sb.AppendLine($"Transaction: {cart.Transaction.Code}");
            sb.AppendLine();

            foreach (var item in cart.Orders)
                sb.AppendLine($"{item.Name,-12} x{item.Quantity,-2} {item.Price * item.Quantity,6:C}");

            sb.AppendLine("--------------------------");
            sb.AppendLine($"TOTAL {cart.Transaction.GrandTotal,18:C}");
            sb.AppendLine();
            sb.AppendLine("Thank you for shopping!");
            return sb.ToString();
        }

        // PDF for system printers

        const int PAPER_WIDTH = 32; // characters for 58mm; use 48 for 80mm

        //public byte[] GenerateReceipt(AppSetting setting, CartDto cart)
        //{
        //    var e = new EPSON();

        //    return ByteSplicer.Combine(
        //        e.CenterAlign(),
        //        e.SetStyles(PrintStyle.Bold | PrintStyle.DoubleHeight),
        //        e.PrintLine(setting.StoreName),
        //        e.SetStyles(PrintStyle.None),
        //        //e.PrintLine(r.Address),
        //        //e.PrintLine(r.Phone),
        //        e.PrintLine(cart.Transaction.DateCreated?.ToString("MM/dd/yyyy  hh:mm tt")),
        //        e.PrintLine(Divider()),

        //        e.LeftAlign(),
        //        e.SetStyles(PrintStyle.Bold),
        //        e.PrintLine(PadRow("ITEM", "PRICE")),
        //        e.SetStyles(PrintStyle.None),
        //        e.PrintLine(Divider()),

        //        BuildItems(e, cart.Orders),

        //        e.PrintLine(Divider()),
        //        e.SetStyles(PrintStyle.Bold),
        //        e.PrintLine(PadRow("TOTAL", cart!.Transaction!.GrandTotal!.ToString())),
        //        e.SetStyles(PrintStyle.None),
        //        //e.PrintLine(PadRow("CASH", r.CashTendered.ToString("C"))),
        //        //e.PrintLine(PadRow("CHANGE", r.Change.ToString("C"))),
        //        e.PrintLine(Divider()),

        //        e.CenterAlign(),
        //        e.PrintLine("Thank you for your purchase!"),
        //        //e.PrintLine(r.Footer),
        //        e.FeedLines(4),
        //        e.PartialCutAfterFeed(1) // auto-cut!
        //    );
        //}

        //static byte[] BuildItems(EPSON e, List<Order> items)
        //{
        //    var bytes = new List<byte[]>();
        //    foreach (var item in items)
        //        bytes.Add(e.PrintLine(PadRow($"{item.Quantity}x {item.Name}", item.Price?.ToString("C"))));
        //    return ByteSplicer.Combine(bytes.ToArray());
        //}

        //static string Divider() => new string('-', PAPER_WIDTH);

        //static string PadRow(string left, string right)
        //{
        //    var space = PAPER_WIDTH - left.Length - right.Length;
        //    return space > 0
        //        ? left + new string(' ', space) + right
        //        : left[..(PAPER_WIDTH - right.Length - 1)] + " " + right;
        //}
    }
}
