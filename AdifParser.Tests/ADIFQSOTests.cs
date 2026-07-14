using AdifParser;

namespace AdifParser.Tests;

public class ADIFQSOTests
{
    [Fact]
    public void ParseQSO_Basic()
    {
        var qso = new ADIFQSO("<CALL:4>NV9U<BAND:3>80M<MODE:3>SSB");

        Assert.Equal(3, qso.Count);
        Assert.Equal("CALL", qso[0].Name);
        Assert.Equal("NV9U", qso[0].Data);
        Assert.Equal("BAND", qso[1].Name);
        Assert.Equal("80M", qso[1].Data);
        Assert.Equal("MODE", qso[2].Name);
        Assert.Equal("SSB", qso[2].Data);
    }

    [Fact]
    public void ParseQSO_EmptyString()
    {
        var qso = new ADIFQSO("");
        Assert.Empty(qso);
    }

    [Fact]
    public void Constructor_WithTokenNameDataList()
    {
        var list = new TokenNameDataList
        {
            new TokenNameData("CALL", "NV9U"),
            new TokenNameData("BAND", "80M")
        };

        var qso = new ADIFQSO(list);

        Assert.Equal(2, qso.Count);
        Assert.Equal("CALL", qso[0].Name);
        Assert.Equal("BAND", qso[1].Name);
    }

    [Fact]
    public void ToString_RoundTrip()
    {
        var original = "<CALL:4>NV9U <BAND:3>80M <eor>";
        var qso = new ADIFQSO("<CALL:4>NV9U<BAND:3>80M");
        var result = qso.ToString();

        Assert.Equal(original, result);
    }

    [Fact]
    public void ParseStringToADIFQSO_ReplacesContent()
    {
        var qso = new ADIFQSO("<CALL:4>NV9U<BAND:3>80M");
        qso.Clear();
        qso.ParseStringToADIFQSO("<CALL:4>W1AW<MODE:3>SSB");

        Assert.Equal(2, qso.Count);
        Assert.Equal("CALL", qso[0].Name);
        Assert.Equal("W1AW", qso[0].Data);
        Assert.Equal("MODE", qso[1].Name);
    }
}