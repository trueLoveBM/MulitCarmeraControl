using Base.DirectShow.Device;
using Base.DirectShow.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Base.DirectShow
{
    public class TestMulitCamera : ISampleGrabberCB
    {

        #region 只读属性

        /// <summary>
        /// 当前使用的分辨率
        /// </summary>
        public string Resolution { get { return this._Resolution; } }

        #endregion

        #region  字段
        /// <summary>
        /// 当前正在使用的摄像头名称
        /// </summary>
        private string _bindCameraName;

        /// <summary>
        /// 指定的音频设备的名称
        /// </summary>
        private string _bindAudioName;

        /// <summary>
        /// 选择摄像头 
        /// </summary>
        IBaseFilter _theCamera = null;

        /// <summary>
        /// 选择视频压缩器
        /// </summary>
        IBaseFilter _theCameraCompressor = null;

        /// <summary>
        /// 选择摄像头 
        /// </summary>
        IBaseFilter _theAudio = null;

        /// <summary>
        /// 选择声音压缩器
        /// </summary>
        IBaseFilter _theAudioCompressor = null;

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
        /// 图像
        /// </summary>
        IFilterGraph2 _graphBuilder = null;

        /// <summary>
        /// capture graph builder interface.捕捉图表构建器接口
        /// </summary>
        ICaptureGraphBuilder2 _captureGraphBuilder = null;

        /// <summary>
        /// 媒体控制器接口
        /// 即摄像头流显示的控件
        /// </summary>
        IMediaControl _mediaControl = null;

        /// <summary>
        /// samp Grabber interface 采样器接口
        /// </summary>
        ISampleGrabber sampleGrabber = null;

        private string sVideoType = "wmv";//录制视频格式
        private int _VideoWidth;//视频宽度
        private int _VideoHeight;//视频高度
        private int _VideoBitCount;//视频比特
        private int _ImageSize;//图片大小

        /// <summary>
        /// TODO
        /// </summary>
        private volatile ManualResetEvent m_PictureReady = null;

        /// <summary>
        /// TODO
        /// </summary>
        private volatile bool m_bWantOneFrame = false;

        /// <summary>
        /// TODO
        /// </summary>
        private IntPtr m_ipBuffer = IntPtr.Zero;

        /// <summary>
        /// 当前预览控件的句柄
        /// </summary>
        private IntPtr _priviewControlHandle;
        /// <summary>
        /// 当前预览控件的宽度
        /// </summary>
        private int _priviewControlWidth;
        /// <summary>
        /// 当前预览控件的高度
        /// </summary>
        private int _priviewControlHeigh;


        //SetOutputFileName为我们创建图形的文件编写器部分，并返回mux和sink TODO
        IBaseFilter mux;
        IFileSinkFilter sink;


        /// <summary>
        ///  COM ISpecifyPropertyPages interface 
        ///  摄像头配置页面
        /// </summary>
        protected ISpecifyPropertyPages specifyPropertyPages;
        #endregion

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

        #region 参数设置

        /// <summary>
        /// 设置使用摄像头
        /// </summary>
        /// <param name="CameraName"></param>
        public void BindCamera(string CameraName)
        {
            this._bindCameraName = CameraName;
        }

        /// <summary>
        /// 设置使用的音频
        /// </summary>
        /// <param name="AudioName"></param>
        public void BindAudio(string AudioName)
        {
            this._bindAudioName = AudioName;
        }

        /// <summary>
        /// 修改分辨率
        /// </summary>
        /// <param name="Resolution"></param>
        public void SetResolution(string Resolution)
        {
            this._Resolution = Resolution;

            if (VerStarPreview())
            {
                this.Preview(this._priviewControlHandle, this._priviewControlWidth, this._priviewControlHeigh);
            }
        }
        #endregion

        #region 接口实现

        /// <summary>
        ///  ISampleGrabberCB接口  未使用
        ///  sample callback, NOT USED
        /// </summary>
        /// <param name="SampleTime"></param>
        /// <param name="pSample"></param>
        /// <returns></returns>
        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        /// <summary>
        /// ISampleGrabberCB 接口
        /// 采集回调：buffer callback, COULD BE FROM FOREIGN THREAD. 缓冲区回调，可以来自外线程
        /// </summary>
        /// <param name="SampleTime"></param>
        /// <param name="pBuffer"></param>
        /// <param name="BufferLen"></param>
        /// <returns></returns>
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

        #endregion

        #region 预览摄像头

        /// <summary>
        /// 预览视频
        /// </summary>
        /// <param name="PriviewControlHandle">预览控件的句柄</param>
        /// <param name="PriviewControlWidth">预览宽度</param>
        /// <param name="PriviewControlHeight">预览高度</param>
        /// <returns></returns>
        public bool Preview(IntPtr PriviewControlHandle, int PriviewControlWidth, int PriviewControlHeight)
        {
            if (!InitDevice())//初始化设备
            {
                return false;
            }
            //先停止播放
            StopRun();

            //记录当前显示控件句柄及宽高信息
            _priviewControlHandle = PriviewControlHandle;
            _priviewControlWidth = PriviewControlWidth;
            _priviewControlHeigh = PriviewControlHeight;
            SetVideoShow(_Resolution, _Frames);

            return StartRun();
        }

        #endregion

        #region 录像

        /// <summary>
        /// 开始录制
        /// </summary>
        public bool StartMonitorRecord(string sViewPath)
        {
            if (!InitGraph(sViewPath))
            {
                return false;
            }
            return StartRun();
        }




        /// <summary>
        /// 停止录制
        /// </summary>
        public void StopMonitorRecord()
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
        #endregion

        #region 获取当前帧，拍照

        /// <summary>
        /// 抓拍照片
        /// </summary>
        /// <param name="SavePath">抓拍照片完整路径，包含文件名</param>
        /// <param name="iImageWidth">抓拍图片宽度，默认获取视频信息宽度</param>
        /// <param name="iImageHeight">抓拍图片高度，默认获取视频信息高度</param>
        /// <param name="bSamll">是否抓小图，默认不抓拍</param>
        /// <param name="samllPath">抓拍小图保存路径，抓拍小图时则必须填写</param>
        public bool CameraShot(string SavePath, int iImageWidth = 0, int iImageHeight = 0, bool bSamll = false, string samllPath = "")
        {
            if (string.IsNullOrEmpty(SavePath))
            {
                MessageBox.Show("请输入抓拍照片保存路径！");
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
            bitmap_clone.Save(SavePath, ImageFormat.Jpeg);
            if (bSamll)
            {
                Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(() => { return false; });
                Image myThumbnail = bitmap_clone.GetThumbnailImage(120, 120, myCallback, IntPtr.Zero);
                myThumbnail.Save(samllPath);
            }
            return true;
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 获取当前使用的摄像头支持的所有分辨率
        /// 请在摄像头预览完成后调用此方法，否则请调用TestManager的GetCameraSupportResolution的方法
        /// </summary>
        /// <returns></returns>
        public List<string> GetCameraSupportResolution()
        {
            if (_theCamera == null || _captureGraphBuilder == null)
                return null;

            List<string> AvailableResolutions = new List<string>();
            object streamConfig;

            // 获取配置接口
            int hr = _captureGraphBuilder.FindInterface(PinCategory.Capture,
                                             MediaType.Video,
                                             _theCamera,
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
        /// 打开摄像头配置页面
        /// 系统原生方式
        /// </summary>
        public void ChangeCameraSetting(IntPtr FrmHandle)
        {
            DsCAUUID cauuid = new DsCAUUID();
            try
            {
                specifyPropertyPages = _theCamera as ISpecifyPropertyPages;
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
                hr = OleCreatePropertyFrame(FrmHandle, 30, 30, null, 1,
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
        public bool SaveCameraConfigToSetting(string VideoSettingName, bool AsDefault)
        {
            IAMVideoProcAmp videoProcAmp = _theCamera as IAMVideoProcAmp;
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

            CameraParamPlanEntity setting = new CameraParamPlanEntity();
            setting.PrimaryKey = Guid.NewGuid().ToString();
            setting.CameraName = this._bindCameraName;
            setting.Brightness = LightValue;
            setting.ParamPlanName = VideoSettingName;
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
            setting.Save();

            return true;
        }

        /// <summary>
        /// 切换摄像头配置为指定配置方案
        /// 配置方案可以由GetAllCameraSettings进行传参
        /// </summary>
        /// <param name="setting">摄像头配置方案</param>
        /// <param name="asDefault">是否将此配置设为默认配置</param>
        /// <returns></returns>
        public int ChangeCameraConfigToSetting(CameraParamPlanEntity setting, bool asDefault = false)
        {
            if (asDefault)
            {
                List<CameraParamPlanEntity> list = XmlHelper.XmlHelper.FindAll<CameraParamPlanEntity>();
                list.ForEach(m => m.DefaultSetting = false);
                var oldPlan = list.Find(m => m.CameraName == this._bindCameraName && m.ParamPlanName == setting.ParamPlanName);
                oldPlan.DefaultSetting = true;
                XmlHelper.XmlHelper.SaveList<CameraParamPlanEntity>(list);
            }

            int iResult = 0;
            IAMVideoProcAmp videoProcAmp = _theCamera as IAMVideoProcAmp;
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


        /// <summary>
        /// 获取当前摄像头的所有配置方案
        /// </summary>
        /// <returns></returns>
        public List<CameraParamPlanEntity> GetCameraParamPlans()
        {
            List<CameraParamPlanEntity> plans = XmlHelper.XmlHelper.FindAll<CameraParamPlanEntity>();
            return plans.FindAll(m => m.CameraName == this._bindCameraName);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 停止播放
        /// </summary>
        private void StopRun()
        {
            if (_mediaControl != null)
            {
                //Stop the Graph
                _mediaControl.Stop();
            }
            if (_graphBuilder != null)
            {
                //Release COM objects
                Marshal.ReleaseComObject(_graphBuilder);
                _graphBuilder = null;
            }
            if (_captureGraphBuilder != null)
            {
                Marshal.ReleaseComObject(_captureGraphBuilder);
                _captureGraphBuilder = null;
            }
        }

        /// <summary>
        /// 开始运行
        /// </summary>
        private bool StartRun()
        {
            if (_mediaControl == null)
            {
                MessageBox.Show("播放对象为空！");
                return false;
            }

            int iRun = _mediaControl.Run();//运行图表

            if (iRun != 1)
            {
                //    DsError.ThrowExceptionForHR(iRun);
                //    MessageBox.Show("播放失败！");
                //    return false;
            }
            return true;
        }

        /// <summary>
        ///设置视频显示
        /// </summary>
        private void SetVideoShow(string Resolution, int Frames)
        {
            if (_theCamera == null)
            {
                return;
            }
            if (_graphBuilder != null)
            {
                _mediaControl.Stop();
            }
            CreateGraph(Resolution, Frames);

            //渲染设备的任何预览引脚 并且把sampleGrabber添加到预览
            int hr = _captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, _theCamera, (sampleGrabber as IBaseFilter), null);
            //DsError.ThrowExceptionForHR(hr);
            hr = _captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Audio, _theAudio, null, null);
            //DsError.ThrowExceptionForHR(hr);
            //获取视频头文件信息
            GetVideoHeaderInfo(sampleGrabber);

            //从图中获取视频窗口
            IVideoWindow videoWindow = null;
            videoWindow = (IVideoWindow)_graphBuilder;

            //将视频窗口的所有者设置为某种IntPtr（任何控件的句柄 - 可以是窗体/按钮等）
            hr = videoWindow.put_Owner(_priviewControlHandle);
            //DsError.ThrowExceptionForHR(hr);

            //设置视频窗口的样式
            hr = videoWindow.put_WindowStyle((int)WindowStyle.Child | (int)WindowStyle.ClipChildren);
            //DsError.ThrowExceptionForHR(hr);

            //在主应用程序窗口的客户端中定位视频窗口
            hr = videoWindow.SetWindowPosition(0, 0, _priviewControlWidth, _priviewControlHeigh);
            //DsError.ThrowExceptionForHR(hr);

            //使视频窗口可见
            hr = videoWindow.put_Visible((int)OABool.True);
            //DsError.ThrowExceptionForHR(hr);
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
        /// 构建捕获图
        /// </summary>
        private void CreateGraph(string Resolution, int Frames)
        {
            if (_graphBuilder != null)
            {
                return;
            }
            _graphBuilder = (IFilterGraph2)new FilterGraph();// 获取IFilterGraph2接口对象
            _captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();//获取ICaptureGraphBuilder2接口对象

            int hr = _captureGraphBuilder.SetFiltergraph(this._graphBuilder);//将过滤器图形附加到捕获图
            DsError.ThrowExceptionForHR(hr);

            //将视频输入设备添加到图形
            hr = _graphBuilder.AddFilter(_theCamera, "source filter");
            DsError.ThrowExceptionForHR(hr);

            //将视频压缩器过滤器添加到图形
            if (_theCameraCompressor != null)
            {
                hr = _graphBuilder.AddFilter(_theCameraCompressor, "devicecompressor filter");
                DsError.ThrowExceptionForHR(hr);
            }
            //将音频输入设备添加到图形
            if (_theAudio != null)
            {
                hr = _graphBuilder.AddFilter(_theAudio, "audio filter");
                DsError.ThrowExceptionForHR(hr);
            }
            //将音频压缩器过滤器添加到图形
            if (_theAudioCompressor != null)
            {
                hr = _graphBuilder.AddFilter(_theAudioCompressor, "audiocompressor filter");
                DsError.ThrowExceptionForHR(hr);
            }
            _mediaControl = (IMediaControl)this._graphBuilder;//获取IMediaControl接口对象

            m_PictureReady = new ManualResetEvent(false);

            //添加采样器接口.
            sampleGrabber = new SampleGrabber() as ISampleGrabber;

            // 配置SampleGrabber。添加预览回调
            ConfigureSampleGrabber(sampleGrabber);
            hr = _graphBuilder.AddFilter(sampleGrabber as IBaseFilter, "Frame Callback");// 将SampleGrabber添加到图形.
            DsError.ThrowExceptionForHR(hr);


            AMMediaType mediaType = new AMMediaType();
            IntPtr pmt = IntPtr.Zero;
            object oVideoStreamConfig;//视频流配置信息
            hr = _captureGraphBuilder.FindInterface(PinCategory.Capture, MediaType.Video, _theCamera, typeof(IAMStreamConfig).GUID, out oVideoStreamConfig);
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
        /// 初始化设备
        /// </summary>
        private bool InitDevice()
        {
            try
            {
                //检查DirectX版本
                if (!TestManager.IsCorrectDirectXVersion())
                {
                    throw new Exception("未安装DirectX 8.1！");
                }

                //如果摄像头不存在则抛出异常
                if (_bindCameraName == null || string.IsNullOrEmpty(_bindCameraName))
                {
                    throw new Exception("请指定要使用的摄像头！");
                }


                //如果指定的音频不存在则抛出异常
                if (_bindAudioName == null || string.IsNullOrEmpty(_bindAudioName))
                {
                    throw new Exception("请指定要使用的录音设备！");
                }

                var dsVideoDevice = TestManager.GetAllVideoDevice();

                _theCamera = TestManager.CreateFilter(dsVideoDevice, _bindCameraName);
                if (_theCamera == null)
                {
                    MessageBox.Show("输入摄像机名称不存在！");
                }


                var dsAudioDevice = TestManager.GetAllAudioDevice();
                _theAudio = TestManager.CreateFilter(dsAudioDevice, _bindAudioName);//获取音频设备IBaseFilter
                if (_theAudio == null)
                {
                    MessageBox.Show("输入音频设备不存在！");
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
        /// 验证是否开始预览
        /// </summary>
        /// <returns></returns>
        private bool VerStarPreview()
        {
            if (_theCamera == null || _theAudio == null)
            {
                return false;
            }
            return true;
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

        /// <summary>
        /// 初始化图表
        /// </summary>
        private bool InitGraph(string sViewPath)
        {
            if (!VerStarPreview())
            {
                MessageBox.Show("请先开始预览！");
                return false;
            }
            _mediaControl.Stop();
            CreateGraph(_Resolution, _Frames);
            int hr;
            if (sVideoType == "wmv")
            {
                hr = _captureGraphBuilder.SetOutputFileName(MediaSubType.Asf, sViewPath, out mux, out sink);
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
                hr = _captureGraphBuilder.SetOutputFileName(MediaSubType.Avi, sViewPath, out mux, out sink);
                DsError.ThrowExceptionForHR(hr);
            }
            //将设备和压缩器连接到mux，以呈现图形的捕获部分
            hr = _captureGraphBuilder.RenderStream(PinCategory.Capture, MediaType.Interleaved, _theCamera, _theCameraCompressor, mux);
            //DsError.ThrowExceptionForHR(hr);
            if (hr < 0)
            {
                hr = _captureGraphBuilder.RenderStream(PinCategory.Capture, MediaType.Video, _theCamera, _theCameraCompressor, mux);
                if (hr < 0) { DsError.ThrowExceptionForHR(hr); }
            }

            //将音频添加进去
            hr = _captureGraphBuilder.RenderStream(PinCategory.Capture, MediaType.Audio, _theAudio, _theAudioCompressor, mux);
            DsError.ThrowExceptionForHR(hr);

            Marshal.ReleaseComObject(mux);
            Marshal.ReleaseComObject(sink);
            return true;
        }
        #endregion

        #region 关闭视频设备
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_mediaControl != null)
            {
                _mediaControl.Stop();
                _mediaControl = null;
            }
            if (_graphBuilder != null)
            {
                Marshal.ReleaseComObject(_graphBuilder);
                _graphBuilder = null;
            }
            if (_captureGraphBuilder != null)
            {
                Marshal.ReleaseComObject(_captureGraphBuilder);
                _captureGraphBuilder = null;
            }
            if (sampleGrabber != null)
            {
                Marshal.ReleaseComObject(sampleGrabber);
                sampleGrabber = null;
            }
            if (_theCamera != null)
            {
                Marshal.ReleaseComObject(_theCamera);
                _theCamera = null;
            }
            if (_theCameraCompressor != null)
            {
                Marshal.ReleaseComObject(_theCameraCompressor);
                _theCameraCompressor = null;
            }
            if (_theAudio != null)
            {
                Marshal.ReleaseComObject(_theAudio);
                _theAudio = null;
            }
            if (_theAudioCompressor != null)
            {
                Marshal.ReleaseComObject(_theAudioCompressor);
                _theAudioCompressor = null;
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
            //DisposeDevice(dsVideoDevice);//释放视频设备
            //DisposeDevice(dsVideoCompressorCategory);//释放视频解码器设备
            //DisposeDevice(dsAudioInputDevice);//释放语音设备
            //DisposeDevice(dsAudioCompressorCategory);//释放语音解码器设备
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
    }
}
