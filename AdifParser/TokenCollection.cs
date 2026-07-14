using System.Text;

namespace AdifParser;

/// <summary>
/// Collection of QSO tokens. Base class for ADIFHeader and ADIFQSO.
/// </summary>
public class TokenCollection : List<Token>
{
    public TokenCollection()
    {
    }

    /// <summary>
    /// Instantiate a collection of tokens. Header or QSO is determined by the trailing tag.
    /// </summary>
    public TokenCollection(string LineToPullApart)
    {
        PullApartLine(LineToPullApart);
    }

    /// <summary>
    /// Instantiate a collection of tokens. Header or token is specified.
    /// </summary>
    public TokenCollection(string LineToPullApart, bool IsHeader)
    {
        PullApartLine(LineToPullApart, IsHeader);
    }

    /// <summary>
    /// Pull apart a line of text and parse into a collection of tokens. Header or QSO is determined by the trailing tag.
    /// </summary>
    public void PullApartLine(string LineToPullApart)
    {
        InternalPullApart(LineToPullApart.AsSpan(), LineToPullApart.EndsWith("<EOH>", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Pull apart a line of text and parse into a collection of tokens. Header or token is specified.
    /// </summary>
    public void PullApartLine(string LineToPullApart, bool IsHeader)
    {
        InternalPullApart(LineToPullApart.AsSpan(), IsHeader);
    }

    /// <summary>
    /// Internal method that performs the function of pulling apart the passed string into individual tokens.
    /// Uses span-based parsing to avoid Split allocations.
    /// </summary>
    private void InternalPullApart(ReadOnlySpan<char> line, bool IsHeader)
    {
        // Manual span-based split on '<' to avoid string.Split allocations
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

            // Peek at the tag name between < and : or >
            var tagEnd = tokenSpan.Slice(1).IndexOfAny(':', '>');
            if (tagEnd < 0)
                break;

            var tagName = tokenSpan.Slice(1, tagEnd);

            // Skip EOH and EOR terminators
            if (!tagName.Equals("EOH".AsSpan(), StringComparison.OrdinalIgnoreCase) &&
                !tagName.Equals("EOR".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                Add(new Token(tokenSpan.ToString(), IsHeader));
            }

            remaining = tokenSpan.Slice(gtIndex + 1);
        }
    }

    /// <summary>
    /// Add a token.
    /// </summary>
    public void AddToken(string TokenString, bool IsHeader)
    {
        Add(new Token(TokenString, IsHeader));
    }

    /// <summary>
    /// Add a token.
    /// </summary>
    public void AddToken(string TokenString)
    {
        Add(new Token(TokenString));
    }

    /// <summary>
    /// Add a token.
    /// </summary>
    public void AddToken(string TagName, string Data, bool IsHeader = false)
    {
        Add(new Token(TagName, Data, IsHeader));
    }

    /// <summary>
    /// Add a token.
    /// </summary>
    public void AddToken(string TagName, string Data, char DataType, string Enumerations = "")
    {
        Add(new Token(TagName, Data, DataType, Enumerations));
    }

    /// <summary>
    /// Add a token.
    /// </summary>
    public void AddToken(TokenNameData TheTokenData)
    {
        Add(new Token(TheTokenData.TagName, TheTokenData.Data));
    }

    /// <summary>
    /// Add several tokens.
    /// </summary>
    public void AddTokens(TokenNameDataList TheTokenNameDataList)
    {
        foreach (var AnItem in TheTokenNameDataList)
            Add(new Token(AnItem.TagName, AnItem.Data));
    }

    /// <summary>
    /// Return the token collection as a proper ADIF string.
    /// </summary>
    public override string ToString()
    {
        if (Count == 0)
            return string.Empty;

        var sb = new StringBuilder(Count * 32);
        foreach (var thisToken in this)
        {
            sb.Append(thisToken);
            sb.Append(' ');
        }
        return sb.ToString();
    }
}