using AdifParser;
using Xunit;

namespace AdifParser.Tests;

public class AdifQsoCollectionTests
{
    [Fact]
    public void AddQso_And_ToString()
    {
        var collection = new AdifQsoCollection();
        collection.Add(new AdifQso("<CALL:4>NV9U<BAND:3>80M"));
        collection.Add(new AdifQso("<CALL:4>W1AW<BAND:3>20M"));

        Assert.Equal(2, collection.Count);
        var result = collection.ToString();
        Assert.Contains("<CALL:4>NV9U", result);
        Assert.Contains("<CALL:4>W1AW", result);
        Assert.Contains("<eor>", result);
    }

    [Fact]
    public void EmptyCollection_ToString_ReturnsEmpty()
    {
        var collection = new AdifQsoCollection();
        Assert.Equal("", collection.ToString());
    }

    [Fact]
    public void Indexer_Access()
    {
        var collection = new AdifQsoCollection();
        var qso = new AdifQso("<CALL:4>NV9U<BAND:3>80M");
        collection.Add(qso);

        Assert.Same(qso, collection[0]);
    }

    [Fact]
    public void Clear_RemovesAll()
    {
        var collection = new AdifQsoCollection();
        collection.Add(new AdifQso("<CALL:4>NV9U<BAND:3>80M"));
        collection.Clear();

        Assert.Equal(0, collection.Count);
        Assert.Equal("", collection.ToString());
    }
}