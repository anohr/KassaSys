using KassaSys.Campaign;
using KassaSys.Product;
using System.Text.RegularExpressions;

namespace KassaSys.Register;

public class CashRegister : ICashRegister
{
	private string _dirPath = @".\receipts";
	private string _filePath = @".\receipts\receipt_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
	public List<CashRegisterList> _receiptList = new List<CashRegisterList>();

	ShopProduct ProductList = new ShopProduct();
	ShopCampaign CampaignList = new ShopCampaign();

	public int FetchTotalReceipts()
	{
		int countReceipts = 0;

		if (!Directory.Exists(_dirPath))
		{
			Directory.CreateDirectory(_dirPath);
		}

		foreach (string file in Directory.GetFiles(_dirPath))
		{
			string lastLine = File.ReadLines(file).LastOrDefault();

			if (lastLine != null && Regex.IsMatch(lastLine, @"&&==\s*(\d+)*\s==&&"))
			{
				int number = int.Parse(Regex.Match(lastLine, @"&&==\s*(\d+)*\s==&&").Groups[1].Value);

				if (number > countReceipts)
				{
					countReceipts = number;
				}
			}
		}

		return countReceipts + 1;
	}

	public double FetchTotalPrice()
	{
		return Math.Round(_receiptList.Sum(receipt =>
		{
			string discountValue = receipt.Discount.Trim();
			double discount = 0;

			if (discountValue.EndsWith("kr"))
			{
				discount = double.Parse(discountValue.Trim().Replace("kr", ""));
			}
			else if (discountValue.EndsWith("%"))
			{
				double discountPercent = double.Parse(discountValue.Trim().Replace("%", ""));
				discount = 1 - (discountPercent / 100);
			}

			double price = receipt.Price * receipt.Count;
			double discountedPrice = price;

			if (discount >= 1)
			{
				discountedPrice = (receipt.Price - discount) * receipt.Count;
			}
			else if (discount > 0 && discount < 1)
			{
				discountedPrice = receipt.Price * discount * receipt.Count;
			}

			return discountedPrice;
		}), 2);
	}

	public bool CheckIfProductExicsts(int id)
	{
		return _receiptList.Any(receipt => receipt.Id == id);
	}

	public void UpdateProductInReceipt(int id, int amount)
	{
		_receiptList.Where(receipt => receipt.Id == id).ToList().ForEach(receipt =>
		{
			int newAmount = receipt.Count += amount;
			if (newAmount > 0)
			{
				receipt.Count = newAmount;
			}
			else
			{
				_receiptList.Remove(receipt);
			}
		});
	}

	public void AddToReceipt(int id, int amount)
	{
		if (amount < 0)
		{
			return;
		}

		if (!ProductList.CheckIfProductExists(id))
		{
			return;
		}

		string campaign = CampaignList.GetBestDiscount(id);

		if (!CheckIfProductExicsts(id))
		{
			_receiptList.Add(new CashRegisterList { Id = id, Name = ProductList.FetchProductName(id), Count = amount, Price = ProductList.FetchProductPrice(id), Type = ProductList.FetchProductType(id), Discount = campaign });
		}
		else
		{
			UpdateProductInReceipt(id, amount);
		}
	}

	public void PrintReceipt()
	{
		if (_receiptList.Count > 0)
		{
			Console.WriteLine("==================================");
			foreach (var item in _receiptList)
			{
				Console.WriteLine("{0,-13}{1,-12}{2,9}", $"{item.Name.Substring(0, Math.Min(item.Name.Length, 11))}", (item.Count > 1) ? item.Count + " * " + item.Price : "", $"{Math.Round(item.Count * item.Price, 2):F2}");

				if (item.Discount.Contains("kr") && item.Discount != "0kr")
				{
					Console.WriteLine("{0,-20}{1,14}", $"   *Rabatt: {int.Parse(item.Discount.Replace("kr", ""))}kr/st", $"-{int.Parse(item.Discount.Replace("kr", "")) * item.Count:F2}");
				}
				if (item.Discount.Contains('%') && item.Discount != "0%")
				{
					Console.WriteLine("{0,-20}{1,14}", $"   *Rabatt: {double.Parse(item.Discount.Replace("%", ""))}%", $"-{item.Count * item.Price * (double.Parse(item.Discount.Replace("%", "")) / 100):F2}");
				}
			}
		}
	}

	public void SaveReceipt()
	{
		if (!File.Exists(_filePath))
		{
			File.Create(_filePath).Dispose();
		}

		if (_receiptList.Count == 0)
		{
			return;
		}

		string receiptString = string.Format("\nKVITTO {0,7} {1,19}\n", $"#{FetchTotalReceipts():D4}", $"{DateTime.Now}");
		receiptString += string.Format("==================================\n");

		foreach (var item in _receiptList)
		{
			receiptString += string.Format("{0,-13}{1,-12}{2,10}", $"{item.Name.Substring(0, Math.Min(item.Name.Length, 11))}", (item.Count > 1) ? (item.Count + " * " + item.Price) : (""), $"{Math.Round(item.Count * item.Price, 2):F2}\n");
			if (item.Discount.Contains("kr") && item.Discount != "0kr")
			{
				receiptString += string.Format("{0,-20}{1,15}", $"   *Rabatt: {int.Parse(item.Discount.Replace("kr", ""))}kr/st", $"-{int.Parse(item.Discount.Replace("kr", "")) * item.Count:F2}\n");
			}
			if (item.Discount.Contains('%') && item.Discount != "0%")
			{
				receiptString += string.Format("{0,-20}{1,15}", $"   *Rabatt: {double.Parse(item.Discount.Replace("%", ""))}%", $"-{item.Count * item.Price * (double.Parse(item.Discount.Replace("%", "")) / 100):F2}\n");
			}
		}
		receiptString += string.Format("==================================\n");
		receiptString += string.Format("{0,31} kr\n", $"Total: {FetchTotalPrice():F2}");
		receiptString += string.Format($"&&== {FetchTotalReceipts()} ==&&");

		File.AppendAllText(_filePath, receiptString);
	}
}