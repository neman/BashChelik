using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.Native
{

    [StructLayout(LayoutKind.Sequential)]
    public struct EID_VARIABLE_PERSONAL_DATA
    {

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_State)]
        public byte[] state;

        /// int
        public int stateSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_Community)]
        public byte[] community;

        /// int
        public int communitySize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_Place)]
        public byte[] place;

        /// int
        public int placeSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_Street)]
        public byte[] street;

        /// int
        public int streetSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_HouseNumber)]
        public byte[] houseNumber;

        /// int
        public int houseNumberSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_HouseLetter)]
        public byte[] houseLetter;

        /// int
        public int houseLetterSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_Entrance)]
        public byte[] entrance;

        /// int
        public int entranceSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_Floor)]
        public byte[] floor;

        /// int
        public int floorSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_ApartmentNumber)]
        public byte[] apartmentNumber;

        /// int
        public int apartmentNumberSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_AddressDate)]
        public byte[] addressDate;

        /// int
        public int addressDateSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_AddressLabel)]
        public byte[] addressLabel;

        /// int
        public int addressLabelSize;
    }
}
