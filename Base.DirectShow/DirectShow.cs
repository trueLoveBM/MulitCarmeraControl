using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Base.DirectShow.Device;
using System.Collections;
using Base.DirectShow.Entity;
using System.Collections.Generic;
using BaseDirectShow.SharePreferences;
using Base.DirectShow.SharePreferences;
using System.Linq;

namespace Base.DirectShow
{
    public class DirectShow : ISampleGrabberCB
    {
        private static readonly object locker = new object();// 定义一个标识确保线程同步
        private static DirectShow _instance;
        public static DirectShow Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new DirectShow();
                        }
                    }
                }
                return _instance;
            }
        }

        #region 委托选择摄像头

        /// <summary>
        /// 选择摄像头,请在自己的窗体里赋值
        /// </summary>
        public Action<DeviceSecEntity> SecDevice;
        #endregion

        #region 属性
        private string _VideoDeviceName;
        /// <summary>
        /// 视频设备名称（为空系统自动加载）
        /// </summary>
        internal string VideoDeviceName
        {
            get { return _VideoDeviceName; }
            set { _VideoDeviceName = value; }
        }

        private string _VideoCompressorCategory;
        /// <summary>
        /// 视频压缩器（为空系统自动加载）
        /// </summary>
        internal string VideoCompressorCategory
        {
            get { return _VideoCompressorCategory; }
            set { _VideoCompressorCategory = value; }
        }

        private string _AudioInputDeviceName;
        /// <summary>
        /// 语音设备名称（为空系统自动加载）
        /// </summary>
        internal string AudioInputDeviceName
        {
            get { return _AudioInputDeviceName; }
            set { _AudioInputDeviceName = value; }
        }

        private string _AudioCompressor;
        /// <summary>
        /// 语音压缩器（为空系统自动加载）
        /// </summary>
        internal string AudioCompressor
        {
            get { return _AudioCompressor; }
            set { _AudioCompressor = value; }
        }
        private int _Frames;
        /// <summary>
        /// 帧数
        /// </summary>
        internal int Frames
        {
            get { return _Frames; }
            set { _Frames = value; }
        }

        private string _Resolution;
        /// <summary>
        /// 分辨率
        /// </summary>
        internal string Resolution
        {
            get { return _Resolution; }
            set { _Resolution = value; }
        }
        #endregion

        private Control ctVideo;//显示视频的控件对象
        IFilterGraph2 graphBuilder = null;
        /// <summary> capture graph builder interface.捕捉图表构建器接口 </summary>
        ICaptureGraphBuilder2 captureGraphBuilder = null;
        /// <summary>samp Grabber interface 采样器接口</summary>
        ISampleGrabber sampleGrabber = null;
        /// <summary> control interface.媒体控制器接口 </summary>
        IMediaControl mediaControl = null;

        #region 录像变量
        DsDevice[] dsVideoDevice;//所有的视频设备
        DsDevice[] dsVideoCompressorCategory;//所有视频解码器
        DsDevice[] dsAudioInputDevice;//所有的音频设备
        DsDevice[] dsAudioCompressorCategory;//所有的音频解码器
        /// <summary> 选择摄像头 </summary>
        IBaseFilter theDevice = null;
        /// <summary> 选择视频压缩器 </summary>
        IBaseFilter theDeviceCompressor = null;
        /// <summary> 选择声音设备 </summary>
        IBaseFilter theAudio = null;
        /// <summary> 选择声音压缩器 </summary>
        IBaseFilter theAudioCompressor = null;

        //SetOutputFileName为我们创建图形的文件编写器部分，并返回mux和sink
        IBaseFilter mux;
        IFileSinkFilter sink;
        #endregion

        private int _VideoWidth;//视频宽度
        private int _VideoHeight;//视频高度
        private int _VideoBitCount;//视频比特
        private int _ImageSize;//图片大小
        private string sVideoType = "wmv";//录制视频格式

        private bool BSamll = false;//是否拍摄小图
        private IntPtr m_ipBuffer = IntPtr.Zero;
        private volatile ManualResetEvent m_PictureReady = null;
        private volatile bool m_bWantOneFrame = false;

        /// <summary>
        ///  COM ISpecifyPropertyPages interface 
        ///  摄像头配置页面
        /// </summary>
        protected ISpecifyPropertyPages specifyPropertyPages;

        #region APIs
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] int Length);

        /// <summary>
        /// 系统配置页面
        /// </summary>
        [DllImport("olepro32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int OleCreatePropertyFrame(
            IntPtr hwndOwner, int x, int y,
            string lpszCaption, int cObjects,
            [In, MarshalAs(UnmanagedType.Interface)] ref object ppUnk,
            int cPages, IntPtr pPageClsID, int lcid, int dwReserved, IntPtr pvReserved);
        #endregion

        /// <summary> sample callback, NOT USED. </summary>
        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        /// <summary> 采集回调：buffer callback, COULD BE FROM FOREIGN THREAD. 缓冲区回调，可以来自外线程</summary>
        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            Debug.Assert(BufferLen == Math.Abs(_VideoBitCount / 8 * _VideoWidth) * _VideoHeight, "错误的 buffer 长度");
            if (m_bWantOneFrame)
            {
                m_bWantOneFrame = false;
                Debug.Assert(m_ipBuffer != IntPtr.Zero, "空的 buffer");
                // Save the buffer
                CopyMemory(m_ipBuffer, pBuffer, BufferLen);
                // Picture is ready.
                m_PictureReady.Set();
            }
            return 0;
        }


        #region 预览视频

        /// <summary>
        /// 预览视频
        /// </summary>
        /// <param name="clVideo">视频显示的控件对象</param>
        internal bool Preview(Control clVideo)
        {
            if (!InitDevice())//初始化设备
            {
                return false;
            }
            StopRun();//先停止播放
            ctVideo = clVideo;
            SetVideoShow(_Resolution, _Frames);
            return StartRun();
        }

        /// <summary>
        /// 初始化设备
        /// </summary>
        internal bool InitDevice()
        {
            try
            {
                if (!IsCorrectDirectXVersion())//检查DirectX版本
                {
                    throw new Exception("未安装DirectX 8.1！");
                }
                dsVideoDevice = GetAllVideoDevice();//读摄像头信息
                if (dsVideoDevice == null || dsVideoDevice.Length == 0)//如果摄像头不存在则抛出异常
                {
                    throw new Exception("未找到摄像头设备！");
                }
                if (!string.IsNullOrEmpty(_VideoDeviceName))
                {
                    theDevice = CreateFilter(dsVideoDevice, _VideoDeviceName);
                    if (theDevice == null)
                    {
                        MessageBox.Show("输入摄像机名称不存在！");
                    }
                }
                bool blManualSettings = false;//是否手动设置
                if (theDevice == null)
                {
                    dsVideoDevice = dsVideoDevice.ToList().Take(1).ToArray();
                    if (dsVideoDevice.Length == 1)
                    {
                        theDevice = CreateFilter(dsVideoDevice, dsVideoDevice[0].Name);
                    }
                    else
                    {

                        //上次使用的摄像头名称
                        var LastUseCameraName = CameraUtils.Instance.GetLastCamera();
                        //查找这个摄像头
                        bool FindLastCamera = dsVideoDevice.ToList().Find(m => m.Name == LastUseCameraName) != null;


                        if (string.IsNullOrEmpty(LastUseCameraName) || !FindLastCamera)
                        {
                            if (SecDevice != null)
                            {
                                DeviceSecEntity entity = new DeviceSecEntity();
                                //用户自定义选择相应设备
                                SecDevice(entity);
                                if (entity.SetFisished)
                                {
                                    //获取设备
                                    if (!dsVideoDevice.ToList().ConvertAll(m => m.Name).Contains(entity.VideoDeviceName))
                                    {
                                        throw new Exception($"不存在该摄像头设备{entity.VideoDeviceName}");
                                    }

                                    CameraUtils.Instance.SetLastCamera(entity.VideoDeviceName);
                                    //获取当前的所有音频设备
                                    DsDevice[] dsArray = GetAllAudioDevice();
                                    //获取音频
                                    if (!dsArray.ToList().ConvertAll(m => m.Name).Contains(entity.AudioInputDeviceName))
                                    {
                                        throw new Exception($"不存在该设备{entity.VideoDeviceName}");
                                    }

                                    //选择用户使用的设备及分辨率
                                    theDevice = CreateFilter(dsVideoDevice, entity.VideoDeviceName);
                                    theAudio = CreateFilter(dsArray, entity.AudioInputDeviceName);
                                    _Frames = entity.Frames;
                                    _Resolution = entity.Resolution;

                                    blManualSettings = true;//添加摄像机设置
                                }
                                else
                                {
                                    throw new Exception("用户取消了操作");
                                }
                            }
                            else
                            {
                                throw new Exception("请实现摄像头选择");
                            }
                        }
                        else
                        {
                            theDevice = CreateFilter(dsVideoDevice, LastUseCameraName);
                        }
                    }
                }
                if (!blManualSettings)
                {
                    //dsVideoCompressorCategory = DsDevice.GetDevicesOfCat(FilterCategory.VideoCompressorCategory);//读视频压缩器信息
                    //theDeviceCompressor = CreateFilter(dsVideoCompressorCategory, _VideoCompressorCategory);//获取视频压缩器IBaseFilter

                    dsAudioInputDevice = GetAllAudioDevice();
                    if (string.IsNullOrEmpty(_AudioInputDeviceName))//若用户指定了音频设备
                        theAudio = CreateFilter(dsAudioInputDevice, _AudioInputDeviceName);//获取音频设备IBaseFilter
                    else //TODO 目前默认使用第一个音频设备
                        theAudio = CreateFilter(dsAudioInputDevice, dsAudioInputDevice[0].Name);

                    //dsAudioCompressorCategory = DsDevice.GetDevicesOfCat(FilterCategory.AudioCompressorCategory);//读取音频压缩器信息
                    //theDeviceCompressor = CreateFilter(dsAudioCompressorCategory, _AudioCompressor);//获取音频压缩器IBaseFilter
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化设备失败：" + ex.Message);
                return false;
            }
        }


        /// <summary>
        /// 获取当前用户可用的所有音频设备
        /// </summary>
        /// <returns></returns>
        internal DsDevice[] GetAllAudioDevice()
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
        internal DsDevice[] GetAllVideoDevice()
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
        /// 验证是否安装DirectX
        /// </summary>
        /// <returns></returns>
        internal bool IsCorrectDirectXVersion()
        {
            return File.Exists(Path.Combine(Environment.SystemDirectory, "dpnhpast.dll"));
        }

        /// <summary>
        /// 根据设备集合、设备名称获取指定设备的BaseFilter
        /// </summary>
        /// <param name="dsDevice">设备集合</param>
        /// <param name="friendlyname">设备名</param>
        /// <returns></returns>
        private IBaseFilter CreateFilter(DsDevice[] dsDevice, string friendlyname)
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
        ///设置视频显示
        /// </summary>
        internal void SetVideoShow(string Resolution, int Frames)
        {
            if (theDevice == null)
            {
                return;
            }
            if (graphBuilder != null)
            {
                mediaControl.Stop();
            }
            CreateGraph(Resolution, Frames);
            //渲染设备的任何预览引脚 并且把sampleGrabber添加到预览
            int hr = captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, theDevice, (sampleGrabber as IBaseFilter), null);
            //DsError.ThrowExceptionForHR(hr);
            hr = captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Audio, theAudio, null, null);
            //DsError.ThrowExceptionForHR(hr);
            GetVideoHeaderInfo(sampleGrabber);//获取视频头文件信息

            //从图中获取视频窗口
            IVideoWindow videoWindow = null;
            videoWindow = (IVideoWindow)graphBuilder;

            //将视频窗口的所有者设置为某种IntPtr（任何控件的句柄 - 可以是窗体/按钮等）
            hr = videoWindow.put_Owner(ctVideo.Handle);
            //DsError.ThrowExceptionForHR(hr);

            //设置视频窗口的样式
            hr = videoWindow.put_WindowStyle((int)WindowStyle.Child | (int)WindowStyle.ClipChildren);
            //DsError.ThrowExceptionForHR(hr);

            //在主应用程序窗口的客户端中定位视频窗口
            hr = videoWindow.SetWindowPosition(0, 0, ctVideo.Width, ctVideo.Height);
            //DsError.ThrowExceptionForHR(hr);

            //使视频窗口可见
            hr = videoWindow.put_Visible((int)OABool.True);
            //DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// 构建捕获图
        /// </summary>
        internal void CreateGraph(string Resolution, int Frames)
        {
            if (graphBuilder != null)
            {
                return;
            }
            graphBuilder = (IFilterGraph2)new FilterGraph();// 获取IFilterGraph2接口对象
            captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();//获取ICaptureGraphBuilder2接口对象

            int hr = captureGraphBuilder.SetFiltergraph(this.graphBuilder);//将过滤器图形附加到捕获图
            DsError.ThrowExceptionForHR(hr);

            //将视频输入设备添加到图形
            hr = graphBuilder.AddFilter(theDevice, "source filter");
            DsError.ThrowExceptionForHR(hr);

            //将视频压缩器过滤器添加到图形
            if (theDeviceCompressor != null)
            {
                hr = graphBuilder.AddFilter(theDeviceCompressor, "devicecompressor filter");
                DsError.ThrowExceptionForHR(hr);
            }
            //将音频输入设备添加到图形
            if (theAudio != null)
            {
                hr = graphBuilder.AddFilter(theAudio, "audio filter");
                DsError.ThrowExceptionForHR(hr);
            }
            //将音频压缩器过滤器添加到图形
            if (theAudioCompressor != null)
            {
                hr = graphBuilder.AddFilter(theAudioCompressor, "audiocompressor filter");
                DsError.ThrowExceptionForHR(hr);
            }
            mediaControl = (IMediaControl)this.graphBuilder;//获取IMediaControl接口对象

            m_PictureReady = new ManualResetEvent(false);

            sampleGrabber = new SampleGrabber() as ISampleGrabber;//添加采样器接口.
            ConfigureSampleGrabber(sampleGrabber);// 配置SampleGrabber。添加预览回调
            hr = this.graphBuilder.AddFilter(sampleGrabber as IBaseFilter, "Frame Callback");// 将SampleGrabber添加到图形.
            DsError.ThrowExceptionForHR(hr);


            AMMediaType mediaType = new AMMediaType();
            IntPtr pmt = IntPtr.Zero;
            object oVideoStreamConfig;//视频流配置信息
            hr = captureGraphBuilder.FindInterface(PinCategory.Capture, MediaType.Video, theDevice, typeof(IAMStreamConfig).GUID, out oVideoStreamConfig);
            if (!(oVideoStreamConfig is IAMStreamConfig videoStreamConfig))
            {
                throw new Exception("Failed to get IAMStreamConfig");
            }

            hr = videoStreamConfig.GetFormat(out pmt);
            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);
            Marshal.PtrToStructure(pmt, mediaType);


            VideoInfoHeader videoInfoHeader = new VideoInfoHeader();
            Marshal.PtrToStructure(mediaType.formatPtr, videoInfoHeader);

            // 设置帧率
            if (Frames > 0)
            {
                videoInfoHeader.AvgTimePerFrame = 10000000 / Frames;
            }

            //获取当前摄像头所支持的所有分辨率
            var allResolution = GetCameraSupportResolution();
            //当前所设置的分辨率此摄像头不支持
            if (!allResolution.Contains(Resolution))
            {
                Resolution = allResolution[0];
            }


            // 设置宽度 设置高度
            if (!string.IsNullOrEmpty(Resolution) && Resolution.Split('*').Length > 1)
            {
                videoInfoHeader.BmiHeader.Width = Convert.ToInt32(Resolution.Split('*')[0]);
                videoInfoHeader.BmiHeader.Height = Convert.ToInt32(Resolution.Split('*')[1]);
            }
            // 复制媒体结构
            Marshal.StructureToPtr(videoInfoHeader, mediaType.formatPtr, false);
            // 设置新的视频格式
            hr = videoStreamConfig.SetFormat(mediaType);
            DsError.ThrowExceptionForHR(hr);
            DsUtils.FreeAMMediaType(mediaType);
            mediaType = null;
        }

        /// <summary>
        /// 配置SampleGrabber
        /// </summary>
        /// <param name="sampGrabber"></param>
        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
        {
            AMMediaType media = new AMMediaType();
            media.majorType = MediaType.Video;
            media.subType = MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;
            int hr = sampGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(hr);
            DsUtils.FreeAMMediaType(media);
            media = null;
            hr = sampGrabber.SetCallback(this, 1);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// 读取视频头文件信息
        /// </summary>
        /// <param name="sampleGrabber"></param>
        private void GetVideoHeaderInfo(ISampleGrabber sampleGrabber)
        {
            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();
            int hr = sampleGrabber.GetConnectedMediaType(media);//读取视频文件信息
            DsError.ThrowExceptionForHR(hr);
            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }
            // 获取视频文件信息
            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            _VideoWidth = videoInfoHeader.BmiHeader.Width;
            _VideoHeight = videoInfoHeader.BmiHeader.Height;
            _VideoBitCount = videoInfoHeader.BmiHeader.BitCount;
            _ImageSize = videoInfoHeader.BmiHeader.ImageSize;
            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        /// <summary>
        /// 开始运行
        /// </summary>
        internal bool StartRun()
        {
            if (mediaControl == null)
            {
                MessageBox.Show("播放对象为空！");
                return false;
            }

            int iRun = mediaControl.Run();//运行图表


            if (iRun != 1)
            {
                //    DsError.ThrowExceptionForHR(iRun);
                //    MessageBox.Show("播放失败！");
                //    return false;
            }
            return true;
        }
        #endregion

        #region 抓拍图片
        /// <summary>
        /// 抓拍照片
        /// </summary>
        /// <param name="path">抓拍照片完整路径，包含文件名</param>
        /// <param name="iImageWidth">抓怕图片宽度，默认获取视频信息宽度</param>
        /// <param name="iImageHeight">抓拍图片高度，默认获取视频信息高度</param>
        /// <param name="bSamll">是否抓小图，默认不抓拍</param>
        /// <param name="samllPath">抓拍小图路径，抓拍小图时则必须填写</param>
        internal bool TakeAPicture(string path, int iImageWidth = 0, int iImageHeight = 0, bool bSamll = false, string samllPath = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("请输入抓拍照片完整路径！");
                return false;
            }
            if (bSamll && string.IsNullOrEmpty(samllPath))
            {
                MessageBox.Show("请输入抓拍小照片完整路径！");
                return false;
            }
            if (!VerStarPreview())
            {
                MessageBox.Show("请先开始预览！");
                return false;
            }
            IntPtr ip = GetNextFrame();
            int iCapWidth = 0; int iCapHeight = 0;
            if (iImageWidth == 0 && iImageHeight == 0)
            {
                iCapWidth = _VideoWidth;
                iCapHeight = _VideoHeight;
            }
            else
            {
                iCapWidth = iImageWidth;
                iCapHeight = iImageHeight;
            }
            if (((iCapWidth & 0x03) != 0) || (iCapWidth < 32) || (iCapWidth > 4096) || (iCapHeight < 32) || (iCapHeight > 4096))
            {
                MessageBox.Show("输入的宽度或高度必须在32到4096之间");
                return false;
            }
            BSamll = bSamll;
            Bitmap bitmap = new Bitmap(iCapWidth, iCapHeight, (_VideoBitCount / 8) * iCapWidth, PixelFormat.Format24bppRgb, ip);

            Bitmap bitmap_clone = bitmap.Clone(new Rectangle(0, 0, _VideoWidth, _VideoHeight), PixelFormat.Format24bppRgb);
            bitmap_clone.RotateFlip(RotateFlipType.RotateNoneFlipY);

            // Release any previous buffer
            if (ip != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(ip);
                ip = IntPtr.Zero;
            }
            bitmap.Dispose();
            bitmap = null;
            bitmap_clone.Save(path, ImageFormat.Jpeg);
            if (BSamll)
            {
                Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                Image myThumbnail = bitmap_clone.GetThumbnailImage(120, 120, myCallback, IntPtr.Zero);
                myThumbnail.Save(samllPath);
            }
            return true;
        }

        /// <summary>
        /// 截取摄像头
        /// </summary>
        /// <returns>截取成功则返回其Bitmap对象，否则为空</returns>
        internal Bitmap CaptureCamera(int iImageWidth = 0, int iImageHeight = 0)
        {
            IntPtr ip = GetNextFrame();
            int iCapWidth = 0; int iCapHeight = 0;
            if (iImageWidth == 0 && iImageHeight == 0)
            {
                iCapWidth = _VideoWidth;
                iCapHeight = _VideoHeight;
            }
            else
            {
                iCapWidth = iImageWidth;
                iCapHeight = iImageHeight;
            }

            if (((iCapWidth & 0x03) != 0) || (iCapWidth < 32) || (iCapWidth > 4096) || (iCapHeight < 32) || (iCapHeight > 4096))
            {
                MessageBox.Show("输入的宽度或高度必须在32到4096之间");
                return null;
            }

            Bitmap bitmap = new Bitmap(iCapWidth, iCapHeight, (_VideoBitCount / 8) * iCapWidth, PixelFormat.Format24bppRgb, ip);
            return bitmap;
        }

        /// <summary>
        /// 获取视频下一帧
        /// </summary>
        /// <returns></returns>
        private IntPtr GetNextFrame()
        {
            // get ready to wait for new image
            m_PictureReady.Reset();
            m_ipBuffer = Marshal.AllocCoTaskMem(Math.Abs(_VideoBitCount / 8 * _VideoWidth) * _VideoHeight);
            try
            {
                m_bWantOneFrame = true;
                if (!m_PictureReady.WaitOne(5000, false))// 开始等待
                {
                    throw new Exception("获取图片超时");
                }
            }
            catch
            {
                Marshal.FreeCoTaskMem(m_ipBuffer);
                m_ipBuffer = IntPtr.Zero;
                throw;
            }
            // 返回图片
            return m_ipBuffer;
        }
        internal bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// 验证是否开始预览
        /// </summary>
        /// <returns></returns>
        private bool VerStarPreview()
        {
            if (theDevice == null || graphBuilder == null)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 开始录制
        /// <summary>
        /// 开始录制
        /// </summary>
        internal bool StartMonitorRecord(string sViewPath)
        {
            if (!InitGraph(sViewPath))
            {
                return false;
            }
            return StartRun();
        }

        /// <summary>
        /// 初始化图表
        /// </summary>
        internal bool InitGraph(string sViewPath)
        {
            if (!VerStarPreview())
            {
                MessageBox.Show("请先开始预览！");
                return false;
            }
            mediaControl.Stop();
            CreateGraph(_Resolution, _Frames);
            int hr;
            if (sVideoType == "wmv")
            {
                hr = captureGraphBuilder.SetOutputFileName(MediaSubType.Asf, sViewPath, out mux, out sink);
                Marshal.ThrowExceptionForHR(hr);
                try
                {
                    IConfigAsfWriter lConfig = mux as IConfigAsfWriter;

                    // Windows Media Video (audio, 700 Kbps)
                    // READ THE README for info about using guids
                    Guid cat = new Guid("ec298949-639b-45e2-96fd-4ab32d5919c2");
                    hr = lConfig.ConfigureFilterUsingProfileGuid(cat);
                    Marshal.ThrowExceptionForHR(hr);
                }
                finally
                {
                    Marshal.ReleaseComObject(sink);
                }
            }
            else if (sVideoType == "avi")
            {
                hr = captureGraphBuilder.SetOutputFileName(MediaSubType.Avi, sViewPath, out mux, out sink);
                DsError.ThrowExceptionForHR(hr);
            }
            //将设备和压缩器连接到mux，以呈现图形的捕获部分
            hr = captureGraphBuilder.RenderStream(PinCategory.Capture, MediaType.Interleaved, theDevice, theDeviceCompressor, mux);
            //DsError.ThrowExceptionForHR(hr);
            if (hr < 0)
            {
                hr = captureGraphBuilder.RenderStream(PinCategory.Capture, MediaType.Video, theDevice, theDeviceCompressor, mux);
                if (hr < 0) { DsError.ThrowExceptionForHR(hr); }
            }

            //将音频添加进去
            hr = captureGraphBuilder.RenderStream(PinCategory.Capture, MediaType.Audio, theAudio, theAudioCompressor, mux);
            DsError.ThrowExceptionForHR(hr);

            Marshal.ReleaseComObject(mux);
            Marshal.ReleaseComObject(sink);
            return true;
        }
        #endregion

        #region 停止录制
        /// <summary>
        /// 停止录制
        /// </summary>
        internal void StopMonitorRecord()
        {
            if (!VerStarPreview())
            {
                MessageBox.Show("请先开始预览！");
                return;
            }
            StopRun();
            SetVideoShow(_Resolution, _Frames);
            StartRun();
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        internal void StopRun()
        {
            if (mediaControl != null)
            {
                //Stop the Graph
                mediaControl.Stop();
            }
            if (graphBuilder != null)
            {
                //Release COM objects
                Marshal.ReleaseComObject(graphBuilder);
                graphBuilder = null;
            }
            if (captureGraphBuilder != null)
            {
                Marshal.ReleaseComObject(captureGraphBuilder);
                captureGraphBuilder = null;
            }
        }
        #endregion

        #region 关闭视频设备
        /// <summary>
        /// 释放资源
        /// </summary>
        internal void Dispose()
        {
            if (mediaControl != null)
            {
                mediaControl.Stop();
                mediaControl = null;
            }
            if (graphBuilder != null)
            {
                Marshal.ReleaseComObject(graphBuilder);
                graphBuilder = null;
            }
            if (captureGraphBuilder != null)
            {
                Marshal.ReleaseComObject(captureGraphBuilder);
                captureGraphBuilder = null;
            }
            if (sampleGrabber != null)
            {
                Marshal.ReleaseComObject(sampleGrabber);
                sampleGrabber = null;
            }
            if (theDevice != null)
            {
                Marshal.ReleaseComObject(theDevice);
                theDevice = null;
            }
            if (theDeviceCompressor != null)
            {
                Marshal.ReleaseComObject(theDeviceCompressor);
                theDeviceCompressor = null;
            }
            if (theAudio != null)
            {
                Marshal.ReleaseComObject(theAudio);
                theAudio = null;
            }
            if (theAudioCompressor != null)
            {
                Marshal.ReleaseComObject(theAudioCompressor);
                theAudioCompressor = null;
            }
            if (mux != null)
            {
                Marshal.ReleaseComObject(mux);
                mux = null;
            }
            if (sink != null)
            {
                Marshal.ReleaseComObject(sink);
                sink = null;
            }
            DisposeDevice(dsVideoDevice);//释放视频设备
            DisposeDevice(dsVideoCompressorCategory);//释放视频解码器设备
            DisposeDevice(dsAudioInputDevice);//释放语音设备
            DisposeDevice(dsAudioCompressorCategory);//释放语音解码器设备
        }

        /// <summary>
        ///释放设备资源
        /// </summary>
        /// <param name="dsDevice"></param>
        private void DisposeDevice(DsDevice[] dsDevice)
        {
            if (dsDevice != null)
            {
                foreach (DsDevice ds in dsDevice)
                {
                    ds.Dispose();
                }
                dsDevice = null;
            }
        }
        #endregion


        #region 视频属性配置页面  调系统dll方式
        /// <summary>
        /// 打开摄像头配置页面，系统原生方式
        /// </summary>
        internal void ChangeCameraSetting(IntPtr Handle)
        {
            DsCAUUID cauuid = new DsCAUUID();
            try
            {
                specifyPropertyPages = theDevice as ISpecifyPropertyPages;
                if (specifyPropertyPages == null)
                {
                    MessageBox.Show("请先打开视频设备！");
                    return;
                }
                //返回filter所支持的属性页的CLSID
                int hr = specifyPropertyPages.GetPages(out cauuid);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);

                object o = specifyPropertyPages;
                //获取属性页
                hr = OleCreatePropertyFrame(Handle, 30, 30, null, 1,
                    ref o, cauuid.cElems, cauuid.pElems, 0, 0, IntPtr.Zero);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable display property page. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        /// <summary>
        /// 读取当前摄像头配置并保存到方案中
        /// </summary>
        /// <param name="VideoSettingName">当前配置的方案名</param>
        /// <param name="AsDefault">是否设为摄像头默认设置</param>
        internal void SaveCameraConfigToSetting(string VideoSettingName, bool AsDefault)
        {
            IAMVideoProcAmp videoProcAmp = theDevice as IAMVideoProcAmp;
            VideoProcAmpFlags flags = VideoProcAmpFlags.Manual;
            //亮度值 0到255
            int LightValue = 0;
            videoProcAmp.Get(VideoProcAmpProperty.Brightness, out LightValue, out flags);
            //对比度 0到255
            int ContrastValue = 0;
            videoProcAmp.Get(VideoProcAmpProperty.Contrast, out ContrastValue, out flags);
            //饱和度 0到255 
            int SaturationValue = 0;
            videoProcAmp.Get(VideoProcAmpProperty.Saturation, out SaturationValue, out flags);
            //色调 -127 到127
            int HueValue = 0;
            videoProcAmp.Get(VideoProcAmpProperty.Hue, out HueValue, out flags);
            //清晰度 0到15
            int SharpnessValue = 0;
            videoProcAmp.Get(VideoProcAmpProperty.Sharpness, out SharpnessValue, out flags);
            //伽玛 1到8
            int GammaValue = 0;
            videoProcAmp.Get(VideoProcAmpProperty.Gamma, out GammaValue, out flags);
            //启用颜色 不支持
            int ColorEnable = 0;
            videoProcAmp.Get(VideoProcAmpProperty.ColorEnable, out ColorEnable, out flags);
            //白平衡 不支持
            int WhiteBalanceValue = 0;
            videoProcAmp.Get(VideoProcAmpProperty.WhiteBalance, out WhiteBalanceValue, out flags);
            //背光补偿 1 到 5
            int BacklightCompensation = 0;
            videoProcAmp.Get(VideoProcAmpProperty.BacklightCompensation, out BacklightCompensation, out flags);
            //增益 不支持
            int Gain = 0;
            videoProcAmp.Get(VideoProcAmpProperty.Gain, out Gain, out flags);

            VideoSettingEntity setting = new VideoSettingEntity();
            setting.Brightness = LightValue;
            setting.VideoSettingName = VideoSettingName;
            setting.ContrastRatio = ContrastValue;
            setting.Saturation = SaturationValue;
            setting.Hue = HueValue;
            setting.Sharpness = SharpnessValue;
            setting.Gamma = GammaValue;
            setting.ColorEnable = Convert.ToBoolean(ColorEnable);
            setting.WhiteBalance = WhiteBalanceValue;
            setting.BacklightCompensation = BacklightCompensation;
            setting.Gain = Gain;
            setting.DefaultSetting = AsDefault;
            VideoSettingUtils.Instance.SaveVideoSetting(setting, AsDefault);
        }

        /// <summary>
        /// 保存摄像头配置到方案中,推荐使用 SaveCameraConfigToSetting(string VideoSettingName, bool AsDefault 方法
        /// </summary>
        /// <param name="setting">配置信息实体</param>
        [Obsolete]
        internal void SaveCameraConfigToSetting(VideoSettingEntity setting)
        {
            VideoSettingUtils.Instance.SaveVideoSetting(setting);
        }

        /// <summary>
        /// 获取摄像头所有配置方案
        /// 配合 ChangeCameraConfigToSetting  方法切换摄像头所有方案
        /// </summary>
        /// <returns></returns>
        internal List<VideoSettingEntity> GetAllCameraSettings()
        {
            return VideoSettingUtils.Instance.GetAllVideoSettings();
        }

        /// <summary>
        /// 获取当前摄像头采用的默认设置
        /// </summary>
        /// <returns></returns>
        internal VideoSettingEntity GetCurrentDefaultCameraSetting()
        {
            var result = VideoSettingUtils.Instance.GetAllVideoSettings();
            if (result.Count > 0)
                return result.Find(m => m.DefaultSetting);
            else
                return null;
        }


        /// <summary>
        /// 切换摄像头配置为指定配置方案
        /// 配置方案可以由GetAllCameraSettings进行传参
        /// </summary>
        /// <param name="setting">摄像头配置方案</param>
        /// <param name="asDefault">是否将此配置设为默认配置</param>
        /// <returns></returns>
        internal int ChangeCameraConfigToSetting(VideoSettingEntity setting, bool asDefault = false)
        {
            if (asDefault)
                VideoSettingUtils.Instance.SetDefaultSettings(setting);

            int iResult = 0;
            IAMVideoProcAmp videoProcAmp = theDevice as IAMVideoProcAmp;
            if (videoProcAmp == null)
            {
                iResult = -1;
                return iResult;
            }
            int val;
            int min;
            int max;
            int step;
            int defaultValue;
            VideoProcAmpFlags flags = VideoProcAmpFlags.Manual;
            // 设置亮度
            if (setting.Brightness != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.Brightness, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Brightness, out val, out flags);
                    //val = min + (max - min) * setting.Brightness / 255;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.Brightness, setting.Brightness, flags);
                }
            }
            //设置对比度
            if (setting.ContrastRatio != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.Contrast, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Contrast, out val, out flags);
                    //val = min + (max - min) * setting.ContrastRatio / 100;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.Contrast, setting.ContrastRatio, flags);
                }
            }//设置饱和度
            if (setting.Saturation != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.Saturation, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Saturation, out val, out flags);
                    //val = min + (max - min) * setting.Saturation / 100;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.Saturation, setting.Saturation, flags);
                }
            }
            //设置色调
            if (setting.Hue != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.Hue, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Saturation, out val, out flags);
                    //val = min + (max - min) * setting.Saturation / 100;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.Hue, setting.Hue, flags);
                }
            }
            //设置清晰度
            if (setting.Sharpness != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.Sharpness, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Saturation, out val, out flags);
                    //val = min + (max - min) * setting.Saturation / 100;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.Sharpness, setting.Sharpness, flags);
                }
            }
            //设置伽玛
            if (setting.Gamma != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.Gamma, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Saturation, out val, out flags);
                    //val = min + (max - min) * setting.Saturation / 100;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.Gamma, setting.Gamma, flags);
                }
            }
            //设置启用颜色
            if (setting.Gamma != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.ColorEnable, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Saturation, out val, out flags);
                    //val = min + (max - min) * setting.Saturation / 100;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.ColorEnable, Convert.ToInt32(setting.ColorEnable), flags);
                }
            }
            //白平衡
            if (setting.WhiteBalance != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.WhiteBalance, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Saturation, out val, out flags);
                    //val = min + (max - min) * setting.Saturation / 100;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.WhiteBalance, setting.WhiteBalance, flags);
                }
            }
            //背光补偿
            if (setting.WhiteBalance != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.BacklightCompensation, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Saturation, out val, out flags);
                    //val = min + (max - min) * setting.Saturation / 100;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.BacklightCompensation, setting.BacklightCompensation, flags);
                }
            }
            //增益
            if (setting.Gain != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.Gain, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Saturation, out val, out flags);
                    //val = min + (max - min) * setting.Saturation / 100;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.Gain, setting.Gain, flags);
                }
            }
            return iResult;
        }
        #endregion

        #region 视频属性配置页面 获取or设置摄像头亮度、对比度或者饱和度
        /// <summary>
        /// 设置摄像头亮度
        /// </summary>
        /// <param name="lightValue">亮度值0 到 100</param>
        /// <returns></returns>
        internal int SetLightValue(int lightValue)
        {
            int iResult = 0;
            IAMVideoProcAmp videoProcAmp = theDevice as IAMVideoProcAmp;
            if (videoProcAmp == null)
            {
                iResult = -1;
                return iResult;
            }
            int val;
            int min;
            int max;
            int step;
            int defaultValue;
            VideoProcAmpFlags flags = VideoProcAmpFlags.Manual;
            // 设置亮度
            if (lightValue != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.Brightness, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Brightness, out val, out flags);
                    //val = min + (max - min) * lightValue / 255;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.Brightness, lightValue, flags);
                }
            }


            return iResult;
        }

        /// <summary>
        /// 获取摄像头当前亮度值
        /// </summary>
        /// <returns></returns>
        internal int GetLightValue()
        {
            int LightValue = 0;
            IAMVideoProcAmp videoProcAmp = theDevice as IAMVideoProcAmp;
            VideoProcAmpFlags flags = VideoProcAmpFlags.Manual;
            videoProcAmp.Get(VideoProcAmpProperty.Brightness, out LightValue, out flags);
            return LightValue;
        }

        /// <summary>
        /// 设置摄像头对比度
        /// </summary>
        /// <param name="ContrastValue">对比度值，0到100之间</param>
        /// <returns></returns>
        internal int SetContrastValue(int ContrastValue)
        {
            int iResult = 0;
            IAMVideoProcAmp videoProcAmp = theDevice as IAMVideoProcAmp;
            if (videoProcAmp == null)
            {
                iResult = -1;
                return iResult;
            }
            int val;
            int min;
            int max;
            int step;
            int defaultValue;
            //设置对比度
            VideoProcAmpFlags flags = VideoProcAmpFlags.Manual;
            if (ContrastValue != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.Contrast, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Contrast, out val, out flags);
                    //val = min + (max - min) * ContrastValue / 100;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.Contrast, ContrastValue, flags);
                }
            }
            return iResult;
        }

        /// <summary>
        /// 获取摄像头当前对比度值
        /// </summary>
        /// <returns></returns>
        internal int GetContrastValue()
        {
            int ContrastValue = 0;
            IAMVideoProcAmp videoProcAmp = theDevice as IAMVideoProcAmp;
            VideoProcAmpFlags flags = VideoProcAmpFlags.Manual;
            videoProcAmp.Get(VideoProcAmpProperty.Contrast, out ContrastValue, out flags);
            return ContrastValue;
        }

        /// <summary>
        /// 设置摄像头饱和度
        /// </summary>
        /// <param name="SaturationValue">饱和度 0到 100</param>
        /// <returns></returns>
        internal int SetSaturationValue(int SaturationValue)
        {
            int iResult = 0;
            IAMVideoProcAmp videoProcAmp = theDevice as IAMVideoProcAmp;
            if (videoProcAmp == null)
            {
                iResult = -1;
                return iResult;
            }
            int val;
            int min;
            int max;
            int step;
            int defaultValue;
            VideoProcAmpFlags flags = VideoProcAmpFlags.Manual;
            //设置饱和度
            if (SaturationValue != -1)
            {
                int hr = videoProcAmp.GetRange(VideoProcAmpProperty.Saturation, out min, out max, out step, out defaultValue, out flags);
                if (0 == hr)
                {
                    //videoProcAmp.Get(VideoProcAmpProperty.Saturation, out val, out flags);
                    //val = min + (max - min) * SaturationValue / 100;
                    iResult = videoProcAmp.Set(VideoProcAmpProperty.Saturation, SaturationValue, flags);
                }
            }

            return iResult;
        }

        /// <summary>
        /// 获取摄像头当前饱和度值
        /// </summary>
        /// <returns></returns>
        internal int GetSaturationValue()
        {
            int SaturationValue = 0;
            IAMVideoProcAmp videoProcAmp = theDevice as IAMVideoProcAmp;
            VideoProcAmpFlags flags = VideoProcAmpFlags.Manual;
            videoProcAmp.Get(VideoProcAmpProperty.Saturation, out SaturationValue, out flags);
            return SaturationValue;
        }
        #endregion

        #region 获取分辨率

        /// <summary>
        /// 获取指定摄像头所有支持的分辨率
        /// </summary>
        /// <returns></returns>
        internal List<string> GetCameraSupportResolutionByCameraName(string CameraName)
        {
            //获取指定摄像头
            var dsVideoDevice = GetAllVideoDevice();//读摄像头信息
            var theDevice = CreateFilter(dsVideoDevice, _VideoDeviceName);
            var captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();//获取ICaptureGraphBuilder2接口对象


            if (theDevice == null)
                return null;


            List<string> AvailableResolutions = new List<string>();
            object streamConfig;

            // 获取配置接口
            int hr = captureGraphBuilder.FindInterface(PinCategory.Capture,
                                             MediaType.Video,
                                             theDevice,
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
        /// 获取当前使用的摄像头支持的所有分辨率
        /// </summary>
        /// <returns></returns>
        internal List<string> GetCameraSupportResolution()
        {
            if (theDevice == null || captureGraphBuilder == null)
                return null;

            List<string> AvailableResolutions = new List<string>();
            object streamConfig;

            // 获取配置接口
            int hr = captureGraphBuilder.FindInterface(PinCategory.Capture,
                                             MediaType.Video,
                                             theDevice,
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
        /// 获取摄像头上次使用的分辨率
        /// </summary>
        /// <returns></returns>
        internal string GetLastCameraResolution()
        {
            return ResolutionUtils.Instance.GetLastCameraResolution();
        }

        #endregion

        /// <summary>
        /// 设置默认使用摄像头
        /// </summary>
        /// <param name="CameraName"></param>
        /// <returns></returns>
        public void SetDefaultUseCamera(string CameraName)
        {
            CameraUtils.Instance.SetLastCamera(CameraName);
        }
    }
}
