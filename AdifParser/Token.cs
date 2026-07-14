using System.Diagnostics;
using System.Text;

namespace AdifParser;

/// <summary>
/// ADIF Token: represents a single ADIF tag like <![CDATA[<CALL:4>NV9U]]> or <![CDATA[<BAND:3>80M]]>.
/// </summary>
public class Token
{
    private string _name = "";
    private string _data = "";
    private string _enumerationItems = "";
    private bool _lengthNeedsUpdate = true;
    private uint _length;

    /// <summary>
    /// Instantiate an empty token.
    /// </summary>
    public Token()
    {
    }

    /// <summary>
    /// Instantiate an ADIF Token.
    /// </summary>
    /// <param name="TokenString">Single token string assuming there are no embedded special characters.</param>
    /// <param name="IsHeader">Specifies whether this token is a header or QSO token.</param>
    public Token(string TokenString, bool IsHeader)
    {
        this.IsHeader = IsHeader;
        if (TokenString.Trim() != "")
            ParseToken(TokenString);
    }

    /// <summary>
    /// Instantiate an ADIF Token.
    /// </summary>
    /// <param name="TokenString">Single token string assuming there are no embedded special characters.</param>
    public Token(string TokenString)
    {
        if (TokenString.Trim() != "")
            ParseToken(TokenString);
    }

    /// <summary>
    /// Instantiate an ADIF token with Tag Name and Data.
    /// </summary>
    public Token(string TagName, string Data, bool IsHeader = false)
    {
        _name = TagName;
        _data = Data;
        _isHeader = IsHeader;
        _lengthNeedsUpdate = true;
    }

    /// <summary>
    /// Instantiate an ADIF token with specific header items.
    /// </summary>
    public Token(string TagName, string Data, char DataType, string Enumerations = "")
    {
        _name = TagName;
        _data = Data;
        _isHeader = true;
        _enumerationItems = Enumerations;
        UserDefType = DataType;
        _lengthNeedsUpdate = true;
    }

    public string Name
    {
        get => _name.Trim();
        set
        {
            _name = value;
            _lengthNeedsUpdate = true;
        }
    }

    public uint Length
    {
        get
        {
            if (_lengthNeedsUpdate)
                UpdateLength();
            return _length;
        }
        set
        {
            _length = value;
            _lengthNeedsUpdate = false;
        }
    }

    public char UserDefType { get; set; } = ' ';

    public string Data
    {
        get => _data;
        set
        {
            _data = value;
            _lengthNeedsUpdate = true;
        }
    }

    public string EnumerationItems
    {
        get => _enumerationItems;
        set
        {
            _enumerationItems = value;
            _lengthNeedsUpdate = true;
        }
    }

    private bool _isHeader;
    public bool IsHeader
    {
        get => _isHeader;
        set
        {
            _isHeader = value;
            _lengthNeedsUpdate = true;
        }
    }

    private void ParseToken(string TokenString)
    {
        var span = TokenString.AsSpan().TrimStart();
        if (span.Length > 0 && span[0] == '<')
            span = span.Slice(1);

        // Parse Name: advance until ':'
        int colonIdx = span.IndexOf(':');
        if (colonIdx < 0)
            throw new Exception($"Invalid ADIF token string: {TokenString}");

        _name = span.Slice(0, colonIdx).ToString();
        span = span.Slice(colonIdx + 1);

        // Parse Length: advance until ':' or '>'
        int lenEnd = span.IndexOfAny(':', '>');
        if (lenEnd < 0)
            throw new Exception($"The LENGTH is required in the ADIF token string: {TokenString}");

        var lengthSpan = span.Slice(0, lenEnd);
        if (lengthSpan.Length == 0)
            throw new Exception($"The LENGTH is required in the ADIF token string: {TokenString}");

        if (!uint.TryParse(lengthSpan, out _length))
            throw new Exception($"LENGTH must be an integer in the ADIF token string: {TokenString}");

        _lengthNeedsUpdate = false;
        span = span.Slice(lenEnd);

        // Is there an ENUMERATIONTYPE? (':' following the length)
        if (span[0] == ':')
        {
            span = span.Slice(1);
            UserDefType = span[0];
            span = span.Slice(1);
        }

        // Current character should be '>'
        if (span[0] != '>')
            throw new Exception($"Invalid ADIF token string: {TokenString}");

        span = span.Slice(1);

        // Get value string: look for '<', or ',' for USERDEF tags
        int dataEnd = 0;
        while (dataEnd < span.Length && span[dataEnd] != '<')
        {
            if (span[dataEnd] == ',' && _name.StartsWith("USERDEF", StringComparison.OrdinalIgnoreCase))
                break;
            dataEnd++;
        }

        _data = span.Slice(0, dataEnd).ToString();
        span = span.Slice(dataEnd);

        // Are there enumerations?
        if (span.Length > 0 && span[0] == ',')
        {
            span = span.Slice(1);
            if (span.Length > 0 && span[0] == '{')
            {
                span = span.Slice(1);
                int enumEnd = span.IndexOf('}');
                if (enumEnd < 0)
                    throw new Exception($"Unexpected data after value: {TokenString}");

                _enumerationItems = span.Slice(0, enumEnd).ToString();
            }
            else
            {
                throw new Exception($"Unexpected data after value: {TokenString}");
            }
        }
    }

    /// <summary>
    /// Return the properly constructed ADIF tag.
    /// </summary>
    public override string ToString()
    {
        if (_isHeader)
            UpdateLength();

        var sb = new StringBuilder(128);
        sb.Append('<');
        sb.Append(_name);
        sb.Append(':');
        sb.Append(_length);

        if (_isHeader && UserDefType != ' ')
        {
            sb.Append(':');
            sb.Append(UserDefType);
        }

        sb.Append('>');
        sb.Append(_data);

        if (_isHeader && _enumerationItems.Length > 0)
        {
            sb.Append(",{");
            sb.Append(_enumerationItems);
            sb.Append('}');
        }

        return sb.ToString();
    }

    private void UpdateLength()
    {
        _length = (uint)_data.Length;
        if (_isHeader && _name.StartsWith("USERDEF", StringComparison.OrdinalIgnoreCase) && _enumerationItems.Length > 0)
            _length += 3 + (uint)_enumerationItems.Length; // comma + {..}
        _lengthNeedsUpdate = false;
    }
}