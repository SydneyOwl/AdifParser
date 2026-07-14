namespace AdifParser;

/// <summary>
/// A name/data pair used for building ADIF tokens programmatically.
/// </summary>
public class TokenField
{
    public string TagName { get; set; } = "";
    public string Data { get; set; } = "";

    public TokenField()
    {
    }

    public TokenField(string tagName, string data)
    {
        TagName = tagName;
        Data = data;
    }
}

/// <summary>
/// A list of <see cref="TokenField"/> items.
/// </summary>
public class TokenFieldList : List<TokenField>
{
}

/// <summary>
/// Represents a USERDEF header field with its type and enumeration metadata.
/// </summary>
public class UserDefField
{
    public string TagName { get; set; } = "";
    public string Data { get; set; } = "";
    public char UserDefType { get; set; } = ' ';
    public string EnumerationItems { get; set; } = "";

    public UserDefField(string tagName, string data, char userDefType, string enumerationItems)
    {
        TagName = tagName;
        Data = data;
        UserDefType = userDefType;
        EnumerationItems = enumerationItems;
    }
}

/// <summary>
/// A list of <see cref="UserDefField"/> items.
/// </summary>
public class UserDefFieldList : List<UserDefField>
{
}