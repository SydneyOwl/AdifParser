using System.Collections;
using System.Text;

namespace AdifParser;

/// <summary>
/// A collection of ADIF QSO records.
/// </summary>
public class AdifQsoCollection : IList<AdifQso>, IReadOnlyList<AdifQso>
{
    private readonly List<AdifQso> _qsos = new();

    /// <summary>
    /// Return all QSOs as a single ADIF-formatted string.
    /// </summary>
    public override string ToString()
    {
        if (_qsos.Count == 0)
            return string.Empty;

        var sb = new StringBuilder(_qsos.Count * 256);
        foreach (var qso in _qsos)
        {
            sb.Append(qso);
            sb.Append('\n');
        }
        return sb.ToString();
    }

    // ── IList<AdifQso> / IReadOnlyList<AdifQso> implementation ──

    public int Count => _qsos.Count;
    public bool IsReadOnly => false;

    public AdifQso this[int index]
    {
        get => _qsos[index];
        set => _qsos[index] = value;
    }

    public void Add(AdifQso item) => _qsos.Add(item);
    public void Clear() => _qsos.Clear();
    public bool Contains(AdifQso item) => _qsos.Contains(item);
    public void CopyTo(AdifQso[] array, int arrayIndex) => _qsos.CopyTo(array, arrayIndex);
    public int IndexOf(AdifQso item) => _qsos.IndexOf(item);
    public void Insert(int index, AdifQso item) => _qsos.Insert(index, item);
    public bool Remove(AdifQso item) => _qsos.Remove(item);
    public void RemoveAt(int index) => _qsos.RemoveAt(index);

    public IEnumerator<AdifQso> GetEnumerator() => _qsos.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _qsos.GetEnumerator();
}