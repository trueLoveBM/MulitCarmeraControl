using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;

namespace Base.DirectShow
{
    #region Declarations

    /// <summary>
    /// From KSMULTIPLE_ITEM - Note that data is returned in the memory IMMEDIATELY following this struct.
    /// The Size parm indicates ths size of the KSMultipleItem plus the extra bytes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class KSMultipleItem
    {
        public int Size;
        public int Count;
    }

    #endregion

    #region Interfaces

 
  

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("b61178d1-a2d9-11cf-9e53-00aa00a216a1"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKsPin
    {
        /// <summary>
        /// The caller must free the returned structures, using the CoTaskMemFree function
        /// </summary>
        [PreserveSig]
        int KsQueryMediums(
            out IntPtr ip);
    }

   

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("55272A00-42CB-11CE-8135-00AA004BB851"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyBag
    {
        [PreserveSig]
        int Read(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [Out, MarshalAs(UnmanagedType.Struct)] out object pVar,
            [In] IErrorLog pErrorLog
            );

        [PreserveSig]
        int Write(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [In, MarshalAs(UnmanagedType.Struct)] ref object pVar
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("3127CA40-446E-11CE-8135-00AA004BB851"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IErrorLog
    {
        [PreserveSig]
        int AddError(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
#if USING_NET11
            [In] EXCEPINFO pExcepInfo);
#else
            [In] System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
#endif
    }

    #endregion
}
