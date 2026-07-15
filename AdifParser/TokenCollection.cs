using System.Collections;
using System.Text;

namespace AdifParser;

/// <summary>
/// Collection of ADIF tokens. Base class for AdifHeader and AdifQso.
/// </summary>
public class TokenCollection : IList<Token>, IReadOnlyList<Token>
{
    private readonly List<Token> _tokens = new();

    public TokenCollection()
    {
    }

    /// <summary>
    /// Instantiate a collection of tokens. Header or QSO is determined by the trailing tag.
    /// </summary>
    public TokenCollection(string line)
    {
        ParseLine(line);
    }

    /// <summary>
    /// Instantiate a collection of tokens. Header or token is specified.
    /// </summary>
    public TokenCollection(string line, bool isHeader)
    {
        ParseLine(line, isHeader);
    }

    /// <summary>
    /// Parse a line of text into tokens. Header or QSO is determined by the trailing tag.
    /// </summary>
    public void ParseLine(string line)
    {
        InternalParse(line.AsSpan(), line.EndsWith("<EOH>", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Parse a line of text into tokens. Header or token is specified.
    /// </summary>
    public void ParseLine(string line, bool isHeader)
    {
        InternalParse(line.AsSpan(), isHeader);
    }

    /// <summary>
    /// Internal span-based parsing of a line into individual tokens.
    /// </summary>
    private void InternalParse(ReadOnlySpan<char> line, bool isHeader)
    {
        var remaining = line;
        while (remaining.Length > 0)
        {
            var ltIndex = remaining.IndexOf('<');
            if (ltIndex < 0)
                break;

            var tokenSpan = remaining.Slice(ltIndex);
            var gtIndex = tokenSpan.IndexOf('>');
            if (gtIndex < 0)
                break;

            var tagContent = tokenSpan.Slice(1, gtIndex - 1);
            var nameEnd = tagContent.IndexOf(':');
            var tagName = nameEnd < 0 ? tagContent : tagContent.Slice(0, nameEnd);

            if (tagName.Length == 0)
                break;

            // Skip EOH and EOR terminators
            if (tagName.Equals("EOH".AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                tagName.Equals("EOR".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                remaining = tokenSpan.Slice(gtIndex + 1);
                continue;
            }

            if (nameEnd < 0)
                break;

            var lengthStart = nameEnd + 1;
            var lengthEnd = tagContent.Slice(lengthStart).IndexOf(':');
            if (lengthEnd < 0)
                lengthEnd = tagContent.Length - lengthStart;

            var lengthSpan = tagContent.Slice(lengthStart, lengthEnd);
            if (!int.TryParse(lengthSpan, out var length) || length < 0)
                throw new AdifParseException($"LENGTH must be an integer in the ADIF token string: {tokenSpan.ToString()}");

            var tokenEnd = gtIndex + 1 + length;
            if (tokenEnd > tokenSpan.Length)
                throw new AdifParseException($"The declared LENGTH exceeds the available data in the ADIF token string: {tokenSpan.ToString()}");

            var separatorEnd = SkipWhitespaceSeparator(tokenSpan, tokenEnd);
            if (separatorEnd < tokenSpan.Length && tokenSpan[separatorEnd] == '<')
            {
                _tokens.Add(new Token(tokenSpan.Slice(0, tokenEnd).ToString(), isHeader));
                remaining = tokenSpan.Slice(separatorEnd);
                continue;
            }

            if (tokenEnd < tokenSpan.Length && tokenSpan[tokenEnd] != '<')
            {
                var nextTag = tokenSpan.Slice(tokenEnd).IndexOf('<');
                tokenEnd = nextTag < 0 ? tokenSpan.Length : tokenEnd + nextTag;
            }

            tokenEnd = IncludeTrailingUserDefEnumeration(tokenSpan, tokenEnd, tagName);

            _tokens.Add(new Token(tokenSpan.Slice(0, tokenEnd).ToString(), isHeader));
            remaining = tokenSpan.Slice(tokenEnd);
        }
    }

    private static int SkipWhitespaceSeparator(ReadOnlySpan<char> tokenSpan, int index)
    {
        while (index < tokenSpan.Length && char.IsWhiteSpace(tokenSpan[index]))
            index++;

        return index;
    }

    private static int IncludeTrailingUserDefEnumeration(ReadOnlySpan<char> tokenSpan, int tokenEnd, ReadOnlySpan<char> tagName)
    {
        if (!tagName.StartsWith("USERDEF".AsSpan(), StringComparison.OrdinalIgnoreCase))
            return tokenEnd;

        if (tokenEnd + 1 >= tokenSpan.Length || tokenSpan[tokenEnd] != ',' || tokenSpan[tokenEnd + 1] != '{')
            return tokenEnd;

        var enumEnd = tokenSpan.Slice(tokenEnd + 2).IndexOf('}');
        if (enumEnd < 0)
            return tokenEnd;

        return tokenEnd + 2 + enumEnd + 1;
    }

    /// <summary>
    /// Add a token.
    /// </summary>
    public void AddToken(string tokenString, bool isHeader)
    {
        _tokens.Add(new Token(tokenString, isHeader));
    }

    /// <summary>
    /// Add a token.
    /// </summary>
    public void AddToken(string tokenString)
    {
        _tokens.Add(new Token(tokenString));
    }

    /// <summary>
    /// Add a token.
    /// </summary>
    public void AddToken(string tagName, string data, bool isHeader = false)
    {
        _tokens.Add(new Token(tagName, data, isHeader));
    }

    /// <summary>
    /// Add a token.
    /// </summary>
    public void AddToken(string tagName, string data, char dataType, string enumerations = "")
    {
        _tokens.Add(new Token(tagName, data, dataType, enumerations));
    }

    /// <summary>
    /// Add a token.
    /// </summary>
    public void AddToken(TokenField tokenField)
    {
        _tokens.Add(new Token(tokenField.TagName, tokenField.Data));
    }

    /// <summary>
    /// Add several tokens.
    /// </summary>
    public void AddTokens(TokenFieldList fieldList)
    {
        foreach (var item in fieldList)
            _tokens.Add(new Token(item.TagName, item.Data));
    }

    /// <summary>
    /// Return the token collection as a proper ADIF string.
    /// </summary>
    public override string ToString()
    {
        if (_tokens.Count == 0)
            return string.Empty;

        var sb = new StringBuilder(_tokens.Count * 32);
        foreach (var token in _tokens)
        {
            sb.Append(token);
            sb.Append(' ');
        }
        return sb.ToString();
    }

    /// <summary>
    /// Get the first token with the given tag name, or null if not found.
    /// </summary>
    /// <param name="name">Tag name to search for.</param>
    /// <param name="ignoreCase">Whether to ignore case (default true, per ADIF convention).</param>
    public Token? GetField(string name, bool ignoreCase = true)
    {
        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        foreach (var token in _tokens)
        {
            if (token.Name.Equals(name, comparison))
                return token;
        }
        return null;
    }

    /// <summary>
    /// Get the data value of the first token with the given tag name, or null if not found.
    /// </summary>
    /// <param name="name">Tag name to search for.</param>
    /// <param name="ignoreCase">Whether to ignore case (default true, per ADIF convention).</param>
    public string? GetFieldValue(string name, bool ignoreCase = true)
    {
        return GetField(name, ignoreCase)?.Data;
    }

    // ── IList<Token> / IReadOnlyList<Token> implementation ──

    public int Count => _tokens.Count;
    public bool IsReadOnly => false;

    public Token this[int index]
    {
        get => _tokens[index];
        set => _tokens[index] = value;
    }

    public void Add(Token item) => _tokens.Add(item);
    public void Clear() => _tokens.Clear();
    public bool Contains(Token item) => _tokens.Contains(item);
    public void CopyTo(Token[] array, int arrayIndex) => _tokens.CopyTo(array, arrayIndex);
    public int IndexOf(Token item) => _tokens.IndexOf(item);
    public void Insert(int index, Token item) => _tokens.Insert(index, item);
    public bool Remove(Token item) => _tokens.Remove(item);
    public void RemoveAt(int index) => _tokens.RemoveAt(index);

    public IEnumerator<Token> GetEnumerator() => _tokens.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _tokens.GetEnumerator();
}
