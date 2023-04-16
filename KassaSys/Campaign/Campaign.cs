using KassaSys.Services;

namespace KassaSys.Campaign;

public class Campaign : ICampaign
{
    private IFileService _fileService = new FileService();
    private string _folder = "campaign";
    private string _filePath = "campaign.txt";
    private string _splitString = " | ";

    public List<CampaignList> campaignList = new List<CampaignList>();

    public Campaign()
    {
        campaignList = FetchCampaignFromFile();
    }

    public List<CampaignList> FetchCampaignFromFile()
    {
        return _fileService.ReadCampaignFile(_folder, _filePath);
    }

    public bool CheckIfCampaignExists(int campaignId)
    {
        return campaignList.Any(campaign => campaign.Id == campaignId);
    }

    public int FetchProductIdFromCampaign(int campaignId)
    {
        return campaignList.Where(campaign => campaign.Id == campaignId).Select(campaign => campaign.ProductID).FirstOrDefault();
    }

    public int FetchCampaignEndDay(int campaignId)
    {
        return campaignList.Where(campaign => campaign.Id == campaignId).Select(campaign => campaign.EndDate).FirstOrDefault();
    }

    public DateTime FetchCampaignStartDate(int campaignId)
    {
        return campaignList.Where(campaign => campaign.Id == campaignId).Select(campaign => campaign.StartDate).FirstOrDefault();
    }

    public string FetchCampaignDiscount(int campaignId)
    {
        return campaignList.Where(campaign => campaign.Id == campaignId).Select(campaign => campaign.Discount).FirstOrDefault();
    }

    public string FetchBestDiscountForEachType(int productId, string typeOfDiscount)
    {
        var bestDiscount = campaignList
                                .Where(campaign => campaign.ProductID == productId && campaign.Discount.Contains(typeOfDiscount) && (campaign.StartDate <= DateTime.Now && campaign.StartDate.AddDays(campaign.EndDate) >= DateTime.Now))
                                .OrderByDescending(campaign => campaign.Discount.Replace(typeOfDiscount, ""))
                                .Select(campaign => campaign.Discount)
                                .FirstOrDefault();

        return bestDiscount ?? "0" + typeOfDiscount;
    }

