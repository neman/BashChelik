using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.Native
{
    class NativeMethods
    {
        private const string ImportedDLL = "CelikApi.dll";

        /// Return Type: int
        ///nOptionID: int
        ///nOptionValue: UINT_PTR->unsigned int
        [DllImport(ImportedDLL)]
        internal static extern int EidSetOption(int nOptionID, [MarshalAs(UnmanagedType.SysUInt)] uint nOptionValue);


        /// Return Type: int
        ///nApiVersion: int
        [DllImport(ImportedDLL)]
        internal static extern int EidStartup(int nApiVersion);


        /// Return Type: int
        [DllImportAttribute(ImportedDLL)]
        internal static extern int EidCleanup();

        /// Return Type: int
        /// pnCardVersion: pnCardVersion
        [DllImport(ImportedDLL)]
        internal static extern int EidBeginRead(string szReader, out int pnCardVersion);

        /// Return Type: int
        [DllImport(ImportedDLL)]
        internal static extern int EidEndRead();


        /// Return Type: int
        ///pData: PEID_DOCUMENT_DATA->EID_DOCUMENT_DATA*
        [DllImport(ImportedDLL)]
        internal static extern int EidReadDocumentData(ref EidDocumentData pData);

        /// Return Type: int
        ///pData: PEID_FIXED_PERSONAL_DATA->tagEID_FIXED_PERSONAL_DATA*
        [DllImport(ImportedDLL)]
        public static extern int EidReadFixedPersonalData(ref EID_FIXED_PERSONAL_DATA pData);

        /// Return Type: int
        ///pData: PEID_VARIABLE_PERSONAL_DATA->tagEID_VARIABLE_PERSONAL_DATA*
        [DllImport(ImportedDLL)]
        public static extern int EidReadVariablePersonalData(ref EID_VARIABLE_PERSONAL_DATA pData);


        /// Return Type: int
        ///pData: PEID_PORTRAIT->tagEID_PORTRAIT*
        [DllImport(ImportedDLL)]
        public static extern int EidReadPortrait(ref EID_PORTRAIT pData);


        /// Return Type: int
        ///pData: PEID_CERTIFICATE->tagEID_CERTIFICATE*
        ///certificateType: int
        [DllImport(ImportedDLL)]
        public static extern int EidReadCertificate(ref EID_CERTIFICATE pData, int certificateType);




        //TODO: Implement this Native Methods which are commented


        //        /// Return Type: int
        //        ///szOldPassword: LPCSTR->CHAR*
        //        ///szNewPassword: LPCSTR->CHAR*
        //        ///pnTriesLeft: int*
        //        [System.Runtime.InteropServices.DllImportAttribute("<Unknown>", EntryPoint = "EidChangePassword")]
        //        public static extern int EidChangePassword([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string szOldPassword, [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string szNewPassword, ref int pnTriesLeft);


        //        /// Return Type: int
        //        ///nSignatureID: UINT->unsigned int
        //        [System.Runtime.InteropServices.DllImportAttribute("<Unknown>", EntryPoint = "EidVerifySignature")]
        //        public static extern int EidVerifySignature(uint nSignatureID);

    }
}
