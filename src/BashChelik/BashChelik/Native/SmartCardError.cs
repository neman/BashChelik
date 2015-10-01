using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.Native
{
    public static class SmartCardError
    {
        [Description("An internal consistency check failed.")]
        public const uint SCARD_F_INTERNAL_ERROR = 0x80100001;
        [Description("The action was cancelled by an SCardCancel request.")]
        public const uint SCARD_E_CANCELLED = 0x80100002;
        [Description("The supplied handle was invalid.")]
        public const uint SCARD_E_INVALID_HANDLE = 0x80100003;
        [Description("One or more of the supplied parameters could not be properly interpreted.")]
        public const uint SCARD_E_INVALID_PARAMETER = 0x80100004;
        [Description("Registry startup information is missing or invalid.")]
        public const uint SCARD_E_INVALID_TARGET = 0x80100005;
        [Description("Not enough memory available to complete this command.")]
        public const uint SCARD_E_NO_MEMORY = 0x80100006;
        [Description("An internal consistency timer has expired.")]
        public const uint SCARD_F_WAITED_TOO_LONG = 0x80100007;
        [Description("The data buffer to receive returned data is too small for the returned data.")]
        public const uint SCARD_E_INSUFFICIENT_BUFFER = 0x80100008;
        [Description("The specified reader name is not recognized.")]
        public const uint SCARD_E_UNKNOWN_READER = 0x80100009;
        [Description("The user-specified timeout value has expired.")]
        public const uint SCARD_E_TIMEOUT = 0x8010000A;
        [Description("The smart card cannot be accessed because of other connections outstanding.")]
        public const uint SCARD_E_SHARING_VIOLATION = 0x8010000B;
        [Description("The operation requires a Smart Card, but no Smart Card is currently in the device.")]
        public const uint SCARD_E_NO_SMARTCARD = 0x8010000C;
        [Description("The specified smart card name is not recognized.")]
        public const uint SCARD_E_UNKNOWN_CARD = 0x8010000D;
        [Description("The system could not dispose of the media in the requested manner.")]
        public const uint SCARD_E_CANT_DISPOSE = 0x8010000E;
        [Description("The requested protocols are incompatible with the protocol currently in use with the smart card.")]
        public const uint SCARD_E_PROTO_MISMATCH = 0x8010000F;
        [Description("The reader or smart card is not ready to accept commands.")]
        public const uint SCARD_E_NOT_READY = 0x80100010;
        [Description("One or more of the supplied parameters values could not be properly interpreted.")]
        public const uint SCARD_E_INVALID_VALUE = 0x80100011;
        [Description("The action was cancelled by the system, presumably to log off or shut down.")]
        public const uint SCARD_E_SYSTEM_CANCELLED = 0x80100012;
        [Description("An internal communications error has been detected.")]
        public const uint SCARD_F_COMM_ERROR = 0x80100013;
        [Description("An internal error has been detected, but the source is unknown.")]
        public const uint SCARD_F_UNKNOWN_ERROR = 0x80100014;
        [Description("An ATR obtained from the registry is not a valid ATR string.")]
        public const uint SCARD_E_INVALID_ATR = 0x80100015;
        [Description("An attempt was made to end a non-existent transaction.")]
        public const uint SCARD_E_NOT_TRANSACTED = 0x80100016;
        [Description("The specified reader is not currently available for use.")]
        public const uint SCARD_E_READER_UNAVAILABLE = 0x80100017;
        [Description("The operation has been aborted to allow the server application to exit.")]
        public const uint SCARD_P_SHUTDOWN = 0x80100018;
        [Description("The PCI Receive buffer was too small.")]
        public const uint SCARD_E_PCI_TOO_SMALL = 0x80100019;
        [Description("The reader driver does not meet minimal requirements for support.")]
        public const uint SCARD_E_READER_UNSUPPORTED = 0x8010001A;
        [Description("The reader driver did not produce a unique reader name.")]
        public const uint SCARD_E_DUPLICATE_READER = 0x8010001B;
        [Description("The smart card does not meet minimal requirements for support.")]
        public const uint SCARD_E_CARD_UNSUPPORTED = 0x8010001C;
        [Description("The Smart card resource manager is not running.")]
        public const uint SCARD_E_NO_SERVICE = 0x8010001D;
        [Description("The Smart card resource manager has shut down.")]
        public const uint SCARD_E_SERVICE_STOPPED = 0x8010001E;
        [Description("An unexpected card error has occurred.")]
        public const uint SCARD_E_UNEXPECTED = 0x8010001F;
        [Description("No Primary Provider can be found for the smart card.")]
        public const uint SCARD_E_ICC_INSTALLATION = 0x80100020;
        [Description("The requested order of object creation is not supported.")]
        public const uint SCARD_E_ICC_CREATEORDER = 0x80100021;
        [Description("This smart card does not support the requested feature.")]
        public const uint SCARD_E_UNSUPPORTED_FEATURE = 0x80100022;
        [Description("The identified directory does not exist in the smart card.")]
        public const uint SCARD_E_DIR_NOT_FOUND = 0x80100023;
        [Description("The identified file does not exist in the smart card.")]
        public const uint SCARD_E_FILE_NOT_FOUND = 0x80100024;
        [Description("The supplied path does not represent a smart card directory.")]
        public const uint SCARD_E_NO_DIR = 0x80100025;
        [Description("The supplied path does not represent a smart card file.")]
        public const uint SCARD_E_NO_FILE = 0x80100026;
        [Description("Access is denied to this file.")]
        public const uint SCARD_E_NO_ACCESS = 0x80100027;
        [Description("The smartcard does not have enough memory to store the information.")]
        public const uint SCARD_E_WRITE_TOO_MANY = 0x80100028;
        [Description("There was an error trying to set the smart card file object pointer.")]
        public const uint SCARD_E_BAD_SEEK = 0x80100029;
        [Description("The supplied PIN is incorrect.")]
        public const uint SCARD_E_INVALID_CHV = 0x8010002A;
        [Description("An unrecognized error code was returned from a layered component.")]
        public const uint SCARD_E_UNKNOWN_RES_MNG = 0x8010002B;
        [Description("The requested certificate does not exist.")]
        public const uint SCARD_E_NO_SUCH_CERTIFICATE = 0x8010002C;
        [Description("The requested certificate could not be obtained.")]
        public const uint SCARD_E_CERTIFICATE_UNAVAILABLE = 0x8010002D;
        [Description("Cannot find a smart card reader.")]
        public const uint SCARD_E_NO_READERS_AVAILABLE = 0x8010002E;
        [Description("A communications error with the smart card has been detected. Retry the operation.")]
        public const uint SCARD_E_COMM_DATA_LOST = 0x8010002F;
        [Description("The requested key container does not exist on the smart card.")]
        public const uint SCARD_E_NO_KEY_CONTAINER = 0x80100030;
        [Description("The Smart card resource manager is too busy to complete this operation.")]
        public const uint SCARD_E_SERVER_TOO_BUSY = 0x80100031;
        [Description("The smart card PIN cache has expired.")]
        public const uint SCARD_E_PIN_CACHE_EXPIRED = 0x80100032;
        [Description("The smart card PIN cannot be cached.")]
        public const uint SCARD_E_NO_PIN_CACHE = 0x80100033;
        [Description("The smart card is read only and cannot be written to.")]
        public const uint SCARD_E_READ_ONLY_CARD = 0x80100034;
        [Description("The reader cannot communicate with the smart card, due to ATR configuration conflicts.")]
        public const uint SCARD_W_UNSUPPORTED_CARD = 0x80100065;
        [Description("The smart card is not responding to a reset.")]
        public const uint SCARD_W_UNRESPONSIVE_CARD = 0x80100066;
        [Description("Power has been removed from the smart card, so that further communication is not possible.")]
        public const uint SCARD_W_UNPOWERED_CARD = 0x80100067;
        [Description("The smart card has been reset, so any shared state information is invalid.")]
        public const uint SCARD_W_RESET_CARD = 0x80100068;
        [Description("The smart card has been removed, so that further communication is not possible.")]
        public const uint SCARD_W_REMOVED_CARD = 0x80100069;
        [Description("Access was denied because of a security violation.")]
        public const uint SCARD_W_SECURITY_VIOLATION = 0x8010006A;
        [Description("The card cannot be accessed because the wrong PIN was presented.")]
        public const uint SCARD_W_WRONG_CHV = 0x8010006B;
        [Description("The card cannot be accessed because the maximum number of PIN entry attempts has been reached.")]
        public const uint SCARD_W_CHV_BLOCKED = 0x8010006C;
        [Description("The end of the smart card file has been reached.")]
        public const uint SCARD_W_EOF = 0x8010006D;
        [Description("The action was cancelled by the user.")]
        public const uint SCARD_W_CANCELLED_BY_USER = 0x8010006E;
        [Description("No PIN was presented to the smart card.")]
        public const uint SCARD_W_CARD_NOT_AUTHENTICATED = 0x8010006F;
        [Description("The requested item could not be found in the cache.")]
        public const uint SCARD_W_CACHE_ITEM_NOT_FOUND = 0x80100070;
        [Description("The requested cache item is too old and was deleted from the cache.")]
        public const uint SCARD_W_CACHE_ITEM_STALE = 0x80100071;
        [Description("The new cache item exceeds the maximum per-item size defined for the cache.")]
        public const uint SCARD_W_CACHE_ITEM_TOO_BIG = 0x80100072;

    }
}
