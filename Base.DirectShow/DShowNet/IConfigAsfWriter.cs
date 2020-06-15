using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Base.DirectShow
{
    [SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("45086030-F7E4-486a-B504-826BB5792A3B")]
    [ComImport]
    public interface IConfigAsfWriter
    {
        // Token: 0x060008FB RID: 2299
        [Obsolete("This method is now obsolete because it assumes version 4.0 Windows Media Format SDK profiles. Use GetCurrentProfile or GetCurrentProfileGuid instead to correctly identify a profile.", false)]
        [PreserveSig]
        int ConfigureFilterUsingProfileId([In] int dwProfileId);

        // Token: 0x060008FC RID: 2300
        [Obsolete("This method is now obsolete because it assumes version 4.0 Windows Media Format SDK profiles. Use GetCurrentProfile or GetCurrentProfileGuid instead to correctly identify a profile.", false)]
        [PreserveSig]
        int GetCurrentProfileId(out int pdwProfileId);

        // Token: 0x060008FD RID: 2301
        [Obsolete("Using Guids is considered obsolete by MS.  The preferred approach is using an IWMProfile.  See ConfigureFilterUsingProfile", false)]
        [PreserveSig]
        int ConfigureFilterUsingProfileGuid([MarshalAs(UnmanagedType.LPStruct)] [In] Guid guidProfile);

        // Token: 0x060008FE RID: 2302
        [Obsolete("Using Guids is considered obsolete by MS.  The preferred approach is using an IWMProfile.  See GetCurrentProfile", false)]
        [PreserveSig]
        int GetCurrentProfileGuid(out Guid pProfileGuid);

        // Token: 0x060008FF RID: 2303
        [Obsolete("This method requires IWMProfile, which in turn requires several other interfaces.  Rather than duplicate all those interfaces here, it is recommended that you use the WindowsMediaLib from http://DirectShowNet.SourceForge.net", false)]
        [PreserveSig]
        int ConfigureFilterUsingProfile([In] IntPtr pProfile);

        // Token: 0x06000900 RID: 2304
        [Obsolete("This method requires IWMProfile, which in turn requires several other interfaces.  Rather than duplicate all those interfaces here, it is recommended that you use the WindowsMediaLib from http://DirectShowNet.SourceForge.net", false)]
        [PreserveSig]
        int GetCurrentProfile(out IntPtr ppProfile);

        // Token: 0x06000901 RID: 2305
        [PreserveSig]
        int SetIndexMode([MarshalAs(UnmanagedType.Bool)] [In] bool bIndexFile);

        // Token: 0x06000902 RID: 2306
        [PreserveSig]
        int GetIndexMode([MarshalAs(UnmanagedType.Bool)] out bool pbIndexFile);
    }
}
