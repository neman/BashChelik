using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik
{
    [Serializable]
    public class BashChelikException : Exception
    {
        public BashChelikException()
        {
        }

        public BashChelikException(string message) : base(message)
        {
        }

        public BashChelikException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public BashChelikException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BashChelikException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public int ErrorCode { get; private set; }
    }
}
