using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.Native
{

    [StructLayout(LayoutKind.Sequential)]
    public struct EID_FIXED_PERSONAL_DATA
    {
        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_PersonalNumber)]
        public byte[] personalNumber;

        /// int
        public int personalNumberSize;
        
        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_Surname)]
        public byte[] surname;

        /// int
        public int surnameSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_GivenName)]
        public byte[] givenName;

        /// int
        public int givenNameSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_ParentGivenName)]
        public byte[] parentGivenName;

        /// int
        public int parentGivenNameSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_Sex)]
        public byte[] sex;

        /// int
        public int sexSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_PlaceOfBirth)]
        public byte[] placeOfBirth;

        /// int
        public int placeOfBirthSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_StateOfBirth)]
        public byte[] stateOfBirth;

        /// int
        public int stateOfBirthSize;
        
        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_DateOfBirth)]
        public byte[] dateOfBirth;

        /// int
        public int dateOfBirthSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_CommunityOfBirth)]
        public byte[] communityOfBirth;

        /// int
        public int communityOfBirthSize;
     
    }
}
