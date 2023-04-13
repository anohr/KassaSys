using KassaSys.Enum;
using KassaSys.Services;

namespace KassaSys.Product;

public class Product : IProduct
{
    private IFileService _fileService = new FileService();
    private string _folder = "products";
    private string _filePath = "products.txt";
    private string _splitString = " | ";

    public List<ProductList> productList = new List<ProductList>();

    public Product()
    {
        productList = FetchProductFromFile();
    }

    public List<ProductList> FetchProductFromFile()
    {
        return _fileService.ReadProductFile(_folder, _filePath);
    }

    private void SaveToFile(List<ProductList> tempProductList)
    {
        var stringList = tempProductList.Select(product => $"{product.Id}{_splitString}{product.Name}{_splitString}{product.Price}{_splitString}{product.Type}").ToList();

        _fileService.SaveListToFile("products", "products.txt", stringList);
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

    #region Addproduct

    public void AddProduct()
    {
        string productName;
        double productPrice;
        ProductType productType = ProductType.kg;

        string inputProductName;
        string inputProductPrice;
        string inputProductType;

        Console.Clear();
        Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
        Console.WriteLine("  - Lägg till en ny produkt -\n");

        while (true)
        {
            inputProductName = Program.AskForInput("   Ange produkt namn: ");

            if (!string.IsNullOrWhiteSpace(inputProductName))
            {
                if (string.Compare(inputProductName, "0", StringComparison.Ordinal) == 0)
                {
                    return;
                }

                productName = inputProductName;

                break;
            }

            Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
        }

        while (true)
        {
            inputProductPrice = Program.AskForInput("   Ange produkt pris [ex 12,34]: ");

            if (!string.IsNullOrWhiteSpace(inputProductPrice))
            {
                if (string.Compare(inputProductPrice, "0", StringComparison.Ordinal) == 0)
                {
                    return;
                }

                if (double.TryParse(inputProductPrice.Replace('.', ','), out productPrice) && productPrice > 0)
                {
                    productPrice = Math.Round(productPrice, 2);

                    break;
                }
            }

            Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
        }

        while (true)
        {
            inputProductType = Program.AskForInput($"   Ange prisgrupp [{string.Join(", ", System.Enum.GetNames(typeof(ProductType)))}]: ");

            if (!string.IsNullOrWhiteSpace(inputProductType))
            {
                if (string.Compare(inputProductType, "0", StringComparison.Ordinal) == 0)
                {
                    return;
                }

                if (System.Enum.TryParse<ProductType>(inputProductType, true, out productType))
                {
                    break;
                }
            }

            Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
        }

        productList.Add(new ProductList { Id = productList.Count > 0 ? productList.Last().Id + 1 : 1, Name = productName, Price = productPrice, Type = productType });

        SaveToFile(productList);
    }

    #endregion Addproduct

    #region UpdateProduct

    public void UpdateProduct()
    {
        while (true)
        {
            int productId;
            string productName;
            double productPrice;
            ProductType productType = ProductType.kg;

            string inputProductId;
            string inputProductPrice;
            string inputProductType;

            Console.Clear();
            Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
            Console.WriteLine("  - Uppdatera namn och pris på en produkt -\n");

            Console.WriteLine($"    {"Id",-3} {"Namn",-15} {"Pris",-9}\n");

            if (productList.Count == 0)
            {
                Console.WriteLine("     Inga produkter att uppdatera...\n");
                Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
                Console.ReadKey();

                return;
            }
            else
            {
                for (int i = 0; i < productList.Count; i++)
                {
                    var product = productList[i];

                    if (i % 2 == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }

                    Console.WriteLine($"    {product.Id,-3} {product.Name,-15} {product.Price,8:F2}   {product.Type}");

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

                    if (int.TryParse(inputProductId, out productId) && productId > 0 && CheckIfProductExists(productId))
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
                var fetchedProductName = FetchProductName(productId);

                productName = Program.AskForInput($"    Ange nytt produkt namn på ({fetchedProductName}): ");

                if (!string.IsNullOrWhiteSpace(productName))
                {
                    if (string.Compare(productName, "0", StringComparison.Ordinal) == 0)
                    {
                        return;
                    }

                    break;
                }
                else if (string.IsNullOrEmpty(productName))
                {
                    productName = fetchedProductName;

                    break;
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            while (true)
            {
                var fetcedProductPrice = FetchProductPrice(productId);

                inputProductPrice = Program.AskForInput($"    Ange nytt produkt pris ({fetcedProductPrice:F2}): ");

                if (!string.IsNullOrWhiteSpace(inputProductPrice))
                {
                    if (string.Compare(inputProductPrice, "0", StringComparison.Ordinal) == 0)
                    {
                        return;
                    }

                    if (double.TryParse(inputProductPrice.Replace('.', ','), out productPrice) && productPrice > 0)
                    {
                        break;
                    }
                }
                else if (string.IsNullOrEmpty(inputProductPrice))
                {
                    productPrice = fetcedProductPrice;

                    break;
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            while (true)
            {
                var fetchedProductType = FetchProductType(productId);

                inputProductType = Program.AskForInput($"    Ange ny prisgrupp [{string.Join(", ", System.Enum.GetNames(typeof(ProductType)))}] ({fetchedProductType}): ");

                if (!string.IsNullOrWhiteSpace(inputProductType))
                {
                    if (string.Compare(inputProductType, "0", StringComparison.Ordinal) == 0)
                    {
                        return;
                    }

                    if (System.Enum.TryParse<ProductType>(inputProductType, true, out productType))
                    {
                        break;
                    }
                }
                else if (string.IsNullOrEmpty(inputProductType))
                {
                    productType = fetchedProductType;
                    break;
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            productList.Where(product => product.Id == productId).ToList().ForEach(product =>
            {
                product.Name = productName;
                product.Price = productPrice;
                product.Type = productType;
            });

            SaveToFile(productList);
        }
    }

    #endregion UpdateProduct

    #region RemoveProduct

    public void RemoveProduct()
    {
        while (true)
        {
            int productId;

            string inputProductId;

            Console.Clear();
            Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
            Console.WriteLine("  - Ta bort en produkt -\n");

            Console.WriteLine($"    {"Id",-3} {"Namn",-15} {"Pris",-10} {"Prisgrupp"}\n");

            if (productList.Count == 0)
            {
                Console.WriteLine("     Inga produkter att ta bort...\n");
                Console.Write("    Tryck på valfri knapp för att återgå till menyn...");
                Console.ReadKey();

                return;
            }
            else
            {
                for (int i = 0; i < productList.Count; i++)
                {
                    var product = productList[i];

                    if (i % 2 == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }

                    Console.WriteLine($"    {product.Id,-3} {product.Name,-15} {product.Price,8:F2}   {product.Type}");

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

                    if (int.TryParse(inputProductId, out productId) && productId > 0 && CheckIfProductExists(productId))
                    {
                        break;
                    }
                }

                Program.ErrorPrint("     Felaktig inmatning. Försök igen.");
            }

            productList.Where(product => product.Id == productId).ToList().ForEach(product =>
            {
                productList.Remove(product);
            });

            SaveToFile(productList);

            Campaign.Campaign campaignList = new Campaign.Campaign();

            campaignList.RemoveCampaign(productId);
        }
    }

    #endregion RemoveProduct
}