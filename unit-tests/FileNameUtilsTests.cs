using property_price_common;
namespace unit_tests;

public class FileNameUtilsTests
{
    [Test]
    public void ShouldReturnRandomInt()
    {
        Assert.That(FileNameUtils.GenerateTransactionCsvFileName(), Does.Contain("transactions-export-"));
    }
}
