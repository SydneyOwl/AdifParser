using AdifParser;
using Xunit;

namespace AdifParser.Tests;

public class AdifHeaderTests
{
    [Fact]
    public void Parse_Basic()
    {
        var header = new AdifHeader("<PROGRAMID:9>AdifParser<EOH>");

        Assert.Single(header);
        Assert.Equal("PROGRAMID", header[0].Name);
        Assert.Equal("AdifParser", header[0].Data);
    }

    [Fact]
    public void Parse_WithPreamble()
    {
        var header = new AdifHeader("MY ADIF HEADER<PROGRAMID:9>AdifParser<EOH>");

        Assert.Equal("MY ADIF HEADER", header.Preamble);
        Assert.Single(header);
    }

    [Fact]
    public void Parse_OnlyPreamble()
    {
        var header = new AdifHeader("Just some text");

        Assert.Equal("Just some text", header.Preamble);
        Assert.Empty(header);
    }

    [Fact]
    public void Parse_EmptyString()
    {
        var header = new AdifHeader("");

        Assert.Equal("", header.Preamble);
        Assert.Empty(header);
    }

    [Fact]
    public void ToString_RoundTrip()
    {
        var header = new AdifHeader("MY HEADER<PROGRAMID:10>AdifParser<EOH>");
        var result = header.ToString();

        Assert.Equal("MY HEADER <PROGRAMID:10>AdifParser <eoh>", result);
    }

    [Fact]
    public void Constructor_WithTokenFieldList()
    {
        var list = new TokenFieldList
        {
            new TokenField("PROGRAMID", "AdifParser"),
            new TokenField("PROGRAMVERSION", "2.0")
        };

        var header = new AdifHeader("PREAMBLE ", list);

        Assert.Equal("PREAMBLE ", header.Preamble);
        Assert.Equal(2, header.Count);
        Assert.Equal("PROGRAMID", header[0].Name);
        Assert.Equal("PROGRAMVERSION", header[1].Name);
    }

    [Fact]
    public void Add_TokenField()
    {
        var header = new AdifHeader();
        header.Add(new TokenField("PROGRAMID", "AdifParser"));

        Assert.Single(header);
        Assert.Equal("PROGRAMID", header[0].Name);
    }

    [Fact]
    public void Parse_ReplacesContent()
    {
        var header = new AdifHeader("<PROGRAMID:9>AdifParser<EOH>");
        header.Clear();
        header.Parse("<APP_MYAPP:5>MyApp<EOH>");

        Assert.Single(header);
        Assert.Equal("APP_MYAPP", header[0].Name);
        Assert.Equal("MyApp", header[0].Data);
    }

    [Fact]
    public void Preamble_DefaultIsEmpty()
    {
        var header = new AdifHeader();
        Assert.Equal("", header.Preamble);
    }

    [Fact]
    public void ToString_NoPreamble()
    {
        var header = new AdifHeader("<PROGRAMID:9>AdifParser<EOH>");
        var result = header.ToString();

        Assert.Equal("<PROGRAMID:10>AdifParser <eoh>", result);
    }
}