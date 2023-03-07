using KassaSys.Enum;

namespace KassaSys;

public class Product
{
	private string _filePath = @".\product.txt";
	private string _splitString = " | ";
	public List<ProductList> ProductList = new List<ProductList>();

	public Product()
	{
		ProductList = InitializeFromFile();
	}
	public List<ProductList> InitializeFromFile()
	{
		var tempProductList = new List<ProductList>();

		if (!File.Exists(_filePath))
			return tempProductList;

		foreach (var line in File.ReadLines(_filePath))
		{
			var args = line.Split(_splitString);
			var product = new ProductList();

			product.Id = Convert.ToInt32(args[0]);
			product.Name = args[1];
			product.Price = Convert.ToDouble(args[2]);
			product.Type = (ProductType)Enum.ProductType.Parse(typeof(ProductType), args[3]);

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
		return InitializeFromFile();
	}
	public string FetchProductName(int id)
	{
		return ProductList.Where(product => product.Id == id).Select(product => product.Name).FirstOrDefault();
	}
	public double FetchProductPrice(int id)
	{
		return ProductList.Where(product => product.Id == id).Select(product => product.Price).FirstOrDefault();
	}

	public void AddProduct()
	{
		double price;
		int group;
		string name;

		Console.Clear();
		Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
		Console.WriteLine("  - Lägg till en ny produkt -\n");

		while (true)
		{
			Console.Write("   Ange namn: ");
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
			Console.Write("   Ange pris (ex 12,34): ");
			double.TryParse(Console.ReadLine(), out price);

			if (price == 0)
			{
				return;
			}
			if (price >= 0)
			{
				break;
			}
		}

		while (true)
		{
			int i = 1;
			Console.WriteLine("   Ange prisgrupp: ");

			foreach (var type in Enum.ProductType.GetValues(typeof(ProductType)))
			{
				Console.WriteLine($"     {i}. {type}");
				i++;
			}

			Console.Write("     Val: ");
			int.TryParse(Console.ReadLine(), out group);

			if (group == 0)
			{
				return;
			}
			if (group > 0 && group < i)
			{
				group--;
				break;
			}
		}

		ProductList.Add(new ProductList { Id = ((ProductList.Count > 0) ? ProductList.Last().Id + 1 : 1), Name = name, Price = price, Type = (ProductType)group });

		SaveAllToFile(ProductList);
	}

	public void UpdateProduct()
	{
		while (true)
		{
			int productID;
			string newName;
			double newPrice;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Uppdatera namn och pris på en produkt -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2}\n", "Id", "Namn", "Pris");

			foreach (var product in ProductList)
			{
				Console.WriteLine($"    {product.Id,-3} {product.Name,-15} {product.Price:F2} / {product.Type}");
			}

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
				Console.Write("  Välj ID: ");
				int.TryParse(Console.ReadLine(), out productID);

				if (productID == 0)
				{
					return;
				}
				if (productID > 0)
				{
					break;
				}
			}

			Console.Write("\n");

			while (true)
			{
				Console.Write($"    Ange nytt namn på ({FetchProductName(productID)}): ");
				newName = Console.ReadLine();

				if (newName == "0")
				{
					return;
				}
				if (newName.Length >= 3)
				{
					break;
				}
			}

			while (true)
			{
				Console.Write($"    Ange nytt pris (tidigare: {FetchProductPrice(productID):F2}): ");
				double.TryParse(Console.ReadLine(), out newPrice);

				if (newPrice == 0)
				{
					return;
				}
				if (newPrice > 0)
				{
					break;
				}
			}

			ProductList.Where(product => product.Id == productID).ToList().ForEach(product =>
			{
				product.Name = newName;
				product.Price = newPrice;
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

			foreach (var product in ProductList)
			{
				Console.WriteLine($"    {product.Id,-3} {product.Name,-15} {product.Price,-10:F2} {product.Type}");
			}

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
				Console.Write("  Välj ID: ");
				int.TryParse(Console.ReadLine(), out productID);

				if (productID == 0)
				{
					return;
				}
				if (productID > 0)
				{
					break;
				}
			}

			ProductList.Where(product => product.Id == productID).ToList().ForEach(product =>
			{
				ProductList.Remove(product);
			});

			SaveAllToFile(ProductList);

			ShopCampaine campaineList = new ShopCampaine();

			campaineList.RemoveCampaineId(productID);
		}
	}
}
