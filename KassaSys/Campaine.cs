using KassaSys.Enum;

namespace KassaSys;

public class ShopCampaine
{
	private List<CampaineList> _campaineList = new List<CampaineList>();

	Product ProductList = new Product();

	public ShopCampaine()
	{
		/*
		 *
		 *   SPARA TILL FIL !!!!
		 *   när man tar bort produkt ta bort relationer i  kampanj etc där saker överlappar
		 *   kolla om det är % eller kr rabatten ska vara i
		 *   string TempCampaine = $"{id}%&%{productID}%&%{DateTime.Now}%&%{10}%&%{7}";
		 *
		 *   https://stackoverflow.com/questions/3717028/access-list-from-another-class#3717187
		 *
		 */
		/*_campaineList.Add(new CampaineList { Id = 1, ProductID = 1, StartDate = DateTime.Now.AddDays(-2), EndDate = 10, Discount = 7 });
		_campaineList.Add(new CampaineList { Id = 2, ProductID = 2, StartDate = DateTime.Now, EndDate = 3, Discount = 1 });
		_campaineList.Add(new CampaineList { Id = 3, ProductID = 7, StartDate = DateTime.Now.AddDays(3), EndDate = 10, Discount = 3 });*/
	}
	public List<CampaineList> GetList()
	{
		return _campaineList;
	}
	public void AddCampaine()
	{
		int id = (_campaineList.Count > 0) ? _campaineList.Last().Id + 1 : 1;
		int campaineID;
		double discount;
		DateTime startDate;
		int endDate;

		Console.Clear();
		Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
		Console.WriteLine("  - Lägg till en ny kampanj -\n");

		while (true)
		{
			int i = 1;
			Console.WriteLine("    Ange produkt till din kampanj: \n");
			Console.WriteLine("    {0,-3} {1,-20} {2}\n", "Id", "Produkt namn", "Pris");

			foreach (var product in ProductList.GetList())
			{
				Console.WriteLine($"    {product.Id,-3} {product.Name,-20} {product.Price:F2}");
				i++;
			}

			if (ProductList.GetList().Count == 0)
			{
				Console.WriteLine("     Inga produkter att lägga kampanj på...\n");
				Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
				Console.ReadKey();

				return;
			}

			Console.Write("\n  Välj ID: ");
			int.TryParse(Console.ReadLine(), out campaineID);

			if (campaineID == 0)
			{
				return;
			}
			else if (campaineID > 0)
			{
				break;
			}
		}

		while (true)
		{
			string format = "yyyy-MM-dd";

			Console.Write("  Ange start datum (YYYY-MM-DD): ");
			string inputDate = Console.ReadLine();

			if (inputDate == "0")
			{
				return;
			}

			bool dateCheck = DateTime.TryParseExact(inputDate, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out startDate);

			if (dateCheck && startDate.Date >= DateTime.Now.Date)
			{
				break;
			}
		}

		while (true)
		{
			Console.Write("  Ange hur många dagar kampanje ska gälla: ");
			int.TryParse(Console.ReadLine(), out endDate);

			if (endDate == 0)
			{
				return;
			}
			if (endDate > 0)
			{
				break;
			}
		}

		while (true)
		{
			Console.Write("  Ange hur mycket rabbat i kr: ");
			double.TryParse(Console.ReadLine(), out discount);

			if (discount == 0)
			{
				return;
			}
			if (discount > 0 && ProductList.FetchProductPrice(campaineID)! > discount)
			{
				break;
			}
		}

		_campaineList.Add(new CampaineList { Id = id, ProductID = campaineID, StartDate = startDate, EndDate = endDate, Discount = discount });
	}

	public void UpdateCampaine()
	{
		while (true)
		{
			int campaineID;
			DateTime startDate;
			int endDate;
			double discount;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Uppdatera en kampanj -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-10}    {3,-10} {4}\n", "Id", "Produktnamn", "Start", "Slut", "Rabatt");

			foreach (var campaine in _campaineList)
			{
				Console.WriteLine($"    {campaine.Id,-3} {ProductList.FetchProductName(campaine.ProductID),-15} {campaine.StartDate.ToString("yyyy-MM-dd")} -> {campaine.StartDate.AddDays(campaine.EndDate).ToString("yyyy-MM-dd")} {campaine.Discount} kr");
			}

			if (_campaineList.Count == 0)
			{
				Console.WriteLine("     Inga kampanjer att uppdatera...\n");
				Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
				Console.ReadKey();

				return;
			}

			Console.Write("\n");

			while (true)
			{
				Console.Write("  Välj ID: ");
				int.TryParse(Console.ReadLine(), out campaineID);

				if (campaineID == 0)
				{
					return;
				}
				if (campaineID > 0)
				{
					break;
				}
			}

			Console.Write("\n");

			while (true)
			{
				string format = "yyyy-MM-dd";

				Console.Write($"  Ange nytt start datum för kampanjen med ({ProductList.FetchProductName(campaineID)}): ");
				string inputDate = Console.ReadLine();

				if (inputDate == "0")
				{
					return;
				}

				bool dateCheck = DateTime.TryParseExact(inputDate, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out startDate);

				if (dateCheck && startDate.Date >= DateTime.Now.Date)
				{
					break;
				}
			}

			while (true)
			{
				Console.Write($"  Ange hur många dagar kampanje ska gälla för ({ProductList.FetchProductName(campaineID)}): ");
				int.TryParse(Console.ReadLine(), out endDate);

				if (endDate == 0)
				{
					return;
				}
				if (endDate > 0)
				{
					break;
				}
			}

			while (true)
			{
				Console.Write("  Ange hur mycket rabbat i kr: ");
				double.TryParse(Console.ReadLine(), out discount);

				if (discount == 0)
				{
					return;
				}
				if (discount > 0 && ProductList.FetchProductPrice(campaineID)! > discount)
				{
					break;
				}
			}

			_campaineList.Where(campaine => campaine.Id == campaineID).ToList().ForEach(campaine =>
			{
				campaine.StartDate = startDate;
				campaine.EndDate = endDate;
				campaine.Discount = discount;
			});
		}
	}

	public void RemoveCampaine()
	{
		while (true)
		{
			int campaineID;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Ta bort en kampanj -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-10}    {3,-10} {4}\n", "Id", "Produktnamn", "Start", "Slut", "Rabatt");


			foreach (var campaine in _campaineList)
			{
				Console.WriteLine($"    {campaine.Id,-3} {ProductList.FetchProductName(campaine.ProductID),-15} {campaine.StartDate.ToString("yyyy-MM-dd")} -> {campaine.StartDate.AddDays(campaine.EndDate).ToString("yyyy-MM-dd")} {campaine.Discount} kr");
			}

			if (_campaineList.Count == 0)
			{
				Console.WriteLine("     Inga kampanjer att ta bort...\n");
				Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
				Console.ReadKey();

				return;
			}

			Console.Write("\n");

			while (true)
			{
				Console.Write("  Välj ID: ");
				int.TryParse(Console.ReadLine(), out campaineID);

				if (campaineID == 0)
				{
					return;
				}
				if (campaineID > 0)
				{
					break;
				}
			}

			_campaineList.Where(campaine => campaine.Id == campaineID).ToList().ForEach(campaine =>
			{
				_campaineList.Remove(campaine);
			});
		}
	}
}