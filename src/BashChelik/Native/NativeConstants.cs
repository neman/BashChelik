using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.Native
{
    class NativeConstants
    {
        ///Size of all UTF-8 and binary fields in bytes

        #region EID_DOCUMENT_DATA Size

        internal const int EID_MAX_DocRegNo = 9;
        internal const int EID_MAX_DocumentType = 2;
        internal const int EID_MAX_IssuingDate = 10;
        internal const int EID_MAX_ExpiryDate = 10;
        internal const int EID_MAX_IssuingAuthority = 100;
        internal const int EID_MAX_DocumentSerialNumber = 10;
        internal const int EID_MAX_ChipSerialNumber = 14;
        internal const int EID_MAX_DocumentName = 100;

        #endregion EID_DOCUMENT_DATA Size

        ///Size of all UTF-8 and binary fields in bytes
        #region EID_FIXED_PERSONAL_DATA Size        
        internal const int EID_MAX_PersonalNumber = 13;
        internal const int EID_MAX_Surname = 200;
        internal const int EID_MAX_GivenName = 200;
        internal const int EID_MAX_ParentGivenName = 200;
        internal const int EID_MAX_Sex = 2;
        internal const int EID_MAX_PlaceOfBirth = 200;
        internal const int EID_MAX_StateOfBirth = 200;
        internal const int EID_MAX_DateOfBirth = 10;
        internal const int EID_MAX_CommunityOfBirth = 200;
        #endregion

        ///Size of all UTF-8 and binary fields in bytes
        #region EID_VARIABLE_PERSONAL_DATA Size 
        internal const int EID_MAX_State = 100;
        internal const int EID_MAX_Community = 200;
        internal const int EID_MAX_Place = 200;
        internal const int EID_MAX_Street = 200;
        internal const int EID_MAX_HouseNumber = 20;
        internal const int EID_MAX_HouseLetter = 8;
        internal const int EID_MAX_Entrance = 10;
        internal const int EID_MAX_Floor = 6;
        internal const int EID_MAX_ApartmentNumber = 12;
        internal const int EID_MAX_AddressDate = 10;
        internal const int EID_MAX_AddressLabel = 60;
        #endregion

        ///Size of all UTF-8 and binary fields in bytes
        #region EID_PORTRAIT
        internal const int EID_MAX_Portrait = 7700;
        #endregion

        ///Size of all UTF-8 and binary fields in bytes
        #region EID_CERTIFICATE
        internal const int EID_MAX_Certificate = 2048;
        #endregion


        //
        // Option identifiers, used in function EidSetOption
        //
        internal int EID_O_KEEP_CARD_CLOSED = 1;

        //
        // Certificate types, used in function EidReadCertificate
        //
        internal const int EID_Cert_MoiIntermediateCA = 1;
        internal const int EID_Cert_User1 = 2;
        internal const int EID_Cert_User2 = 3;

        //
        // Block types, used in function EidVerifySignature
        //
        internal const int EID_SIG_CARD = 1;
        internal const int EID_SIG_FIXED = 2;
        internal const int EID_SIG_VARIABLE = 3;
        internal const int EID_SIG_PORTRAIT = 4;

        // For new card version EidVerifySignature function will return EID_E_UNABLE_TO_EXECUTE for
        // parameter EID_SIG_PORTRAIT. Portrait is in new cards part of EID_SIG_FIXED. To determine
        // the card version use second parameter of function EidBeginRead

    }
}
