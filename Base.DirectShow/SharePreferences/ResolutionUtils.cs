using Base.DirectShow.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Base.DirectShow.SharePreferences
{
    /// <summary>
    /// 分辨率保存偏好的类
    /// 原本想要使用sqllite数据库方式进行保存，现由于需求，使用xml方式进行保存
    /// 原本使用的是  XmlTextReader/XmlTextWriter方式读写数据库，所以文件读写完成后会进行加密解密
    /// </summary>
    public class ResolutionUtils
    {
        #region 单例
        private static Lazy<ResolutionUtils> lazy = new Lazy<ResolutionUtils>(() => { return new ResolutionUtils(); });

        public static ResolutionUtils Instance { get { return lazy.Value; } }

        private ResolutionUtils()
        {
            //尝试从config中读取视频配置文件存放路径，如果没有读取到，则默认存放在Config/VideoSetting路径下
            _ResolutionSettingFilePath = ConfigurationManager.AppSettings["ResolutionFilepath"];
            if (string.IsNullOrEmpty(_ResolutionSettingFilePath))
                //throw new Exception("未找到视频配置文件路径");
                _ResolutionSettingFilePath = "Config/ResolutionFilepath";
            //尝试从config中读取视频配置文件名称，如果没有读取到，则默认文件名为VideoSetting.xml
            _ResolutionSettingFileName = ConfigurationManager.AppSettings["ResolutionFileName"];
            if (string.IsNullOrEmpty(_ResolutionSettingFileName))
                //    throw new Exception("未找到视频配置文件名称");
                _ResolutionSettingFileName = "Resolution.xml";
            //判断配置文件的路径是否存在，不存在则创建相关的文件路径
            if (!Directory.Exists(System.Environment.CurrentDirectory + "/" + _ResolutionSettingFilePath))
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "/" + _ResolutionSettingFilePath);
            }

            //初始化文件存放的最终绝对路径
            _VideoSettingRealPath = System.Environment.CurrentDirectory + "/" + _ResolutionSettingFilePath + "/" + _ResolutionSettingFileName;
        }
        #endregion


        #region 私有成员变量

        /// <summary>
        /// 视频保存路径
        /// </summary>
        private string _ResolutionSettingFilePath;

        /// <summary>
        /// 配置文件名称
        /// </summary>
        private string _ResolutionSettingFileName;

        /// <summary>
        /// 文件的最终绝对路径
        /// </summary>
        private string _VideoSettingRealPath;
        #endregion

        #region 公开方法

        /// <summary>
        /// 获取用户上次使用的分辨率
        /// </summary>
        /// <returns></returns>
        public string GetLastCameraResolution()
        {
            string Resolution = null;
            if (!File.Exists(_VideoSettingRealPath))
            {
                return null;
            }

            //先解密这个文件
            Base64Helper.Base64Decode4txtFile(_VideoSettingRealPath);

            XmlTextReader reader = new XmlTextReader(_VideoSettingRealPath);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "LastCameraResolution")
                    {
                        Resolution = reader.ReadElementContentAsString();
                    }
                }
            }
            //关闭流
            reader.Close();
            reader = null;
            GC.Collect();
            //重新加密这个文件
            Base64Helper.Base64Encode4txtFile(_VideoSettingRealPath);
            return Resolution;
        }


        /// <summary>
        /// 获取用户上次使用的分辨率
        /// </summary>
        /// <returns></returns>
        public void SetLastCameraResolution(string Resolution)
        {
            XmlTextWriter myXmlTextWriter = new XmlTextWriter(_VideoSettingRealPath, null);
            //使用 Formatting 属性指定希望将 XML 设定为何种格式。 这样，子元素就可以通过使用 Indentation 和 IndentChar 属性来缩进。
            myXmlTextWriter.Formatting = Formatting.Indented;
            myXmlTextWriter.WriteStartDocument(true);

            myXmlTextWriter.WriteStartElement("ResolutionCfg");
            myXmlTextWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            myXmlTextWriter.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

            myXmlTextWriter.WriteElementString("LastCameraResolution", Resolution);
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
            myXmlTextWriter = null;
            GC.Collect();


            //TODO 加密这个文件
            Base64Helper.Base64Encode4txtFile(_VideoSettingRealPath);

        }
        #endregion
    }
}