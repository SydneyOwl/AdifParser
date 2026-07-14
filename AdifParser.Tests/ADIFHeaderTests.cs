using AdifParser;

namespace AdifParser.Tests;

public class ADIFHeaderTests
{
    [Fact]
    public void ParseHeader_Basic()
    {
        var header = new ADIFHeader("<PROGRAMID:9>AdifParser<EOH>");

        Assert.Single(header);
        Assert.Equal("PROGRAMID", header[0].Name);
        Assert.Equal("AdifParser", header[0].Data);
    }

    [Fact]
    public void ParseHeader_WithPreText()
    {
        var header = new ADIFHeader("MY ADIF HEADER<PROGRAMID:9>AdifParser<EOH>");

        Assert.Equal("MY ADIF HEADER", header.HeaderPreText);
        Assert.Single(header);
    }

    [Fact]
    public void ParseHeader_OnlyPreText()
    {
        var header = new ADIFHeader("Just some text");

        Assert.Equal("Just some text", header.HeaderPreText);
        Assert.Empty(header);
    }

    [Fact]
    public void ParseHeader_EmptyString()
    {
        var header = new ADIFHeader("");

        Assert.Equal("", header.HeaderPreText);
        Assert.Empty(header);
    }

    [Fact]
    public void ToString_RoundTrip()
    {
        var header = new ADIFHeader("MY HEADER<PROGRAMID:10>AdifParser<EOH>");
        var result = header.ToString();

        Assert.Equal("MY HEADER <PROGRAMID:10>AdifParser <eoh>", result);
    }

    [Fact]
    public void Constructor_WithTokenNameDataList()
    {
        var list = new TokenNameDataList
        {
            new TokenNameData("PROGRAMID", "AdifParser"),
            new TokenNameData("PROGRAMVERSION", "2.0")
        };

        var header = new ADIFHeader("PREAMBLE ", list);

        Assert.Equal("PREAMBLE ", header.HeaderPreText);
        Assert.Equal(2, header.Count);
        Assert.Equal("PROGRAMID", header[0].Name);
        Assert.Equal("PROGRAMVERSION", header[1].Name);
    }

    [Fact]
    public void Add_TokenNameData()
    {
        var header = new ADIFHeader();
        header.Add(new TokenNameData("PROGRAMID", "AdifParser"));

        Assert.Single(header);
        Assert.Equal("PROGRAMID", header[0].Name);
    }

    [Fact]
    public void ParseStringToADIFHeader_ReplacesContent()
    {
        var header = new ADIFHeader("<PROGRAMID:9>AdifParser<EOH>");
        header.Clear();
        header.ParseStringToADIFHeader("<APP_MYAPP:5>MyApp<EOH>");

        Assert.Single(header);
        Assert.Equal("APP_MYAPP", header[0].Name);
        Assert.Equal("MyApp", header[0].Data);
    }
}