using KassaSys.Enum;

namespace KassaSys.Register;

[TestClass]
public class CashRegisterTest
{
    private readonly CashRegister sut;

    public CashRegisterTest()
    {
        sut = new CashRegister();

        sut.receiptList.Add(new CashRegisterList { Id = 1, Name = "Banan", Count = 3, Price = 23.45, Discount = "1kr", Type = ProductType.kg });
        sut.receiptList.Add(new CashRegisterList { Id = 2, Name = "Boll", Count = 1, Price = 425, Discount = "10%", Type = ProductType.st });
    }

    [TestMethod]
    public void Fetch_total_price_on_receipt()
    {
        // Arrange

        // Act
        var result = sut.CalculateTotalPrice();

        // Assert
        Assert.AreEqual(449.85, result);
    }

    [TestMethod]
    public void Check_if_product_exists()
    {
        // Arrange
        var productId = 2;

        // Act
        var result = sut.CheckIfProductExistsInList(productId);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Check_if_product_not_exists()
    {
        // Arrange
        var productId = 4;

        // Act
        var result = sut.CheckIfProductExistsInList(productId);

        // Assert
        Assert.IsFalse(result);
    }
}