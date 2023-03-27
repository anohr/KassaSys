﻿using KassaSys.Product;
using Microsoft.VisualBasic;


namespace KassaSys.Campaign;

public class ShopCampaign : ICampaign
{
    private string _filePath = @".\campaign.txt";
    private string _splitString = " | ";

    public List<CampaignList> campaignList = new List<CampaignList>();

    public ShopCampaign()
    {
        campaignList = FetchCampaignFromFile();
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

    public List<CampaignList> FetchCampaignList()
    {
        return FetchCampaignFromFile();
    }

    public bool CheckIfCampaignExists(int campaignId)
    {
        return campaignList.Any(campaign => campaign.Id == campaignId);
    }
    public int FetchProductIdFromCampaign(int campaignId)
    {
        return campaignList.Where(campaign => campaign.Id == campaignId).Select(campaign => campaign.ProductID).FirstOrDefault();
    }

    public string FetchBestDiscountForEachType(int productId, string typeOfDiscount)
    {
        var bestDiscount = campaignList
                                .Where(campaign => campaign.ProductID == productId && campaign.Discount.Contains(typeOfDiscount) && (campaign.StartDate <= DateTime.Now && campaign.StartDate.AddDays(campaign.EndDate) >= DateTime.Now))
                                .OrderByDescending(campaign => campaign.Discount.Replace(typeOfDiscount, ""))
                                .Select(campaign => campaign.Discount)
                                .SingleOrDefault();

        return bestDiscount ?? "0" + typeOfDiscount;
    }

    public string CalculateBestDiscount(int productId, ShopProduct ProductList = null)
    {
        // Stefan fix. :)
        if (ProductList == null)
            ProductList = new ShopProduct();

        var bestPercentDiscount = FetchBestDiscountForEachType(productId, "%");
        var bestMoneyDiscount = FetchBestDiscountForEachType(productId, "kr");

        var produPrice = ProductList.FetchProductPrice(productId);

        var bestPercentAfterDiscount = (produPrice * Math.Round((1 - (double.Parse(bestPercentDiscount.Replace("%", "")) / 100)), 2));
        var bestMoneyAfterDiscount = (produPrice - double.Parse(bestMoneyDiscount.Replace("kr", "")));

        return (bestMoneyAfterDiscount < bestPercentAfterDiscount) ? bestMoneyDiscount : bestPercentDiscount;
    }

    public void SaveToFile(List<CampaignList> tempCampaignList)
    {
        var stringList = tempCampaignList.Select(campaign => $"{campaign.Id}{_splitString}{campaign.ProductID}{_splitString}{campaign.StartDate}{_splitString}{campaign.EndDate}{_splitString}{campaign.Discount}").ToList();

        File.WriteAllLines(_filePath, stringList);

        campaignList = FetchCampaignFromFile();
    }

    public void RemoveCampaign(int productId)
    {
        campaignList.Where(campaign => campaign.ProductID == productId).ToList().ForEach(campaign =>
        {
            campaignList.Remove(campaign);
        });

        SaveToFile(campaignList);

        campaignList = FetchCampaignFromFile();
    }

    public void AddCampaign()
    {
        ShopProduct productList = new ShopProduct();

        string tempInput = string.Empty;
        string dateFormat = "yyyy-MM-dd";
        int productId;
        string discount = "";
        DateTime startDate;
        int endDate;

        Console.Clear();
        Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
        Console.WriteLine("  - Lägg till en ny kampanj -\n");

        Console.WriteLine("    {0,-3} {1,-20} {2,-9}\n", "Id", "Produkt namn", "Pris");

        for (int i = 0; i < productList.FetchList().Count; i++)
        {
            var product = productList.FetchList()[i];

            if (i % 2 == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }

            Console.WriteLine($"    {product.Id,-3} {product.Name,-20} {product.Price:F2} kr");

            Console.ResetColor();
        }

        Console.ResetColor();

        if (productList.FetchList().Count == 0)
        {
            Console.WriteLine("     Inga produkter att lägga kampanj på...\n");
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

                if (int.TryParse(tempInput, out productId) && productId > 0 && productList.CheckIfProductExists(productId))
                {
                    break;
                }
            }

            Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
        }

        while (true)
        {
            tempInput = Program.AskForInput($"  Ange start datum ({DateTime.Now.ToString("yyyy-MM-dd")}): ");

            if (!string.IsNullOrWhiteSpace(tempInput))
            {
                if (tempInput == "0")
                {
                    return;
                }

                if (DateTime.TryParseExact(tempInput, dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out startDate) && startDate.Date >= DateTime.Now.Date)
                {
                    break;
                }
            }
            else if (string.IsNullOrEmpty(tempInput))
            {
                startDate = DateTime.Now;
                break;
            }

            Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
        }

        while (true)
        {
            tempInput = Program.AskForInput("  Ange hur många dagar kampanjen ska gälla: ");

            if (!string.IsNullOrWhiteSpace(tempInput))
            {
                if (tempInput == "0")
                {
                    return;
                }

                if (int.TryParse(tempInput, out endDate) && endDate > 0)
                {
                    break;
                }
            }

            Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
        }

        while (true)
        {
            tempInput = Program.AskForInput("  Ange hur mycket rabatt (avsluta med kr eller %): ");

            if (!string.IsNullOrWhiteSpace(tempInput))
            {
                if (tempInput == "0")
                {
                    return;
                }

                double productPrice = productList.FetchProductPrice(productId);

                if (tempInput.EndsWith("kr") && double.TryParse(tempInput.Replace("kr", ""), out double discountMoney))
                {
                    if (discountMoney < productPrice)
                    {
                        discount = $"{discountMoney}kr";
                        break;
                    }
                }
                else if (tempInput.EndsWith("%") && double.TryParse(tempInput.Replace("%", ""), out double discountPercent))
                {
                    if (discountPercent < productPrice)
                    {
                        discount = $"{discountPercent}%";
                        break;
                    }
                }
            }

            Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
        }

        campaignList.Add(new CampaignList
        {
            Id = campaignList.Count > 0 ? campaignList.Last().Id + 1 : 1,
            ProductID = productId,
            StartDate = startDate,
            EndDate = endDate,
            Discount = discount
        });

        SaveToFile(campaignList);
    }

