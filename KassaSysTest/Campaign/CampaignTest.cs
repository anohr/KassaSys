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

		sut._campaignList.Add(new CampaignList { Id = 1, ProductID = 1, StartDate = DateTime.Now.AddDays(-1), EndDate = 7, Discount = "10%" });
		sut._campaignList.Add(new CampaignList { Id = 2, ProductID = 1, StartDate = DateTime.Now, EndDate = 7, Discount = "90%" });

		sut2.ProductList.Add(new ProductList { Id = 1, Name = "Citron", Price = 10, Type = ProductType.st });
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
	public void Fetch_best_discound_depending_on_secound_arg()
	{
		// Arrange

		// Act
		var resultPc = sut.FetchBestDiscount(1, "%");
		var resultKr = sut.FetchBestDiscount(1, "kr");

		// Assert
		Assert.AreEqual("90%", resultPc);
		Assert.AreEqual("4kr", resultKr);
	}

	[TestMethod]
	public void GetBestDiscount_Returns_Best_Discount_For_Product()
	{
		// Arrange

		// Act
		var result = sut.GetBestDiscount(1);

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual("90%", result);
		Assert.IsTrue(result.Contains("%") || result.Contains("kr"));
	}
}