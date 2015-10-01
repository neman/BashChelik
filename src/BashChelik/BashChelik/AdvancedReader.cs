using BashChelik.DataModel;
using BashChelik.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik
{
    public class AdvancedReader : IDisposable
    {
        public AdvancedReader(int apiVersion)
        {
            int nativeResult = NativeMethods.EidStartup(apiVersion);
            CheckNativeResult(nativeResult);
        }
                
       
        public DocumentData ReadDocumentData()
        {
            EID_DOCUMENT_DATA nativeDocumentData = new EID_DOCUMENT_DATA();
            int nativeResult = NativeMethods.EidReadDocumentData(ref nativeDocumentData);
            CheckNativeResult(nativeResult);
            DocumentData result = new DocumentData(nativeDocumentData);

            return result;
        }

        public FixedPersonalData ReadFixedPersonalData()
        {
            EID_FIXED_PERSONAL_DATA nativeFixedPersonalData = new EID_FIXED_PERSONAL_DATA();
            var nativeResult = NativeMethods.EidReadFixedPersonalData(ref nativeFixedPersonalData);
            CheckNativeResult(nativeResult);
            FixedPersonalData result = new FixedPersonalData(nativeFixedPersonalData);

            return result;
        }

        public VariablePersonalData ReadVariablePersonalData()
        {
            EID_VARIABLE_PERSONAL_DATA nativeVariablePersonalData = new EID_VARIABLE_PERSONAL_DATA();
            var nativeResult = NativeMethods.EidReadVariablePersonalData(ref nativeVariablePersonalData);
            CheckNativeResult(nativeResult);
            VariablePersonalData result = new VariablePersonalData(nativeVariablePersonalData);

            return result;
        }

        public PortraitData ReadPortraitData()
        {
            EID_PORTRAIT nativePortraitData = new EID_PORTRAIT();
            var nativeResult = NativeMethods.EidReadPortrait(ref nativePortraitData);
            CheckNativeResult(nativeResult);
            PortraitData result = new PortraitData(nativePortraitData);

            return result;
        }

        public CertificateData ReadCertificateData()
        {
            EID_CERTIFICATE nativeCertificateData = new EID_CERTIFICATE();
            //TODO: Get all available certificates at read
            var nativeResult = NativeMethods.EidReadCertificate(ref nativeCertificateData, 1);
            CheckNativeResult(nativeResult);
            CertificateData result = new CertificateData(nativeCertificateData);

            return result;
        }


        internal static void CheckNativeResult(int nativeResult)
        {
            if (nativeResult == EidErrorCodes.EID_OK)
                return;

            var message = GetErrorMessage(nativeResult);

            
            throw new BashChelikException(message, nativeResult);
        }

        private static string GetErrorMessage(int errorCode)
        {
            string result = "Unrecognized error code: " + errorCode;

            var errorFields = typeof(EidErrorCodes).GetFields().Union(typeof(SmartCardError).GetFields());
            var foundField = errorFields
                .Select(x => new { Field = x, Code = (short)x.GetValue(x)})
                .FirstOrDefault(x => x.Code == errorCode);

            if (foundField != null)
            {
                var attributes = foundField.Field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    var descriptionAttribute = (DescriptionAttribute)attributes[0];
                    result = string.Format("{0} Error code: {1} ({2})", descriptionAttribute.Description, foundField.Field.Name, errorCode);
                }
                else
                {
                    result = string.Format("An error occurred. Error code: {0} ({1})", foundField.Field.Name, errorCode);
                }
            }

            return result;
        }


        private bool disposed;

        public void Dispose()
        {
            if (disposed)
                return;

            int nativeResult = NativeMethods.EidEndRead();
            ResolveNativeResult(nativeResult);
            nativeResult = NativeMethods.EidCleanup();
            ResolveNativeResult(nativeResult);

        }

        private static void ResolveNativeResult(int nativeResult)
        {
            if (nativeResult != EidErrorCodes.EID_OK)
            {
                var errorMessage = GetErrorMessage(nativeResult);
                Debug.Print(errorMessage);
                Trace.TraceError(errorMessage);
            }
        }
    }
}
