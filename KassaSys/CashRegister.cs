using KassaSys.Enum;

namespace KassaSys;

public class CashRegister
{
	private List<CashRegisterList> _receiptList = new List<CashRegisterList>();
	private string _dirPath = @".\receipts";
	private string _filePath = @".\receipts\receipt_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
	private string _receiptEnd = "&&==END==&&";

	Product ProductList = new Product();
	ShopCampaine CampaineList = new ShopCampaine();

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
		return _receiptList.Sum(receipt => receipt.Count * (receipt.Price - receipt.Discount));
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

		double campaine = CampaineList.GetList().OrderBy(discount => discount.StartDate).Where(discount => discount.ProductID == id && (discount.StartDate <= DateTime.Now && discount.StartDate.AddDays(discount.EndDate) >= DateTime.Now)).Select(discount => discount.Discount).FirstOrDefault();

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
			Console.WriteLine($"{item.Name} {item.Count} {item.Type} * {item.Price:F2} = {(Math.Round(item.Count * item.Price, 2)):F2}");
			if (item.Discount > 0)
			{
				Console.WriteLine($"   Rabatt: -{item.Count * item.Discount:F2}");
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

		string receiptString = $"KVITTO  #{FetchTotalReceipts():D4}  {DateTime.Now}\n";

		foreach (var item in _receiptList)
		{
			receiptString += $"{item.Name} {item.Count} {item.Type} * {item.Price:F2} = {(Math.Round(item.Count * item.Price, 2)):F2}\n";
			if (item.Discount > 0)
			{
				receiptString += $"   Rabatt: -{item.Count * item.Discount:F2}\n";
			}
		}

		receiptString += $"Total: {FetchTotalPrice():F2}\n";
		receiptString += _receiptEnd + "\n";

		File.AppendAllText(_filePath, receiptString);
	}
}