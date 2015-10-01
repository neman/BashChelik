using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.Native
{
    public class EidErrorCodes
    {
        public const short EID_OK = 0;
        public const short EID_E_GENERAL_ERROR = -1;
        public const short EID_E_INVALID_PARAMETER = -2;
        public const short EID_E_VERSION_NOT_SUPPORTED = -3;
        public const short EID_E_NOT_INITIALIZED = -4;
        public const short EID_E_UNABLE_TO_EXECUTE = -5;
        public const short EID_E_READER_ERROR = -6;
        public const short EID_E_CARD_MISSING = -7;
        public const short EID_E_CARD_UNKNOWN = -8;
        public const short EID_E_CARD_MISMATCH = -9;
        public const short EID_E_UNABLE_TO_OPEN_SESSION = -10;
        public const short EID_E_DATA_MISSING = -11;
        public const short EID_E_CARD_SECFORMAT_CHECK_ERROR = -12;
        public const short EID_E_SECFORMAT_CHECK_CERT_ERROR = -13;
        public const short EID_E_INVALID_PASSWORD = -14;
        public const short EID_E_PIN_BLOCKED = -15;
    }
}
