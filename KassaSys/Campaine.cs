using KassaSys.Enum;

namespace KassaSys;

public class ShopCampaine
{
	private List<CampaineList> campaineList = new List<CampaineList>();

	Product productList = new Product();

	public ShopCampaine()
	{
		/*
		 *
		 *   SPARA TILL FIL !!!!
		 *   ta bort produkt, kampanj etc där saker överlappar
		 *   skriv ut i kvittot att det är rabatt
		 *
		 */
		campaineList.Add(new CampaineList { Id = 1, ProductID = 1, StartDate = DateTime.Now, EndDate = 10, Discount = 7 });
		campaineList.Add(new CampaineList { Id = 2, ProductID = 2, StartDate = DateTime.Now, EndDate = 3, Discount = 1 });
		campaineList.Add(new CampaineList { Id = 3, ProductID = 7, StartDate = DateTime.Now, EndDate = 10, Discount = 3 });
	}
	public List<CampaineList> GetList()
	{
		return campaineList;
	}

	public double FetchCampaineDiscount(int Id)
	{
		var discounted = campaineList.OrderBy(discount => discount.StartDate).Where(discount => discount.ProductID == Id).Select(discount => discount.Discount).FirstOrDefault();

		Console.WriteLine(discounted);

		return 0.0;
	}

	public void AddCampaine()
	{
		int id = (campaineList.Count > 0) ? campaineList.Last().Id + 1 : 1;
		int campaineId = 0;
		double discount = 0;
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
			foreach (var product in productList.GetList())
			{
				Console.WriteLine($"    {product.Id,-3} {product.Name,-20} {product.Price:F2}");
				i++;
			}
			Console.Write("\n  Välj ID: ");
			int.TryParse(Console.ReadLine(), out campaineId);

			if (campaineId == 0)
			{
				return;
			}
			else if (campaineId > 0)
			{
				break;
			}
		}

		while (true)
		{
			Console.Write("  Ange start datum (YYYY-MM-DD): ");
			string inputDate = Console.ReadLine();

			string format = "yyyy-MM-dd";

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

			if (discount > 0 && productList.FetchProductPrice(campaineId)! > discount)
			{
				break;
			}
		}

		campaineList.Add(new CampaineList { Id = id, ProductID = campaineId, StartDate = startDate, EndDate = endDate, Discount = discount });
		;
	}

	public void UpdateCampaine()
	{
		while (true)
		{
			int pID = 0;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Uppdatera en kampanj -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-10}    {3,-10} {4}\n", "Id", "Produktnamn", "Start", "Slut", "Rabatt");

			if (campaineList.Count > 0)
			{
				foreach (var campaine in campaineList)
				{
					Console.WriteLine($"    {campaine.Id,-3} {productList.FetchProductName(campaine.ProductID),-15} {campaine.StartDate.ToString("yyyy-MM-dd")} -> {campaine.StartDate.AddDays(campaine.EndDate).ToString("yyyy-MM-dd")} {campaine.Discount} kr");
				}
			}
			else
			{
				Console.WriteLine("    Inga kampanjer i registret...");
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

			DateTime startDate;

			while (true)
			{
				Console.Write($"  Ange nytt start datum för kampanjen med ({productList.FetchProductName(pID)}): ");
				string inputDate = Console.ReadLine();

				string format = "yyyy-MM-dd";

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

			int endDate;

			while (true)
			{
				Console.Write($"  Ange hur många dagar kampanje ska gälla för ({productList.FetchProductName(pID)}): ");
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

			double discount;

			while (true)
			{
				Console.Write("  Ange hur mycket rabbat i kr: ");
				double.TryParse(Console.ReadLine(), out discount);

				if (discount == 0)
				{
					return;
				}

				if (discount > 0 && productList.FetchProductPrice(pID)! > discount)
				{
					break;
				}
			}

			campaineList.Where(campaine => campaine.Id == pID).ToList().ForEach(campaine =>
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
			int pID = 0;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Ta bort en kampanj -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-10}    {3,-10} {4}\n", "Id", "Produktnamn", "Start", "Slut", "Rabatt");

			if (campaineList.Count > 0)
			{
				foreach (var campaine in campaineList)
				{
					Console.WriteLine($"    {campaine.Id,-3} {productList.FetchProductName(campaine.ProductID),-15} {campaine.StartDate.ToString("yyyy-MM-dd")} -> {campaine.StartDate.AddDays(campaine.EndDate).ToString("yyyy-MM-dd")} {campaine.Discount} kr");
				}
			}
			else
			{
				Console.WriteLine("    Inga kampanjer i registret...");
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

			campaineList.Where(campaine => campaine.Id == pID).ToList().ForEach(campaine =>
			{
				campaineList.Remove(campaine);
			});
		}
	}
}