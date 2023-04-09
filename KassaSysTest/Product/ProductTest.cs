using KassaSys.Enum;
using KassaSys.Product;

namespace KassaSysTest.Product;

[TestClass]
public class ProductTest
{
    private readonly KassaSys.Product.Product sut;

    public ProductTest()
    {
        sut = new KassaSys.Product.Product();

        sut.productList.Add(new ProductList { Id = 1, Name = "aa", Price = 1, Type = ProductType.kg });
        sut.productList.Add(new ProductList { Id = 2, Name = "bb", Price = 2, Type = ProductType.st });
    }

    [TestMethod]
    public void Check_productId_and_return_product_name()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = sut.FetchProductName(productId);

        // Assert
        Assert.AreEqual("aa", result);
    }

    [TestMethod]
    public void Check_not_existing_productId_and_return_null()
    {
        // Arrange
        var productId = 2323;

        // Act
        var result = sut.FetchProductName(productId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Check_productId_and_return_product_price()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = sut.FetchProductPrice(productId);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Check_productId_and_return_product_type()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = sut.FetchProductType(productId);

        // Assert
        Assert.AreEqual(ProductType.kg, result);
    }

    [TestMethod]
    public void Check_productId_if_it_exist()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = sut.CheckIfProductExists(productId);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Check_productId_if_it_not_exist()
    {
        // Arrange
        var productId = 4;

        // Act
        var result = sut.CheckIfProductExists(productId);

        // Assert
        Assert.IsFalse(result);
    }
}