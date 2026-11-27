using System;

namespace BashChelik.Chip;

/// <summary>
/// Ends a PC/SC transaction without throwing. Ending fails when the card was removed during the read,
/// and that failure must not replace the error the read itself reported.
/// </summary>
internal sealed class QuietTransaction(IDisposable transaction) : IDisposable
{
    public void Dispose()
    {
        try
        {
            transaction.Dispose();
        }
        catch (Exception)
        {
            // The read already succeeded or already reported why it failed.
        }
    }
}