    public void UpdateCampaign()
    {
        while (true)
        {
            ShopProduct productList = new ShopProduct();

            string tempInput = string.Empty;
            string dateFormat = "yyyy-MM-dd";
            int campaignId;
            DateTime startDate;
            int endDate;
            string discount = "0";

            Console.Clear();
            Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
            Console.WriteLine("  - Uppdatera en kampanj -\n");

            Console.WriteLine("    {0,-3} {1,-15} {2,-15} {3,-15} {4}\n", "Id", "Produktnamn", "Start", "Slut", "Rabatt");

            for (int i = 0; i < campaignList.Count; i++)
            {
                var campaign = campaignList[i];

                if (i % 2 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                Console.WriteLine($"    {campaign.Id,-3} {productList.FetchProductName(campaign.ProductID),-15} {campaign.StartDate.ToString("yyyy-MM-dd"),-15} {campaign.StartDate.AddDays(campaign.EndDate).ToString("yyyy-MM-dd"),-15} {(campaign.Discount)}");

                Console.ResetColor();
            }

            Console.ResetColor();

            if (campaignList.Count == 0)
            {
                Console.WriteLine("     Inga kampanjer att uppdatera...\n");
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

                    if (int.TryParse(tempInput, out campaignId) && campaignId > 0 && CheckIfCampaignExists(campaignId))
                    {
                        break;
                    }
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            Console.Write("\n");

            while (true)
            {
                tempInput = Program.AskForInput($"  Ange nytt start datum för kampanjen ({DateTime.Now.ToString("yyyy-MM-dd")}): ");

                if (!string.IsNullOrWhiteSpace(tempInput))
                {
                    if (tempInput == "0")
                    {
                        return;
                    }

                    if (DateTime.TryParseExact(tempInput, dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out startDate) && startDate.Date >= DateTime.Now.Date)
                    {
                        break;
                    }
                }
                else if (string.IsNullOrEmpty(tempInput))
                {
                    startDate = DateTime.Now;
                    break;
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            while (true)
            {
                tempInput = Program.AskForInput($"  Ange hur många dagar kampanjen ska gälla : ");

                if (!string.IsNullOrWhiteSpace(tempInput))
                {
                    if (tempInput == "0")
                    {
                        return;
                    }

                    if (int.TryParse(tempInput, out endDate) && endDate > 0)
                    {
                        break;
                    }
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            while (true)
            {
                tempInput = Program.AskForInput("  Ange hur mycket rabatt (avsluta med kr eller %): ");

                if (!string.IsNullOrWhiteSpace(tempInput))
                {
                    if (tempInput == "0")
                    {
                        return;
                    }

                    int productId = FetchProductIdFromCampaign(campaignId);
                    double productPrice = productList.FetchProductPrice(productId);

                    if (tempInput.EndsWith("kr") && double.TryParse(tempInput.Replace("kr", ""), out double discountMoney))
                    {
                        if (discountMoney < productPrice)
                        {
                            discount = $"{discountMoney}kr";
                            break;
                        }
                    }
                    else if (tempInput.EndsWith("%") && double.TryParse(tempInput.Replace("%", ""), out double discountPercent))
                    {
                        if (discountPercent < productPrice)
                        {
                            discount = $"{discountPercent}%";
                            break;
                        }
                    }
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            campaignList.Where(campaign => campaign.Id == campaignId).ToList().ForEach(campaign =>
            {
                campaign.StartDate = startDate;
                campaign.EndDate = endDate;
                campaign.Discount = discount;
            });

            SaveToFile(campaignList);
        }
    }

    public void RemoveCampaign()
    {
        while (true)
        {
            ShopProduct ProductList = new ShopProduct();

            string tempInput = string.Empty;
            int campaignId;

            Console.Clear();
            Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
            Console.WriteLine("  - Ta bort en kampanj -\n");

            Console.WriteLine("    {0,-3} {1,-15} {2,-15} {3,-15} {4}\n", "Id", "Produktnamn", "Start datum", "Slut datum", "Rabatt");

            for (int i = 0; i < campaignList.Count; i++)
            {
                var campaign = campaignList[i];

                if (i % 2 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                Console.WriteLine($"    {campaign.Id,-3} {ProductList.FetchProductName(campaign.ProductID),-15} {campaign.StartDate.ToString("yyyy-MM-dd"),-15} {campaign.StartDate.AddDays(campaign.EndDate).ToString("yyyy-MM-dd"),-15} {(campaign.Discount)}");

                Console.ResetColor();
            }

            Console.ResetColor();

            if (campaignList.Count == 0)
            {
                Console.WriteLine("     Inga kampanjer att ta bort...\n");
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

                    if (int.TryParse(tempInput, out campaignId) && campaignId > 0 && CheckIfCampaignExists(campaignId))
                    {
                        break;
                    }
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            campaignList.Where(campaign => campaign.Id == campaignId).ToList().ForEach(campaign =>
            {
                campaignList.Remove(campaign);
            });

            SaveToFile(campaignList);
        }
    }
}