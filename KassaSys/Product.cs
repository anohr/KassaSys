using KassaSys.Enum;

namespace KassaSys;

public class Product
{
	public List<ProductList> ProductList = new List<ProductList>();

	public Product()
	{

		/*
		 *
		 *   SPARA TILL FIL !!!!
		 *   och ladda in den till listan sen...
		 *
		 */
		/*ProductList.Add(new ProductList { Id = 1, Name = "Bananer", Price = 12.50, Type = ProductType.kg });
		ProductList.Add(new ProductList { Id = 2, Name = "Kaffe", Price = 35.50, Type = ProductType.st });
		ProductList.Add(new ProductList { Id = 3, Name = "Honungsmelon", Price = 17.29, Type = ProductType.kg });
		ProductList.Add(new ProductList { Id = 4, Name = "Gurka", Price = 7.83, Type = ProductType.st });
		ProductList.Add(new ProductList { Id = 5, Name = "Färsk lax", Price = 52.78, Type = ProductType.kg });
		ProductList.Add(new ProductList { Id = 6, Name = "Citroner", Price = 3.84, Type = ProductType.kg });
		ProductList.Add(new ProductList { Id = 7, Name = "Blomkål", Price = 7.78, Type = ProductType.st });
		ProductList.Add(new ProductList { Id = 8, Name = "Kyckling", Price = 58.29, Type = ProductType.kg });
		ProductList.Add(new ProductList { Id = 9, Name = "Mjöl", Price = 25.83, Type = ProductType.st });
		ProductList.Add(new ProductList { Id = 10, Name = "Lammkött", Price = 82.97, Type = ProductType.st });*/
	}
	public List<ProductList> GetList()
	{
		return ProductList;
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
			Console.Write("   Ange namn (minst 3 tecken): ");
			name = Console.ReadLine();

			if (name == "0")
			{
				return;
			}
			if (name.Length >= 3)
			{
				break;
			}
		}

		while (true)
		{
			Console.Write("   Ange pris: ");
			double.TryParse(Console.ReadLine(), out price);

			if (price == 0)
			{
				return;
			}
			if (price > 0)
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
		}
	}
}
