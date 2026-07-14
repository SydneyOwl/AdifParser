using AdifParser;
using Xunit;

namespace AdifParser.Tests;

public class TokenCollectionTests
{
    [Fact]
    public void ParseLine_Qso_ParsesCorrectly()
    {
        var collection = new TokenCollection();
        collection.ParseLine("<CALL:4>NV9U<BAND:3>80M<MODE:3>SSB");

        Assert.Equal(3, collection.Count);
        Assert.Equal("CALL", collection[0].Name);
        Assert.Equal("NV9U", collection[0].Data);
        Assert.Equal("BAND", collection[1].Name);
        Assert.Equal("80M", collection[1].Data);
        Assert.Equal("MODE", collection[2].Name);
        Assert.Equal("SSB", collection[2].Data);
    }

    [Fact]
    public void Constructor_String_Qso()
    {
        var collection = new TokenCollection("<CALL:4>NV9U<BAND:3>80M");

        Assert.Equal(2, collection.Count);
        Assert.Equal("CALL", collection[0].Name);
        Assert.Equal("BAND", collection[1].Name);
    }

    [Fact]
    public void Constructor_String_Header()
    {
        var collection = new TokenCollection("<PROGRAMID:9>AdifParser<EOH>", true);

        Assert.Single(collection);
        Assert.Equal("PROGRAMID", collection[0].Name);
        Assert.Equal("AdifParser", collection[0].Data);
    }

    [Fact]
    public void ParseLine_SkipsEorTerminator()
    {
        var collection = new TokenCollection("<CALL:4>NV9U<EOR>");

        Assert.Single(collection);
        Assert.Equal("CALL", collection[0].Name);
    }

    [Fact]
    public void ParseLine_SkipsEohTerminator()
    {
        var collection = new TokenCollection("<PROGRAMID:9>AdifParser<EOH>");

        Assert.Single(collection);
        Assert.Equal("PROGRAMID", collection[0].Name);
    }

    [Fact]
    public void ToString_RoundTrip_Qso()
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
    public void AddToken_ByTokenField()
    {
        var collection = new TokenCollection();
        collection.AddToken(new TokenField("CALL", "NV9U"));

        Assert.Single(collection);
        Assert.Equal("CALL", collection[0].Name);
    }

    [Fact]
    public void AddTokens_Multiple()
    {
        var list = new TokenFieldList
        {
            new TokenField("CALL", "NV9U"),
            new TokenField("BAND", "80M")
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

    [Fact]
    public void AddToken_WithDataTypeAndEnumerations()
    {
        var collection = new TokenCollection();
        collection.AddToken("USERDEF1", "Hello", 'S', "A,B,C");

        Assert.Single(collection);
        Assert.Equal("USERDEF1", collection[0].Name);
        Assert.Equal("Hello", collection[0].Data);
        Assert.Equal('S', collection[0].UserDefType);
        Assert.Equal("A,B,C", collection[0].EnumerationItems);
    }

    [Fact]
    public void GetField_ReturnsMatchingToken()
    {
        var collection = new TokenCollection("<CALL:4>NV9U<BAND:3>80M<MODE:3>SSB");

        var token = collection.GetField("CALL");
        Assert.NotNull(token);
        Assert.Equal("NV9U", token!.Data);

        var token2 = collection.GetField("band");
        Assert.NotNull(token2);
        Assert.Equal("80M", token2!.Data);
    }

    [Fact]
    public void GetField_CaseSensitive()
    {
        var collection = new TokenCollection("<CALL:4>NV9U<BAND:3>80M");

        Assert.NotNull(collection.GetField("CALL", ignoreCase: false));
        Assert.Null(collection.GetField("call", ignoreCase: false));
    }

    [Fact]
    public void GetField_NotFound_ReturnsNull()
    {
        var collection = new TokenCollection("<CALL:4>NV9U<BAND:3>80M");

        Assert.Null(collection.GetField("FREQ"));
    }

    [Fact]
    public void GetFieldValue_ReturnsData()
    {
        var collection = new TokenCollection("<CALL:4>NV9U<BAND:3>80M");

        Assert.Equal("NV9U", collection.GetFieldValue("CALL"));
        Assert.Equal("80M", collection.GetFieldValue("BAND"));
    }

    [Fact]
    public void GetFieldValue_CaseSensitive()
    {
        var collection = new TokenCollection("<CALL:4>NV9U");

        Assert.Equal("NV9U", collection.GetFieldValue("CALL", ignoreCase: false));
        Assert.Null(collection.GetFieldValue("call", ignoreCase: false));
    }

    [Fact]
    public void GetFieldValue_NotFound_ReturnsNull()
    {
        var collection = new TokenCollection("<CALL:4>NV9U");

        Assert.Null(collection.GetFieldValue("FREQ"));
    }
}