using property_price_cosmos_db.Helper;
using System.Text;

namespace unit_tests;

public class StreamHelperUnitTests
{
    [Test]
    public void GenerateStreamFromString_RegularString_ReturnsCorrectStreamContent()
    {
        // Arrange
        string input = "This is a test string.";
        byte[] expectedBytes = Encoding.UTF8.GetBytes(input);

        // Act
        using var stream = StreamHelper.GenerateStreamFromString(input);
        // Assert
        Assert.That(expectedBytes.Length, Is.EqualTo(stream.Length));


        byte[] actualBytes = new byte[stream.Length];
        stream.Read(actualBytes, 0, (int)stream.Length);
        CollectionAssert.AreEqual(expectedBytes, actualBytes);
    }

    [Test]
    public void GenerateStreamFromString_EmptyString_ReturnsEmptyStream()
    {
        // Arrange
        string input = "";
        // Act
        using var stream = StreamHelper.GenerateStreamFromString(input);
        // Assert
        Assert.That(stream.Length, Is.EqualTo(0));
    }
}
