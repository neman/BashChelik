namespace BashChelik;

/// <summary>The holder's photograph, as the JPEG stored on the document.</summary>
public sealed class Portrait
{
    private readonly byte[] _jpeg;

    /// <summary>Creates a portrait from JPEG bytes; the bytes are copied.</summary>
    public Portrait(byte[] jpeg) => _jpeg = (byte[])jpeg.Clone();

    /// <summary>A copy of the JPEG bytes.</summary>
    public byte[] ToJpeg() => (byte[])_jpeg.Clone();
}
