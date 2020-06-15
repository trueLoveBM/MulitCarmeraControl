using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Base.DirectShow
{
    public sealed class DsError
    {
        private DsError()
        {
        }

        [DllImport("quartz.dll", CharSet = CharSet.Auto)]
        public static extern int AMGetErrorText(int hr, StringBuilder buf, int max);


        public static void ThrowExceptionForHR(int hr)
        {
            if (hr < 0)
            {
                string errorText = DsError.GetErrorText(hr);
                if (errorText != null)
                {
                    throw new COMException(errorText, hr);
                }
                Marshal.ThrowExceptionForHR(hr);
            }
        }

        public static string GetErrorText(int hr)
        {
            StringBuilder stringBuilder = new StringBuilder(160, 160);
            if (DsError.AMGetErrorText(hr, stringBuilder, 160) > 0)
            {
                return stringBuilder.ToString();
            }
            return null;
        }

    }


}
