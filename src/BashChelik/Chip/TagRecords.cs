using System;
using System.Collections.Generic;
using System.Text;

namespace BashChelik.Chip;

/// <summary>
/// The records of a chip file: <c>tag (2, LE) | length (2, LE) | value</c>, one after another.
/// The first occurrence of a tag wins; tags no layout asks for are kept and ignored.
/// </summary>
internal sealed class TagRecords
{
    private readonly Dictionary<ushort, byte[]> _values;

    private TagRecords(Dictionary<ushort, byte[]> values) => _values = values;

    public static TagRecords Parse(byte[] content)
    {
        var values = new Dictionary<ushort, byte[]>();
        for (int i = 0; i + 4 <= content.Length;)
        {
            var tag = (ushort)(content[i] | (content[i + 1] << 8));
            int length = content[i + 2] | (content[i + 3] << 8);
            if (i + 4 + length > content.Length)
                throw new DocumentReadException(DocumentReadFailure.ChipError, $"Record {tag} runs past the end of its file.");
            if (!values.ContainsKey(tag))
            {
                var value = new byte[length];
                Array.Copy(content, i + 4, value, 0, length);
                values[tag] = value;
            }

            i += 4 + length;
        }

        return new TagRecords(values);
    }

    /// <summary>The record's value as UTF-8 text, or <c>null</c> when it is absent or empty.</summary>
    public string? Text(ushort tag) => Bytes(tag) is { } bytes ? Encoding.UTF8.GetString(bytes) : null;

    /// <summary>The record's value, or <c>null</c> when it is absent or empty.</summary>
    public byte[]? Bytes(ushort tag) => _values.TryGetValue(tag, out var value) && value.Length > 0 ? value : null;
}
