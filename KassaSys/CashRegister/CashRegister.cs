using System.Text.RegularExpressions;

namespace KassaSys.Register;

public class CashRegister : ICashRegister
{
    private string _dirPath = @".\receipts";
    private string _filePath = $@".\receipts\receipt_{DateTime.Now:yyyyMMdd}.txt";
    private string _receiptRegex = @"&&==\s*(\d+)*\s==&&";

    public List<CashRegisterList> receiptList = new List<CashRegisterList>();

    private Product.Product productList = new Product.Product();
    private Campaign.Campaign campaignList = new Campaign.Campaign();

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

    public bool CheckIfProductExists(int receiptId)
    {
        return receiptList.Any(receipt => receipt.Id == receiptId);
    }

    public int FetchProductsInCashRegister()
    {
        return receiptList.Sum(receipt => receipt.Count);
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

        if (!CheckIfProductExists(productId))
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

        Console.WriteLine($"KVITTO {($"#{FetchReceiptNumber():D5}"),27}");
        Console.WriteLine($"{(DateTime.Now.ToString("yyyy-MM-dd")),-17}{(DateTime.Now.ToString("HH:mm:ss")),17}");

        if (receiptList.Count > 0)
        {
            Console.WriteLine("==================================");

            foreach (var item in receiptList)
            {
                Console.WriteLine($"{($"{item.Name.Substring(0, Math.Min(item.Name.Length, 11))}"),-13}{((item.Count > 1) ? $"{item.Count}*{item.Price}" : ""),-12}{($"{Math.Round(item.Count * item.Price, 2):F2}"),9}");

                if (item.Discount.EndsWith("kr") && string.Compare(item.Discount, "0kr", StringComparison.Ordinal) != 0)
                {
                    Console.WriteLine($"{($"   *Rabatt: {double.Parse(item.Discount.Replace("kr", ""))}kr/st"),-20}{($"-{double.Parse(item.Discount.Replace("kr", "")) * item.Count:F2}"),12}");
                }
                else if (item.Discount.EndsWith('%') && string.Compare(item.Discount, "0%", StringComparison.Ordinal) != 0)
                {
                    Console.WriteLine($"{($"   *Rabatt: {double.Parse(item.Discount.Replace("%", ""))}%"),-20}{($"-{item.Count * item.Price * (double.Parse(item.Discount.Replace("%", "")) / 100):F2}"),12}");
                }
            }
        }

        Console.WriteLine("==================================");

        Console.WriteLine($"{($"Total {FetchProductsInCashRegister()} varor"),34}");

        Console.WriteLine($"{($"Total: {CalculateTotalPrice():F2}"),31} kr");

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

        string receiptString = string.Format($"\nKVITTO {($"#{FetchReceiptNumber():D5}"),27}\n");
        receiptString += string.Format($"{(DateTime.Now.ToString("yyyy-MM-dd")),-17}{(DateTime.Now.ToString("HH:mm:ss")),17}\n");
        receiptString += string.Format("==================================\n");

        foreach (var item in receiptList)
        {
            receiptString += $"{($"{item.Name.Substring(0, Math.Min(item.Name.Length, 11))}"),-13}{((item.Count > 1) ? ($"{item.Count}*{item.Price}kr") : ("")),-12}{($"{Math.Round(item.Count * item.Price, 2):F2}\n"),10}";
            if (item.Discount.EndsWith("kr") && string.Compare(item.Discount, "0kr", StringComparison.Ordinal) != 0)
            {
                receiptString += $"{($"   *Rabatt: {double.Parse(item.Discount.Replace("kr", ""))}kr/st"),-20}{($"-{double.Parse(item.Discount.Replace("kr", "")) * item.Count:F2}\n"),13}";
            }
            else if (item.Discount.EndsWith('%') && string.Compare(item.Discount, "0%", StringComparison.Ordinal) != 0)
            {
                receiptString += $"{($"   *Rabatt: {double.Parse(item.Discount.Replace("%", ""))}%"),-20}{($"-{item.Count * item.Price * (double.Parse(item.Discount.Replace("%", "")) / 100):F2}\n"),13}";
            }
        }
        receiptString += string.Format("==================================\n");
        receiptString += string.Format($"{($"Total {FetchProductsInCashRegister()} varor"),34}\n");
        receiptString += string.Format($"{($"Total: {CalculateTotalPrice():F2}"),31} kr\n");
        receiptString += string.Format($"&&== {FetchReceiptNumber()} ==&&");

        File.AppendAllText(_filePath, receiptString);
    }
}