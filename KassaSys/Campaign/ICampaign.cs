using KassaSys.Product;

namespace KassaSys.Campaign
{
	public interface ICampaign
	{
		string CalculateBestDiscount(int productId, ShopProduct ProductList = null);
		void AddCampaign();
		void RemoveCampaign();
	}
}
