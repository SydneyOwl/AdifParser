using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace AdifParser;

/// <summary>
/// Main ADIF Class: represents an ADIF file or collection of QSO records.
/// </summary>
public class ADIF
{
    /// <summary>
    /// The ADIF header.
    /// </summary>
    public ADIFHeader? TheADIFHeader;

    /// <summary>
    /// The collection of QSO records within the ADIF.
    /// </summary>
    public ADIFQSOCollection TheQSOs = new();

    /// <summary>
    /// Should an exception be thrown when a non-blank line doesn't end with &lt;eoh&gt; or &lt;eor&gt;?
    /// </summary>
    public bool ThrowExceptionOnUnknownLine;

    /// <summary>
    /// Instantiate an empty ADIF.
    /// </summary>
    public ADIF()
    {
    }

    /// <summary>
    /// Instantiate an ADIF and populate it from the contents of specified file.
    /// </summary>
    public ADIF(string FileName)
    {
        ReadFromFile(FileName);
    }

    /// <summary>
    /// Does the ADIF have a header?
    /// </summary>
    public bool HasHeader => TheADIFHeader != null;

    /// <summary>
    /// Number of QSOs within the ADIF.
    /// </summary>
    public int QSOCount => TheQSOs.Count;

    /// <summary>
    /// Get AdifParser version.
    /// </summary>
    public string Version
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVerInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVerInfo.FileVersion ?? "0.0.0";
        }
    }

    /// <summary>
    /// Add the passed header to the ADIF.
    /// </summary>
    public void AddHeader(ADIFHeader Header)
    {
        TheADIFHeader = Header;
    }

    /// <summary>
    /// Parse and add the passed string as the ADIF header.
    /// </summary>
    public void AddHeader(string RawHeader)
    {
        TheADIFHeader = new ADIFHeader(RawHeader);
    }

    /// <summary>
    /// Add the passed QSO to the ADIF.
    /// </summary>
    public void AddQSO(ADIFQSO QSO)
    {
        TheQSOs.Add(QSO);
    }

    /// <summary>
    /// Parse and add the passed string as an ADIF QSO.
    /// </summary>
    public void AddQSO(string RawQSO)
    {
        TheQSOs.Add(new ADIFQSO(RawQSO));
    }

    /// <summary>
    /// Save the ADIF to a file.
    /// </summary>
    public void SaveToFile(string FileName, bool OverWrite = false)
    {
        if (string.IsNullOrEmpty(FileName))
            throw new ArgumentException("Filename cannot be empty!", nameof(FileName));

        if (!OverWrite && File.Exists(FileName))
            throw new Exception($"File already exists: {FileName}");

        File.WriteAllText(FileName, ToString());
    }

    /// <summary>
    /// Read a file into an ADIF object.
    /// </summary>
    public void ReadFromFile(string FileName)
    {
        if (!File.Exists(FileName))
            throw new Exception($"File does not exist: {FileName}");

        uint lineNumber = 0;
        using var stream = new StreamReader(FileName);
        try
        {
            ReadFromStream(stream, ref lineNumber, CancellationToken.None);
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex.Message} {FileName}:({lineNumber})", ex);
        }
    }

    /// <summary>
    /// Parse an ADIF string.
    /// </summary>
    public void ReadFromString(string adifString, CancellationToken cancellationToken = default)
    {
        uint lineNumber = 0;
        var byteArray = Encoding.UTF8.GetBytes(adifString);
        using var memoryStream = new MemoryStream(byteArray);
        using var streamReader = new StreamReader(memoryStream);

        try
        {
            ReadFromStream(streamReader, ref lineNumber, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error parsing ADIF string at line {lineNumber}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Return the entire ADIF as an ADIF formatted string.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        if (HasHeader)
            sb.Append(TheADIFHeader);
        sb.Append(TheQSOs);
        return sb.ToString();
    }

    /// <summary>
    /// Read from a stream. Allows multiple lines per header or QSO.
    /// </summary>
    private void ReadFromStream(StreamReader TheStream, ref uint LineNumber, CancellationToken cancellation)
    {
        var theLine = new StringBuilder();

        while (!TheStream.EndOfStream && !cancellation.IsCancellationRequested)
        {
            var curLine = TheStream.ReadLine();
            if (curLine == null)
                break;

            curLine = curLine.Trim();

            // Skip lines where someone's name contains < — avoid naughty cases
            if (curLine.Contains("<NAME", StringComparison.OrdinalIgnoreCase))
            {
                LineNumber++;
                continue;
            }

            theLine.Append(curLine);
            var lineStr = theLine.ToString();

            if (lineStr.Length == 0)
                continue;

            if (lineStr.EndsWith("<EOH>", StringComparison.OrdinalIgnoreCase))
            {
                if (TheADIFHeader != null)
                    throw new Exception($"File cannot contain more than one header. See line {LineNumber}");

                TheADIFHeader = new ADIFHeader(lineStr);
                LineNumber++;
                theLine.Clear();
            }
            else if (lineStr.EndsWith("<EOR>", StringComparison.OrdinalIgnoreCase))
            {
                TheQSOs.Add(new ADIFQSO(lineStr));
                LineNumber++;
                theLine.Clear();
            }
            else if (ThrowExceptionOnUnknownLine)
            {
                throw new Exception($"Unknown line in ADIF file, line {LineNumber}");
            }
        }
    }
}