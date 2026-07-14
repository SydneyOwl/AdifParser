using AdifParser;

namespace AdifParser.Tests;

public class ADIFQSOCollectionTests
{
    [Fact]
    public void AddQSO_And_ToString()
    {
        var collection = new ADIFQSOCollection();
        collection.Add(new ADIFQSO("<CALL:4>NV9U<BAND:3>80M"));
        collection.Add(new ADIFQSO("<CALL:4>W1AW<BAND:3>20M"));

        Assert.Equal(2, collection.Count);
        var result = collection.ToString();
        Assert.Contains("<CALL:4>NV9U", result);
        Assert.Contains("<CALL:4>W1AW", result);
        Assert.Contains("<eor>", result);
    }

    [Fact]
    public void EmptyCollection_ToString_ReturnsEmpty()
    {
        var collection = new ADIFQSOCollection();
        Assert.Equal("", collection.ToString());
    }
}