using KassaSys.Campaign;
using KassaSys.Product;
using System.Text.RegularExpressions;

namespace KassaSys.Register;

public class CashRegister : ICashRegister
{
    private string _dirPath = @".\receipts";
    private string _filePath = @".\receipts\receipt_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
    private string _receiptRegex = @"&&==\s*(\d+)*\s==&&";

    public List<CashRegisterList> receiptList = new List<CashRegisterList>();

    private ShopProduct productList = new ShopProduct();
    private ShopCampaign campaignList = new ShopCampaign();

    public int FetchReceiptNumber()
    {
        var countReceipts = 0;

        if (!Directory.Exists(_dirPath))
        {
            Directory.CreateDirectory(_dirPath);
        }

        foreach (var file in Directory.GetFiles(_dirPath))
        {
            var lastLine = File.ReadLines(file).LastOrDefault();

            if (!string.IsNullOrEmpty(lastLine) && Regex.IsMatch(lastLine, _receiptRegex))
            {
                if (int.TryParse(Regex.Match(lastLine, _receiptRegex).Groups[1].Value, out int number))
                {
                    if (number > countReceipts)
                    {
                        countReceipts = number;
                    }
                }
            }
        }

        return countReceipts + 1;
    }

    public double CalculateTotalPrice()
    {
        double totalPrice = 0;

        foreach (var receipt in receiptList)
        {
            double discount = 0;

            if (double.TryParse(receipt.Discount.Replace("kr", ""), out double discountValue))
            {
                discount = receipt.Count * discountValue;
            }
            else if (double.TryParse(receipt.Discount.Replace("%", ""), out double discountPercent))
            {
                discount = receipt.Price * receipt.Count * (discountPercent / 100);
            }

            double price = (receipt.Price * receipt.Count) - discount;

            totalPrice += price;
        }

        return Math.Round(totalPrice, 2);
    }

    public bool CheckIfProductExicsts(int receiptId)
    {
        return receiptList.Any(receipt => receipt.Id == receiptId);
    }

    public void UpdateProductInReceipt(int receiptId, int amount)
    {
        receiptList.Where(receipt => receipt.Id == receiptId).ToList().ForEach(receipt =>
        {
            int newAmount = receipt.Count + amount;

            if (newAmount > 0)
            {
                receipt.Count = newAmount;
            }
            else
            {
                receiptList.Remove(receipt);
            }
        });
    }

    public void AddToReceipt(int productId, int amount)
    {
        if (amount < 0)
        {
            return;
        }

        if (!productList.CheckIfProductExists(productId))
        {
            return;
        }

        string campaign = campaignList.CalculateBestDiscount(productId);

        if (!CheckIfProductExicsts(productId))
        {
            receiptList.Add(new CashRegisterList
            {
                Id = productId,
                Name = productList.FetchProductName(productId),
                Count = amount,
                Price = productList.FetchProductPrice(productId),
                Type = productList.FetchProductType(productId),
                Discount = campaign
            });
        }
        else
        {
            UpdateProductInReceipt(productId, amount);
        }
    }

    public void PrintReceipt()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Red;

        Console.WriteLine("KVITTO {0,7} {1,19}", $"#{FetchReceiptNumber():D4}", $"{DateTime.Now}");

        if (receiptList.Count > 0)
        {
            Console.WriteLine("==================================");

            foreach (var item in receiptList)
            {
                Console.WriteLine("{0,-13}{1,-12}{2,9}", $"{item.Name.Substring(0, Math.Min(item.Name.Length, 11))}", (item.Count > 1) ? item.Count + "*" + item.Price : "", $"{Math.Round(item.Count * item.Price, 2):F2}");

                if (item.Discount.EndsWith("kr") && item.Discount != "0kr")
                {
                    Console.WriteLine("{0,-20}{1,14}", $"   *Rabatt: {double.Parse(item.Discount.Replace("kr", ""))}kr/st", $"-{double.Parse(item.Discount.Replace("kr", "")) * item.Count:F2}");
                }
                else if (item.Discount.EndsWith('%') && item.Discount != "0%")
                {
                    Console.WriteLine("{0,-20}{1,14}", $"   *Rabatt: {double.Parse(item.Discount.Replace("%", ""))}%", $"-{item.Count * item.Price * (double.Parse(item.Discount.Replace("%", "")) / 100):F2}");
                }
            }
        }

        Console.WriteLine("==================================");

        Console.WriteLine("{0,31} kr", $"Total: {CalculateTotalPrice():F2}");

        Console.ResetColor();
    }

    public void SaveReceipt()
    {
        if (!File.Exists(_filePath))
        {
            File.Create(_filePath).Dispose();
        }

        if (receiptList.Count == 0)
        {
            return;
        }

        string receiptString = string.Format("\nKVITTO {0,7} {1,19}\n", $"#{FetchReceiptNumber():D4}", $"{DateTime.Now}");
        receiptString += string.Format("==================================\n");

        foreach (var item in receiptList)
        {
            receiptString += string.Format("{0,-13}{1,-12}{2,10}", $"{item.Name.Substring(0, Math.Min(item.Name.Length, 11))}", (item.Count > 1) ? (item.Count + " * " + item.Price + "kr") : (""), $"{Math.Round(item.Count * item.Price, 2):F2}\n");
            if (item.Discount.EndsWith("kr") && item.Discount != "0kr")
            {
                receiptString += string.Format("{0,-20}{1,15}", $"   *Rabatt: {int.Parse(item.Discount.Replace("kr", ""))}kr/st", $"-{int.Parse(item.Discount.Replace("kr", "")) * item.Count:F2}\n");
            }
            else if (item.Discount.EndsWith('%') && item.Discount != "0%")
            {
                receiptString += string.Format("{0,-20}{1,15}", $"   *Rabatt: {double.Parse(item.Discount.Replace("%", ""))}%", $"-{item.Count * item.Price * (double.Parse(item.Discount.Replace("%", "")) / 100):F2}\n");
            }
        }
        receiptString += string.Format("==================================\n");
        receiptString += string.Format("{0,31} kr\n", $"Total: {CalculateTotalPrice():F2}");
        receiptString += string.Format($"&&== {FetchReceiptNumber()} ==&&");

        File.AppendAllText(_filePath, receiptString);
    }
}