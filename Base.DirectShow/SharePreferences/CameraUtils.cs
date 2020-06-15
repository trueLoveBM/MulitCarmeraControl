using Base.DirectShow.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Base.DirectShow.SharePreferences
{
    public class CameraUtils
    {


        #region 私有成员变量

        /// <summary>
        /// 视频保存路径
        /// </summary>
        private string _CameraHistoryFilePath;

        /// <summary>
        /// 配置文件名称
        /// </summary>
        private string _CameraHistoryFileName;

        /// <summary>
        /// 文件的最终绝对路径
        /// </summary>
        private string _CameraHistoryRealPath;
        #endregion


        #region 单例
        private static Lazy<CameraUtils> lazy = new Lazy<CameraUtils>(() => { return new CameraUtils(); });

        public static CameraUtils Instance { get { return lazy.Value; } }

        private CameraUtils()
        {
            //尝试从config中读取视频配置文件存放路径，如果没有读取到，则默认存放在Config/VideoSetting路径下
            _CameraHistoryFilePath = ConfigurationManager.AppSettings["CameraHistoryFilePath"];
            if (string.IsNullOrEmpty(_CameraHistoryFilePath))
                //throw new Exception("未找到视频配置文件路径");
                _CameraHistoryFilePath = "Config/CameraHistoryFilePath";
            //尝试从config中读取视频配置文件名称，如果没有读取到，则默认文件名为VideoSetting.xml
            _CameraHistoryFileName = ConfigurationManager.AppSettings["CameraHistoryFileName"];
            if (string.IsNullOrEmpty(_CameraHistoryFileName))
                //    throw new Exception("未找到视频配置文件名称");
                _CameraHistoryFileName = "CameraHistoryFileName.xml";
            //判断配置文件的路径是否存在，不存在则创建相关的文件路径
            if (!Directory.Exists(System.Environment.CurrentDirectory + "/" + _CameraHistoryFilePath))
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "/" + _CameraHistoryFilePath);
            }

            //初始化文件存放的最终绝对路径
            _CameraHistoryRealPath = System.Environment.CurrentDirectory + "/" + _CameraHistoryFilePath + "/" + _CameraHistoryFileName;
        }
        #endregion


        /// <summary>
        /// 获取用户上次使用的分辨率
        /// </summary>
        /// <returns></returns>
        public string GetLastCamera()
        {
            string Resolution = null;
            if (!File.Exists(_CameraHistoryRealPath))
            {
                return null;
            }

            //先解密这个文件
            Base64Helper.Base64Decode4txtFile(_CameraHistoryRealPath);

            XmlTextReader reader = new XmlTextReader(_CameraHistoryRealPath);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "LastCamera")
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
            Base64Helper.Base64Encode4txtFile(_CameraHistoryRealPath);
            return Resolution;
        }


        /// <summary>
        /// 获取用户上次使用的分辨率
        /// </summary>
        /// <returns></returns>
        public void SetLastCamera(string CameraName)
        {
            XmlTextWriter myXmlTextWriter = new XmlTextWriter(_CameraHistoryRealPath, null);
            //使用 Formatting 属性指定希望将 XML 设定为何种格式。 这样，子元素就可以通过使用 Indentation 和 IndentChar 属性来缩进。
            myXmlTextWriter.Formatting = Formatting.Indented;
            myXmlTextWriter.WriteStartDocument(true);

            myXmlTextWriter.WriteStartElement("ResolutionCfg");
            myXmlTextWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            myXmlTextWriter.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

            myXmlTextWriter.WriteElementString("LastCamera", CameraName);
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
            myXmlTextWriter = null;
            GC.Collect();


            //TODO 加密这个文件
            Base64Helper.Base64Encode4txtFile(_CameraHistoryRealPath);

        }
    }
}
