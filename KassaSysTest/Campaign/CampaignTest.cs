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

		sut._campaignList.Add(new CampaignList { Id = 1, ProductID = 1, StartDate = DateTime.Now.AddDays(-1), EndDate = 7, Discount = "4kr" });
		sut._campaignList.Add(new CampaignList { Id = 2, ProductID = 1, StartDate = DateTime.Now, EndDate = 7, Discount = "50%" });

		sut2.ProductList.Add(new ProductList { Id = 1, Name = "Citron", Price = 8, Type = ProductType.st });
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

	[TestMethod]
	public void Check_for_best_discount()
	{
		// Arrange
		/*var ProductList = new List<ProductList>
		{
			new ProductList { Id = 1, Name = "Citron", Price = 10, Type = ProductType.st }
		};*/

		// Act
		var result = sut.GetBestDiscount(1, sut2);

		// Assert
		Assert.IsNotNull(result);
		Assert.IsTrue(result.Contains("%") || result.Contains("kr"));
		Assert.AreEqual("50%", result);
	}
}