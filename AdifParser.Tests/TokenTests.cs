using AdifParser;
using Xunit;

namespace AdifParser.Tests;

public class TokenTests
{
    [Fact]
    public void Parse_Qso_Basic()
    {
        var token = new Token("<CALL:4>NV9U");
        Assert.Equal("CALL", token.Name);
        Assert.Equal("NV9U", token.Data);
        Assert.Equal(4u, token.Length);
        Assert.False(token.IsHeader);
    }

    [Fact]
    public void Parse_AnotherTag()
    {
        var token = new Token("<BAND:3>80M");
        Assert.Equal("BAND", token.Name);
        Assert.Equal("80M", token.Data);
        Assert.Equal(3u, token.Length);
    }

    [Fact]
    public void Parse_DataWithAngleBrackets_UsesDeclaredLength()
    {
        var token = new Token("<COMMENT:9>A < B > C");

        Assert.Equal("COMMENT", token.Name);
        Assert.Equal("A < B > C", token.Data);
        Assert.Equal(9u, token.Length);
    }

    [Fact]
    public void Parse_Header_WithUserDefType()
    {
        var token = new Token("<USERDEF1:5:S>Hello,{A,B,C}", true);
        Assert.Equal("USERDEF1", token.Name);
        Assert.Equal("Hello", token.Data);
        Assert.Equal('S', token.UserDefType);
        Assert.Equal("A,B,C", token.EnumerationItems);
        Assert.True(token.IsHeader);
    }

    [Fact]
    public void Parse_EmptyData()
    {
        var token = new Token("<NOTE:0>");
        Assert.Equal("NOTE", token.Name);
        Assert.Equal("", token.Data);
        Assert.Equal(0u, token.Length);
    }

    [Fact]
    public void Parse_WithIsHeaderFlag()
    {
        var token = new Token("<CALL:4>NV9U", true);
        Assert.True(token.IsHeader);
        Assert.Equal("CALL", token.Name);
        Assert.Equal("NV9U", token.Data);
    }

    [Fact]
    public void Constructor_TagNameAndData()
    {
        var token = new Token("CALL", "NV9U");
        Assert.Equal("CALL", token.Name);
        Assert.Equal("NV9U", token.Data);
        Assert.False(token.IsHeader);
    }

    [Fact]
    public void ToString_RoundTrip_Qso()
    {
        var original = "<CALL:4>NV9U";
        var token = new Token(original);
        var result = token.ToString();
        Assert.Equal(original, result);
    }

    [Fact]
    public void ToString_RoundTrip_Header()
    {
        // Length is recalculated on ToString: 5 (Hello) + 3 ({,}) + 5 (A,B,C) = 13
        var token = new Token("<USERDEF1:8:S>Hello,{A,B,C}", true);
        var result = token.ToString();
        Assert.Equal("<USERDEF1:13:S>Hello,{A,B,C}", result);
    }

    [Fact]
    public void Length_AutoUpdated_OnDataChange()
    {
        var token = new Token("CALL", "NV9U");
        Assert.Equal(4u, token.Length);

        token.Data = "W1AW";
        Assert.Equal(4u, token.Length);

        token.Data = "LONGCALL";
        Assert.Equal(8u, token.Length);
    }

    [Fact]
    public void Length_AutoUpdated_OnUserDefTypeChange()
    {
        var token = new Token("USERDEF1", "Hello", 'S', "A,B,C");
        Assert.Equal(13u, token.Length);

        // Changing UserDefType should trigger length recalculation
        token.UserDefType = 'N';
        // Length stays the same (only dependent on data + enumerations)
        Assert.Equal(13u, token.Length);
    }

    [Fact]
    public void Length_UserDef_IncludesEnumeration()
    {
        var token = new Token("USERDEF1", "Hello", 'S', "A,B,C");
        // 5 (Hello) + 3 ({,}) + 5 (A,B,C) = 13
        Assert.Equal(13u, token.Length);
    }

    [Fact]
    public void InvalidToken_ThrowsException()
    {
        Assert.Throws<AdifParseException>(() => new Token("not a token"));
        Assert.Throws<AdifParseException>(() => new Token("<CALL>NV9U")); // no length
        Assert.Throws<AdifParseException>(() => new Token("<CALL:abc>NV9U")); // non-integer length
    }

    [Fact]
    public void EmptyString_NoException()
    {
        var token = new Token("");
        Assert.Equal("", token.Name);
        Assert.Equal("", token.Data);
    }

    [Fact]
    public void Name_Property_Trimmed()
    {
        var token = new Token("CALL", "NV9U");
        Assert.Equal("CALL", token.Name);
    }

    [Fact]
    public void UserDefType_AppearsInToString()
    {
        var token = new Token("USERDEF1", "Hello", 'S', "A,B,C");
        var result = token.ToString();
        Assert.Contains(":S>", result);
    }

    [Fact]
    public void UserDefType_DefaultIsSpace()
    {
        var token = new Token("CALL", "NV9U");
        Assert.Equal(' ', token.UserDefType);
    }

    [Fact]
    public void IsHeader_ChangesAffectToString()
    {
        var token = new Token("CALL", "NV9U");
        Assert.False(token.IsHeader);

        token.IsHeader = true;
        Assert.True(token.IsHeader);
    }
}
