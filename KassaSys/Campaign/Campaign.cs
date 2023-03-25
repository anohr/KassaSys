using KassaSys.Product;


namespace KassaSys.Campaign;

public class ShopCampaign : ICampaign
{
	private string _filePath = @".\campaign.txt";
	private string _splitString = " | ";
	public List<CampaignList> _campaignList = new List<CampaignList>();

	public ShopCampaign()
	{
		_campaignList = FetchCampaignFromFile();
	}

	public List<CampaignList> FetchCampaignFromFile()
	{
		var tempCampaignList = new List<CampaignList>();

		if (!File.Exists(_filePath))
		{
			return tempCampaignList;
		}

		tempCampaignList = File.ReadLines(_filePath)
			.Select(line =>
			{
				var args = line.Split(_splitString);
				return new CampaignList
				{
					Id = Convert.ToInt32(args[0]),
					ProductID = Convert.ToInt32(args[1]),
					StartDate = Convert.ToDateTime(args[2]),
					EndDate = Convert.ToInt32(args[3]),
					Discount = args[4]
				};
			})
			.ToList();

		return tempCampaignList;
	}

	public bool CheckIfCampaignExists(int id)
	{
		return _campaignList.Any(campaign => campaign.Id == id);
	}

	public string FetchBestDiscount(int productid, string typeOfDiscount)
	{
		var bestDiscount = _campaignList
								.Where(campaign => campaign.ProductID == productid && campaign.Discount.Contains(typeOfDiscount) && (campaign.StartDate <= DateTime.Now && campaign.StartDate.AddDays(campaign.EndDate) >= DateTime.Now))
								.OrderByDescending(campaign => campaign.Discount.Replace(typeOfDiscount, ""))
								.Select(campaign => campaign.Discount)
								.FirstOrDefault();

		return (bestDiscount != null) ? bestDiscount : "0" + typeOfDiscount;
	}

	public string GetBestDiscount(int productid, ShopProduct ProductList = null)
	{
		// Stefan fix. :)
		if (ProductList == null)
			ProductList = new ShopProduct();

		var bestPercentDiscount = FetchBestDiscount(productid, "%");
		var bestMoneyDiscount = FetchBestDiscount(productid, "kr");

		var ProduPrice = ProductList.FetchProductPrice(productid);

		var bestPercent = (ProduPrice * Math.Round((1 - (double.Parse(bestPercentDiscount.Replace("%", "")) / 100)), 2));
		var bestMoney = (ProduPrice - double.Parse(bestMoneyDiscount.Replace("kr", "")));

		return (bestMoney < bestPercent) ? bestMoneyDiscount : bestPercentDiscount;
	}

	public void SaveAllToFile(List<CampaignList> tempCampaignList)
	{
		var stringList = tempCampaignList.Select(campaign => $"{campaign.Id}{_splitString}{campaign.ProductID}{_splitString}{campaign.StartDate}{_splitString}{campaign.EndDate}{_splitString}{campaign.Discount}").ToList();

		File.WriteAllLines(_filePath, stringList);

		_campaignList = FetchCampaignFromFile();
	}

	public void RemoveCampaignId(int productId)
	{
		_campaignList.Where(campaign => campaign.ProductID == productId).ToList().ForEach(campaign =>
		{
			_campaignList.Remove(campaign);
		});

		SaveAllToFile(_campaignList);

		_campaignList = FetchCampaignFromFile();
	}

	public List<CampaignList> GetList()
	{
		return FetchCampaignFromFile();
	}

