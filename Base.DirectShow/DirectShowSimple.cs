using Base.DirectShow.Device;
using Base.DirectShow.Entity;
using Base.DirectShow.SharePreferences;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Base.DirectShow
{
    public class DirectShowSimple
    {
        private static readonly object locker = new object();// 定义一个标识确保线程同步
        private static DirectShowSimple _instance;
        public static DirectShowSimple Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new DirectShowSimple();
                            _instance.SecDevice = DirectShow.Instance.SecDevice;
                        }
                    }
                }
                return _instance;
            }
        }

        #region 委托


        /// <summary>
        /// 选择摄像头,请在自己的窗体里赋值
        /// </summary>
        public Action<DeviceSecEntity> SecDevice { set { DirectShow.Instance.SecDevice = value; } get { return DirectShow.Instance.SecDevice; } }

        #endregion

        #region 公开属性

        /// <summary>
        /// 视频设备名称（为空系统自动加载）
        /// </summary>
        public string VideoDeviceName
        {
            get { return DirectShow.Instance.VideoDeviceName; }
            set { DirectShow.Instance.VideoDeviceName = value; }
        }

        /// <summary>
        /// 视频压缩器（为空系统自动加载）
        /// </summary>
        public string VideoCompressorCategory
        {
            get { return DirectShow.Instance.VideoCompressorCategory; }
            set { DirectShow.Instance.VideoCompressorCategory = value; }
        }

        /// <summary>
        /// 语音设备名称（为空系统自动加载）
        /// </summary>
        public string AudioInputDeviceName
        {
            get { return DirectShow.Instance.AudioInputDeviceName; }
            set { DirectShow.Instance.AudioInputDeviceName = value; }
        }

        /// <summary>
        /// 语音压缩器（为空系统自动加载）
        /// </summary>
        public string AudioCompressor
        {
            get { return DirectShow.Instance.AudioCompressor; }
            set { DirectShow.Instance.AudioCompressor = value; }
        }

        /// <summary>
        /// 帧数
        /// </summary>
        public int Frames
        {
            get { return DirectShow.Instance.Frames; }
            set { DirectShow.Instance.Frames = value; }
        }

        /// <summary>
        /// 分辨率
        /// </summary>
        public string Resolution
        {
            get { return DirectShow.Instance.Resolution; }
            set { DirectShow.Instance.Resolution = value; }
        }


        #endregion

        #region 公开方法

        /// <summary>
        /// 预览视频
        /// </summary>
        /// <param name="clVideo">视频显示的控件对象</param>
        public bool Preview(Control clVideo)
        {
            return DirectShow.Instance.Preview(clVideo);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            DirectShow.Instance.Dispose();
        }

        /// <summary>
        /// 开始录像
        /// </summary>
        /// <param name="sViewPath">录像文件保存路径</param>
        /// <returns></returns>
        public bool StartMonitorRecord(string sViewPath)
        {
            return DirectShow.Instance.StartMonitorRecord(sViewPath);
        }

        /// <summary>
        /// 停止录像
        /// </summary>
        public void StopMonitorRecord()
        {
            DirectShow.Instance.StopMonitorRecord();
        }

        /// <summary>
        /// 从摄像头抓拍照片
        /// </summary>
        /// <param name="path">抓拍照片完整路径，包含文件名</param>
        /// <param name="iImageWidth">抓怕图片宽度，默认获取视频信息宽度</param>
        /// <param name="iImageHeight">抓拍图片高度，默认获取视频信息高度</param>
        /// <param name="bSamll">是否抓小图，默认不抓拍</param>
        /// <param name="samllPath">抓拍小图路径，抓拍小图时则必须填写</param>
        public bool TakeAPicture(string path, int iImageWidth = 0, int iImageHeight = 0, bool bSamll = false, string samllPath = "")
        {
            return DirectShow.Instance.TakeAPicture(path, iImageWidth, iImageHeight, bSamll, samllPath);
        }

        /// <summary>
        /// 截图摄像头
        /// </summary>
        /// <param name="iImageWidth">抓拍图片宽度，默认获取视频信息宽度</param>
        /// <param name="iImageHeight">抓拍图片高度，默认获取视频信息宽度</param>
        /// <returns></returns>
        public Bitmap CaptureCamera(int iImageWidth = 0, int iImageHeight = 0)
        {
            return DirectShow.Instance.CaptureCamera(iImageWidth,iImageHeight);
        }

        /// <summary>
        /// 打开摄像头配置页面，系统原生方式
        /// </summary>
        public void ChangeCameraSetting(IntPtr Handle)
        {
            DirectShow.Instance.ChangeCameraSetting(Handle);
        }

        /// <summary>
        /// 获取摄像头所有配置方案
        /// 配合 ChangeCameraConfigToSetting  方法切换摄像头所有方案
        /// </summary>
        /// <returns></returns>
        public List<VideoSettingEntity> GetAllCameraSettings()
        {
            return DirectShow.Instance.GetAllCameraSettings();
        }

        /// <summary>
        /// 读取当前摄像头配置并保存到方案中
        /// </summary>
        /// <param name="VideoSettingName">当前配置的方案名</param>
        /// <param name="AsDefault">是否设为摄像头默认设置</param>
        public void SaveCameraConfigToSetting(string VideoSettingName, bool AsDefault)
        {
            DirectShow.Instance.SaveCameraConfigToSetting(VideoSettingName, AsDefault);
        }

        /// <summary>
        /// 切换摄像头配置为指定配置方案
        /// 配置方案可以由GetAllCameraSettings进行传参
        /// </summary>
        /// <param name="setting">摄像头配置方案</param>
        /// <param name="asDefault">是否将此配置设为默认配置</param>
        /// <returns></returns>
        public int ChangeCameraConfigToSetting(VideoSettingEntity setting, bool asDefault = false)
        {
            return DirectShow.Instance.ChangeCameraConfigToSetting(setting, asDefault);
        }

        /// <summary>
        /// 获取当前摄像头采用的默认设置
        /// </summary>
        /// <returns></returns>
        public VideoSettingEntity GetCurrentDefaultCameraSetting()
        {
            return DirectShow.Instance.GetCurrentDefaultCameraSetting();
        }

        /// <summary>
        /// 获取当前用户可用的所有音频设备
        /// </summary>
        /// <returns></returns>
        public DsDevice[] GetAllAudioDevice()
        {
            return DirectShow.Instance.GetAllAudioDevice();
        }

        /// <summary>
        /// 获取当前用户可用的所有摄像头数组
        /// </summary>
        /// <returns>返回当前用户可用的摄像头数组，若没有找到摄像头，则会抛出异常</returns>
        public DsDevice[] GetAllVideoDevice()
        {
            return DirectShow.Instance.GetAllVideoDevice();
        }

        /// <summary>
        /// 获取当前摄像头支持的清晰度
        /// 请保证当前摄像头在预览状态，否则无法获取到支持的分辨率
        /// </summary>
        /// <returns></returns>
        public List<string> GetCameraSupportResolution()
        {
            return DirectShow.Instance.GetCameraSupportResolution();
        }

        /// <summary>
        /// 获取指定摄像头所有支持的分辨率
        /// </summary>
        /// <returns></returns>
        public List<string> GetCameraSupportResolutionByCameraName(string CameraName)
        {
            return DirectShow.Instance.GetCameraSupportResolutionByCameraName(CameraName);
        }

        /// <summary>
        /// 获取摄像头上次使用的分辨率
        /// </summary>
        /// <returns></returns>
        public string GetLastCameraResolution()
        {
            return DirectShow.Instance.GetLastCameraResolution();
        }

        /// <summary>
        /// 设置默认使用摄像头
        /// </summary>
        /// <param name="CameraName"></param>
        /// <returns></returns>
        public void SetDefaultUseCamera(string CameraName)
        {
            DirectShow.Instance.SetDefaultUseCamera(CameraName);
        }
        #endregion

    }
}
