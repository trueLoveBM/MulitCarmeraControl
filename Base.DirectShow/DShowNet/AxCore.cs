using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using static Base.DirectShow.DsGuid;

namespace Base.DirectShow
{
    #region Declarations

    /// <summary>
    /// From AM_GBF_* defines
    /// </summary>
    [Flags]
    public enum AMGBF
    {
        None = 0,
        PrevFrameSkipped = 1,
        NotAsyncPoint = 2,
        NoWait = 4,
        NoDDSurfaceLock = 8
    }

    /// <summary>
    /// From AM_VIDEO_FLAG_* defines
    /// </summary>
    [Flags]
    public enum AMVideoFlag
    {
        FieldMask = 0x0003,
        InterleavedFrame = 0x0000,
        Field1 = 0x0001,
        Field2 = 0x0002,
        Field1First = 0x0004,
        Weave = 0x0008,
        IPBMask = 0x0030,
        ISample = 0x0000,
        PSample = 0x0010,
        BSample = 0x0020,
        RepeatField = 0x0040
    }

    /// <summary>
    /// From AM_SAMPLE_PROPERTY_FLAGS
    /// </summary>
    [Flags]
    public enum AMSamplePropertyFlags
    {
        SplicePoint = 0x01,
        PreRoll = 0x02,
        DataDiscontinuity = 0x04,
        TypeChanged = 0x08,
        TimeValid = 0x10,
        MediaTimeValid = 0x20,
        TimeDiscontinuity = 0x40,
        FlushOnPause = 0x80,
        StopValid = 0x100,
        EndOfStream = 0x200,
        Media = 0,
        Control = 1
    }



    /// <summary>
    /// From AM_SEEKING_SeekingCapabilities
    /// </summary>
    [Flags]
    public enum AMSeekingSeekingCapabilities
    {
        None = 0,
        CanSeekAbsolute = 0x001,
        CanSeekForwards = 0x002,
        CanSeekBackwards = 0x004,
        CanGetCurrentPos = 0x008,
        CanGetStopPos = 0x010,
        CanGetDuration = 0x020,
        CanPlayBackwards = 0x040,
        CanDoSegments = 0x080,
        Source = 0x100
    }

    /// <summary>
    /// From FILTER_STATE
    /// </summary>
    public enum FilterState
    {
        Stopped,
        Paused,
        Running
    }



    /// <summary>
    /// From AM_SEEKING_SeekingFlags
    /// </summary>
    [Flags]
    public enum AMSeekingSeekingFlags
    {
        NoPositioning = 0x00,
        AbsolutePositioning = 0x01,
        RelativePositioning = 0x02,
        IncrementalPositioning = 0x03,
        PositioningBitsMask = 0x03,
        SeekToKeyFrame = 0x04,
        ReturnTime = 0x08,
        Segment = 0x10,
        NoFlush = 0x20
    }

    /// <summary>
    /// From ALLOCATOR_PROPERTIES
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class AllocatorProperties
    {
        public int cBuffers;
        public int cbBuffer;
        public int cbAlign;
        public int cbPrefix;
    }

