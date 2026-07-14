# AdifParser

A .NET library for parsing, building, and generating ADIF (Amateur Data Interchange Format) files. 

Based on the original [ADIFLib](https://github.com/kv9y/ADIFLib).

[test adi](AdifParser.Tests/ADIF_317_test_QSOs_2026_03_22.adi) is downloaded from [ADIF Specifications : Released Version 3.1.7](https://www.adif.org.uk/317/index.htm?utm_source=)

## Installation

```bash
dotnet add package AdifParser
```

## Quick Start

### Parse an ADIF file

```csharp
using AdifParser;

var doc = new AdifDocument("log.adi");
Console.WriteLine($"QSO count: {doc.QsoCount}");

foreach (var qso in doc.Qsos)
{
    foreach (var token in qso)
        Console.WriteLine($"{token.Name}: {token.Data}");
}
```

### Parse an ADIF string

```csharp
var doc = new AdifDocument();
doc.ReadFromString(@"Wavelog ADIF export
<ADIF_VER:5>3.1.7
<PROGRAMID:7>Wavelog
<PROGRAMVERSION:5>3.0.1
<EOH>

<BAND:3>10m
<CALL:6>XXXXXX
<CLUBLOG_QSO_UPLOAD_STATUS:1>Y
<CONT:2>EU
<CQZ:2>14
<DISTANCE:4>11.2
<DXCC:3>287
<EQSL_QSL_SENT:1>Y
<GRIDSQUARE:6>JN47QJ
<ITUZ:2>28
<LAT:9>47.395000
<LON:8>9.290000
<LOTW_QSL_SENT:1>Y
<MODE:3>SSB
<NAME:4>Kurt
<OPERATOR:6>4W7EST
<HRDLOG_QSO_UPLOAD_STATUS:1>Y
<QRZCOM_QSO_UPLOAD_STATUS:1>Y
<QSL_RCVD:1>N
<QSL_SENT:1>Y
<QSL_SENT_VIA:1>B
<QSL_VIA:18>QSL -BUREAU, E-QSL
<QTH:11>St.  Gallen
<RST_RCVD:2>59
<RST_SENT:2>59
<STATE:2>SG
<SUBMODE:3>USB
<TX_PWR:2>20
<LOTW_QSLSDATE:8>20230914
<QSLSDATE:8>20230906
<CLUBLOG_QSO_UPLOAD_DATE:8>20230906
<HRDLOG_QSO_UPLOAD_DATE:8>20231120
<QRZCOM_QSO_UPLOAD_DATE:8>20231103
<COUNTRY:11>Switzerland
<FREQ:6>28.695
<QSO_DATE:8>20201213
<TIME_ON:6>102900
<QSO_DATE_OFF:8>20201213
<TIME_OFF:6>102900
<STATION_CALLSIGN:6>4W7EST
<MY_CITY:4>Dili
<MY_COUNTRY:11>TIMOR-LESTE
<MY_DXCC:3>511
<MY_GRIDSQUARE:6>JN38RG
<MY_IOTA:6>OC-148
<MY_CQ_ZONE:2>28
<MY_ITU_ZONE:2>54
<EOR>");

Console.WriteLine(doc.HasHeader); // True
Console.WriteLine(doc.QsoCount);  // 1
```

### Build and save

```csharp
var doc = new AdifDocument();
doc.AddHeader("<PROGRAMID:9>AdifParser<EOH>");

var qso = new AdifQso();
qso.AddToken("CALL", "NV9U");
qso.AddToken("BAND", "80M");
qso.AddToken("MODE", "SSB");
doc.AddQso(qso);

doc.SaveToFile("output.adi");
```

## Usage Guide

### Working with the Header

```csharp
// Parse a header from a string
var header = new AdifHeader("<PROGRAMID:9>AdifParser<PROGRAMVERSION:3>2.0<EOH>");

Console.WriteLine(header[0].Name); // PROGRAMID
Console.WriteLine(header[0].Data); // AdifParser
Console.WriteLine(header[1].Name); // PROGRAMVERSION
Console.WriteLine(header[1].Data); // 2.0
```

```csharp
// Build a header programmatically
var header = new AdifHeader();
header.Add(new TokenField("PROGRAMID", "AdifParser"));
header.Add(new TokenField("PROGRAMVERSION", "2.0"));

Console.WriteLine(header);
// <PROGRAMID:9>AdifParser <PROGRAMVERSION:3>2.0 <eoh>
```

```csharp
// Headers can have preamble text before the first tag
var header = new AdifHeader("MY LOG EXPORT<PROGRAMID:9>AdifParser<EOH>");
Console.WriteLine(header.Preamble); // MY LOG EXPORT
```

### Working with QSOs

```csharp
// Parse a QSO
var qso = new AdifQso("<CALL:4>NV9U<BAND:3>80M<MODE:3>SSB");

Console.WriteLine(qso[0].Name); // CALL
Console.WriteLine(qso[0].Data); // NV9U
Console.WriteLine(qso.Count);   // 3
```

```csharp
// Build a QSO with TokenFieldList
var fields = new TokenFieldList
{
    new TokenField("CALL", "NV9U"),
    new TokenField("BAND", "80M"),
    new TokenField("FREQ", "3.750"),
    new TokenField("MODE", "SSB")
};
var qso = new AdifQso(fields);

Console.WriteLine(qso);
// <CALL:4>NV9U <BAND:3>80M <FREQ:5>3.750 <MODE:3>SSB <eor>
```

### Working with Tokens

```csharp
// Parse an individual token
var token = new Token("<CALL:4>NV9U");
Console.WriteLine(token.Name);   // CALL
Console.WriteLine(token.Data);   // NV9U
Console.WriteLine(token.Length); // 4
```

```csharp
// Build a token from parts
var token = new Token("CALL", "NV9U");
Console.WriteLine(token.ToString()); // <CALL:4>NV9U
```

```csharp
// Length auto-updates when data changes
var token = new Token("CALL", "NV9U");
Console.WriteLine(token.Length); // 4
token.Data = "W1AW";
Console.WriteLine(token.Length); // 4
token.Data = "LONGCALL";
Console.WriteLine(token.Length); // 8
```

### Working with USERDEF Fields

ADIF allows user-defined header fields with custom data types and enumerated values.

```csharp
// Parse a USERDEF token
var token = new Token("<USERDEF1:5:S>Hello,{A,B,C}", isHeader: true);
Console.WriteLine(token.Name);             // USERDEF1
Console.WriteLine(token.Data);             // Hello
Console.WriteLine(token.UserDefType);      // S
Console.WriteLine(token.EnumerationItems); // A,B,C
```

```csharp
// Build a USERDEF token
var token = new Token("USERDEF1", "Hello", 'S', "A,B,C");
Console.WriteLine(token.ToString()); // <USERDEF1:13:S>Hello,{A,B,C}
```

```csharp
// Build with UserDefField
var field = new UserDefField("USERDEF1", "Hello", 'S', "A,B,C");
```

### Adding QSOs to a Document

```csharp
var doc = new AdifDocument();

// From a raw string
doc.AddQso("<CALL:4>NV9U<BAND:3>80M<eor>");

// From an AdifQso object
var qso = new AdifQso();
qso.AddToken("CALL", "W1AW");
qso.AddToken("BAND", "20M");
doc.AddQso(qso);

Console.WriteLine(doc.QsoCount); // 2
```

### Saving and Exporting

```csharp
var doc = new AdifDocument();
doc.AddQso("<CALL:4>NV9U<BAND:3>80M");

// Save to file (throws AdifFileException if file exists)
doc.SaveToFile("export.adi");

// Save with overwrite
doc.SaveToFile("export.adi", overwrite: true);

// Get the ADIF string without saving
string result = doc.ToString();
// <CALL:4>NV9U <BAND:3>80M <eor>
```

### Error Handling

```csharp
var doc = new AdifDocument();

try
{
    doc.ReadFromFile("nonexistent.adi");
}
catch (AdifFileException ex)
{
    Console.WriteLine(ex.Message); // File does not exist: nonexistent.adi
}
```

```csharp
// Enable strict mode — throw on unrecognized lines
var doc = new AdifDocument { ThrowExceptionOnUnknownLine = true };
try
{
    doc.ReadFromString("<CALL:4>NV9U<BAND:3>80M"); // missing <eor>
}
catch (AdifParseException ex)
{
    Console.WriteLine(ex.Message); // Unknown line in ADIF file, line 0
}
```

### Reading from a Stream

```csharp
using var stream = File.OpenRead("log.adi");
var doc = new AdifDocument();
doc.ReadFromStream(stream);

Console.WriteLine(doc.QsoCount);
```

Also works with network streams, `MemoryStream`, or any `Stream`:

```csharp
var bytes = Encoding.UTF8.GetBytes("<CALL:4>NV9U<BAND:3>80M<eor>");
using var stream = new MemoryStream(bytes);
var doc = new AdifDocument();
doc.ReadFromStream(stream);
```

### Cancellation Support

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var doc = new AdifDocument();
doc.ReadFromString(largeAdifString, cts.Token);
```

### Multi-Line QSOs

The parser accumulates lines until it encounters an `<EOR>` or `<EOH>` terminator, so QSO records can span multiple lines:

```csharp
var doc = new AdifDocument();
doc.ReadFromString(
    "<CALL:4>NV9U\n" +
    "<BAND:3>80M\n" +
    "<MODE:3>SSB\n" +
    "<eor>");

Console.WriteLine(doc.QsoCount);     // 1
Console.WriteLine(doc.Qsos[0].Count); // 3
```

### Building Without a Document

You can use `TokenCollection` directly for low-level work:

```csharp
var tokens = new TokenCollection("<CALL:4>NV9U<BAND:3>80M<MODE:3>SSB");
Console.WriteLine(tokens.Count); // 3
```

```csharp
// Add from a TokenFieldList
var fields = new TokenFieldList
{
    new TokenField("CALL", "NV9U"),
    new TokenField("BAND", "80M")
};
var tokens = new TokenCollection();
tokens.AddTokens(fields);

Console.WriteLine(tokens);
// <CALL:4>NV9U <BAND:3>80M
```

## ADIF Format Overview

ADIF (Amateur Data Interchange Format) is the standard interchange format for amateur radio log data. Each field is a token in the form `<TagName:Length>Data`.

- **Header** fields end with `<EOH>`
- Each **QSO** (contact record) ends with `<EOR>`
- Example: `<CALL:4>NV9U<BAND:3>80M<eor>`

## Changes from ADIFLib

- `ReadOnlySpan<char>` token splitting — fewer allocations than `string.Split`
- Lazy `Length` evaluation — only recalculated on serialization
- Pre-sized `StringBuilder` buffers for `ToString()` output
- `StringComparison.OrdinalIgnoreCase` instead of `ToUpper()` for case-insensitive comparisons

## API Reference

### AdifDocument

| Member | Description |
|--------|-------------|
| `Header` | Gets or sets the ADIF header |
| `Qsos` | The collection of QSO records |
| `HasHeader` | Whether the document has a header |
| `QsoCount` | Number of QSOs in the document |
| `ThrowExceptionOnUnknownLine` | Throw on unrecognized lines |
| `Version` | Library version string |
| `ReadFromFile(string)` | Parse an ADIF file |
| `ReadFromStream(Stream, CancellationToken)` | Parse an ADIF stream |
| `ReadFromString(string, CancellationToken)` | Parse an ADIF string |
| `SaveToFile(string, bool)` | Save to file |
| `AddHeader(string)` / `AddHeader(AdifHeader)` | Set the header |
| `AddQso(string)` / `AddQso(AdifQso)` | Add a QSO |
| `ToString()` | Serialize to ADIF string |

### AdifHeader (extends TokenCollection)

| Member | Description |
|--------|-------------|
| `Preamble` | Text before the first `<` tag |
| `Parse(string)` | Parse a raw header string |
| `Add(TokenField)` | Add a field to the header |

### AdifQso (extends TokenCollection)

| Member | Description |
|--------|-------------|
| `Parse(string)` | Parse a raw QSO string |
| Constructor `(TokenFieldList)` | Build from field list |

### Token

| Member | Description |
|--------|-------------|
| `Name` | Tag name (e.g. "CALL") |
| `Data` | Tag value (e.g. "NV9U") |
| `Length` | Data length (auto-calculated) |
| `IsHeader` | Whether this is a header token |
| `UserDefType` | USERDEF data type character |
| `EnumerationItems` | USERDEF enumeration values |

### TokenCollection

| Member | Description |
|--------|-------------|
| `ParseLine(string, bool)` | Parse a line into tokens |
| `AddToken(string, string, bool)` | Add a token by name and data |
| `AddToken(TokenField)` | Add a token from a field |
| `AddTokens(TokenFieldList)` | Add multiple tokens |

### Exception Types

| Exception | Thrown when |
|-----------|-------------|
| `AdifParseException` | Invalid ADIF format or structure |
| `AdifFileException` | File operation errors |

## License

MIT