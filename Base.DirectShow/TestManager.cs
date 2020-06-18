using Base.DirectShow.Device;
using Base.DirectShow.Entity;
using Base.DirectShow.XmlHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        /// <summary>
        /// 根据设备集合、设备名称获取指定设备的BaseFilter
        /// </summary>
        /// <param name="dsDevice">设备集合</param>
        /// <param name="friendlyname">设备名</param>
        /// <returns></returns>
        internal static IBaseFilter CreateFilter(DsDevice[] dsDevice, string friendlyname)
        {
            if (dsDevice == null || dsDevice.Length == 0)
            {
                return null;
            }
            object source = null;
            Guid gbf = typeof(IBaseFilter).GUID;
            if (string.IsNullOrEmpty(friendlyname))//如果名字为空则默认第一个
            {
                dsDevice[0].Mon.BindToObject(null, null, ref gbf, out source);
                return (IBaseFilter)source;
            }
            bool blExis = false;//是否存在
            foreach (DsDevice device in dsDevice)
            {
                if (device.Name.Equals(friendlyname))
                {
                    blExis = true;
                    device.Mon.BindToObject(null, null, ref gbf, out source);
                    break;
                }
            }
            if (blExis)
            {
                return (IBaseFilter)source;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 验证是否安装DirectX
        /// </summary>
        /// <returns></returns>
        internal static bool IsCorrectDirectXVersion()
        {
            return File.Exists(Path.Combine(Environment.SystemDirectory, "dpnhpast.dll"));
        }

        /// <summary>
        /// 使用key方式创建摄像头操作对象
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static TestMulitCamera CreateCamera(string Key)
        {
            refreshDeviceInfo();

            BindInfoEntity bindInfoEntity = XmlHelper.XmlHelper.FindByPrimaryKey<BindInfoEntity>(Key);


            if (bindInfoEntity == null)
            {
                var remindCamera = XmlHelper.XmlHelper.FindAll<CameraEntity>();
                var remindAudio = XmlHelper.XmlHelper.FindAll<AudioEntity>();

                //找到一个可用的摄像头和可用的音频设备
                var idleCamera = remindCamera.Find(m => m.Status == 0);
                var idleAudio = remindAudio.Find(m => m.Status == 0);

                if (idleCamera == null)
                    throw new Exception("无可用摄像头");

                if (idleAudio == null)
                    throw new Exception("无可用音频设备");

                bindInfoEntity = new BindInfoEntity();
                bindInfoEntity.Id = Key;
                bindInfoEntity.CameraName = idleCamera.Name;
                bindInfoEntity.AudioName = idleAudio.Name;
                bindInfoEntity.Save();

                idleCamera.Status = 1;
                idleAudio.Status = 1;
                idleCamera.Save();
                idleAudio.Save();
            }
            else
            {
                var BindCamera = XmlHelper.XmlHelper.FindByPrimaryKey<CameraEntity>(bindInfoEntity.CameraName);
                var BindAudio = XmlHelper.XmlHelper.FindByPrimaryKey<AudioEntity>(bindInfoEntity.AudioName);

                if (BindCamera.Status == 3 || BindAudio.Status == 3)
                {
                    //TODO 提醒是否更换摄像头或者音频设备

                    //TODO 现在默认替换摄像头
                    //找到一个可用的摄像头和可用的音频设备
                    if (BindCamera.Status == 3)
                    {
                        var remindCamera = XmlHelper.XmlHelper.FindAll<CameraEntity>();
                        var idleCamera = remindCamera.Find(m => m.Status == 0);
                        if (idleCamera == null)
                            throw new Exception("无可用摄像头");
                        bindInfoEntity.CameraName = idleCamera.Name;
                        idleCamera.Status = 1;
                        idleCamera.Save();
                    }

                    if (BindAudio.Status == 3)
                    {
                        var remindAudio = XmlHelper.XmlHelper.FindAll<AudioEntity>();
                        var idleAudio = remindAudio.Find(m => m.Status == 0);
                        if (idleAudio == null)
                            throw new Exception("无可用音频设备");
                        bindInfoEntity.AudioName = idleAudio.Name;
                        idleAudio.Status = 1;
                        idleAudio.Save();
                    }

                    bindInfoEntity.Save();
                }
            }

            TestMulitCamera testMulitCamera = new TestMulitCamera();

            testMulitCamera.BindCamera(bindInfoEntity.CameraName);
            testMulitCamera.BindAudio(bindInfoEntity.AudioName);

            //分辨率不可调用
            //testMulitCamera.GetCameraSupportResolution();
            List<string> Resolution = GetCameraSupportResolution(bindInfoEntity.CameraName);
            //默认使用最高的分辨率
            testMulitCamera.SetResolution(Resolution.First());
            return testMulitCamera;
        }

        /// <summary>
        /// 刷新当前机器状态
        /// </summary>
        internal static void refreshDeviceInfo()
        {
            #region 刷新摄像头状态
            var remindCamera = XmlHelper.XmlHelper.FindAll<CameraEntity>();
            if (remindCamera == null) remindCamera = new List<CameraEntity>();
            //当前可以获取到的所有摄像头
            DsDevice[] findCamera = GetAllVideoDevice();

            if (findCamera == null || findCamera.Length == 0)
            {
                //更新集合
                if (remindCamera?.Count > 0)
                {
                    //所有的摄像头不可用
                    remindCamera.ForEach(m =>
                    {
                        m.Status = 3;
                    }
                    );
                    XmlHelper.XmlHelper.SaveList<CameraEntity>(remindCamera);
                }

                throw new Exception("未找到可用的摄像头");
            }

            List<DsDevice> findCameraList = findCamera.ToList();

            //遍历已经记录的摄像，在当前可寻找到的摄像头中查找
            foreach (var item in remindCamera)
            {
                DsDevice CameraDevice = findCameraList.Find(m => m.Name == item.Name);
                if (CameraDevice == null)
                    item.Status = 3;
            }

            //遍历寻找到的摄像头，在记录的摄像头中进行寻找
            foreach (var item in findCameraList)
            {
                CameraEntity cameraEntity = remindCamera.Find(m => m.Name == item.Name);
                if (cameraEntity == null)
                {
                    cameraEntity = new CameraEntity();
                    cameraEntity.Name = item.Name;
                    cameraEntity.Status = 0;
                    remindCamera.Add(cameraEntity);
                }
            }
            XmlHelper.XmlHelper.SaveList<CameraEntity>(remindCamera);
            #endregion


            #region 刷新音频状态
            var remindAudio = XmlHelper.XmlHelper.FindAll<AudioEntity>();
            if (remindAudio == null) remindAudio = new List<AudioEntity>();
            //当前可以获取到的所有摄像头
            DsDevice[] findAudio = GetAllAudioDevice();

            if (findAudio == null || findAudio.Length == 0)
            {
                //更新集合
                if (remindAudio?.Count > 0)
                {
                    //所有的摄像头不可用
                    remindAudio.ForEach(m =>
                    {
                        m.Status = 3;
                    }
                    );
                    XmlHelper.XmlHelper.SaveList<AudioEntity>(remindAudio);
                }

                throw new Exception("未找到可用的音频");
            }

            List<DsDevice> findAudioList = findAudio.ToList();

            //遍历已经记录的摄像，在当前可寻找到的摄像头中查找
            foreach (var item in remindAudio)
            {
                DsDevice CameraDevice = findAudioList.Find(m => m.Name == item.Name);
                if (CameraDevice == null)
                    item.Status = 3;
            }

            //遍历寻找到的摄像头，在记录的摄像头中进行寻找
            foreach (var item in findAudioList)
            {
                AudioEntity audioEntity = remindAudio.Find(m => m.Name == item.Name);
                if (audioEntity == null)
                {
                    audioEntity = new AudioEntity();
                    audioEntity.Name = item.Name;
                    audioEntity.Status = 0;
                    remindAudio.Add(audioEntity);
                }
                XmlHelper.XmlHelper.SaveList<AudioEntity>(remindAudio);
            }
            #endregion
        }

        /// <summary>
        /// 获取指定摄像头支持的分辨率
        /// </summary>
        /// <param name="CameraName">摄像头的名称</param>
        /// <returns></returns>
        public static List<string> GetCameraSupportResolution(string CameraName)
        {
            DsDevice[] dsVideoDevice = GetAllVideoDevice();
            IBaseFilter theCamera = TestManager.CreateFilter(dsVideoDevice, CameraName);

            IFilterGraph2 graphBuilder = (IFilterGraph2)new FilterGraph();// 获取IFilterGraph2接口对象
            ICaptureGraphBuilder2 captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();//获取ICaptureGraphBuilder2接口对象
            int hr = captureGraphBuilder.SetFiltergraph(graphBuilder);//将过滤器图形附加到捕获图
            DsError.ThrowExceptionForHR(hr);
            //将视频输入设备添加到图形
            hr = graphBuilder.AddFilter(theCamera, "source filter");
            DsError.ThrowExceptionForHR(hr);

            //AMMediaType mediaType = new AMMediaType();
            //IntPtr pmt = IntPtr.Zero;
            //object oVideoStreamConfig;//视频流配置信息
            //hr = captureGraphBuilder.FindInterface(PinCategory.Capture, MediaType.Video, theCamera, typeof(IAMStreamConfig).GUID, out oVideoStreamConfig);
            //if (!(oVideoStreamConfig is IAMStreamConfig videoStreamConfig))
            //{
            //    throw new Exception("Failed to get IAMStreamConfig");
            //}


            List<string> AvailableResolutions = new List<string>();
            object streamConfig;

            // 获取配置接口
            hr = captureGraphBuilder.FindInterface(PinCategory.Capture,
                                            MediaType.Video,
                                            theCamera,
                                            typeof(IAMStreamConfig).GUID,
                                            out streamConfig);

            Marshal.ThrowExceptionForHR(hr);

            var videoStreamConfig = streamConfig as IAMStreamConfig;


            if (videoStreamConfig == null)
            {
                throw new Exception("Failed to get IAMStreamConfig");
            }
            var videoInfo = new VideoInfoHeader();
            int iCount;
            int iSize;
            int bitCount = 0;
            //IAMStreamConfig::GetNumberOfCapabilities获得设备所支持的媒体类型的数量。这个方法返回两个值，一个是媒体类型的数量，二是属性所需结构的大小。
            hr = videoStreamConfig.GetNumberOfCapabilities(out iCount, out iSize);
            Marshal.ThrowExceptionForHR(hr);
            IntPtr TaskMemPointer = Marshal.AllocCoTaskMem(iSize);
            AMMediaType pmtConfig = null;

            for (int iFormat = 0; iFormat < iCount; iFormat++)
            {
                IntPtr ptr = IntPtr.Zero;
                //通过函数IAMStreamConfig::GetStreamCaps来枚举媒体类型，要给这个函数传递一个序号作为参数，这个函数返回媒体类型和相应的属性结构体
                videoStreamConfig.GetStreamCaps(iFormat, out ptr, TaskMemPointer);

                pmtConfig = (AMMediaType)Marshal.PtrToStructure(ptr, typeof(AMMediaType));

                videoInfo = (VideoInfoHeader)Marshal.PtrToStructure(pmtConfig.formatPtr, typeof(VideoInfoHeader));

                if (videoInfo.BmiHeader.Size != 0 && videoInfo.BmiHeader.BitCount != 0)
                {
                    if (videoInfo.BmiHeader.BitCount > bitCount)
                    {
                        AvailableResolutions.Clear();
                        bitCount = videoInfo.BmiHeader.BitCount;
                    }
                    AvailableResolutions.Add(videoInfo.BmiHeader.Width + "*" + videoInfo.BmiHeader.Height);
                }
            }

            return AvailableResolutions;
        }

        /// <summary>
        /// 获取当前的摄像头信息
        /// </summary>
        /// <returns></returns>
        public static List<CameraEntity> GetCameras()
        {
            return XmlHelper.XmlHelper.FindAll<CameraEntity>();
        }
    }
}
