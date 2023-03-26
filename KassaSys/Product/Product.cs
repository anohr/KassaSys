using KassaSys.Campaign;
using KassaSys.Enum;

namespace KassaSys.Product;

public class ShopProduct : IProducts
{
	private string _filePath = @".\product.txt";
	private string _splitString = " | ";
	public List<ProductList> productList = new List<ProductList>();

	public ShopProduct()
	{
		productList = FetchProductFromFile();
	}

	public List<ProductList> FetchProductFromFile()
	{
		var tempProductList = new List<ProductList>();

		if (!File.Exists(_filePath))
		{
			return tempProductList;
		}

		tempProductList = File.ReadLines(_filePath)
			.Select(line =>
			{
				var args = line.Split(_splitString);
				return new ProductList
				{
					Id = Convert.ToInt32(args[0]),
					Name = args[1],
					Price = Convert.ToDouble(args[2]),
					Type = (ProductType)System.Enum.Parse(typeof(ProductType), args[3])
				};
			})
			.ToList();

		return tempProductList;
	}

	private void SaveToFile(List<ProductList> tempProductList)
	{
		var stringList = tempProductList.Select(product => $"{product.Id}{_splitString}{product.Name}{_splitString}{product.Price}{_splitString}{product.Type}").ToList();

		File.WriteAllLines(_filePath, stringList);
	}

	public List<ProductList> FetchList()
	{
		return FetchProductFromFile();
	}

	public string FetchProductName(int productId)
	{
		return productList.Where(product => product.Id == productId).Select(product => product.Name).FirstOrDefault();
	}

	public double FetchProductPrice(int productId)
	{
		return productList.Where(product => product.Id == productId).Select(product => product.Price).FirstOrDefault();
	}

	public bool CheckIfProductExists(int productId)
	{
		return productList.Any(product => product.Id == productId);
	}

	public ProductType FetchProductType(int productId)
	{
		return productList.Where(product => product.Id == productId).Select(product => product.Type).FirstOrDefault();
	}

	public void AddProduct()
	{
		string tempInput = string.Empty;
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
			tempInput = Console.ReadLine().Trim();

			if (!string.IsNullOrWhiteSpace(tempInput))
			{
				if (tempInput == "0")
				{
					return;
				}

				name = tempInput.Trim();

				break;
			}

			Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
		}

		while (true)
		{
			tempInput = Program.AskForInput("   Ange produkt pris (ex 12,34): ");

			if (!string.IsNullOrWhiteSpace(tempInput))
			{
				if (tempInput == "0")
				{
					return;
				}

				if (double.TryParse(tempInput.Replace('.', ','), out price) && price > 0)
				{
					break;
				}
			}

			Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
		}

		while (true)
		{
			tempInput = Program.AskForInput($"   Ange prisgrupp ({string.Join(", ", System.Enum.GetNames(typeof(ProductType)))}): ");

			if (!string.IsNullOrWhiteSpace(tempInput))
			{
				if (tempInput == "0")
				{
					return;
				}

				if (System.Enum.TryParse<ProductType>(tempInput, true, out ProductType result))
				{
					inputEnum = result;

					break;
				}
			}

			Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
		}

		productList.Add(new ProductList { Id = productList.Count > 0 ? productList.Last().Id + 1 : 1, Name = name, Price = price, Type = inputEnum });

		SaveToFile(productList);
	}

	public void UpdateProduct()
	{
		while (true)
		{
			string tempInput = string.Empty;
			int productId;
			string newName;
			double newPrice;
			ProductType newType = ProductType.kg;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Uppdatera namn och pris på en produkt -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-9}\n", "Id", "Namn", "Pris");

			for (int i = 0; i < productList.Count; i++)
			{
				var product = productList[i];

				Console.WriteLine($"    {product.Id,-3} {product.Name,-15} {product.Price,8:F2}   {product.Type}");

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

			if (productList.Count == 0)
			{
				Console.WriteLine("     Inga produkter att uppdatera...\n");
				Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
				Console.ReadKey();

				return;
			}

			Console.Write("\n");

			while (true)
			{
				tempInput = Program.AskForInput("  Välj produkt Id: ");

				if (!string.IsNullOrWhiteSpace(tempInput))
				{
					if (tempInput == "0")
					{
						return;
					}

					if (int.TryParse(tempInput, out productId) && productId > 0 && CheckIfProductExists(productId))
					{
						break;
					}
				}

				Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
			}

			Console.Write("\n");

			while (true)
			{
				var productName = FetchProductName(productId);

				tempInput = Program.AskForInput($"    Ange nytt produkt namn på ({productName}): ");

				if (!string.IsNullOrWhiteSpace(tempInput))
				{
					if (tempInput == "0")
					{
						return;
					}

					newName = tempInput.Trim();

					break;
				}
				else if (string.IsNullOrEmpty(tempInput))
				{
					newName = productName;
					break;
				}

				Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
			}

			while (true)
			{
				var productPrice = FetchProductPrice(productId);

				tempInput = Program.AskForInput($"    Ange nytt produkt pris ({productPrice:F2}): ");

				if (!string.IsNullOrWhiteSpace(tempInput))
				{
					if (tempInput == "0")
					{
						return;
					}

					if (double.TryParse(tempInput.Replace('.', ','), out newPrice) && newPrice > 0)
					{
						break;
					}
				}
				else if (string.IsNullOrEmpty(tempInput))
				{
					newPrice = productPrice;
					break;
				}

				Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
			}

			while (true)
			{
				var productType = FetchProductType(productId);

				tempInput = Program.AskForInput($"    Ange ny prisgrupp ({productType}) ({string.Join(", ", System.Enum.GetNames(typeof(ProductType)))}): ");

				if (!string.IsNullOrWhiteSpace(tempInput))
				{
					if (tempInput == "0")
					{
						return;
					}

					if (System.Enum.TryParse<ProductType>(tempInput, true, out ProductType type))
					{
						newType = type;

						break;
					}
				}
				else if (string.IsNullOrEmpty(tempInput))
				{
					newType = productType;
					break;
				}

				Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
			}

			productList.Where(product => product.Id == productId).ToList().ForEach(product =>
			{
				product.Name = newName;
				product.Price = newPrice;
				product.Type = newType;
			});

			SaveToFile(productList);
		}
	}

	public void RemoveProduct()
	{
		while (true)
		{
			string tempInput = string.Empty;
			int productId;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Ta bort en produkt -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-10} {3}\n", "Id", "Namn", "Pris", "Prisgrupp");

			for (int i = 0; i < productList.Count; i++)
			{
				var product = productList[i];

				Console.WriteLine($"    {product.Id,-3} {product.Name,-15} {product.Price,8:F2}   {product.Type}");

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

			if (productList.Count == 0)
			{
				Console.WriteLine("     Inga produkter att ta bort...\n");
				Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
				Console.ReadKey();

				return;
			}

			Console.Write("\n");

			while (true)
			{
				tempInput = Program.AskForInput("  Välj produkt Id: ");

				if (!string.IsNullOrWhiteSpace(tempInput))
				{
					if (tempInput == "0")
					{
						return;
					}

					if (int.TryParse(tempInput, out productId) && productId > 0 && CheckIfProductExists(productId))
					{
						break;
					}
				}

				Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
			}

			productList.Where(product => product.Id == productId).ToList().ForEach(product =>
			{
				productList.Remove(product);
			});

			SaveToFile(productList);

			ShopCampaign campaignList = new ShopCampaign();

			campaignList.RemoveCampaign(productId);
		}
	}
}