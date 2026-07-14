using System.Text;

namespace AdifParser;

/// <summary>
/// List of QSOs.
/// </summary>
public class ADIFQSOCollection : List<ADIFQSO>
{
    /// <summary>
    /// Return the list of QSOs as a string containing all QSOs.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder(Count * 256);
        foreach (var qso in this)
        {
            sb.Append(qso);
            sb.Append('\n');
        }
        return sb.ToString();
    }
}