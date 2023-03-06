using KassaSys.Enum;

namespace KassaSys;

public class CashRegister
{
	private List<CashRegisterList> receiptList = new List<CashRegisterList>();
	private string DirPath = @".\receipts";
	private string FilePath = @".\receipts\receipt_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
	private string ReceiptEnd = "&&==END==&&";

	Product productList = new Product();
	ShopCampaine campaineList = new ShopCampaine();

	public int FetchTotalReceipts()
	{
		if (!Directory.Exists(DirPath))
		{
			Directory.CreateDirectory(DirPath);
		}

		int count = 1;
		foreach (string file in Directory.GetFiles(DirPath))
		{
			int endCount = File.ReadLines(file).Count(line => line.Contains(ReceiptEnd));
			count += endCount;
		}

		return count;
	}

	private string FetchProductName(int id)
	{
		return productList.GetList().Where(product => product.Id == id).Select(product => product.Name).FirstOrDefault();
	}
	private double FetchProductPrice(int id)
	{
		return productList.GetList().Where(product => product.Id == id).Select(product => product.Price).FirstOrDefault();
	}
	private ProductType FetchProductType(int id)
	{
		return productList.GetList().Where(product => product.Id == id).Select(product => product.Type).FirstOrDefault();
	}
	public bool CheckIfProductExists(int id)
	{
		return productList.GetList().Any(product => product.Id == id);
	}

	public double FetchTotal()
	{
		return receiptList.Sum(receipt => receipt.Count * (receipt.Price - receipt.Discount));
	}
	public bool CheckIfProductAdded(int id)
	{
		return receiptList.Any(receipt => receipt.Id == id);
	}

	public void UpdateProductInReceipt(int id, int amount)
	{
		receiptList.Where(receipt => receipt.Id == id).ToList().ForEach(receipt =>
		{
			int newAmount = receipt.Count += amount;
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

	public void AddToReceipt(int id, int amount)
	{
		if (amount < 0)
			return;

		if (!CheckIfProductExists(id))
			return;

		double campaine = campaineList.GetList().Where(discount => discount.ProductID == id && (discount.StartDate <= DateTime.Now && discount.StartDate.AddDays(discount.EndDate) >= DateTime.Now)).Select(discount => discount.Discount).FirstOrDefault();

		if (!CheckIfProductAdded(id))
		{
			receiptList.Add(new CashRegisterList { Id = id, Name = FetchProductName(id), Count = amount, Price = FetchProductPrice(id), Type = FetchProductType(id), Discount = campaine });
		}
		else
		{
			UpdateProductInReceipt(id, amount);
		}
	}

	public void PrintReceipt()
	{
		foreach (var item in receiptList)
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
		if (!File.Exists(FilePath))
		{
			File.Create(FilePath).Dispose();
		}

		if (receiptList.Count == 0)
		{
			return;
		}

		string receiptString = $"KVITTO  #{FetchTotalReceipts():D4}  {DateTime.Now}\n";

		foreach (var item in receiptList)
		{
			receiptString += $"{item.Name} {item.Count} {item.Type} * {item.Price:F2} = {(Math.Round(item.Count * item.Price, 2)):F2}\n";
			if (item.Discount > 0)
			{
				receiptString += $"   Rabatt: -{item.Count * item.Discount:F2}\n";
			}
		}

		receiptString += $"Total: {FetchTotal():F2}\n";
		receiptString += ReceiptEnd + "\n";

		File.AppendAllText(FilePath, receiptString);
	}
}