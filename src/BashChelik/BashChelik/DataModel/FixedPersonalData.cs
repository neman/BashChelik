using BashChelik.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.DataModel
{
    public class FixedPersonalData
    {
        public string PersonalNumber { get; private set; }
        public string Surname { get; private set; }
        public string GivenName { get; private set; }
        public string ParentGivenName { get; private set; }
        public string Sex { get; private set; }
        public string PlaceOfBirth { get; private set; }
        public string StateOfBirth { get; private set; }
        public string DateOfBirth { get; private set; }
        public string CommunityOfBirth { get; private set; }

        public FixedPersonalData(EID_FIXED_PERSONAL_DATA nativeData)
        {
            PersonalNumber = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.personalNumber));
            Surname = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.surname));
            GivenName = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.givenName));
            ParentGivenName = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.parentGivenName));
            Sex = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.sex));
            PlaceOfBirth = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.placeOfBirth));
            StateOfBirth = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.stateOfBirth));
            DateOfBirth = Encoding.UTF8.GetString(nativeData.dateOfBirth);
            CommunityOfBirth = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.communityOfBirth));
        }
    }
}
