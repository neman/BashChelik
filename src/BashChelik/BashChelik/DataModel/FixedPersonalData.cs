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
            PersonalNumber = Encoding.UTF8.GetString(nativeData.personalNumber, 0, nativeData.personalNumberSize);
            Surname = Encoding.UTF8.GetString(nativeData.surname, 0, nativeData.surnameSize);
            GivenName = Encoding.UTF8.GetString(nativeData.givenName, 0, nativeData.givenNameSize);
            ParentGivenName = Encoding.UTF8.GetString(nativeData.parentGivenName, 0, nativeData.parentGivenNameSize);
            Sex = Encoding.UTF8.GetString(nativeData.sex, 0, nativeData.sexSize);
            PlaceOfBirth = Encoding.UTF8.GetString(nativeData.placeOfBirth, 0, nativeData.placeOfBirthSize);
            StateOfBirth = Encoding.UTF8.GetString(nativeData.stateOfBirth, 0, nativeData.stateOfBirthSize);
            DateOfBirth = Encoding.UTF8.GetString(nativeData.dateOfBirth, 0, nativeData.dateOfBirthSize);
            CommunityOfBirth = Encoding.UTF8.GetString(nativeData.communityOfBirth, 0, nativeData.communityOfBirthSize);            
        }
    }
}
