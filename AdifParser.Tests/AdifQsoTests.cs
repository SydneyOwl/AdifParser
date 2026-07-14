using AdifParser;
using Xunit;

namespace AdifParser.Tests;

public class AdifQsoTests
{
    [Fact]
    public void Parse_Basic()
    {
        var qso = new AdifQso("<CALL:4>NV9U<BAND:3>80M<MODE:3>SSB");

        Assert.Equal(3, qso.Count);
        Assert.Equal("CALL", qso[0].Name);
        Assert.Equal("NV9U", qso[0].Data);
        Assert.Equal("BAND", qso[1].Name);
        Assert.Equal("80M", qso[1].Data);
        Assert.Equal("MODE", qso[2].Name);
        Assert.Equal("SSB", qso[2].Data);
    }

    [Fact]
    public void Parse_EmptyString()
    {
        var qso = new AdifQso("");
        Assert.Empty(qso);
    }

    [Fact]
    public void Constructor_WithTokenFieldList()
    {
        var list = new TokenFieldList
        {
            new TokenField("CALL", "NV9U"),
            new TokenField("BAND", "80M")
        };

        var qso = new AdifQso(list);

        Assert.Equal(2, qso.Count);
        Assert.Equal("CALL", qso[0].Name);
        Assert.Equal("BAND", qso[1].Name);
    }

    [Fact]
    public void ToString_RoundTrip()
    {
        var original = "<CALL:4>NV9U <BAND:3>80M <eor>";
        var qso = new AdifQso("<CALL:4>NV9U<BAND:3>80M");
        var result = qso.ToString();

        Assert.Equal(original, result);
    }

    [Fact]
    public void Parse_ReplacesContent()
    {
        var qso = new AdifQso("<CALL:4>NV9U<BAND:3>80M");
        qso.Clear();
        qso.Parse("<CALL:4>W1AW<MODE:3>SSB");

        Assert.Equal(2, qso.Count);
        Assert.Equal("CALL", qso[0].Name);
        Assert.Equal("W1AW", qso[0].Data);
        Assert.Equal("MODE", qso[1].Name);
    }
}