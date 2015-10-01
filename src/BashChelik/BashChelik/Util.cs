using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik
{
    public class Util
    {
        public static byte[] TrimTrailingZeroes(byte[] inputByteArray)
        {
            int i = inputByteArray.Length - 1;
            while (IsNonFirstElementZero(inputByteArray, i))
            {
                --i;
            }
            byte[] result = new byte[i + 1];
            Array.Copy(inputByteArray, result, i + 1);

            return result;
        }

        private static bool IsNonFirstElementZero(byte[] inputByteArray, int i)
        {
            return (inputByteArray[i] == 0) && (i != 0);
        }
    }
}
