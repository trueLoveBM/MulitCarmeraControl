using Base.DirectShow.Device;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.DirectShow
{
    public class TestManager
    {
        /// <summary>
        /// 获取当前用户可用的所有音频设备
        /// </summary>
        /// <returns></returns>
        public static DsDevice[] GetAllAudioDevice()
        {
            ArrayList arrayList = new ArrayList();
            DsDev.GetDevicesOfCat(FilterCategory.AudioInputDevice, out arrayList);
            DsDevice[] dsArray = new DsDevice[arrayList.Count];
            for (int i = 0; i < arrayList.Count; i++)
            {
                dsArray[i] = arrayList[i] as DsDevice;

            }
            return dsArray;
        }

        /// <summary>
        /// 获取当前用户可用的所有摄像头数组
        /// </summary>
        /// <returns>返回当前用户可用的摄像头数组，若没有找到摄像头，则会抛出异常</returns>
        public static DsDevice[] GetAllVideoDevice()
        {
            ArrayList capDevices = new ArrayList();
            DsDev.GetDevicesOfCat(FilterCategory.VideoInputDevice, out capDevices);//读摄像头信息
            if (capDevices == null || capDevices.Count == 0)//如果摄像头不存在则抛出异常
            {
                //throw new Exception("未找到摄像头设备！");
                return new DsDevice[0];
            }
            DsDevice[] dsArray = new DsDevice[capDevices.Count];
            for (int i = 0; i < capDevices.Count; i++)
            {
                dsArray[i] = capDevices[i] as DsDevice;
            }
            return dsArray;
        }
    }
}
