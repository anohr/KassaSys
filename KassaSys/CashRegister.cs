using KassaSys.Enum;

namespace KassaSys;

public class CashRegister
{
	private List<CashRegisterList> _receiptList = new List<CashRegisterList>();
	private string _dirPath = @".\receipts";
	private string _filePath = @".\receipts\receipt_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
	private string _receiptEnd = "&&==END==&&";

	Product ProductList = new Product();
	ShopCampaign CampaignList = new ShopCampaign();

	public int FetchTotalReceipts()
	{
		int countReceipts = 1;

		if (!Directory.Exists(_dirPath))
		{
			Directory.CreateDirectory(_dirPath);
		}

		foreach (string file in Directory.GetFiles(_dirPath))
		{
			countReceipts += File.ReadLines(file).Count(line => line.Contains(_receiptEnd));
		}

		return countReceipts;
	}

	private string FetchProductName(int id)
	{
		return ProductList.GetList().Where(product => product.Id == id).Select(product => product.Name).FirstOrDefault();
	}
	private double FetchProductPrice(int id)
	{
		return ProductList.GetList().Where(product => product.Id == id).Select(product => product.Price).FirstOrDefault();
	}
	private ProductType FetchProductType(int id)
	{
		return ProductList.GetList().Where(product => product.Id == id).Select(product => product.Type).FirstOrDefault();
	}
	public bool CheckIfProductExists(int id)
	{
		return ProductList.GetList().Any(product => product.Id == id);
	}
	public double FetchTotalPrice()
	{
		return _receiptList.Sum(receipt => (receipt.Discount > 1) ? ((receipt.Price - receipt.Discount) * receipt.Count) : ((receipt.Discount == 0) ? (receipt.Price * receipt.Count) : ((receipt.Discount * receipt.Price) * receipt.Count)));
	}
	public bool CheckIfProductAdded(int id)
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
			return;

		if (!CheckIfProductExists(id))
			return;

		double campaine = CampaignList.GetList().OrderBy(discount => discount.Discount < 1 ? discount.Discount : (((discount.Discount * 100) - 100) * -1)).Where(discount => discount.ProductID == id && (discount.StartDate <= DateTime.Now && discount.StartDate.AddDays(discount.EndDate) >= DateTime.Now)).Select(discount => discount.Discount).FirstOrDefault();

		if (!CheckIfProductAdded(id))
		{
			_receiptList.Add(new CashRegisterList { Id = id, Name = FetchProductName(id), Count = amount, Price = FetchProductPrice(id), Type = FetchProductType(id), Discount = campaine });
		}
		else
		{
			UpdateProductInReceipt(id, amount);
		}
	}
	public void PrintReceipt()
	{
		foreach (var item in _receiptList)
		{
			Console.WriteLine("{0,-13}{1,-12}{2,9}", $"{item.Name.Substring(0, Math.Min(item.Name.Length, 11))}", $"{item.Count} * {item.Price}", $"{(Math.Round(item.Count * item.Price, 2)):F2}");
			if (item.Discount < 1 && item.Discount != 0)
			{
				Console.WriteLine("{0,11}{1,23}", "*Rabatt:", $"-{(item.Price * item.Count) - (item.Price * item.Count * item.Discount):F2}");
			}
			if (item.Discount >= 1)
			{
				Console.WriteLine("{0,11}{1,23}", "*Rabatt:", $"-{item.Count * item.Discount:F2}");
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

		string receiptString = String.Format($"KVITTO  #{FetchTotalReceipts():D4}  {DateTime.Now}\n");
		receiptString += String.Format("==================================\n");

		foreach (var item in _receiptList)
		{
			receiptString += String.Format("{0,-13}{1,-12}{2,9}", $"{item.Name.Substring(0, Math.Min(item.Name.Length, 11))}", $"{item.Count} * {item.Price}", $"{(Math.Round(item.Count * item.Price, 2)):F2}\n");
			if (item.Discount < 1 && item.Discount != 0)
			{
				receiptString += String.Format("{0,11}{1,23}", "*Rabatt:", $"-{(item.Price * item.Count) - (item.Price * item.Count * item.Discount):F2}\n");
			}
			if (item.Discount >= 1)
			{
				receiptString += String.Format("{0,11}{1,23}", "*Rabatt:", $"-{item.Count * item.Discount:F2}\n");
			}
		}
		receiptString += String.Format("==================================\n");
		receiptString += String.Format("{0,34}", $"Total: {FetchTotalPrice():F2}\n");
		receiptString += String.Format(_receiptEnd + "\n");

		File.AppendAllText(_filePath, receiptString);
	}
}