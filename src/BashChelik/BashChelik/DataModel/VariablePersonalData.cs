using BashChelik.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.DataModel
{
    public class VariablePersonalData
    {
        public string State { get; private set; }
        public string Community { get; private set; }
        public string Place { get; private set; }
        public string Street { get; private set; }
        public string HouseNumber { get; private set; }
        public string HouseLetter { get; private set; }
        public string Entrance { get; private set; }
        public string Floor { get; private set; }
        public string ApartmentNumber { get; private set; }
        public string AddressDate { get; private set; }
        public string AddressLabel { get; private set; }

        public VariablePersonalData(EID_VARIABLE_PERSONAL_DATA nativeData)
        {
            State = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.state));
            Community = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.community));
            Place = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.place));
            Street = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.street));
            HouseNumber = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.houseNumber));
            HouseLetter = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.houseLetter));
            Entrance = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.entrance));
            Floor = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.floor));
            ApartmentNumber = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.apartmentNumber));
            AddressDate = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.addressDate));
            AddressLabel = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.addressLabel));
        }

    }
}
