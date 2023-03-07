﻿using KassaSys.Enum;

namespace KassaSys;

public class ShopCampaign
{
	private string _filePath = @".\campaign.txt";
	private string _splitString = " | ";
	private List<CampaignList> _campaignList = new List<CampaignList>();

	Product ProductList = new Product();

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

		foreach (var line in File.ReadLines(_filePath))
		{
			var args = line.Split(_splitString);
			var campaign = new CampaignList();

			campaign.Id = Convert.ToInt32(args[0]);
			campaign.ProductID = Convert.ToInt32(args[1]);
			campaign.StartDate = Convert.ToDateTime(args[2]);
			campaign.EndDate = Convert.ToInt32(args[3]);
			campaign.Discount = Convert.ToDouble(args[4]);

			tempCampaignList.Add(campaign);
		}

		return tempCampaignList;
	}
	public void SaveAllToFile(List<CampaignList> tempCampaignList)
	{
		var stringList = new List<string>();

		foreach (var campaign in tempCampaignList)
		{
			string campaignString = $"{campaign.Id}{_splitString}{campaign.ProductID}{_splitString}{campaign.StartDate}{_splitString}{campaign.EndDate}{_splitString}{campaign.Discount}";
			stringList.Add(campaignString);
		}

		File.WriteAllLines(_filePath, stringList);
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
		int campaignID;
		double discount = 0;
		DateTime startDate;
		int endDate;

		Console.Clear();
		Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
		Console.WriteLine("  - Lägg till en ny kampanj -\n");

		while (true)
		{
			int i = 1;
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

			Console.Write("\n  Välj produkt ID: ");
			int.TryParse(Console.ReadLine(), out campaignID);

			if (campaignID == 0)
			{
				return;
			}
			else if (campaignID > 0)
			{
				break;
			}
		}

		while (true)
		{
			string format = "yyyy-MM-dd";

			Console.Write("\n                    Ange start datum (YYYY-MM-DD): ");
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
			Console.Write("         Ange hur många dagar kampanjen ska gälla: ");
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
			Console.Write("  Ange hur mycket rabatt (avsluta med kr eller %): ");
			string tempDiscount = Console.ReadLine();

			if (tempDiscount == "0")
			{
				return;
			}

			if (tempDiscount.Contains("kr"))
			{
				tempDiscount = tempDiscount.Replace("kr", "").TrimEnd();
				double.TryParse(tempDiscount, out discount);
			}
			else if (tempDiscount.Contains("%"))
			{
				tempDiscount = tempDiscount.Replace("%", "").TrimEnd();
				double.TryParse(tempDiscount, out discount);
				discount /= 100;
				discount = 1 - discount;
			}

			if (discount > 0 && ProductList.FetchProductPrice(campaignID)! > discount)
			{
				break;
			}
		}

		_campaignList.Add(new CampaignList { Id = ((_campaignList.Count > 0) ? _campaignList.Last().Id + 1 : 1), ProductID = campaignID, StartDate = startDate, EndDate = endDate, Discount = discount });

		SaveAllToFile(_campaignList);
	}

	public void UpdateCampaign()
	{
		while (true)
		{
			int campaignID;
			DateTime startDate;
			int endDate;
			double discount = 0;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Uppdatera en kampanj -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-10}    {3,-10} {4}\n", "Id", "Produktnamn", "Start", "Slut", "Rabatt");

			foreach (var campaign in _campaignList)
			{
				Console.WriteLine($"    {campaign.Id,-3} {ProductList.FetchProductName(campaign.ProductID),-15} {campaign.StartDate.ToString("yyyy-MM-dd")} -> {campaign.StartDate.AddDays(campaign.EndDate).ToString("yyyy-MM-dd")} {campaign.Discount} {((campaign.Discount > 0) ? "kr" : "%")}");
			}

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
				int.TryParse(Console.ReadLine(), out campaignID);

				if (campaignID == 0)
				{
					return;
				}
				if (campaignID > 0)
				{
					break;
				}
			}

			Console.Write("\n");

			while (true)
			{
				string format = "yyyy-MM-dd";

				Console.Write($"  Ange nytt start datum för kampanjen med ({ProductList.FetchProductName(campaignID)}): ");
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
				Console.Write($"  Ange hur många dagar kampanjen ska gälla för ({ProductList.FetchProductName(campaignID)}): ");
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
				Console.Write("  Ange hur mycket rabatt (avsluta med kr eller %) ");
				string tempDiscount = Console.ReadLine();

				if (tempDiscount == "0")
				{
					return;
				}

				if (tempDiscount.Contains("kr"))
				{
					tempDiscount = tempDiscount.Replace("kr", "").TrimEnd();
					double.TryParse(tempDiscount, out discount);
				}
				else if (tempDiscount.Contains("%"))
				{
					tempDiscount = tempDiscount.Replace("%", "").TrimEnd();
					double.TryParse(tempDiscount, out discount);
					discount /= 100;
					discount = 1 - discount;
				}

				if (discount > 0 && ProductList.FetchProductPrice(campaignID)! > discount)
				{
					break;
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
			int campaignID;

			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine("  - Ta bort en kampanj -\n");

			Console.WriteLine("    {0,-3} {1,-15} {2,-10}    {3,-10} {4}\n", "Id", "Produktnamn", "Start", "Slut", "Rabatt");


			foreach (var campaign in _campaignList)
			{
				Console.WriteLine($"    {campaign.Id,-3} {ProductList.FetchProductName(campaign.ProductID),-15} {campaign.StartDate.ToString("yyyy-MM-dd")} -> {campaign.StartDate.AddDays(campaign.EndDate).ToString("yyyy-MM-dd")} {campaign.Discount} {((campaign.Discount > 0) ? "kr" : "%")}");
			}

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
				int.TryParse(Console.ReadLine(), out campaignID);

				if (campaignID == 0)
				{
					return;
				}
				if (campaignID > 0)
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