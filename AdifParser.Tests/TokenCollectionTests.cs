using AdifParser;

namespace AdifParser.Tests;

public class TokenCollectionTests
{
    [Fact]
    public void PullApartLine_QSO_ParsesCorrectly()
    {
        var collection = new TokenCollection();
        collection.PullApartLine("<CALL:4>NV9U<BAND:3>80M<MODE:3>SSB");

        Assert.Equal(3, collection.Count);
        Assert.Equal("CALL", collection[0].Name);
        Assert.Equal("NV9U", collection[0].Data);
        Assert.Equal("BAND", collection[1].Name);
        Assert.Equal("80M", collection[1].Data);
        Assert.Equal("MODE", collection[2].Name);
        Assert.Equal("SSB", collection[2].Data);
    }

    [Fact]
    public void PullApartLine_Constructor_QSO()
    {
        var collection = new TokenCollection("<CALL:4>NV9U<BAND:3>80M");

        Assert.Equal(2, collection.Count);
        Assert.Equal("CALL", collection[0].Name);
        Assert.Equal("BAND", collection[1].Name);
    }

    [Fact]
    public void PullApartLine_Constructor_Header()
    {
        var collection = new TokenCollection("<PROGRAMID:9>AdifParser<EOH>", true);

        Assert.Single(collection);
        Assert.Equal("PROGRAMID", collection[0].Name);
        Assert.Equal("AdifParser", collection[0].Data);
    }

    [Fact]
    public void PullApartLine_SkipsEOR_Terminator()
    {
        var collection = new TokenCollection("<CALL:4>NV9U<EOR>");

        Assert.Single(collection);
        Assert.Equal("CALL", collection[0].Name);
    }

    [Fact]
    public void PullApartLine_SkipsEOH_Terminator()
    {
        var collection = new TokenCollection("<PROGRAMID:9>AdifParser<EOH>");

        Assert.Single(collection);
        Assert.Equal("PROGRAMID", collection[0].Name);
    }

    [Fact]
    public void ToString_RoundTrip_QSO()
    {
        var original = "<CALL:4>NV9U <BAND:3>80M <MODE:3>SSB ";
        var collection = new TokenCollection("<CALL:4>NV9U<BAND:3>80M<MODE:3>SSB");
        var result = collection.ToString();

        Assert.Equal(original, result);
    }

    [Fact]
    public void AddToken_ByTagNameAndData()
    {
        var collection = new TokenCollection();
        collection.AddToken("CALL", "NV9U");

        Assert.Single(collection);
        Assert.Equal("CALL", collection[0].Name);
        Assert.Equal("NV9U", collection[0].Data);
    }

    [Fact]
    public void AddToken_ByTokenNameData()
    {
        var collection = new TokenCollection();
        collection.AddToken(new TokenNameData("CALL", "NV9U"));

        Assert.Single(collection);
        Assert.Equal("CALL", collection[0].Name);
    }

    [Fact]
    public void AddTokens_Multiple()
    {
        var list = new TokenNameDataList
        {
            new TokenNameData("CALL", "NV9U"),
            new TokenNameData("BAND", "80M")
        };

        var collection = new TokenCollection();
        collection.AddTokens(list);

        Assert.Equal(2, collection.Count);
    }

    [Fact]
    public void EmptyCollection_ToString_ReturnsEmpty()
    {
        var collection = new TokenCollection();
        Assert.Equal("", collection.ToString());
    }
}