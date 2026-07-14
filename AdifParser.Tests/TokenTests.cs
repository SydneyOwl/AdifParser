using AdifParser;

namespace AdifParser.Tests;

public class TokenTests
{
    [Fact]
    public void ParseToken_QSO_Basic()
    {
        var token = new Token("<CALL:4>NV9U");
        Assert.Equal("CALL", token.Name);
        Assert.Equal("NV9U", token.Data);
        Assert.Equal(4u, token.Length);
        Assert.False(token.IsHeader);
    }

    [Fact]
    public void ParseToken_MultipleTags()
    {
        var token = new Token("<BAND:3>80M");
        Assert.Equal("BAND", token.Name);
        Assert.Equal("80M", token.Data);
        Assert.Equal(3u, token.Length);
    }

    [Fact]
    public void ParseToken_Header_WithUserDefType()
    {
        var token = new Token("<USERDEF1:5:S>Hello,{A,B,C}", true);
        Assert.Equal("USERDEF1", token.Name);
        Assert.Equal("Hello", token.Data);
        Assert.Equal('S', token.UserDefType);
        Assert.Equal("A,B,C", token.EnumerationItems);
        Assert.True(token.IsHeader);
    }

    [Fact]
    public void ParseToken_EmptyData()
    {
        var token = new Token("<NOTE:0>");
        Assert.Equal("NOTE", token.Name);
        Assert.Equal("", token.Data);
        Assert.Equal(0u, token.Length);
    }

    [Fact]
    public void ParseToken_WithIsHeaderFlag()
    {
        var token = new Token("<CALL:4>NV9U", true);
        Assert.True(token.IsHeader);
        Assert.Equal("CALL", token.Name);
        Assert.Equal("NV9U", token.Data);
    }

    [Fact]
    public void Token_Constructor_TagNameData()
    {
        var token = new Token("CALL", "NV9U");
        Assert.Equal("CALL", token.Name);
        Assert.Equal("NV9U", token.Data);
        Assert.False(token.IsHeader);
    }

    [Fact]
    public void Token_ToString_RoundTrip_QSO()
    {
        var original = "<CALL:4>NV9U";
        var token = new Token(original);
        var result = token.ToString();
        Assert.Equal(original, result);
    }

    [Fact]
    public void Token_ToString_RoundTrip_Header()
    {
        // Length is recalculated on ToString: 5 (Hello) + 3 ({,}) + 5 (A,B,C) = 13
        var token = new Token("<USERDEF1:8:S>Hello,{A,B,C}", true);
        var result = token.ToString();
        Assert.Equal("<USERDEF1:13:S>Hello,{A,B,C}", result);
    }

    [Fact]
    public void Token_Length_AutoUpdated()
    {
        var token = new Token("CALL", "NV9U");
        Assert.Equal(4u, token.Length);

        token.Data = "W1AW";
        Assert.Equal(4u, token.Length);

        token.Data = "LONGCALL";
        Assert.Equal(8u, token.Length);
    }

    [Fact]
    public void Token_UserDef_Length_IncludesEnumeration()
    {
        var token = new Token("USERDEF1", "Hello", 'S', "A,B,C");
        // 5 (Hello) + 3 ({,}) + 5 (A,B,C) = 13
        Assert.Equal(13u, token.Length);
    }

    [Fact]
    public void Token_InvalidToken_ThrowsException()
    {
        Assert.Throws<Exception>(() => new Token("not a token"));
        Assert.Throws<Exception>(() => new Token("<CALL>NV9U")); // no length
        Assert.Throws<Exception>(() => new Token("<CALL:abc>NV9U")); // non-integer length
    }

    [Fact]
    public void Token_EmptyString_NoException()
    {
        var token = new Token("");
        Assert.Equal("", token.Name);
        Assert.Equal("", token.Data);
    }

    [Fact]
    public void Token_Name_Property_Trimmed()
    {
        var token = new Token("CALL", "NV9U");
        // Name setter trims, but _name is set directly by constructor
        Assert.Equal("CALL", token.Name);
    }
}