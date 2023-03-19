using KassaSys.Enum;
using KassaSys.Campaign;

namespace KassaSys.Product;

public class ShopProduct : IShopProducts
{
	private string _filePath = @".\product.txt";
	private string _splitString = " | ";
	public List<ProductList> ProductList = new List<ProductList>();

	public ShopProduct()
	{
		ProductList = FetchProductFromFile();
	}
	public List<ProductList> FetchProductFromFile()
	{
		var tempProductList = new List<ProductList>();

		if (!File.Exists(_filePath))
		{
			return tempProductList;
		}

		foreach (var line in File.ReadLines(_filePath))
		{
			var args = line.Split(_splitString);
			var product = new ProductList();

			product.Id = Convert.ToInt32(args[0]);
			product.Name = args[1];
			product.Price = Convert.ToDouble(args[2]);
			product.Type = (ProductType)System.Enum.Parse(typeof(ProductType), args[3]);

			tempProductList.Add(product);
		}

		return tempProductList;
	}
	private void SaveAllToFile(List<ProductList> tempProductList)
	{
		var stringList = new List<string>();

		foreach (var product in tempProductList)
		{
			string productString = $"{product.Id}{_splitString}{product.Name}{_splitString}{product.Price}{_splitString}{product.Type}";
			stringList.Add(productString);
		}

		File.WriteAllLines(_filePath, stringList);
	}
	public List<ProductList> GetList()
	{
		return FetchProductFromFile();
	}
	public string FetchProductName(int id)
	{
		return ProductList.Where(product => product.Id == id).Select(product => product.Name).FirstOrDefault();
	}
	public double FetchProductPrice(int id)
	{
		return ProductList.Where(product => product.Id == id).Select(product => product.Price).FirstOrDefault();
	}
	public bool CheckIfProductExists(int id)
	{
		return ProductList.Any(product => product.Id == id);
	}
	public ProductType FetchProductType(int id)
	{
		return ProductList.Where(product => product.Id == id).Select(product => product.Type).FirstOrDefault();
	}

	public void AddProduct()
	{
		double price;
		int group;
		string name;
		ProductType inputEnum = ProductType.kg;

		Console.Clear();
		Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
		Console.WriteLine("  - Lägg till en ny produkt -\n");

		while (true)
		{
			Console.Write("   Ange produkt namn: ");
			name = Console.ReadLine();

			if (name == "0")
			{
				return;
			}
			if (name.Length > 0)
			{
				break;
			}
		}

		while (true)
		{
			Console.Write("   Ange produkt pris (ex 12,34): ");
			bool check = double.TryParse(Console.ReadLine(), out price);

			if (check && price == 0)
			{
				return;
			}
			if (check && price > 0)
			{
				break;
			}
		}

		while (true)
		{
			Console.Write("   Ange prisgrupp (");

			int i = 0;
			foreach (var type in System.Enum.GetValues(typeof(ProductType)))
			{
				Console.Write($"{type}");

				if (i == 0)
				{
					Console.Write(", ");
					i++;
				}
			}

			Console.Write(") : ");

			string val = Console.ReadLine();

			if (val == "0")
			{
				break;
			}

			if (System.Enum.TryParse(val, true, out inputEnum))
			{
				break;
			}
		}

		ProductList.Add(new ProductList { Id = ProductList.Count > 0 ? ProductList.Last().Id + 1 : 1, Name = name, Price = price, Type = inputEnum });

		SaveAllToFile(ProductList);
	}

	public void UpdateProduct()
	{
		while (true)
		{
			int productID;
			string newName;
			double newPrice;
			ProductType newType = ProductType.kg;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Uppdatera namn och pris på en produkt -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-9}\n", "Id", "Namn", "Pris");

			int i = 1;
			foreach (var product in ProductList)
			{
				Console.WriteLine($"    {product.Id,-3} {product.Name,-15} {product.Price,9:F2} per {product.Type}");

				i++;

				if (i % 2 == 0)
				{
					Console.ForegroundColor = ConsoleColor.DarkGray;
				}
				else
				{
					Console.ResetColor();
				}
			}

			Console.ResetColor();

			if (ProductList.Count == 0)
			{
				Console.WriteLine("     Inga produkter att uppdatera...\n");
				Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
				Console.ReadKey();

				return;
			}

			Console.Write("\n");

			while (true)
			{
				Console.Write("  Välj produkt ID: ");
				bool check = int.TryParse(Console.ReadLine(), out productID);

				if (check && productID == 0)
				{
					return;
				}
				if (check && productID > 0)
				{
					break;
				}
			}

			Console.Write("\n");

			while (true)
			{
				Console.Write($"    Ange nytt produkt namn på ({FetchProductName(productID)}): ");
				newName = Console.ReadLine();

				if (newName == "0")
				{
					return;
				}
				if (newName.Length > 0)
				{
					break;
				}
			}

			while (true)
			{
				Console.Write($"    Ange nytt produkt pris (tidigare: {FetchProductPrice(productID):F2}): ");
				bool check = double.TryParse(Console.ReadLine(), out newPrice);

				if (check && newPrice == 0)
				{
					return;
				}
				if (check && newPrice > 0)
				{
					break;
				}
			}

			while (true)
			{
				Console.Write($"   Ange ny prisgrupp (tidigare: {FetchProductType(productID)}) (");

				int j = 0;
				foreach (var type in System.Enum.GetValues(typeof(ProductType)))
				{
					Console.Write($"{type}");

					if (j == 0)
					{
						Console.Write(", ");
						j++;
					}
				}

				Console.Write(") : ");

				string val = Console.ReadLine();

				if (val == "0")
				{
					break;
				}

				if (System.Enum.TryParse(val, true, out newType))
				{
					break;
				}
			}

			ProductList.Where(product => product.Id == productID).ToList().ForEach(product =>
			{
				product.Name = newName;
				product.Price = newPrice;
				product.Type = newType;
			});

			SaveAllToFile(ProductList);
		}
	}

	public void RemoveProduct()
	{
		while (true)
		{
			int productID;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Ta bort en produkt -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-10} {3}\n", "Id", "Namn", "Pris", "Prisgrupp");

			int i = 1;

			foreach (var product in ProductList)
			{
				Console.WriteLine($"    {product.Id,-3} {product.Name,-15} {product.Price,8:F2}   {product.Type}");

				i++;

				if (i % 2 == 0)
				{
					Console.ForegroundColor = ConsoleColor.DarkGray;
				}
				else
				{
					Console.ResetColor();
				}
			}

			Console.ResetColor();

			if (ProductList.Count == 0)
			{
				Console.WriteLine("     Inga produkter att ta bort...\n");
				Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
				Console.ReadKey();

				return;
			}

			Console.Write("\n");

			while (true)
			{
				Console.Write("  Välj produkt ID: ");
				bool check = int.TryParse(Console.ReadLine(), out productID);

				if (check && productID == 0)
				{
					return;
				}
				if (check && productID > 0)
				{
					break;
				}
			}

			ProductList.Where(product => product.Id == productID).ToList().ForEach(product =>
			{
				ProductList.Remove(product);
			});

			SaveAllToFile(ProductList);

			ShopCampaign campaignList = new ShopCampaign();

			campaignList.RemoveCampaignId(productID);
		}
	}
}