    /// <summary>
    /// From AM_SAMPLE2_PROPERTIES
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class AMSample2Properties
    {
        public int cbData;
        public AMVideoFlag dwTypeSpecificFlags;
        public AMSamplePropertyFlags dwSampleFlags;
        public int lActual;
        public long tStart;
        public long tStop;
        public int dwStreamId;
        public IntPtr pMediaType;
        public IntPtr pbBuffer; // BYTE *
        public int cbBuffer;
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("68961E68-832B-41ea-BC91-63593F3E70E3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaSample2Config
    {
        [PreserveSig]
        int GetSurface(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppDirect3DSurface9
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("36b73885-c2c8-11cf-8b46-00805f6cef60"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IReferenceClock2 : IReferenceClock
    {
    #region IReferenceClock Methods

        [PreserveSig]
        new int GetTime([Out] out long pTime);

        [PreserveSig]
        new int AdviseTime(
            [In] long baseTime,
            [In] long streamTime,
            [In] IntPtr hEvent, // System.Threading.WaitHandle?
            [Out] out int pdwAdviseCookie
            );

        [PreserveSig]
        new int AdvisePeriodic(
            [In] long startTime,
            [In] long periodTime,
            [In] IntPtr hSemaphore, // System.Threading.WaitHandle?
            [Out] out int pdwAdviseCookie
            );

        [PreserveSig]
        new int Unadvise([In] int dwAdviseCookie);

    #endregion
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a8689d-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemInputPin
    {
        [PreserveSig]
        int GetAllocator([Out] out IMemAllocator ppAllocator);

        [PreserveSig]
        int NotifyAllocator(
            [In] IMemAllocator pAllocator,
            [In, MarshalAs(UnmanagedType.Bool)] bool bReadOnly
            );

        [PreserveSig]
        int GetAllocatorRequirements([Out] out AllocatorProperties pProps);

        [PreserveSig]
        int Receive([In] IMediaSample pSample);

        [PreserveSig]
        int ReceiveMultiple(
            [In] IntPtr pSamples, // IMediaSample[]
            [In] int nSamples,
            [Out] out int nSamplesProcessed
            );

        [PreserveSig]
        int ReceiveCanBlock();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("a3d8cec0-7e5a-11cf-bbc5-00805f6cef20"),
    Obsolete("This interface has been deprecated.", false),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMovieSetup
    {
        [PreserveSig]
        int Register();

        [PreserveSig]
        int Unregister();
    }

#endif














    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("89c31040-846b-11ce-97d3-00aa0055595a"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumMediaTypes
    {
        [PreserveSig]
        int Next(
            [In] int cMediaTypes,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(EMTMarshaler), SizeParamIndex = 0)] AMMediaType[] ppMediaTypes,
            [Out] out int pcFetched
            );

        [PreserveSig]
        int Skip([In] int cMediaTypes);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone([Out] out IEnumMediaTypes ppEnum);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("36b73884-c2c8-11cf-8b46-00805f6cef60"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaSample2 : IMediaSample
    {
        #region IMediaSample Methods

        [PreserveSig]
        new int GetPointer([Out] out IntPtr ppBuffer); // BYTE **

        [PreserveSig]
        new int GetSize();

        [PreserveSig]
        new int GetTime(
            [Out] out long pTimeStart,
            [Out] out long pTimeEnd
            );

        [PreserveSig]
        new int SetTime(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeStart,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeEnd
            );

        [PreserveSig]
        new int IsSyncPoint();

        [PreserveSig]
        new int SetSyncPoint([In, MarshalAs(UnmanagedType.Bool)] bool bIsSyncPoint);

        [PreserveSig]
        new int IsPreroll();

        [PreserveSig]
        new int SetPreroll([In, MarshalAs(UnmanagedType.Bool)] bool bIsPreroll);

        [PreserveSig]
        new int GetActualDataLength();

        [PreserveSig]
        new int SetActualDataLength([In] int len);

        [PreserveSig]
        new int GetMediaType([Out] out AMMediaType ppMediaType);

        [PreserveSig]
        new int SetMediaType([In] AMMediaType pMediaType);

        [PreserveSig]
        new int IsDiscontinuity();

        [PreserveSig]
        new int SetDiscontinuity([In, MarshalAs(UnmanagedType.Bool)] bool bDiscontinuity);

        [PreserveSig]
        new int GetMediaTime(
            [Out] out long pTimeStart,
            [Out] out long pTimeEnd
            );

        [PreserveSig]
        new int SetMediaTime(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeStart,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeEnd
            );

        #endregion

        [PreserveSig]
        int GetProperties(
            [In] int cbProperties,
            [In] IntPtr pbProperties // BYTE *
            );

        [PreserveSig]
        int SetProperties(
            [In] int cbProperties,
            [In] IntPtr pbProperties // BYTE *
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("92980b30-c1de-11d2-abf5-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemAllocatorNotifyCallbackTemp
    {
        [PreserveSig]
        int NotifyRelease();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("379a0cf0-c1de-11d2-abf5-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemAllocatorCallbackTemp : IMemAllocator
    {
        #region IMemAllocator Methods

        [PreserveSig]
        new int SetProperties(
            [In] AllocatorProperties pRequest,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pActual
            );

        [PreserveSig]
        new int GetProperties([Out] AllocatorProperties pProps);

        [PreserveSig]
        new int Commit();

        [PreserveSig]
        new int Decommit();

        [PreserveSig]
        new int GetBuffer(
            [Out] out IMediaSample ppBuffer,
            [In] long pStartTime,
            [In] long pEndTime,
            [In] AMGBF dwFlags
            );

        [PreserveSig]
        new int ReleaseBuffer([In] IMediaSample pBuffer);

        #endregion

        [PreserveSig]
        int SetNotify([In] IMemAllocatorNotifyCallbackTemp pNotify);

        [PreserveSig]
        int GetFreeCount([Out] out int plBuffersFree);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a8689c-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemAllocator
    {
        [PreserveSig]
        int SetProperties(
            [In, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pRequest,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pActual
            );

        [PreserveSig]
        int GetProperties(
            [Out, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pProps
            );

        [PreserveSig]
        int Commit();

        [PreserveSig]
        int Decommit();

        [PreserveSig]
        int GetBuffer(
            [Out] out IMediaSample ppBuffer,
            [In] long pStartTime,
            [In] long pEndTime,
            [In] AMGBF dwFlags
            );

        [PreserveSig]
        int ReleaseBuffer(
            [In] IMediaSample pBuffer
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("ebec459c-2eca-4d42-a8af-30df557614b8"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IReferenceClockTimerControl
    {
        [PreserveSig]
        int SetDefaultTimerResolution(
            long timerResolution
            );

        [PreserveSig]
        int GetDefaultTimerResolution(
            out long pTimerResolution
            );
    }

    #endregion
}
