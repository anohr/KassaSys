using KassaSys.Enum;
using KassaSys.Product;
using KassaSys.Register;

namespace KassaSys.Campaign;

[TestClass]
public class CampaignTest
{
	private readonly ShopCampaign sut;
	private readonly ShopProduct sut2;

	public CampaignTest()
	{
		sut = new ShopCampaign();
		sut2 = new ShopProduct();

		sut2.ProductList.Add(new ProductList { Id = 1, Name = "Banan", Price = 10, Type = ProductType.st });
		sut._campaignList.Add(new CampaignList { Id = 1, ProductID = 1, StartDate = DateTime.Now, EndDate = 7, Discount = "10%" });
		sut._campaignList.Add(new CampaignList { Id = 2, ProductID = 1, StartDate = DateTime.Now, EndDate = 7, Discount = "8kr" });
	}

	[TestMethod]
	public void Check_for_best_discount()
	{
		// Arrange


		// Act
		var result = sut.GetBestDiscount(1);

		// Assert
		Assert.AreEqual("10%", result);
	}

	[TestMethod]
	public void Check_if_campaign_exists()
	{
		// Arrange

		// Act
		var result = sut.CheckIfCampaignExists(1);

		// Assert
		Assert.IsTrue(result);
	}

	[TestMethod]
	public void Check_if_campaign_not_exists()
	{
		// Arrange

		// Act
		var result = sut.CheckIfCampaignExists(1337);

		// Assert
		Assert.IsFalse(result);
	}
}