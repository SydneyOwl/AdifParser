namespace AdifParser;

/// <summary>
/// ADIF Header Class.
/// </summary>
public class ADIFHeader : TokenCollection
{
    /// <summary>
    /// Text from the header before the first &lt;
    /// </summary>
    public string HeaderPreText = "";

    /// <summary>
    /// Instantiate ADIFHeader with no initial population.
    /// </summary>
    public ADIFHeader()
    {
    }

    /// <summary>
    /// Instantiate ADIFHeader with an initial QSO string to parse.
    /// </summary>
    public ADIFHeader(string ParseThisString)
    {
        ParseStringToADIFHeader(ParseThisString);
    }

    /// <summary>
    /// Instantiate ADIFHeader with header pre text and a collection of name/data items.
    /// </summary>
    public ADIFHeader(string HeaderPreText, TokenNameDataList TagNameDataListForThisQSO)
    {
        this.HeaderPreText = HeaderPreText;
        if (TagNameDataListForThisQSO != null)
            foreach (var anItem in TagNameDataListForThisQSO)
                Add(new Token(anItem.TagName, anItem.Data));
    }

    /// <summary>
    /// Add a token to the header using TokenNameData.
    /// </summary>
    public void Add(TokenNameData NewItem)
    {
        Add(new Token(NewItem.TagName, NewItem.Data));
    }

    /// <summary>
    /// Populate this ADIFHeader with this header string.
    /// </summary>
    public void ParseStringToADIFHeader(string ParseThisString)
    {
        var ltPosition = ParseThisString.IndexOf('<');
        if (ltPosition == -1 && ParseThisString != "")
        {
            HeaderPreText = ParseThisString;
        }
        else
        {
            if (ltPosition > 0)
            {
                HeaderPreText = ParseThisString.Substring(0, ltPosition);
                PullApartLine(ParseThisString.Substring(ltPosition), true);
            }
            else
            {
                PullApartLine(ParseThisString, true);
            }
        }
    }

    /// <summary>
    /// Return the Header as a proper ADIF string.
    /// </summary>
    public override string ToString()
    {
        var pre = HeaderPreText.Trim();
        return $"{pre}{(pre.Length == 0 ? "" : " ")}{base.ToString()}<eoh>";
    }
}