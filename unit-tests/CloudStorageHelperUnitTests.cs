using System;
using property_price_api.Helpers;

namespace unit_tests;

public class CloudStorageHelperUnitTests
{
    private const string ObjectUrl = "https://storage.googleapis.com/property-price-engine-assets/example-properties.csv";

    [Test]
    public void ShouldReturnTrueValidUkPostcode()
    {
        var expected = ("property-price-engine-assets", "example-properties.csv");
        Assert.That(CloudStorageHelper.GetBucketAndObjectNamesFromObjectUrl(ObjectUrl), Is.EqualTo(expected));
    }
}
