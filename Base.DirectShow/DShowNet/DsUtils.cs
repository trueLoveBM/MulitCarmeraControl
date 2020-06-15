/******************************************************
                  DirectShow .NET
		      netmaster@swissonline.ch
*******************************************************/
//					DsUtils
// DirectShow utility classes, partial from the SDK Common sources

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Base.DirectShow
{

		[ComVisible(false)]
	public class DsUtils
	{

		public static bool IsCorrectDirectXVersion()
		{
			return File.Exists( Path.Combine( Environment.SystemDirectory, @"dpnhpast.dll" ) );
		}


		public static bool ShowCapPinDialog( ICaptureGraphBuilder2 bld, IBaseFilter flt, IntPtr hwnd )
		{
			int hr;
			object comObj = null;
			ISpecifyPropertyPages	spec = null;
			DsCAUUID cauuid = new DsCAUUID();

			try {
				Guid cat  = PinCategory.Capture;
				Guid type = MediaType.Interleaved;
				Guid iid = typeof(IAMStreamConfig).GUID;
				hr = bld.FindInterface( ref cat, ref type, flt, ref iid, out comObj );
				if( hr != 0 )
				{
					type = MediaType.Video;
					hr = bld.FindInterface( ref cat, ref type, flt, ref iid, out comObj );
					if( hr != 0 )
						return false;
				}
				spec = comObj as ISpecifyPropertyPages;
				if( spec == null )
					return false;

				hr = spec.GetPages( out cauuid );
				hr = OleCreatePropertyFrame( hwnd, 30, 30, null, 1,
						ref comObj, cauuid.cElems, cauuid.pElems, 0, 0, IntPtr.Zero );
				return true;
			}
			catch( Exception ee )
			{
				Trace.WriteLine( "!Ds.NET: ShowCapPinDialog " + ee.Message );
				return false;
			}
			finally
			{
				if( cauuid.pElems != IntPtr.Zero )
					Marshal.FreeCoTaskMem( cauuid.pElems );
					
				spec = null;
				if( comObj != null )
					Marshal.ReleaseComObject( comObj ); comObj = null;
			}
		}

		public static bool ShowTunerPinDialog( ICaptureGraphBuilder2 bld, IBaseFilter flt, IntPtr hwnd )
		{
			int hr;
			object comObj = null;
			ISpecifyPropertyPages	spec = null;
			DsCAUUID cauuid = new DsCAUUID();

			try {
				Guid cat  = PinCategory.Capture;
				Guid type = MediaType.Interleaved;
				Guid iid = typeof(IAMTVTuner).GUID;
				hr = bld.FindInterface( ref cat, ref type, flt, ref iid, out comObj );
				if( hr != 0 )
				{
					type = MediaType.Video;
					hr = bld.FindInterface( ref cat, ref type, flt, ref iid, out comObj );
					if( hr != 0 )
						return false;
				}
				spec = comObj as ISpecifyPropertyPages;
				if( spec == null )
					return false;

				hr = spec.GetPages( out cauuid );
				hr = OleCreatePropertyFrame( hwnd, 30, 30, null, 1,
						ref comObj, cauuid.cElems, cauuid.pElems, 0, 0, IntPtr.Zero );
				return true;
			}
			catch( Exception ee )
			{
				Trace.WriteLine( "!Ds.NET: ShowCapPinDialog " + ee.Message );
				return false;
			}
			finally
			{
				if( cauuid.pElems != IntPtr.Zero )
					Marshal.FreeCoTaskMem( cauuid.pElems );
					
				spec = null;
				if( comObj != null )
					Marshal.ReleaseComObject( comObj ); comObj = null;
			}
		}


		// from 'DShowUtil.cpp'
		public int GetPin( IBaseFilter filter, PinDirection dirrequired, int num, out IPin ppPin )
		{
			ppPin = null;
			int hr;
			IEnumPins pinEnum;
			hr = filter.EnumPins( out pinEnum );
			if( (hr < 0) || (pinEnum == null) )
				return hr;

			IPin[] pins = new IPin[1];
			int f;
			PinDirection dir;
			do
			{
				hr = pinEnum.Next( 1, pins, out f );
				if( (hr != 0) || (pins[0] == null) )
					break;
				dir = (PinDirection) 3;
				hr = pins[0].QueryDirection( out dir );
				if( (hr == 0) && (dir == dirrequired) )
				{
					if( num == 0 )
					{
						ppPin = pins[0];
						pins[0] = null;
						break;
					}
					num--;
				}
				Marshal.ReleaseComObject( pins[0] ); pins[0] = null;
			}
			while( hr == 0 );

			Marshal.ReleaseComObject( pinEnum ); pinEnum = null;
			return hr;
		}

		/// <summary> 
		///  Free the nested structures and release any 
		///  COM objects within an AMMediaType struct.
		/// </summary>
		public static void FreeAMMediaType(AMMediaType mediaType)
		{
			if ( mediaType.formatSize != 0 )
				Marshal.FreeCoTaskMem( mediaType.formatPtr );
			if ( mediaType.unkPtr != IntPtr.Zero ) 
				Marshal.Release( mediaType.unkPtr );
			mediaType.formatSize = 0;
			mediaType.formatPtr = IntPtr.Zero;
			mediaType.unkPtr = IntPtr.Zero;
		}

		[DllImport("olepro32.dll", CharSet=CharSet.Unicode, ExactSpelling=true) ]
		private static extern int OleCreatePropertyFrame( IntPtr hwndOwner, int x, int y,
			string lpszCaption, int cObjects,
			[In, MarshalAs(UnmanagedType.Interface)] ref object ppUnk,
			int cPages,	IntPtr pPageClsID, int lcid, int dwReserved, IntPtr pvReserved );
	}


// ---------------------------------------------------------------------------------------

	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
public struct DsPOINT		// POINT
{
	public int		X;
	public int		Y;
}


// ---------------------------------------------------------------------------------------

	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
public struct DsRECT		// RECT
{
	public int		Left;
	public int		Top;
	public int		Right;
	public int		Bottom;
}


// ---------------------------------------------------------------------------------------

	[StructLayout(LayoutKind.Sequential, Pack=2), ComVisible(false)]
public struct BitmapInfoHeader
	{
	public int      Size;
	public int      Width;
	public int      Height;
	public short    Planes;
	public short    BitCount;
	public int      Compression;
	public int      ImageSize;
	public int      XPelsPerMeter;
	public int      YPelsPerMeter;
	public int      ClrUsed;
	public int      ClrImportant;
	}




// ---------------------------------------------------------------------------------------

		[ComVisible(false)]
	public class DsROT
	{
		public static bool AddGraphToRot( object graph, out int cookie )
		{
			cookie = 0;
			int hr = 0;
			UCOMIRunningObjectTable rot = null;
			UCOMIMoniker mk = null;
			try {
				hr = GetRunningObjectTable( 0, out rot );
				if( hr < 0 )
					Marshal.ThrowExceptionForHR( hr );

				int id = GetCurrentProcessId();
				IntPtr iuPtr = Marshal.GetIUnknownForObject( graph );
				int iuInt = (int) iuPtr;
				Marshal.Release( iuPtr );
				string item = string.Format( "FilterGraph {0} pid {1}", iuInt.ToString("x8"), id.ToString("x8") );
				hr = CreateItemMoniker( "!", item, out mk );
				if( hr < 0 )
					Marshal.ThrowExceptionForHR( hr );
				
				rot.Register( ROTFLAGS_REGISTRATIONKEEPSALIVE, graph, mk, out cookie );
				return true;
			}
			catch( Exception )
			{
				return false;
			}
			finally
			{
				if( mk != null )
					Marshal.ReleaseComObject( mk ); mk = null;
				if( rot != null )
					Marshal.ReleaseComObject( rot ); rot = null;
			}
		}

		public static bool RemoveGraphFromRot( ref int cookie )
		{
			UCOMIRunningObjectTable rot = null;
			try {
				int hr = GetRunningObjectTable( 0, out rot );
				if( hr < 0 )
					Marshal.ThrowExceptionForHR( hr );

				rot.Revoke( cookie );
				cookie = 0;
				return true;
			}
			catch( Exception )
			{
				return false;
			}
			finally
			{
				if( rot != null )
					Marshal.ReleaseComObject( rot ); rot = null;
			}
		}

		private const int ROTFLAGS_REGISTRATIONKEEPSALIVE	= 1;

		[DllImport("ole32.dll", ExactSpelling=true) ]
		private static extern int GetRunningObjectTable( int r,
			out UCOMIRunningObjectTable pprot );

		[DllImport("ole32.dll", CharSet=CharSet.Unicode, ExactSpelling=true) ]
		private static extern int CreateItemMoniker( string delim,
			string item, out UCOMIMoniker ppmk );

		[DllImport("kernel32.dll", ExactSpelling=true) ]
		private static extern int GetCurrentProcessId();
	}





// ---------------------------------- ocidl.idl ------------------------------------------------

	[ComVisible(true), ComImport,
	Guid("B196B28B-BAB4-101A-B69C-00AA00341D07"),
	InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface ISpecifyPropertyPages
{
		[PreserveSig]
	int GetPages( out DsCAUUID pPages );
}

	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
public struct DsCAUUID		// CAUUID
{
	public int		cElems;
	public IntPtr	pElems;
}

// ---------------------------------------------------------------------------------------


	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
public class DsOptInt64
{
	public DsOptInt64( long Value )
	{
		this.Value = Value;
	}
	public long		Value;
}


	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
public class DsOptIntPtr
{
	public IntPtr	Pointer;
}

    /// <summary>
    /// DirectShowLib.DsGuid is a wrapper class around a System.Guid value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null paramters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public class DsGuid
    {
        [FieldOffset(0)]
        private Guid guid;

        public static readonly DsGuid Empty = Guid.Empty;

        /// <summary>
        /// Empty constructor. 
        /// Initialize it with System.Guid.Empty
        /// </summary>
        public DsGuid()
        {
            this.guid = Guid.Empty;
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid string representation.
        /// </summary>
        /// <param name="g">A valid System.Guid as string</param>
        public DsGuid(string g)
        {
            this.guid = new Guid(g);
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid.
        /// </summary>
        /// <param name="g">A System.Guid value type</param>
        public DsGuid(Guid g)
        {
            this.guid = g;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.guid.ToString();
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance with a specific format.
        /// </summary>
        /// <param name="format"><see cref="System.Guid.ToString"/> for a description of the format parameter.</param>
        /// <returns>A string representing this instance according to the format parameter</returns>
        public string ToString(string format)
        {
            return this.guid.ToString(format);
        }

        public override int GetHashCode()
        {
            return this.guid.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsGuid and System.Guid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.ToGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new DsGuid instance
        ///   DsGuid dsG = new DsGuid("{33D57EBF-7C9D-435e-A15E-D300B52FBD91}");
        ///   // Do implicit cast between DsGuid and Guid
        ///   Guid g = dsG;
        ///
        ///   Console.WriteLine(g.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">DirectShowLib.DsGuid to be cast</param>
        /// <returns>A casted System.Guid</returns>
        public static implicit operator Guid(DsGuid g)
        {
            return g.guid;
        }

        /// <summary>
        /// Define implicit cast between System.Guid and DirectShowLib.DsGuid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new Guid instance
        ///   Guid g = new Guid("{B9364217-366E-45f8-AA2D-B0ED9E7D932D}");
        ///   // Do implicit cast between Guid and DsGuid
        ///   DsGuid dsG = g;
        ///
        ///   Console.WriteLine(dsG.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Guid to be cast</param>
        /// <returns>A casted DirectShowLib.DsGuid</returns>
        public static implicit operator DsGuid(Guid g)
        {
            return new DsGuid(g);
        }

        /// <summary>
        /// Get the System.Guid equivalent to this DirectShowLib.DsGuid instance.
        /// </summary>
        /// <returns>A System.Guid</returns>
        public Guid ToGuid()
        {
            return this.guid;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsGuid instance for a given System.Guid
        /// </summary>
        /// <param name="g">The System.Guid to wrap into a DirectShowLib.DsGuid</param>
        /// <returns>A new instance of DirectShowLib.DsGuid</returns>
        public static DsGuid FromGuid(Guid g)
        {
            return new DsGuid(g);
        }


        /// <summary>
        /// DirectShowLib.DsLong is a wrapper class around a <see cref="System.Int64"/> value type.
        /// </summary>
        /// <remarks>
        /// This class is necessary to enable null paramters passing.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        public class DsLong
        {
            private long Value;

            /// <summary>
            /// Constructor
            /// Initialize a new instance of DirectShowLib.DsLong with the Value parameter
            /// </summary>
            /// <param name="Value">Value to assign to this new instance</param>
            public DsLong(long Value)
            {
                this.Value = Value;
            }

            /// <summary>
            /// Get a string representation of this DirectShowLib.DsLong Instance.
            /// </summary>
            /// <returns>A string representing this instance</returns>
            public override string ToString()
            {
                return this.Value.ToString();
            }

            public override int GetHashCode()
            {
                return this.Value.GetHashCode();
            }

            /// <summary>
            /// Define implicit cast between DirectShowLib.DsLong and System.Int64 for languages supporting this feature.
            /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsLong.ToInt64"/> for similar functionality.
            /// <code>
            ///   // Define a new DsLong instance
            ///   DsLong dsL = new DsLong(9876543210);
            ///   // Do implicit cast between DsLong and Int64
            ///   long l = dsL;
            ///
            ///   Console.WriteLine(l.ToString());
            /// </code>
            /// </summary>
            /// <param name="g">DirectShowLib.DsLong to be cast</param>
            /// <returns>A casted System.Int64</returns>
            public static implicit operator long(DsLong l)
            {
                return l.Value;
            }

            /// <summary>
            /// Define implicit cast between System.Int64 and DirectShowLib.DsLong for languages supporting this feature.
            /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromInt64"/> for similar functionality.
            /// <code>
            ///   // Define a new Int64 instance
            ///   long l = 9876543210;
            ///   // Do implicit cast between Int64 and DsLong
            ///   DsLong dsl = l;
            ///
            ///   Console.WriteLine(dsl.ToString());
            /// </code>
            /// </summary>
            /// <param name="g">System.Int64 to be cast</param>
            /// <returns>A casted DirectShowLib.DsLong</returns>
            public static implicit operator DsLong(long l)
            {
                return new DsLong(l);
            }

            /// <summary>
            /// Get the System.Int64 equivalent to this DirectShowLib.DsLong instance.
            /// </summary>
            /// <returns>A System.Int64</returns>
            public long ToInt64()
            {
                return this.Value;
            }

            /// <summary>
            /// Get a new DirectShowLib.DsLong instance for a given System.Int64
            /// </summary>
            /// <param name="g">The System.Int64 to wrap into a DirectShowLib.DsLong</param>
            /// <returns>A new instance of DirectShowLib.DsLong</returns>
            public static DsLong FromInt64(long l)
            {
                return new DsLong(l);
            }
        }
    }


    // This abstract class contains definitions for use in implementing a custom marshaler.
    //
    // MarshalManagedToNative() gets called before the COM method, and MarshalNativeToManaged() gets
    // called after.  This allows for allocating a correctly sized memory block for the COM call,
    // then to break up the memory block and build an object that c# can digest.

    internal abstract class DsMarshaler : ICustomMarshaler
    {
        #region Data Members

        // The cookie isn't currently being used.
        protected string m_cookie;

        // The managed object passed in to MarshalManagedToNative, and modified in MarshalNativeToManaged
        protected object m_obj;

        #endregion

        // The constructor.  This is called from GetInstance (below)
        public DsMarshaler(string cookie)
        {
            // If we get a cookie, save it.
            m_cookie = cookie;
        }

        // Called just before invoking the COM method.  The returned IntPtr is what goes on the stack
        // for the COM call.  The input arg is the parameter that was passed to the method.
        public virtual IntPtr MarshalManagedToNative(object managedObj)
        {
            // Save off the passed-in value.  Safe since we just checked the type.
            m_obj = managedObj;

            // Create an appropriately sized buffer, blank it, and send it to the marshaler to
            // make the COM call with.
            int iSize = GetNativeDataSize() + 3;
            IntPtr p = Marshal.AllocCoTaskMem(iSize);

            for (int x = 0; x < iSize / 4; x++)
            {
                Marshal.WriteInt32(p, x * 4, 0);
            }

            return p;
        }

        // Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        // from MarshalManagedToNative.  The return value is unused.
        public virtual object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return m_obj;
        }

        // Release the (now unused) buffer
        public virtual void CleanUpNativeData(IntPtr pNativeData)
        {
            if (pNativeData != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pNativeData);
            }
        }

        // Release the (now unused) managed object
        public virtual void CleanUpManagedData(object managedObj)
        {
            m_obj = null;
        }

        // This routine is (apparently) never called by the marshaler.  However it can be useful.
        public abstract int GetNativeDataSize();

        // GetInstance is called by the marshaler in preparation to doing custom marshaling.  The (optional)
        // cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.

        // It is commented out in this abstract class, but MUST be implemented in derived classes
        //public static ICustomMarshaler GetInstance(string cookie)
    }


    internal class EMTMarshaler : DsMarshaler
    {
        public EMTMarshaler(string cookie)
            : base(cookie)
        {
        }

        // Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        // from MarshalManagedToNative.  The return value is unused.
        public override object MarshalNativeToManaged(IntPtr pNativeData)
        {
            AMMediaType[] emt = m_obj as AMMediaType[];

            for (int x = 0; x < emt.Length; x++)
            {
                // Copy in the value, and advance the pointer
                IntPtr p = Marshal.ReadIntPtr(pNativeData, x * IntPtr.Size);
                if (p != IntPtr.Zero)
                {
                    emt[x] = (AMMediaType)Marshal.PtrToStructure(p, typeof(AMMediaType));
                }
                else
                {
                    emt[x] = null;
                }
            }

            return null;
        }

        // The number of bytes to marshal out
        public override int GetNativeDataSize()
        {
            // Get the array size
            int i = ((Array)m_obj).Length;

            // Multiply that times the size of a pointer
            int j = i * IntPtr.Size;

            return j;
        }

        // This method is called by interop to create the custom marshaler.  The (optional)
        // cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.
        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new EMTMarshaler(cookie);
        }
    }


} // namespace Base.DirectShow
