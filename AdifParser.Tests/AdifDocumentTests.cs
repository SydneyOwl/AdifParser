using AdifParser;
using Xunit;

namespace AdifParser.Tests;

public class AdifDocumentTests
{
    [Fact]
    public void Parse_FromString_Basic()
    {
        var doc = new AdifDocument();
        doc.ReadFromString("MY ADIF HEADER<PROGRAMID:9>AdifParser<EOH>\n<CALL:4>NV9U<BAND:3>80M<eor>\n<CALL:4>W1AW<BAND:3>20M<eor>");

        Assert.True(doc.HasHeader);
        Assert.Equal(2, doc.QsoCount);
        Assert.Equal("CALL", doc.Qsos[0][0].Name);
        Assert.Equal("NV9U", doc.Qsos[0][0].Data);
    }

    [Fact]
    public void Parse_FromString_NoHeader()
    {
        var doc = new AdifDocument();
        doc.ReadFromString("<CALL:4>NV9U<BAND:3>80M<eor>");

        Assert.False(doc.HasHeader);
        Assert.Equal(1, doc.QsoCount);
    }

    [Fact]
    public void Parse_FromString_SkipsNameLines()
    {
        var doc = new AdifDocument();
        // Lines starting with <NAME: should be skipped (operator name can contain angle brackets)
        doc.ReadFromString("<PROGRAMID:9>AdifParser<EOH>\n<NAME:5>Joe\n<CALL:4>NV9U<BAND:3>80M<eor>");

        Assert.True(doc.HasHeader);
        Assert.Equal(1, doc.QsoCount);
        Assert.Equal(2, doc.Qsos[0].Count);
        Assert.Equal("CALL", doc.Qsos[0][0].Name);
        Assert.Equal("BAND", doc.Qsos[0][1].Name);
    }

    [Fact]
    public void Parse_MultipleHeaders_ThrowsException()
    {
        var doc = new AdifDocument();
        Assert.Throws<AdifParseException>(() =>
            doc.ReadFromString("<PROGRAMID:9>First<EOH>\n<PROGRAMID:9>Second<EOH>\n<CALL:4>NV9U<eor>"));
    }

    [Fact]
    public void AddHeader_And_AddQso_Programmatically()
    {
        var doc = new AdifDocument();
        doc.AddHeader("<PROGRAMID:9>AdifParser<EOH>");

        var qso = new AdifQso();
        qso.AddToken("CALL", "NV9U");
        qso.AddToken("BAND", "80M");
        doc.AddQso(qso);

        Assert.True(doc.HasHeader);
        Assert.Equal(1, doc.QsoCount);
    }

    [Fact]
    public void ToString_RoundTrip()
    {
        var doc = new AdifDocument();
        doc.AddHeader("<PROGRAMID:9>AdifParser<EOH>");
        doc.AddQso("<CALL:4>NV9U<BAND:3>80M");

        var result = doc.ToString();

        Assert.Contains("PROGRAMID", result);
        Assert.Contains("<eoh>", result);
        Assert.Contains("<CALL:4>NV9U", result);
        Assert.Contains("<eor>", result);
    }

    [Fact]
    public void Empty_ToString_ReturnsEmpty()
    {
        var doc = new AdifDocument();
        Assert.Equal("", doc.ToString());
    }

    [Fact]
    public void SaveToFile_EmptyFileName_ThrowsException()
    {
        var doc = new AdifDocument();
        Assert.Throws<ArgumentException>(() => doc.SaveToFile(""));
    }

    [Fact]
    public void ReadFromFile_NonExistentFile_ThrowsException()
    {
        var doc = new AdifDocument();
        Assert.Throws<AdifFileException>(() => doc.ReadFromFile("/nonexistent/path/file.adi"));
    }

    [Fact]
    public void QsoCount_Empty_ReturnsZero()
    {
        var doc = new AdifDocument();
        Assert.Equal(0, doc.QsoCount);
    }

    [Fact]
    public void ThrowExceptionOnUnknownLine_Enabled()
    {
        var doc = new AdifDocument { ThrowExceptionOnUnknownLine = true };
        Assert.Throws<AdifParseException>(() =>
            doc.ReadFromString("<CALL:4>NV9U<BAND:3>80M"));
    }

    [Fact]
    public void Version_ReturnsNonEmpty()
    {
        var doc = new AdifDocument();
        var version = doc.Version;
        Assert.NotNull(version);
        Assert.NotEmpty(version);
    }

    private static string GetTempFilePath()
    {
        var path = Path.GetTempFileName();
        File.Delete(path); // GetTempFileName creates the file — remove it so SaveToFile can create it
        return path;
    }

    [Fact]
    public void SaveToFile_WritesContent()
    {
        var doc = new AdifDocument();
        doc.AddHeader("<PROGRAMID:9>AdifParser<EOH>");
        doc.AddQso("<CALL:4>NV9U<BAND:3>80M");

        var path = GetTempFilePath();
        try
        {
            doc.SaveToFile(path);
            var content = File.ReadAllText(path);
            Assert.Contains("<eoh>", content);
            Assert.Contains("<eor>", content);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void SaveToFile_Overwrite_Succeeds()
    {
        var doc = new AdifDocument();
        doc.AddQso("<CALL:4>NV9U<BAND:3>80M");

        var path = GetTempFilePath();
        try
        {
            doc.SaveToFile(path);
            doc.SaveToFile(path, overwrite: true);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void SaveToFile_NoOverwrite_ThrowsWhenFileExists()
    {
        var doc = new AdifDocument();
        doc.AddQso("<CALL:4>NV9U<BAND:3>80M");

        var path = GetTempFilePath();
        try
        {
            doc.SaveToFile(path);
            Assert.Throws<AdifFileException>(() => doc.SaveToFile(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Constructor_FromFile_LoadsContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "<PROGRAMID:9>AdifParser<EOH>\n<CALL:4>NV9U<BAND:3>80M<eor>");

            var doc = new AdifDocument(path);
            Assert.True(doc.HasHeader);
            Assert.Equal(1, doc.QsoCount);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ReadFromString_MultiLineQso()
    {
        var doc = new AdifDocument();
        // A QSO record can span multiple lines in ADIF files
        doc.ReadFromString("<CALL:4>NV9U\n<BAND:3>80M<eor>");

        Assert.Equal(1, doc.QsoCount);
        Assert.Equal(2, doc.Qsos[0].Count);
        Assert.Equal("CALL", doc.Qsos[0][0].Name);
        Assert.Equal("BAND", doc.Qsos[0][1].Name);
    }

    [Fact]
    public void ReadFromString_CancellationToken()
    {
        var doc = new AdifDocument();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() =>
            doc.ReadFromString("<CALL:4>NV9U<BAND:3>80M<eor>", cts.Token));
    }
}