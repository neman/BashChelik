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
            State = Encoding.UTF8.GetString(nativeData.state, 0, nativeData.stateSize);
            Community = Encoding.UTF8.GetString(nativeData.community, 0, nativeData.communitySize);
            Place = Encoding.UTF8.GetString(nativeData.place, 0, nativeData.placeSize);
            Street = Encoding.UTF8.GetString(nativeData.street, 0, nativeData.streetSize);
            HouseNumber = Encoding.UTF8.GetString(nativeData.houseNumber, 0, nativeData.houseNumberSize);
            HouseLetter = Encoding.UTF8.GetString(nativeData.houseLetter, 0, nativeData.houseLetterSize);
            Entrance = Encoding.UTF8.GetString(nativeData.entrance, 0, nativeData.entranceSize);
            Floor = Encoding.UTF8.GetString(nativeData.floor, 0, nativeData.floorSize);
            ApartmentNumber = Encoding.UTF8.GetString(nativeData.apartmentNumber, 0, nativeData.apartmentNumberSize);
            AddressDate = Encoding.UTF8.GetString(nativeData.addressDate, 0, nativeData.addressDateSize);
            AddressLabel = Encoding.UTF8.GetString(nativeData.addressLabel, 0, nativeData.addressLabelSize);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"State: {State}");
            sb.AppendLine($"Community: {Community}");
            sb.AppendLine($"Place: {Place}");
            sb.AppendLine($"Street: {Street}");
            sb.AppendLine($"HouseNumber: {HouseNumber}");
            sb.AppendLine($"HouseLetter: {HouseLetter}");
            sb.AppendLine($"Entrance: {Entrance}");
            sb.AppendLine($"Floor: {Floor}");
            sb.AppendLine($"ApartmentNumber: {ApartmentNumber}");
            sb.AppendLine($"AddressDate: {AddressDate}");
            sb.AppendLine($"AddressLabel: {AddressLabel}");
            return sb.ToString();
        }
    }
}