using KassaSys.Campaign;
using KassaSys.Enum;
using KassaSys.Product;
using System.Text.RegularExpressions;

namespace KassaSys.Services
{
    public class FileService : IFileService
    {
        private const string _baseFolder = @".\outputs\";

        public FileService()
        {
            if (!Directory.Exists(_baseFolder))
            {
                Directory.CreateDirectory(_baseFolder);
            }
        }

        public List<CampaignList> ReadCampaignFile(string folder, string fileName)
        {
            List<CampaignList> campaignList = new List<CampaignList>();

            if (!Directory.Exists($"{_baseFolder}\\{folder}"))
            {
                Directory.CreateDirectory($"{_baseFolder}\\{folder}");
            }

            if (!File.Exists($"{_baseFolder}\\{folder}\\{fileName}"))
            {
                return campaignList;
            }

            using (StreamReader reader = new StreamReader($"{_baseFolder}\\{folder}\\{fileName}"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] fields = line.Split(" | ");

                    int id = Convert.ToInt32(fields[0]);
                    int productID = Convert.ToInt32(fields[1]);
                    DateTime startDate = Convert.ToDateTime(fields[2]);
                    int endDate = Convert.ToInt32(fields[3]);
                    string discount = fields[4];

                    campaignList.Add(new CampaignList { Id = id, ProductID = productID, StartDate = startDate, EndDate = endDate, Discount = discount });
                }
            }

            return campaignList;
        }

        public List<ProductList> ReadProductFile(string folder, string fileName)
        {
            List<ProductList> productList = new List<ProductList>();

            if (!Directory.Exists($"{_baseFolder}\\{folder}"))
            {
                Directory.CreateDirectory($"{_baseFolder}\\{folder}");
            }

            if (!File.Exists($"{_baseFolder}\\{folder}\\{fileName}"))
            {
                return productList;
            }

            using (StreamReader reader = new StreamReader($"{_baseFolder}\\{folder}\\{fileName}"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] fields = line.Split(" | ");

                    int id = Convert.ToInt32(fields[0]);
                    string name = fields[1];
                    double price = Convert.ToDouble(fields[2]);
                    ProductType type = (ProductType)System.Enum.Parse(typeof(ProductType), fields[3]);

                    productList.Add(new ProductList { Id = id, Name = name, Price = price, Type = type });
                }
            }

            return productList;
        }

        public void SaveListToFile<T>(string folder, string fileName, List<T> listToSave)
        {
            if (!Directory.Exists($"{_baseFolder}\\{folder}"))
            {
                Directory.CreateDirectory($"{_baseFolder}\\{folder}");
            }

            var path = $"{_baseFolder}\\{folder}\\{fileName}";

            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (T item in listToSave)
                {
                    writer.WriteLine(item);
                }
            }
        }

        public void AppendToFile(string folder, string fileName, string stringToSave)
        {
            if (!Directory.Exists($"{_baseFolder}\\{folder}"))
            {
                Directory.CreateDirectory($"{_baseFolder}\\{folder}");
            }

            var path = $"{_baseFolder}\\{folder}\\{fileName}";

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(stringToSave);
            }
        }

        public int FetchReceiptNumberFromFile(string folder)
        {
            int countReceipts = 0;

            if (!Directory.Exists($"{_baseFolder}\\{folder}"))
            {
                Directory.CreateDirectory($"{_baseFolder}\\{folder}");
            }

            foreach (var file in Directory.GetFiles($"{_baseFolder}\\{folder}"))
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    string line;
                    string lastLine = null;

                    while ((line = reader.ReadLine()) != null)
                    {
                        lastLine = line;
                    }

                    Match match = Regex.Match(lastLine, @"\d+");

                    if (match.Success)
                    {
                        int number = int.Parse(match.Value);

                        if (number > countReceipts)
                        {
                            countReceipts = number;
                        }
                    }
                }
            }

            return countReceipts + 1;
        }
    }
}