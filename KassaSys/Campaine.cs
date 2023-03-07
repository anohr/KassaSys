using KassaSys.Enum;

namespace KassaSys;

public class ShopCampaine
{
	private string _filePath = @".\campaine.txt";
	private string _splitString = " | ";
	private List<CampaineList> _campaineList = new List<CampaineList>();

	Product ProductList = new Product();

	public ShopCampaine()
	{
		_campaineList = InitializeFromFile();

		/*
		 *
		 *   kolla om det är % eller kr rabatten ska vara i
		 *
		 *   https://stackoverflow.com/questions/3717028/access-list-from-another-class#3717187
		 *
		 */
	}
	public List<CampaineList> InitializeFromFile()
	{
		var tempCampaineList = new List<CampaineList>();

		if (!File.Exists(_filePath))
			return tempCampaineList;

		foreach (var line in File.ReadLines(_filePath))
		{
			var args = line.Split(_splitString);
			var campaine = new CampaineList();

			campaine.Id = Convert.ToInt32(args[0]);
			campaine.ProductID = Convert.ToInt32(args[1]);
			campaine.StartDate = Convert.ToDateTime(args[2]);
			campaine.EndDate = Convert.ToInt32(args[3]);
			campaine.Discount = Convert.ToDouble(args[4]);

			tempCampaineList.Add(campaine);
		}

		return tempCampaineList;
	}
	public void SaveAllToFile(List<CampaineList> tempCampaineList)
	{
		var stringList = new List<string>();

		foreach (var campaine in tempCampaineList)
		{
			string campaineString = $"{campaine.Id}{_splitString}{campaine.ProductID}{_splitString}{campaine.StartDate}{_splitString}{campaine.EndDate}{_splitString}{campaine.Discount}";
			stringList.Add(campaineString);
		}

		File.WriteAllLines(_filePath, stringList);
	}

	public void RemoveCampaineId(int productId)
	{
		_campaineList.Where(campaine => campaine.ProductID == productId).ToList().ForEach(campaine =>
		{
			_campaineList.Remove(campaine);
		});

		SaveAllToFile(_campaineList);
		_campaineList = InitializeFromFile();
	}

	public List<CampaineList> GetList()
	{
		return InitializeFromFile();
	}
	public void AddCampaine()
	{
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
			Console.Write("  Ange hur många dagar kampanjen ska gälla: ");
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

		_campaineList.Add(new CampaineList { Id = ((_campaineList.Count > 0) ? _campaineList.Last().Id + 1 : 1), ProductID = campaineID, StartDate = startDate, EndDate = endDate, Discount = discount });

		SaveAllToFile(_campaineList);
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
				Console.Write($"  Ange hur många dagar kampanjen ska gälla för ({ProductList.FetchProductName(campaineID)}): ");
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

			SaveAllToFile(_campaineList);
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

			SaveAllToFile(_campaineList);
		}
	}
}