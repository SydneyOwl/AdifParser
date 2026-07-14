namespace AdifParser;

/// <summary>
/// ADIF QSO Class.
/// </summary>
public class ADIFQSO : TokenCollection
{
    /// <summary>
    /// Instantiate ADIFQSO with no initial population.
    /// </summary>
    public ADIFQSO()
    {
    }

    /// <summary>
    /// Instantiate ADIFQSO with an initial QSO string to parse.
    /// </summary>
    public ADIFQSO(string ParseThisString)
    {
        ParseStringToADIFQSO(ParseThisString);
    }

    /// <summary>
    /// Instantiate ADIFQSO with an initial collection of name/data items.
    /// </summary>
    public ADIFQSO(TokenNameDataList TagNameDataListForThisQSO)
    {
        foreach (var anItem in TagNameDataListForThisQSO)
            Add(new Token(anItem.TagName, anItem.Data));
    }

    /// <summary>
    /// Populate this ADIFQSO with this QSO string.
    /// </summary>
    public void ParseStringToADIFQSO(string ParseThisString)
    {
        PullApartLine(ParseThisString, false);
    }

    /// <summary>
    /// Return the QSO as a proper ADIF string.
    /// </summary>
    public override string ToString()
    {
        return $"{base.ToString()}<eor>";
    }
}