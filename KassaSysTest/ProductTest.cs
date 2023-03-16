
using KassaSys;
using KassaSys.Enum;

namespace ProductTest;

[TestClass]
public class ProductTest
{
    private readonly Product sut;
    public List<ProductList> Products { get; set; }

    public ProductTest()
    {
        sut = new Product();
    }

    [TestMethod]
    public void Check_productid_and_return_product_name()
    {
        // Arrange
        sut.ProductList.Add(new ProductList { Id = 1, Name = "aa", Price = 1, Type = ProductType.kg });

        var productId = 1;

        // Act
        var result = sut.FetchProductName(productId);

        // Assert
        Assert.AreEqual("aa", result);
    }

    [TestMethod]
    public void Check_productid_and_return_product_price()
    {
        // Arrange
        sut.ProductList.Add(new ProductList { Id = 1, Name = "aa", Price = 1, Type = ProductType.kg });

        var productId = 1;

        // Act
        var result = sut.FetchProductPrice(productId);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Check_if_productid_excist()
    {
        // Arrange
        sut.ProductList.Add(new ProductList { Id = 1, Name = "aa", Price = 1, Type = ProductType.kg });

        var productId = 1;

        // Act
        var result = sut.CheckIfProductExists(productId);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Fetch_product_type_from_productid()
    {
        // Arrange
        sut.ProductList.Add(new ProductList { Id = 1, Name = "aa", Price = 1, Type = ProductType.kg });

        var productId = 1;

        // Act
        var result = sut.FetchProductType(productId);

        // Assert
        Assert.AreEqual(ProductType.kg, result);
    }
}