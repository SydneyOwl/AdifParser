namespace AdifParser;

/// <summary>
/// ADIF Header class.
/// </summary>
public class AdifHeader : TokenCollection
{
    /// <summary>
    /// Text before the first &lt; in the header.
    /// </summary>
    public string Preamble { get; set; } = "";

    /// <summary>
    /// Instantiate an empty header.
    /// </summary>
    public AdifHeader()
    {
    }

    /// <summary>
    /// Instantiate a header from a raw ADIF string.
    /// </summary>
    public AdifHeader(string rawString)
    {
        Parse(rawString);
    }

    /// <summary>
    /// Instantiate a header with preamble text and a collection of fields.
    /// </summary>
    public AdifHeader(string preamble, TokenFieldList fields)
    {
        Preamble = preamble;
        if (fields != null)
            foreach (var item in fields)
                Add(new Token(item.TagName, item.Data));
    }

    /// <summary>
    /// Add a token to the header.
    /// </summary>
    public void Add(TokenField item)
    {
        Add(new Token(item.TagName, item.Data));
    }

    /// <summary>
    /// Populate this header from a raw ADIF string.
    /// </summary>
    public void Parse(string rawString)
    {
        var ltPosition = rawString.IndexOf('<');
        if (ltPosition == -1 && rawString != "")
        {
            Preamble = rawString;
        }
        else
        {
            if (ltPosition > 0)
            {
                Preamble = rawString.Substring(0, ltPosition);
                ParseLine(rawString.Substring(ltPosition), true);
            }
            else
            {
                ParseLine(rawString, true);
            }
        }
    }

    /// <summary>
    /// Return the header as a proper ADIF string.
    /// </summary>
    public override string ToString()
    {
        var pre = Preamble.Trim();
        return $"{pre}{(pre.Length == 0 ? "" : " ")}{base.ToString()}<eoh>";
    }
}
