namespace KassaSys.Campaign
{
    public interface ICampaign
    {
        string CalculateBestDiscount(int productId, Product.Product ProductList = null);

        void AddCampaign();

        void RemoveCampaign();
    }
}