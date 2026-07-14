namespace AdifParser;

/// <summary>
/// ADIF QSO (contact record) class.
/// </summary>
public class AdifQso : TokenCollection
{
    /// <summary>
    /// Instantiate an empty QSO.
    /// </summary>
    public AdifQso()
    {
    }

    /// <summary>
    /// Instantiate a QSO from a raw ADIF string.
    /// </summary>
    public AdifQso(string rawString)
    {
        Parse(rawString);
    }

    /// <summary>
    /// Instantiate a QSO from a collection of name/data fields.
    /// </summary>
    public AdifQso(TokenFieldList fields)
    {
        foreach (var item in fields)
            Add(new Token(item.TagName, item.Data));
    }

    /// <summary>
    /// Populate this QSO from a raw ADIF string.
    /// </summary>
    public void Parse(string rawString)
    {
        ParseLine(rawString, false);
    }

    /// <summary>
    /// Return the QSO as a proper ADIF string.
    /// </summary>
    public override string ToString()
    {
        return $"{base.ToString()}<eor>";
    }
}