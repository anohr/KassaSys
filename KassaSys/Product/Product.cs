using KassaSys.Campaign;
using KassaSys.Enum;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

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
			tempInput = Console.ReadLine();

			if (!string.IsNullOrWhiteSpace(tempInput))
			{
				if (tempInput == "0")
				{
					return;
				}

				name = tempInput.Trim();

				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("     Felaktig inmatning. Försök igen.");
			Console.ResetColor();
		}

		while (true)
		{
			Console.Write("   Ange produkt pris (ex 12,34): ");
			tempInput = Console.ReadLine();

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

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("     Felaktig inmatning. Försök igen.");
			Console.ResetColor();
		}

		while (true)
		{
			Console.Write($"   Ange prisgrupp ({string.Join(", ", System.Enum.GetNames(typeof(ProductType)))}): ");
			tempInput = Console.ReadLine();

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

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("     Felaktig inmatning. Försök igen.");
			Console.ResetColor();
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

			int i = 1;
			foreach (var product in productList)
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
				Console.Write("  Välj produkt Id: ");
				tempInput = Console.ReadLine();

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

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("      Felaktig inmatning. Försök igen.");
				Console.ResetColor();
			}

			Console.Write("\n");

			while (true)
			{
				Console.Write($"    Ange nytt produkt namn på ({FetchProductName(productId)}): ");
				newName = Console.ReadLine();

				if (!string.IsNullOrWhiteSpace(newName))
				{
					if (newName == "0")
					{
						return;
					}

					newName = newName.Trim();

					break;
				}

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("      Felaktig inmatning. Försök igen.");
				Console.ResetColor();
			}

			while (true)
			{
				Console.Write($"    Ange nytt produkt pris ({FetchProductPrice(productId):F2}): ");
				tempInput = Console.ReadLine();

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

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("      Felaktig inmatning. Försök igen.");
				Console.ResetColor();
			}

			while (true)
			{
				Console.Write($"    Ange ny prisgrupp ({FetchProductType(productId)}) ({string.Join(", ", System.Enum.GetNames(typeof(ProductType)))}): ");
				tempInput = Console.ReadLine();

				if (!string.IsNullOrWhiteSpace(tempInput))
				{
					if (tempInput == "0")
					{
						return;
					}

					if (System.Enum.TryParse<ProductType>(tempInput, true, out ProductType result))
					{
						newType = result;

						break;
					}
				}

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("      Felaktig inmatning. Försök igen.");
				Console.ResetColor();
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
				Console.Write("  Välj produkt Id: ");
				tempInput = Console.ReadLine();

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

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("      Felaktig inmatning. Försök igen.");
				Console.ResetColor();
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