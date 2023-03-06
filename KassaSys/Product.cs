using KassaSys.Enum;

namespace KassaSys;

public class Product
{
	public List<ProductList> productList = new List<ProductList>();

	public Product()
	{

		/*
		 *
		 *   SPARA TILL FIL !!!!
		 *
		 */
		productList.Add(new ProductList { Id = 1, Name = "Bananer", Price = 12.50, Type = ProductType.kg });
		productList.Add(new ProductList { Id = 2, Name = "Kaffe", Price = 35.50, Type = ProductType.st });
		productList.Add(new ProductList { Id = 3, Name = "Honungsmelon", Price = 17.29, Type = ProductType.kg });
		productList.Add(new ProductList { Id = 4, Name = "Gurka", Price = 7.83, Type = ProductType.st });
		productList.Add(new ProductList { Id = 5, Name = "Färsk lax", Price = 52.78, Type = ProductType.kg });
		productList.Add(new ProductList { Id = 6, Name = "Citroner", Price = 3.84, Type = ProductType.kg });
		productList.Add(new ProductList { Id = 7, Name = "Blomkål", Price = 7.78, Type = ProductType.st });
		productList.Add(new ProductList { Id = 8, Name = "Kyckling", Price = 58.29, Type = ProductType.kg });
		productList.Add(new ProductList { Id = 9, Name = "Mjöl", Price = 25.83, Type = ProductType.st });
		productList.Add(new ProductList { Id = 10, Name = "Lammkött", Price = 82.97, Type = ProductType.st });
	}

	public List<ProductList> GetList()
	{
		return productList;
	}

	public string FetchProductName(int id)
	{
		return productList.Where(product => product.Id == id).Select(product => product.Name).FirstOrDefault();
	}
	public double FetchProductPrice(int id)
	{
		return productList.Where(product => product.Id == id).Select(product => product.Price).FirstOrDefault();
	}

	public void AddProduct()
	{
		int id = (productList.Count > 0) ? productList.Last().Id + 1 : 1;
		double price = 0;
		int group = 0;
		string name = "";

		Console.Clear();
		Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
		Console.WriteLine("  - Lägg till en ny produkt -\n");

		while (true)
		{
			Console.Write("   Ange namn (minst 3 tecken): ");
			name = Console.ReadLine();

			if (name.Length >= 3)
			{
				break;
			}
			if (name == "0")
			{
				return;
			}
		}

		while (true)
		{
			Console.Write("   Ange pris: ");
			double.TryParse(Console.ReadLine(), out price);

			if (price > 0)
			{
				break;
			}
			if (price == 0)
			{
				return;
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

			if (group > 0 && group < i)
			{
				group--;
				break;
			}
			if (group == 0)
			{
				return;
			}
		}

		productList.Add(new ProductList { Id = id, Name = name, Price = price, Type = (ProductType)group });
	}

	public void UpdateProduct()
	{
		while (true)
		{
			int pID = 0;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Uppdatera namn och pris på en produkt -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2}\n", "Id", "Namn", "Pris");
			foreach (var product in productList)
			{
				Console.WriteLine($"    {product.Id,-3} {product.Name,-15} {product.Price:F2} / {product.Type}");
			}

			Console.Write("\n");

			while (true)
			{
				Console.Write("  Välj ID: ");
				bool check = int.TryParse(Console.ReadLine(), out pID);

				if (pID > 0)
				{
					break;
				}
				if (pID == 0)
				{
					return;
				}
			}

			Console.Write("\n");

			string newName = "";

			while (true)
			{
				Console.Write($"    Ange nytt namn på ({FetchProductName(pID)}): ");
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

			double newPrice = 0;

			while (true)
			{
				Console.Write($"    Ange nytt pris (tidigare: {FetchProductPrice(pID):F2}): ");
				bool check = double.TryParse(Console.ReadLine(), out newPrice);

				if (newPrice == 0)
				{
					return;
				}
				if (newPrice > 0)
				{
					break;
				}
			}

			productList.Where(product => product.Id == pID).ToList().ForEach(product =>
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
			int pID = 0;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Ta bort en produkt -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-10} {3}\n", "Id", "Namn", "Pris", "Prisgrupp");

			if (productList.Count > 0)
			{
				foreach (var product in productList)
				{
					Console.WriteLine($"    {product.Id,-3} {product.Name,-15} {product.Price,-10:F2} {product.Type}");
				}
			}
			else
			{
				Console.WriteLine("    Inga produkter i registret...");
			}

			Console.Write("\n");

			while (true)
			{
				Console.Write("  Välj ID: ");
				bool check = int.TryParse(Console.ReadLine(), out pID);

				if (pID > 0)
				{
					break;
				}
				if (pID == 0)
				{
					return;
				}
			}

			productList.Where(product => product.Id == pID).ToList().ForEach(product =>
			{
				productList.Remove(product);
			});
		}
	}
}
