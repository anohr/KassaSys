
using KassaSys;
using KassaSys.Enum;

namespace CashRegisterTest;

[TestClass]
public class CashRegisterTest
{
	private readonly CashRegister sut;

	public CashRegisterTest()
	{
		sut = new CashRegister();

		sut._receiptList.Add(new CashRegisterList { Id = 1, Name = "Banan", Count = 3, Price = 23.45, Discount = 5, Type = ProductType.kg });
		sut._receiptList.Add(new CashRegisterList { Id = 2, Name = "Boll", Count = 1, Price = 425, Discount = 0.34, Type = ProductType.st });
	}

	[TestMethod]
	public void Fetch_total_price_on_receipt()
	{
		// Arrange

		// Act
		var result = sut.FetchTotalPrice();

		// Assert
		Assert.AreEqual(199.85, result);
	}

	[TestMethod]
	public void Check_if_product_exicsts()
	{
		// Arrange
		var productId = 2;

		// Act
		var result = sut.CheckIfProductExicsts(productId);

		// Assert
		Assert.IsTrue(result);
	}

	[TestMethod]
	public void Check_if_product_not_exicsts()
	{
		// Arrange
		var productId = 4;

		// Act
		var result = sut.CheckIfProductExicsts(productId);

		// Assert
		Assert.IsFalse(result);
	}
}