    public string CalculateBestDiscount(int productId, Product.Product ProductList = null)
    {
        // Stefan fix. :)
        if (ProductList == null)
            ProductList = new Product.Product();

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

        _fileService.SaveListToFile(_folder, _filePath, stringList);
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

    #region AddCampaign

    public void AddCampaign()
    {
        Product.Product productList = new Product.Product();

        string dateFormat = "yyyy-MM-dd";

        int productId;
        DateTime campaignStartDate;
        int campaignEndDate;
        string campaignDiscount;

        string inputProductId;
        string inputCampaignStartDate;
        string inputCampaignEndDays;
        string inputCampaignDiscount;

        Console.Clear();
        Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
        Console.WriteLine("  - Lägg till en ny kampanj -\n");

        Console.WriteLine($"    {"Id",-3} {"Produkt namn",-20} {"Pris",-9}\n");

        if (productList.FetchProductFromFile().Count == 0)
        {
            Console.WriteLine("     Inga produkter att lägga kampanj på...\n");
            Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
            Console.ReadKey();

            return;
        }
        else
        {
            for (int i = 0; i < productList.FetchProductFromFile().Count; i++)
            {
                var product = productList.FetchProductFromFile()[i];

                if (i % 2 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                Console.WriteLine($"    {product.Id,-3} {product.Name,-20} {product.Price:F2} kr");

                Console.ResetColor();
            }
        }

        Console.Write("\n");

        while (true)
        {
            inputProductId = Program.AskForInput("  Välj produkt Id: ");

            if (!string.IsNullOrWhiteSpace(inputProductId))
            {
                if (string.Compare(inputProductId, "0", StringComparison.Ordinal) == 0)
                {
                    return;
                }

                if (int.TryParse(inputProductId, out productId) && productId > 0 && productList.CheckIfProductExists(productId))
                {
                    break;
                }
            }

            Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
        }

        Console.Write("\n");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("    Tryck enter för att acceptera innehållet inom ()\n");
        Console.ResetColor();

        while (true)
        {
            inputCampaignStartDate = Program.AskForInput($"    Ange start datum ({DateTime.Now.ToString("yyyy-MM-dd")}): ");

            if (!string.IsNullOrWhiteSpace(inputCampaignStartDate))
            {
                if (string.Compare(inputCampaignStartDate, "0", StringComparison.Ordinal) == 0)
                {
                    return;
                }

                if (DateTime.TryParseExact(inputCampaignStartDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out campaignStartDate) && campaignStartDate.Date >= DateTime.Now.Date)
                {
                    break;
                }
            }
            else if (string.IsNullOrEmpty(inputCampaignStartDate))
            {
                campaignStartDate = DateTime.Now;
                break;
            }

            Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
        }

        while (true)
        {
            inputCampaignEndDays = Program.AskForInput("    Ange hur många dagar kampanjen ska gälla: ");

            if (!string.IsNullOrWhiteSpace(inputCampaignEndDays))
            {
                if (string.Compare(inputCampaignEndDays, "0", StringComparison.Ordinal) == 0)
                {
                    return;
                }

                if (int.TryParse(inputCampaignEndDays, out campaignEndDate) && campaignEndDate > 0)
                {
                    break;
                }
            }

            Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
        }

        while (true)
        {
            Random discountRandom = new Random();
            var moneyRandom = discountRandom.Next(1, Convert.ToInt32(productList.FetchProductPrice(productId)));
            var percentRandom = discountRandom.Next(1, 100);

            inputCampaignDiscount = Program.AskForInput($"    Ange hur mycket rabatt [ex {moneyRandom}kr eller {percentRandom}%]: ");

            if (!string.IsNullOrWhiteSpace(inputCampaignDiscount))
            {
                if (string.Compare(inputCampaignDiscount, "0", StringComparison.Ordinal) == 0)
                {
                    return;
                }

                inputCampaignDiscount = inputCampaignDiscount.Replace('.', ',');

                double productPrice = productList.FetchProductPrice(productId);

                if (inputCampaignDiscount.EndsWith("kr") && double.TryParse(inputCampaignDiscount.Replace("kr", ""), out double campaignDiscountMoney))
                {
                    if (campaignDiscountMoney < productPrice)
                    {
                        campaignDiscountMoney = Math.Round(campaignDiscountMoney, 2);

                        campaignDiscount = $"{campaignDiscountMoney}kr";
                        break;
                    }
                }
                else if (inputCampaignDiscount.EndsWith("%") && double.TryParse(inputCampaignDiscount.Replace("%", ""), out double campaignDiscountPercent))
                {
                    if (campaignDiscountPercent is > 0 and < 100)
                    {
                        campaignDiscount = $"{campaignDiscountPercent}%";
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
            StartDate = campaignStartDate,
            EndDate = campaignEndDate,
            Discount = campaignDiscount
        });

        SaveToFile(campaignList);
    }

    #endregion AddCampaign

    #region UpdateCampaign

    public void UpdateCampaign()
    {
        while (true)
        {
            Product.Product ProductList = new Product.Product();
            campaignList = FetchCampaignFromFile();

            string dateFormat = "yyyy-MM-dd";

            int campaignId;
            DateTime campaignStartDate;
            int campaignEndDays;
            string campaignDiscount;

            string inputCampaignId;
            string inputCampaignStartDate;
            string inputCampaignEndDays;
            string inputCampaignDiscount;

            Console.Clear();
            Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
            Console.WriteLine("  - Uppdatera en kampanj -\n");

            Console.WriteLine($"    {"Id",-3} {"Produktnamn",-15} {"Start",-15} {"Slut",-15} {"Pris",-15} {"Rabatt"}\n");

            if (campaignList.Count == 0)
            {
                Console.WriteLine("     Inga kampanjer att uppdatera...\n");
                Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
                Console.ReadKey();

                return;
            }
            else
            {
                for (int i = 0; i < campaignList.Count; i++)
                {
                    var campaign = campaignList[i];

                    if (i % 2 == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }

                    var productId = FetchProductIdFromCampaign(campaign.Id);

                    Console.WriteLine($"    {campaign.Id,-3} {ProductList.FetchProductName(campaign.ProductID),-15} {campaign.StartDate.ToString("yyyy-MM-dd"),-15} {campaign.StartDate.AddDays(campaign.EndDate).ToString("yyyy-MM-dd"),-15} {ProductList.FetchProductPrice(productId),-15:C} {(campaign.Discount)}");

                    Console.ResetColor();
                }
            }

            Console.Write("\n");

            while (true)
            {
                inputCampaignId = Program.AskForInput("    Välj kampanj Id: ");

                if (!string.IsNullOrWhiteSpace(inputCampaignId))
                {
                    if (string.Compare(inputCampaignId, "0", StringComparison.Ordinal) == 0)
                    {
                        return;
                    }

                    if (int.TryParse(inputCampaignId, out campaignId) && campaignId > 0 && CheckIfCampaignExists(campaignId))
                    {
                        break;
                    }
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            Console.Write("\n");

            while (true)
            {
                inputCampaignStartDate = Program.AskForInput($"    Ange nytt start datum för kampanjen ({FetchCampaignStartDate(campaignId).ToString("yyyy-MM-dd")}): ");

                if (!string.IsNullOrWhiteSpace(inputCampaignStartDate))
                {
                    if (string.Compare(inputCampaignStartDate, "0", StringComparison.Ordinal) == 0)
                    {
                        return;
                    }

                    if (DateTime.TryParseExact(inputCampaignStartDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out campaignStartDate) && campaignStartDate.Date >= DateTime.Now.Date)
                    {
                        break;
                    }
                }
                else if (string.IsNullOrEmpty(inputCampaignStartDate))
                {
                    campaignStartDate = FetchCampaignStartDate(campaignId);
                    break;
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            while (true)
            {
                inputCampaignEndDays = Program.AskForInput($"    Ange hur många dagar kampanjen ska gälla ({FetchCampaignEndDay(campaignId)}): ");

                if (!string.IsNullOrWhiteSpace(inputCampaignEndDays))
                {
                    if (string.Compare(inputCampaignEndDays, "0", StringComparison.Ordinal) == 0)
                    {
                        return;
                    }

                    if (int.TryParse(inputCampaignEndDays, out campaignEndDays) && campaignEndDays > 0)
                    {
                        break;
                    }
                }
                else if (string.IsNullOrEmpty(inputCampaignEndDays))
                {
                    campaignEndDays = FetchCampaignEndDay(campaignId);
                    break;
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            while (true)
            {
                Random discountRandom = new Random();
                var moneyRandom = discountRandom.Next(1, Convert.ToInt32(ProductList.FetchProductPrice(FetchProductIdFromCampaign(campaignId))));
                var percentRandom = discountRandom.Next(1, 100);

                inputCampaignDiscount = Program.AskForInput($"    Ange hur mycket rabatt [ex {moneyRandom},28kr eller {percentRandom}%] ({FetchCampaignDiscount(campaignId)}): ");

                if (!string.IsNullOrWhiteSpace(inputCampaignDiscount))
                {
                    if (string.Compare(inputCampaignDiscount, "0", StringComparison.Ordinal) == 0)
                    {
                        return;
                    }

                    inputCampaignDiscount = inputCampaignDiscount.Replace('.', ',');

                    int productId = FetchProductIdFromCampaign(campaignId);
                    double productPrice = ProductList.FetchProductPrice(productId);

                    if (inputCampaignDiscount.EndsWith("kr") && double.TryParse(inputCampaignDiscount.Replace("kr", ""), out double campaignDiscountMoney))
                    {
                        if (campaignDiscountMoney < productPrice)
                        {
                            campaignDiscountMoney = Math.Round(campaignDiscountMoney, 2);

                            campaignDiscount = $"{campaignDiscountMoney}kr";
                            break;
                        }
                    }
                    else if (inputCampaignDiscount.EndsWith("%") && double.TryParse(inputCampaignDiscount.Replace("%", ""), out double campaignDiscountPercent))
                    {
                        if (campaignDiscountPercent is > 0 and < 100)
                        {
                            campaignDiscount = $"{campaignDiscountPercent}%";
                            break;
                        }
                    }
                }
                else if (string.IsNullOrEmpty(inputCampaignDiscount))
                {
                    campaignDiscount = FetchCampaignDiscount(campaignId);
                    break;
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            campaignList.Where(campaign => campaign.Id == campaignId).ToList().ForEach(campaign =>
            {
                campaign.StartDate = campaignStartDate;
                campaign.EndDate = campaignEndDays;
                campaign.Discount = campaignDiscount;
            });

            SaveToFile(campaignList);
        }
    }

    #endregion UpdateCampaign

    #region RemoveCampaign

    public void RemoveCampaign()
    {
        while (true)
        {
            Product.Product ProductList = new Product.Product();
            campaignList = FetchCampaignFromFile();

            int campaignId;

            string inputCampaignId;

            Console.Clear();
            Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
            Console.WriteLine("  - Ta bort en kampanj -\n");

            Console.WriteLine($"    {"Id",-3} {"Produktnamn",-15} {"Start",-15} {"Slut",-15} {"Pris",-15} {"Rabatt"}\n");

            if (campaignList.Count == 0)
            {
                Console.WriteLine("     Inga kampanjer att ta bort...\n");
                Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
                Console.ReadKey();

                return;
            }
            else
            {
                for (int i = 0; i < campaignList.Count; i++)
                {
                    var campaign = campaignList[i];

                    if (i % 2 == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }

                    var productId = FetchProductIdFromCampaign(campaign.Id);

                    Console.WriteLine($"    {campaign.Id,-3} {ProductList.FetchProductName(campaign.ProductID),-15} {campaign.StartDate.ToString("yyyy-MM-dd"),-15} {campaign.StartDate.AddDays(campaign.EndDate).ToString("yyyy-MM-dd"),-15} {ProductList.FetchProductPrice(productId),-15:C} {(campaign.Discount)}");

                    Console.ResetColor();
                }
            }

            Console.Write("\n");

            while (true)
            {
                inputCampaignId = Program.AskForInput("    Välj kampajn Id: ");

                if (!string.IsNullOrWhiteSpace(inputCampaignId))
                {
                    if (string.Compare(inputCampaignId, "0", StringComparison.Ordinal) == 0)
                    {
                        return;
                    }

                    if (int.TryParse(inputCampaignId, out campaignId) && campaignId > 0 && CheckIfCampaignExists(campaignId))
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

    #endregion RemoveCampaign
}