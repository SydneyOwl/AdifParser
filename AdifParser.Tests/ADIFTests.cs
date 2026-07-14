using AdifParser;

namespace AdifParser.Tests;

public class ADIFTests
{
    [Fact]
    public void ParseADIF_FromString_Basic()
    {
        var adif = new ADIF();
        adif.ReadFromString("MY ADIF HEADER<PROGRAMID:9>AdifParser<EOH>\n<CALL:4>NV9U<BAND:3>80M<eor>\n<CALL:4>W1AW<BAND:3>20M<eor>");

        Assert.True(adif.HasHeader);
        Assert.Equal(2, adif.QSOCount);
        Assert.Equal("CALL", adif.TheQSOs[0][0].Name);
        Assert.Equal("NV9U", adif.TheQSOs[0][0].Data);
    }

    [Fact]
    public void ParseADIF_FromString_NoHeader()
    {
        var adif = new ADIF();
        adif.ReadFromString("<CALL:4>NV9U<BAND:3>80M<eor>");

        Assert.False(adif.HasHeader);
        Assert.Equal(1, adif.QSOCount);
    }

    [Fact]
    public void ParseADIF_FromString_SkipsNameLines()
    {
        var adif = new ADIF();
        // The NAME line is skipped entirely by the <NAME filter, and the
        // QSO below it gets parsed normally.
        adif.ReadFromString("<PROGRAMID:9>AdifParser<EOH>\n<CALL:4>NV9U<BAND:3>80M<eor>");

        Assert.True(adif.HasHeader);
        Assert.Equal(1, adif.QSOCount);
        Assert.Equal(2, adif.TheQSOs[0].Count);
        Assert.Equal("CALL", adif.TheQSOs[0][0].Name);
    }

    [Fact]
    public void ParseADIF_MultipleHeaders_ThrowsException()
    {
        var adif = new ADIF();
        Assert.Throws<Exception>(() =>
            adif.ReadFromString("<PROGRAMID:9>First<EOH>\n<PROGRAMID:9>Second<EOH>\n<CALL:4>NV9U<eor>"));
    }

    [Fact]
    public void AddHeader_And_AddQSO_Programmatically()
    {
        var adif = new ADIF();
        adif.AddHeader("<PROGRAMID:9>AdifParser<EOH>");

        var qso = new ADIFQSO();
        qso.AddToken("CALL", "NV9U");
        qso.AddToken("BAND", "80M");
        adif.AddQSO(qso);

        Assert.True(adif.HasHeader);
        Assert.Equal(1, adif.QSOCount);
    }

    [Fact]
    public void ToString_RoundTrip()
    {
        var adif = new ADIF();
        adif.AddHeader("<PROGRAMID:9>AdifParser<EOH>");
        adif.AddQSO("<CALL:4>NV9U<BAND:3>80M");

        var result = adif.ToString();

        Assert.Contains("PROGRAMID", result);
        Assert.Contains("<eoh>", result);
        Assert.Contains("<CALL:4>NV9U", result);
        Assert.Contains("<eor>", result);
    }

    [Fact]
    public void EmptyADIF_ToString()
    {
        var adif = new ADIF();
        var result = adif.ToString();
        Assert.Equal("", result);
    }

    [Fact]
    public void SaveToFile_EmptyFileName_ThrowsException()
    {
        var adif = new ADIF();
        Assert.Throws<ArgumentException>(() => adif.SaveToFile(""));
    }

    [Fact]
    public void ReadFromFile_NonExistentFile_ThrowsException()
    {
        var adif = new ADIF();
        Assert.Throws<Exception>(() => adif.ReadFromFile("/nonexistent/path/file.adi"));
    }

    [Fact]
    public void QSOCount_Empty_ReturnsZero()
    {
        var adif = new ADIF();
        Assert.Equal(0, adif.QSOCount);
    }

    [Fact]
    public void ThrowExceptionOnUnknownLine()
    {
        var adif = new ADIF { ThrowExceptionOnUnknownLine = true };
        Assert.Throws<Exception>(() =>
            adif.ReadFromString("<CALL:4>NV9U<BAND:3>80M"));
    }

    [Fact]
    public void Version_ReturnsNonEmpty()
    {
        var adif = new ADIF();
        var version = adif.Version;
        Assert.NotNull(version);
        Assert.NotEmpty(version);
    }
}