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
    private char _userDefType = ' ';

    /// <summary>
    /// Instantiate an empty token.
    /// </summary>
    public Token()
    {
    }

    /// <summary>
    /// Instantiate an ADIF Token.
    /// </summary>
    /// <param name="tokenString">Single token string assuming there are no embedded special characters.</param>
    /// <param name="isHeader">Specifies whether this token is a header or QSO token.</param>
    public Token(string tokenString, bool isHeader)
    {
        IsHeader = isHeader;
        if (tokenString.Trim() != "")
            ParseToken(tokenString);
    }

    /// <summary>
    /// Instantiate an ADIF Token.
    /// </summary>
    /// <param name="tokenString">Single token string assuming there are no embedded special characters.</param>
    public Token(string tokenString)
    {
        if (tokenString.Trim() != "")
            ParseToken(tokenString);
    }

    /// <summary>
    /// Instantiate an ADIF token with tag name and data.
    /// </summary>
    public Token(string tagName, string data, bool isHeader = false)
    {
        _name = tagName;
        _data = data;
        _isHeader = isHeader;
        _lengthNeedsUpdate = true;
    }

    /// <summary>
    /// Instantiate an ADIF token with specific header items.
    /// </summary>
    public Token(string tagName, string data, char dataType, string enumerations = "")
    {
        _name = tagName;
        _data = data;
        _isHeader = true;
        _enumerationItems = enumerations;
        _userDefType = dataType;
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

    public char UserDefType
    {
        get => _userDefType;
        set
        {
            _userDefType = value;
            _lengthNeedsUpdate = true;
        }
    }

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

    private void ParseToken(string tokenString)
    {
        var span = tokenString.AsSpan().TrimStart();
        if (span.Length > 0 && span[0] == '<')
            span = span.Slice(1);

        // Parse Name: advance until ':'
        int colonIdx = span.IndexOf(':');
        if (colonIdx < 0)
            throw new AdifParseException($"Invalid ADIF token string: {tokenString}");

        _name = span.Slice(0, colonIdx).ToString();
        span = span.Slice(colonIdx + 1);

        // Parse Length: advance until ':' or '>'
        int lenEnd = span.IndexOfAny(':', '>');
        if (lenEnd < 0)
            throw new AdifParseException($"The LENGTH is required in the ADIF token string: {tokenString}");

        var lengthSpan = span.Slice(0, lenEnd);
        if (lengthSpan.Length == 0)
            throw new AdifParseException($"The LENGTH is required in the ADIF token string: {tokenString}");

        if (!uint.TryParse(lengthSpan, out _length))
            throw new AdifParseException($"LENGTH must be an integer in the ADIF token string: {tokenString}");

        _lengthNeedsUpdate = false;
        span = span.Slice(lenEnd);

        // Is there an ENUMERATIONTYPE? (':' following the length)
        if (span[0] == ':')
        {
            span = span.Slice(1);
            _userDefType = span[0];
            span = span.Slice(1);
        }

        // Current character should be '>'
        if (span[0] != '>')
            throw new AdifParseException($"Invalid ADIF token string: {tokenString}");

        span = span.Slice(1);

        // Get value string using the declared ADIF length. Field data may contain
        // angle brackets, so delimiter scanning would corrupt valid values.
        if (_length > (uint)span.Length)
            throw new AdifParseException($"The declared LENGTH exceeds the available data in the ADIF token string: {tokenString}");

        var valueLength = (int)_length;
        if (valueLength < span.Length && span[valueLength] != '<')
        {
            var nextTag = span.Slice(valueLength).IndexOf('<');
            valueLength = nextTag < 0 ? span.Length : valueLength + nextTag;
        }

        var valueSpan = span.Slice(0, valueLength);
        span = span.Slice(valueLength);

        var enumSpan = ReadOnlySpan<char>.Empty;
        if (_name.StartsWith("USERDEF", StringComparison.OrdinalIgnoreCase))
        {
            var commaIndex = valueSpan.IndexOf(',');
            if (commaIndex >= 0)
            {
                enumSpan = valueSpan.Slice(commaIndex + 1);
                valueSpan = valueSpan.Slice(0, commaIndex);
            }
        }

        _data = valueSpan.ToString();

        // Are there enumerations?
        if (enumSpan.Length > 0)
        {
            ParseEnumeration(enumSpan, tokenString);
        }
        else if (span.Length > 0 && span[0] == ',')
        {
            span = span.Slice(1);
            ParseEnumeration(span, tokenString);
        }
    }

    private void ParseEnumeration(ReadOnlySpan<char> span, string tokenString)
    {
        if (span.Length > 0 && span[0] == '{')
        {
            span = span.Slice(1);
            int enumEnd = span.IndexOf('}');
            if (enumEnd < 0)
                throw new AdifParseException($"Unexpected data after value: {tokenString}");

            _enumerationItems = span.Slice(0, enumEnd).ToString();
        }
        else
        {
            throw new AdifParseException($"Unexpected data after value: {tokenString}");
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

        if (_isHeader && _userDefType != ' ')
        {
            sb.Append(':');
            sb.Append(_userDefType);
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
