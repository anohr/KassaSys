using KassaSys.Campaign;
using KassaSys.Product;

namespace KassaSys.Services;

public interface IFileService
{
    void SaveListToFile<T>(string folder, string fileName, List<T> listToSave);

    void AppendToFile(string folder, string fileName, string stringToSave);

    int FetchReceiptNumberFromFile(string folder);

    List<CampaignList> ReadCampaignFile(string folder, string fileName);

    List<ProductList> ReadProductFile(string folder, string fileName);
}