	public void AddCampaign()
	{
		ShopProduct ProductList = new ShopProduct();

		int productID;
		string discount = "";
		DateTime startDate;
		int endDate;
		string tempDiscount = "";

		Console.Clear();
		Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
		Console.WriteLine("  - Lägg till en ny kampanj -\n");

		int i = 1;
		Console.WriteLine("    {0,-3} {1,-20} {2,-9}\n", "Id", "Produkt namn", "Pris");

		foreach (var product in ProductList.GetList())
		{
			Console.WriteLine($"    {product.Id,-3} {product.Name,-20} {product.Price:F2} kr");
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

		if (ProductList.GetList().Count == 0)
		{
			Console.WriteLine("     Inga produkter att lägga kampanj på...\n");
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

		while (true)
		{
			string format = "yyyy-MM-dd";

			Console.Write("\n  Ange start datum (YYYY-MM-DD): ");
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
			bool check = int.TryParse(Console.ReadLine(), out endDate);

			if (check && endDate == 0)
			{
				return;
			}
			if (check && endDate > 0)
			{
				break;
			}
		}

		while (true)
		{
			Console.Write("  Ange hur mycket rabatt (avsluta med kr eller %): ");
			tempDiscount = Console.ReadLine();

			if (tempDiscount == "0")
			{
				return;
			}

			double checkMaxPrice = ProductList.FetchProductPrice(productID);

			if (tempDiscount.Length > 1 && tempDiscount.Contains("kr") || tempDiscount.Contains('%'))
			{
				if (tempDiscount.Contains("kr"))
				{
					tempDiscount = tempDiscount.Replace("kr", "");
					if (double.Parse(tempDiscount) < checkMaxPrice)
					{
						discount = tempDiscount + "kr";
						break;
					}
				}
				if (tempDiscount.Contains("%"))
				{
					tempDiscount = tempDiscount.Replace("%", "");
					if (double.Parse(tempDiscount) < 100)
					{
						discount = tempDiscount + "%";
						break;
					}
				}
			}
		}

		_campaignList.Add(new CampaignList { Id = _campaignList.Count > 0 ? _campaignList.Last().Id + 1 : 1, ProductID = productID, StartDate = startDate, EndDate = endDate, Discount = discount });

		SaveAllToFile(_campaignList);
	}

	public void UpdateCampaign()
	{
		while (true)
		{
			ShopProduct ProductList = new ShopProduct();

			int campaignID;
			DateTime startDate;
			int endDate;
			string discount = "0";

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Uppdatera en kampanj -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-15} {3,-15} {4}\n", "Id", "Produktnamn", "Start", "Slut", "Rabatt");

			int i = 1;
			foreach (var campaign in _campaignList)
			{
				Console.WriteLine($"    {campaign.Id,-3} {ProductList.FetchProductName(campaign.ProductID),-15} {campaign.StartDate.ToString("yyyy-MM-dd"),-15} {campaign.StartDate.AddDays(campaign.EndDate).ToString("yyyy-MM-dd"),-15} {(campaign.Discount)}");

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

			if (_campaignList.Count == 0)
			{
				Console.WriteLine("     Inga kampanjer att uppdatera...\n");
				Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
				Console.ReadKey();

				return;
			}

			Console.Write("\n");

			while (true)
			{
				Console.Write("  Välj kampanj ID: ");
				bool check = int.TryParse(Console.ReadLine(), out campaignID);

				if (check && campaignID == 0)
				{
					return;
				}
				if (check && campaignID > 0 && CheckIfCampaignExists(campaignID))
				{
					break;
				}
			}

			Console.Write("\n");

			while (true)
			{
				string format = "yyyy-MM-dd";

				Console.Write($"  Ange nytt start datum för kampanjen med (ID: {campaignID}) : ");
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
				Console.Write($"  Ange hur många dagar kampanjen ska gälla för (ID: {campaignID}) : ");
				bool check = int.TryParse(Console.ReadLine(), out endDate);

				if (check && endDate == 0)
				{
					return;
				}
				if (check && endDate > 0)
				{
					break;
				}
			}

			while (true)
			{
				Console.Write("  Ange hur mycket rabatt (avsluta med kr eller %) ");
				string tempDiscount = Console.ReadLine();

				if (tempDiscount == "0")
				{
					return;
				}

				int productId = _campaignList.Where(campaign => campaign.Id == campaignID).Select(campaign => campaign.ProductID).FirstOrDefault();

				double checkMaxPrice = ProductList.FetchProductPrice(productId);

				if (tempDiscount.Length > 1 && tempDiscount.Contains("kr") || tempDiscount.Contains('%'))
				{
					if (tempDiscount.Contains("kr"))
					{
						tempDiscount = tempDiscount.Replace("kr", "");
						if (double.Parse(tempDiscount) < checkMaxPrice)
						{
							discount = tempDiscount + "kr";
							break;
						}
					}
					if (tempDiscount.Contains("%"))
					{
						tempDiscount = tempDiscount.Replace("%", "");
						if (double.Parse(tempDiscount) < 100)
						{
							discount = tempDiscount + "%";
							break;
						}
					}
				}
			}

			_campaignList.Where(campaign => campaign.Id == campaignID).ToList().ForEach(campaign =>
			{
				campaign.StartDate = startDate;
				campaign.EndDate = endDate;
				campaign.Discount = discount;
			});

			SaveAllToFile(_campaignList);
		}
	}

	public void RemoveCampaign()
	{
		while (true)
		{
			ShopProduct ProductList = new ShopProduct();

			int campaignID;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Ta bort en kampanj -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-15} {3,-15} {4}\n", "Id", "Produktnamn", "Start datum", "Slut datum", "Rabatt");

			int i = 1;
			foreach (var campaign in _campaignList)
			{
				Console.WriteLine($"    {campaign.Id,-3} {ProductList.FetchProductName(campaign.ProductID),-15} {campaign.StartDate.ToString("yyyy-MM-dd"),-15} {campaign.StartDate.AddDays(campaign.EndDate).ToString("yyyy-MM-dd"),-15} {campaign.Discount}");

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

			if (_campaignList.Count == 0)
			{
				Console.WriteLine("     Inga kampanjer att ta bort...\n");
				Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
				Console.ReadKey();

				return;
			}

			Console.Write("\n");

			while (true)
			{
				Console.Write("  Välj kampanj ID: ");
				bool check = int.TryParse(Console.ReadLine(), out campaignID);

				if (check && campaignID == 0)
				{
					return;
				}
				if (check && campaignID > 0)
				{
					break;
				}
			}

			_campaignList.Where(campaign => campaign.Id == campaignID).ToList().ForEach(campaign =>
			{
				_campaignList.Remove(campaign);
			});

			SaveAllToFile(_campaignList);
		}
	}
}