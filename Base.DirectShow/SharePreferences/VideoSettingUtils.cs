using Base.DirectShow.Entity;
using Base.DirectShow.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BaseDirectShow.SharePreferences
{
    /// <summary>
    /// 视频参数设置
    /// 
    /// author:HuangFan
    /// Date: 2019年11月5日09:26:56
    /// 备注:参考当前的情况下，使用读写xml作为配置信息的持久化方式，
    ///      但最优的方式，为使用SqlLite数据库进行保存，优点有以下两点：
    ///      1.数据操作方便
    ///      2.数据保密程度高
    ///      所以未来将相关方法定义到接口中，而实现类定义为xml实现或者sqlLite实现，以求得配置功能
    /// </summary>
    internal sealed class VideoSettingUtils
    {

        #region 单例实现 使用.NET 4(或更高)的 Lazy 类型来实现单例

        /// <summary>
        /// 使用.NET 4(或更高)的 Lazy 类型
        /// </summary>
        private static readonly Lazy<VideoSettingUtils> lazy = new Lazy<VideoSettingUtils>(() => new VideoSettingUtils());

        /// <summary>
        /// 单例
        /// </summary>
        public static VideoSettingUtils Instance
        {
            get
            {
                //lazy.IsValueCreated 可以判断单例是否被创建
                return lazy.Value;
            }
        }

        /// <summary>
        /// 私有的构造函数
        /// </summary>
        private VideoSettingUtils()
        {
            //尝试从config中读取视频配置文件存放路径，如果没有读取到，则默认存放在Config/VideoSetting路径下
            _VideoSettingFilePath = ConfigurationManager.AppSettings["VideoSettingFilePath"];
            if (string.IsNullOrEmpty(_VideoSettingFilePath))
                //throw new Exception("未找到视频配置文件路径");
                _VideoSettingFilePath = "Config/VideoSetting";
            //尝试从config中读取视频配置文件名称，如果没有读取到，则默认文件名为VideoSetting.xml
            _VideoSettingFileName = ConfigurationManager.AppSettings["VideoSettingFileName"];
            if (string.IsNullOrEmpty(_VideoSettingFileName))
                //    throw new Exception("未找到视频配置文件名称");
                _VideoSettingFileName = "VideoSetting.xml";

            //判断配置文件的路径是否存在，不存在则创建相关的文件路径
            if (!Directory.Exists(System.Environment.CurrentDirectory + "/" + _VideoSettingFilePath))
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "/" + _VideoSettingFilePath);
            }

            //初始化文件存放的最终绝对路径
            _VideoSettingRealPath = System.Environment.CurrentDirectory + "/" + _VideoSettingFilePath + "/" + _VideoSettingFileName;
        }

        #endregion

        #region 私有成员变量及方法

        /// <summary>
        /// 视频保存路径
        /// </summary>
        private string _VideoSettingFilePath;

        /// <summary>
        /// 配置文件名称
        /// </summary>
        private string _VideoSettingFileName;

        /// <summary>
        /// 文件的最终绝对路径
        /// </summary>
        private string _VideoSettingRealPath;

        /// <summary>
        /// 将配置集合持久化到配置文件中
        /// </summary>
        /// <param name="settings">要保存的配置文件集合</param>
        private void FlushDataToXmlFile(List<VideoSettingEntity> settings)
        {
            #region  通过 XmlTextWriter将配置持久化到文件中
            XmlTextWriter myXmlTextWriter = new XmlTextWriter(_VideoSettingRealPath, null);

            //使用 Formatting 属性指定希望将 XML 设定为何种格式。 这样，子元素就可以通过使用 Indentation 和 IndentChar 属性来缩进。
            myXmlTextWriter.Formatting = Formatting.Indented;
            myXmlTextWriter.WriteStartDocument(true);

            myXmlTextWriter.WriteStartElement("LiveChartsMap");
            myXmlTextWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            myXmlTextWriter.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

            myXmlTextWriter.WriteStartElement("VideoSettings");

            settings.ForEach(m =>
            {
                myXmlTextWriter.WriteStartElement(nameof(VideoSettingEntity));

                myXmlTextWriter.WriteElementString(nameof(m.VideoSettingName), m.VideoSettingName);
                myXmlTextWriter.WriteElementString(nameof(m.Brightness), m.Brightness.ToString());
                myXmlTextWriter.WriteElementString(nameof(m.AutoBrightness), Convert.ToInt32(m.AutoBrightness).ToString());
                myXmlTextWriter.WriteElementString(nameof(m.ContrastRatio), m.ContrastRatio.ToString());
                myXmlTextWriter.WriteElementString(nameof(m.AutoContrastRatio), Convert.ToInt32(m.AutoContrastRatio).ToString());
                myXmlTextWriter.WriteElementString(nameof(m.Saturation), m.Saturation.ToString());
                myXmlTextWriter.WriteElementString(nameof(m.AutoSaturation), Convert.ToInt32(m.AutoSaturation).ToString());

                myXmlTextWriter.WriteElementString(nameof(m.Hue), m.Hue.ToString());
                myXmlTextWriter.WriteElementString(nameof(m.AutoHue), Convert.ToInt32(m.AutoHue).ToString());
                myXmlTextWriter.WriteElementString(nameof(m.Sharpness), m.Sharpness.ToString());
                myXmlTextWriter.WriteElementString(nameof(m.AutoSharpness), Convert.ToInt32(m.AutoSharpness).ToString());
                myXmlTextWriter.WriteElementString(nameof(m.Gamma), m.Gamma.ToString());
                myXmlTextWriter.WriteElementString(nameof(m.AutoGamma), Convert.ToInt32(m.AutoGamma).ToString());
                myXmlTextWriter.WriteElementString(nameof(m.ColorEnable), Convert.ToInt32(m.ColorEnable).ToString());
                myXmlTextWriter.WriteElementString(nameof(m.WhiteBalance), m.WhiteBalance.ToString());
                myXmlTextWriter.WriteElementString(nameof(m.AutoWhiteBalance), Convert.ToInt32(m.AutoWhiteBalance).ToString());
                myXmlTextWriter.WriteElementString(nameof(m.BacklightCompensation), m.BacklightCompensation.ToString());
                myXmlTextWriter.WriteElementString(nameof(m.AutoBacklightCompensation), Convert.ToInt32(m.AutoBacklightCompensation).ToString());
                myXmlTextWriter.WriteElementString(nameof(m.Gain), m.Gain.ToString());
                myXmlTextWriter.WriteElementString(nameof(m.AutoGain), Convert.ToInt32(m.AutoGain).ToString());

                myXmlTextWriter.WriteElementString(nameof(m.DefaultSetting), Convert.ToInt32(m.DefaultSetting).ToString());
                myXmlTextWriter.WriteEndElement();
            });
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
            myXmlTextWriter = null;
            GC.Collect();
            #endregion
            //重新加密这个文件
            Base64Helper.Base64Encode4txtFile(_VideoSettingRealPath);
        }
        #endregion

        #region 共有成员变量及方法

        /// <summary>
        /// 获取当前用户保存的所有摄像头设置
        /// </summary>
        /// <returns>当前用户保存的所有摄像头设置</returns>
        public List<VideoSettingEntity> GetAllVideoSettings()
        {
            List<VideoSettingEntity> result = new List<VideoSettingEntity>();
            #region 读取文件
            if (!File.Exists(_VideoSettingRealPath))
            {
                return null;
            }

            //先解密这个文件
            Base64Helper.Base64Decode4txtFile(_VideoSettingRealPath);

            XmlTextReader reader = new XmlTextReader(_VideoSettingRealPath);
            VideoSettingEntity setting = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == nameof(VideoSettingEntity))
                    {
                        setting = new VideoSettingEntity();
                        result.Add(setting);
                    }
                    if (reader.Name == nameof(setting.VideoSettingName))
                    {
                        setting.VideoSettingName = reader.ReadElementContentAsString();

                    }
                    else if (reader.Name == nameof(setting.Brightness))
                    {
                        if (setting != null)
                            setting.Brightness = reader.ReadElementContentAsInt();
                    }
                    else if (reader.Name == nameof(setting.AutoBrightness))
                    {
                        if (setting != null)
                            setting.AutoBrightness = reader.ReadElementContentAsBoolean();
                    }
                    else if (reader.Name == nameof(setting.ContrastRatio))
                    {
                        if (setting != null)
                            setting.ContrastRatio = reader.ReadElementContentAsInt();
                    }
                    else if (reader.Name == nameof(setting.AutoContrastRatio))
                    {
                        if (setting != null)
                            setting.AutoContrastRatio = reader.ReadElementContentAsBoolean();
                    }
                    else if (reader.Name == nameof(setting.Saturation))
                    {
                        if (setting != null)
                            setting.Saturation = reader.ReadElementContentAsInt();
                    }
                    else if (reader.Name == nameof(setting.AutoSaturation))
                    {
                        if (setting != null)
                            setting.AutoSaturation = reader.ReadElementContentAsBoolean();
                    }
                    else if (reader.Name == nameof(setting.DefaultSetting))
                    {
                        if (setting != null)
                            setting.DefaultSetting = reader.ReadElementContentAsBoolean();
                    }
                    else if (reader.Name == nameof(setting.Hue))
                    {
                        if (setting != null)
                            setting.Hue = reader.ReadElementContentAsInt();
                    }
                    else if (reader.Name == nameof(setting.AutoHue))
                    {
                        if (setting != null)
                            setting.AutoHue = reader.ReadElementContentAsBoolean();
                    }
                    else if (reader.Name == nameof(setting.Sharpness))
                    {
                        if (setting != null)
                            setting.Sharpness = reader.ReadElementContentAsInt();
                    }
                    else if (reader.Name == nameof(setting.AutoSharpness))
                    {
                        if (setting != null)
                            setting.AutoSharpness = reader.ReadElementContentAsBoolean();
                    }
                    else if (reader.Name == nameof(setting.Gamma))
                    {
                        if (setting != null)
                            setting.Gamma = reader.ReadElementContentAsInt();
                    }
                    else if (reader.Name == nameof(setting.AutoGamma))
                    {
                        if (setting != null)
                            setting.AutoGamma = reader.ReadElementContentAsBoolean();
                    }
                    else if (reader.Name == nameof(setting.ColorEnable))
                    {
                        if (setting != null)
                            setting.ColorEnable = reader.ReadElementContentAsBoolean();
                    }
                    else if (reader.Name == nameof(setting.WhiteBalance))
                    {
                        if (setting != null)
                            setting.WhiteBalance = reader.ReadElementContentAsInt();
                    }
                    else if (reader.Name == nameof(setting.AutoWhiteBalance))
                    {
                        if (setting != null)
                            setting.AutoWhiteBalance = reader.ReadElementContentAsBoolean();
                    }
                    else if (reader.Name == nameof(setting.BacklightCompensation))
                    {
                        if (setting != null)
                            setting.BacklightCompensation = reader.ReadElementContentAsInt();
                    }
                    else if (reader.Name == nameof(setting.AutoBacklightCompensation))
                    {
                        if (setting != null)
                            setting.AutoBacklightCompensation = reader.ReadElementContentAsBoolean();
                    }
                    else if (reader.Name == nameof(setting.Gain))
                    {
                        if (setting != null)
                            setting.Gain = reader.ReadElementContentAsInt();
                    }
                    else if (reader.Name == nameof(setting.AutoGain))
                    {
                        if (setting != null)
                            setting.AutoGain = reader.ReadElementContentAsBoolean();
                    }
                }
            }
            //关闭流
            reader.Close();
            reader = null;
            GC.Collect();
            #endregion
            //重新加密这个文件
            Base64Helper.Base64Encode4txtFile(_VideoSettingRealPath);
            return result;
        }

        /// <summary>
        /// 保存摄像头参数配置方案
        /// 如果存在什么错误，会通过异常抛出
        /// </summary>
        /// <param name="setting">要保存的摄像头参数配置</param>
        /// <param name="AsDefault">是否将此摄像头参数配置设为默认配置</param>
        public void SaveVideoSetting(VideoSettingEntity setting, bool AsDefault = false)
        {
            List<VideoSettingEntity> settings = GetAllVideoSettings();
            if (settings == null)
                settings = new List<VideoSettingEntity>();

            //判断配置名称字段是否赋值
            if (string.IsNullOrEmpty(setting.VideoSettingName))
            {
                throw new Exception("必须赋值配置名称字段(VideoSettingName)");
            }

            //判断是否已经存在同名的配置方案
            if (settings.Find(m => m.VideoSettingName == setting.VideoSettingName) != null)
            {
                throw new Exception("已经存在同名的配置了");
            }

            //如果要将保存的视频信息设为默认
            if (AsDefault)
            {
                settings.ForEach(m => m.DefaultSetting = false);
                setting.DefaultSetting = true;
            }
            //将摄像头参数配置保存到文件中
            if (setting != null)
                settings.Add(setting);
            FlushDataToXmlFile(settings);
        }

        /// <summary>
        /// 编辑摄像头参数配置方案，并保存
        /// 注意：如果想要将该摄像头参数配置设置为默认，
        /// 在实体中赋值DefaultSetting是没有用的，
        /// 请通过赋值方法第二个参数值AsDefault来进行操作
        /// 如果存在什么错误，会通过异常抛出
        /// </summary>
        /// <param name="settingEntity">要修改的配置</param>
        /// <param name="AsDefault">是否设置为默认的摄像头配置</param>
        public void EditVideoSetting(VideoSettingEntity settingEntity, bool AsDefault = false)
        {
            List<VideoSettingEntity> settings = GetAllVideoSettings();
            if (settings == null)
                settings = new List<VideoSettingEntity>();

            //找到旧的配置方案
            VideoSettingEntity oldSettingEntity = settings.Find(m => m.VideoSettingName == settingEntity.VideoSettingName);

            //判断是否找到旧的配置方案
            if (oldSettingEntity == null)
            {
                throw new Exception("未找到该配置方案的旧方案，无法修改");
            }

            //移除旧的配置方案
            settings.Remove(oldSettingEntity);

            //赋值新的配置方案的是否为默认配置
            if (AsDefault)
            {
                settings.ForEach(m => m.DefaultSetting = false);
            }
            settingEntity.DefaultSetting = AsDefault;

            //将修改后的配置方案保存到方案集合中
            settings.Add(settingEntity);

            //持久化到文件中
            FlushDataToXmlFile(settings);
        }

        /// <summary>
        /// 删除一个摄像头配置
        /// 如果存在什么错误，会通过异常抛出
        /// </summary>
        /// <param name="settingEntity"></param>
        public void DeleteVideoSetting(VideoSettingEntity settingEntity)
        {
            List<VideoSettingEntity> settings = GetAllVideoSettings();
            if (settings == null)
                settings = new List<VideoSettingEntity>();

            //找到这个要删除的摄像头配置
            VideoSettingEntity deleteSettingEntity = settings.Find(m => m.VideoSettingName == settingEntity.VideoSettingName);

            //判断是否找到要删除的配置方案
            if (deleteSettingEntity == null)
            {
                throw new Exception("未找到该配置方案的旧方案，无法修改");
            }

            //删除这个配置方案
            settings.Remove(deleteSettingEntity);

            //持久化到文件中
            FlushDataToXmlFile(settings);
        }

        /// <summary>
        /// 将一个配置方案设置为默认配置，
        /// 该配置方案必须是已经存在的配置
        /// </summary>
        /// <param name="setting">要设为默认配置的摄像头配置</param>
        /// <returns>成功则返回true，失败则返回false</returns>
        public bool SetDefaultSettings(VideoSettingEntity setting)
        {
            List<VideoSettingEntity> settings = GetAllVideoSettings();
            if (settings == null)
                settings = new List<VideoSettingEntity>();
            //将所有的配置设置为非默认
            settings.ForEach(m => m.DefaultSetting = false);
            //找到要设置为默认的配置
            var defaultSetting = settings.Find(m => m.VideoSettingName == setting.VideoSettingName);
            defaultSetting.DefaultSetting = true;
            //持久化到文件中
            FlushDataToXmlFile(settings);
            return true;
        }
        #endregion
    }
}
