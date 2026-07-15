using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace AdifParser;

/// <summary>
/// Represents an ADIF document: a header and a collection of QSO records.
/// </summary>
public class AdifDocument
{
    /// <summary>
    /// The ADIF header.
    /// </summary>
    public AdifHeader? Header { get; set; }

    /// <summary>
    /// The collection of QSO records.
    /// </summary>
    public AdifQsoCollection Qsos { get; } = new();

    /// <summary>
    /// Should an exception be thrown when a non-blank line doesn't end with &lt;eoh&gt; or &lt;eor&gt;?
    /// </summary>
    public bool ThrowExceptionOnUnknownLine { get; set; }

    /// <summary>
    /// Throw when a header or QSO record is malformed. If false, malformed records are skipped.
    /// </summary>
    public bool FailFastOnParseError { get; set; } = true;

    /// <summary>
    /// Instantiate an empty ADIF document.
    /// </summary>
    public AdifDocument()
    {
    }

    /// <summary>
    /// Instantiate an ADIF document and populate it from the specified file.
    /// </summary>
    public AdifDocument(string fileName)
    {
        ReadFromFile(fileName);
    }

    /// <summary>
    /// Does the document have a header?
    /// </summary>
    public bool HasHeader => Header != null;

    /// <summary>
    /// Number of QSOs in the document.
    /// </summary>
    public int QsoCount => Qsos.Count;

    /// <summary>
    /// Get the AdifParser library version.
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
    /// Set the document header.
    /// </summary>
    public void AddHeader(AdifHeader header)
    {
        Header = header;
    }

    /// <summary>
    /// Parse and set the document header from a raw string.
    /// </summary>
    public void AddHeader(string rawHeader)
    {
        Header = new AdifHeader(rawHeader);
    }

    /// <summary>
    /// Add a QSO to the document.
    /// </summary>
    public void AddQso(AdifQso qso)
    {
        Qsos.Add(qso);
    }

    /// <summary>
    /// Parse and add a QSO from a raw string.
    /// </summary>
    public void AddQso(string rawQso)
    {
        Qsos.Add(new AdifQso(rawQso));
    }

    /// <summary>
    /// Save the document to a file.
    /// </summary>
    public void SaveToFile(string fileName, bool overwrite = false)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("Filename cannot be empty!", nameof(fileName));

        if (!overwrite && File.Exists(fileName))
            throw new AdifFileException($"File already exists: {fileName}");

        File.WriteAllText(fileName, ToString());
    }

    /// <summary>
    /// Read a file into this document.
    /// </summary>
    public void ReadFromFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("Filename cannot be null or empty.", nameof(fileName));

        uint lineNumber = 0;
        try
        {
            using var stream = new StreamReader(fileName);
            ReadFromStream(stream, ref lineNumber, CancellationToken.None);
        }
        catch (FileNotFoundException ex)
        {
            throw new AdifFileException($"File does not exist: {fileName}", ex);
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new AdifFileException($"File does not exist: {fileName}", ex);
        }
        catch (AdifParseException ex)
        {
            throw new AdifFileException($"{ex.Message} {fileName}:({lineNumber})", ex);
        }
    }

    /// <summary>
    /// Read a stream into this document.
    /// </summary>
    public void ReadFromStream(Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        uint lineNumber = 0;
        using var reader = new StreamReader(stream, leaveOpen: true);
        try
        {
            ReadFromStream(reader, ref lineNumber, cancellationToken);
        }
        catch (AdifParseException ex)
        {
            throw new AdifParseException($"Error parsing ADIF stream at line {lineNumber}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Parse an ADIF string into this document.
    /// </summary>
    public void ReadFromString(string adifString, CancellationToken cancellationToken = default)
    {
        uint lineNumber = 0;
        using var stringReader = new StringReader(adifString);

        try
        {
            ReadFromStream(stringReader, ref lineNumber, cancellationToken);
        }
        catch (AdifParseException ex)
        {
            throw new AdifParseException($"Error parsing ADIF string at line {lineNumber}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Return the entire document as an ADIF-formatted string.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        if (HasHeader)
            sb.Append(Header);
        sb.Append(Qsos);
        return sb.ToString();
    }

    /// <summary>
    /// Read from a text reader. Allows multiple lines per header or QSO.
    /// </summary>
    private void ReadFromStream(TextReader stream, ref uint lineNumber, CancellationToken cancellation)
    {
        var lineBuilder = new StringBuilder();

        while (true)
        {
            cancellation.ThrowIfCancellationRequested();

            var curLine = stream.ReadLine();
            if (curLine == null)
                break;

            curLine = curLine.Trim();

            // Skip ADIF NAME lines (operator name can contain angle brackets)
            if (curLine.StartsWith("<NAME:", StringComparison.OrdinalIgnoreCase))
            {
                lineNumber++;
                continue;
            }

            if (lineBuilder.Length > 0)
                lineBuilder.Append("\r\n");
            lineBuilder.Append(curLine);
            var lineStr = lineBuilder.ToString();

            if (lineStr.Length == 0)
                continue;

            if (lineStr.EndsWith("<EOH>", StringComparison.OrdinalIgnoreCase))
            {
                if (Header != null)
                {
                    if (FailFastOnParseError)
                        throw new AdifParseException($"File cannot contain more than one header. See line {lineNumber}");

                    lineNumber++;
                    lineBuilder.Clear();
                    continue;
                }

                if (!TryParseHeader(lineStr))
                {
                    lineNumber++;
                    lineBuilder.Clear();
                    continue;
                }
                lineNumber++;
                lineBuilder.Clear();
            }
            else if (lineStr.EndsWith("<EOR>", StringComparison.OrdinalIgnoreCase))
            {
                if (!TryParseQso(lineStr))
                {
                    lineNumber++;
                    lineBuilder.Clear();
                    continue;
                }
                lineNumber++;
                lineBuilder.Clear();
            }
            else if (ThrowExceptionOnUnknownLine)
            {
                throw new AdifParseException($"Unknown line in ADIF file, line {lineNumber}");
            }
        }
    }

    private bool TryParseHeader(string line)
    {
        try
        {
            Header = new AdifHeader(line);
            return true;
        }
        catch (AdifParseException)
        {
            if (FailFastOnParseError)
                throw;

            return false;
        }
    }

    private bool TryParseQso(string line)
    {
        try
        {
            Qsos.Add(new AdifQso(line));
            return true;
        }
        catch (AdifParseException)
        {
            if (FailFastOnParseError)
                throw;

            return false;
        }
    }
}
