using System.Runtime.InteropServices;

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

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_StatusOfForeigner)]
        public byte[] statusOfForeigner;

        /// int
        public int statusOfForeignerSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_NationalityFull)]
        public byte[] nationalityFull;

        /// int
        public int nationalityFullSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_PurposeOfStay)]
        public byte[] purposeOfStay;

        /// int
        public int purposeOfStaySize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_ENote)]
        public byte[] ENote;

        /// int
        public int ENoteSize;
    }
}