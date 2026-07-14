namespace AdifParser;

/// <summary>
/// Thrown when ADIF data cannot be parsed due to invalid format.
/// </summary>
public class AdifParseException : Exception
{
    public AdifParseException() { }
    public AdifParseException(string message) : base(message) { }
    public AdifParseException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown when an ADIF file operation fails.
/// </summary>
public class AdifFileException : Exception
{
    public AdifFileException() { }
    public AdifFileException(string message) : base(message) { }
    public AdifFileException(string message, Exception inner) : base(message, inner) { }
}