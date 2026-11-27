using System;
using BashChelik.Chip;
using PCSC;
using PCSC.Exceptions;
using Xunit;

namespace BashChelik.Tests.Chip;

public class PcscCardChannelTests
{
    private sealed class ThrowsOnDispose : IDisposable
    {
        public void Dispose() => throw new RemovedCardException(SCardError.RemovedCard);
    }

    [Fact]
    public void Ending_a_transaction_never_throws()
    {
        var transaction = new QuietTransaction(new ThrowsOnDispose());

        transaction.Dispose();
    }

    [Theory]
    [InlineData("no service", false, DocumentReadFailure.NoReader)]
    [InlineData("unknown reader", false, DocumentReadFailure.NoReader)]
    [InlineData("no card", false, DocumentReadFailure.NoCard)]
    [InlineData("no card", true, DocumentReadFailure.CardRemoved)]
    [InlineData("removed", true, DocumentReadFailure.CardRemoved)]
    [InlineData("reader unavailable", true, DocumentReadFailure.NoReader)]
    public void PCSC_errors_become_read_failures(string error, bool connected, DocumentReadFailure expected)
    {
        Exception pcsc = error switch
        {
            "no service" => new NoServiceException(SCardError.NoService),
            "unknown reader" => new UnknownReaderException(SCardError.UnknownReader),
            "no card" => new NoSmartcardException(SCardError.NoSmartcard),
            "removed" => new RemovedCardException(SCardError.RemovedCard),
            _ => new ReaderUnavailableException(SCardError.ReaderUnavailable),
        };

        var translated = Assert.IsType<DocumentReadException>(PcscCardChannel.Translate(pcsc, "Reader 0", connected));

        Assert.Equal(expected, translated.Reason);
        Assert.DoesNotContain("\"\"", translated.Message);
    }
}
