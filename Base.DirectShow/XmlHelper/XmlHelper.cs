using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Base.DirectShow.XmlHelper
{
    public class XmlHelper
    {
        private static Dictionary<string, XmlEntityInfo> XmlEntityInfoCache = new Dictionary<string, XmlEntityInfo>();

        internal static XmlEntityInfo GetXmlEntityInfo<T>() where T : XmlEntity
        {
            Type type = typeof(T);
            if (XmlEntityInfoCache.ContainsKey(type.FullName))
            {
                return XmlEntityInfoCache[type.FullName];
            }

            XmlEntityInfo xmlEntityInfo = new XmlEntityInfo();
            //获取当前类的名称,用作持久化Xml文件的名称
            xmlEntityInfo.XmlFileName = type.Name;

            //尝试从config中读取XML文件存放路径，如果没有读取到，则默认存放在Config/{XmlFileName}路径下
            string XmlDb = ConfigurationManager.AppSettings["XmlDb"];
            if (string.IsNullOrEmpty(XmlDb))
                XmlDb = "XmlDb";

            //判断配置文件的路径是否存在，不存在则创建相关的文件路径
            if (!Directory.Exists(System.Environment.CurrentDirectory + "/" + XmlDb))
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "/" + XmlDb);
            }

            //初始化文件存放的最终绝对路径
            xmlEntityInfo.XmlSavePath = System.Environment.CurrentDirectory + "/" + XmlDb + "/" + xmlEntityInfo.XmlFileName + ".xml";

            xmlEntityInfo.Properties = type.GetProperties()?.ToList();

            foreach (PropertyInfo item in xmlEntityInfo.Properties)
            {
                object[] attributes = item.GetCustomAttributes(false);
                if (attributes?.Length > 0)
                    foreach (var attr in attributes)
                    {
                        if (attr is PrimaryKeyAttribute)
                        {
                            xmlEntityInfo.PrimayKey = item;
                            break;
                        }
                    }
                if (xmlEntityInfo.PrimayKey != null)
                    break;
            }

            XmlEntityInfoCache[type.FullName] = xmlEntityInfo;

            return xmlEntityInfo;
        }

        internal static XmlEntityInfo GetXmlEntityInfo(Type type) 
        {
            if (XmlEntityInfoCache.ContainsKey(type.FullName))
            {
                return XmlEntityInfoCache[type.FullName];
            }

            XmlEntityInfo xmlEntityInfo = new XmlEntityInfo();
            //获取当前类的名称,用作持久化Xml文件的名称
            xmlEntityInfo.XmlFileName = type.Name;

            //尝试从config中读取XML文件存放路径，如果没有读取到，则默认存放在Config/{XmlFileName}路径下
            string XmlDb = ConfigurationManager.AppSettings["XmlDb"];
            if (string.IsNullOrEmpty(XmlDb))
                XmlDb = "XmlDb";

            //判断配置文件的路径是否存在，不存在则创建相关的文件路径
            if (!Directory.Exists(System.Environment.CurrentDirectory + "/" + XmlDb))
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "/" + XmlDb);
            }

            //初始化文件存放的最终绝对路径
            xmlEntityInfo.XmlSavePath = System.Environment.CurrentDirectory + "/" + XmlDb + "/" + xmlEntityInfo.XmlFileName + ".xml";

            xmlEntityInfo.Properties = type.GetProperties()?.ToList();

            foreach (PropertyInfo item in xmlEntityInfo.Properties)
            {
                object[] attributes = item.GetCustomAttributes(false);
                if (attributes?.Length > 0)
                    foreach (var attr in attributes)
                    {
                        if (attr is PrimaryKeyAttribute)
                        {
                            xmlEntityInfo.PrimayKey = item;
                            break;
                        }
                    }
                if (xmlEntityInfo.PrimayKey != null)
                    break;
            }

            XmlEntityInfoCache[type.FullName] = xmlEntityInfo;

            return xmlEntityInfo;
        }

        public static T FindByPrimaryKey<T>(string PrimaryKey) where T : XmlEntity
        {
            if (string.IsNullOrEmpty(PrimaryKey))
                return null;

            List<T> tList = FindAll<T>();
            if (tList == null || tList.Count == 0)
                return null;

            XmlEntityInfo xmlEntityInfo = GetXmlEntityInfo<T>();

            foreach (var item in tList)
            {
                object ItemPrimaykey = xmlEntityInfo.PrimayKey.GetValue(item, null);
                if(ItemPrimaykey.ToString()== PrimaryKey)
                {
                    return item;
                }
            }

            return null;
        }

        public static List<T> FindAll<T>() where T : XmlEntity
        {
            XmlEntityInfo xmlEntityInfo = GetXmlEntityInfo<T>();

            List<T> result = new List<T>();

            if (!File.Exists(xmlEntityInfo.XmlSavePath))
            {
                return null;
            }

            //先解密这个文件
            //Base64Helper.Base64Decode4txtFile(XmlSavePath);

            XmlTextReader reader = new XmlTextReader(xmlEntityInfo.XmlSavePath);
            T obj = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == xmlEntityInfo.XmlFileName)
                    {
                        obj = System.Activator.CreateInstance<T>();
                        result.Add(obj);
                    }
                    else
                    {
                        //读取到属性
                        string propertyName = reader.Name;
                        var ProertyInfo = xmlEntityInfo.Properties.Find(m => m.Name == propertyName);
                        if (obj != null && ProertyInfo != null)
                        {
                            string propertyValue = reader.ReadElementContentAsString();
                            if (ProertyInfo.PropertyType == typeof(Int32))
                            {
                                if (Int32.TryParse(propertyValue, out int intValue))
                                    ProertyInfo.SetValue(obj, intValue, null);
                            }
                            else
                                ProertyInfo.SetValue(obj, propertyValue, null);
                        }
                    }
                }
            }

            //关闭流
            reader.Close();
            reader = null;
            GC.Collect();

            return result;
        }

        public static List<T> Find<T>(string PropertyName) where T : XmlEntity
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool SaveList<T>(List<T> tList) where T : XmlEntity
        {
            XmlEntityInfo xmlEntityInfo = GetXmlEntityInfo<T>();

            #region  通过 XmlTextWriter将配置持久化到文件中
            XmlTextWriter myXmlTextWriter = new XmlTextWriter(xmlEntityInfo.XmlSavePath, null);

            //使用 Formatting 属性指定希望将 XML 设定为何种格式。 这样，子元素就可以通过使用 Indentation 和 IndentChar 属性来缩进。
            myXmlTextWriter.Formatting = Formatting.Indented;
            myXmlTextWriter.WriteStartDocument(true);

            myXmlTextWriter.WriteStartElement("XmlDatas");
            myXmlTextWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            myXmlTextWriter.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            myXmlTextWriter.WriteStartElement(xmlEntityInfo.XmlFileName + "s");

            tList.ForEach(m =>
            {
                myXmlTextWriter.WriteStartElement(xmlEntityInfo.XmlFileName);
                //循环所有属性
                foreach (PropertyInfo item in xmlEntityInfo.Properties)
                {
                    object value = item.GetValue(m, null);
                    myXmlTextWriter.WriteElementString(item.Name, value?.ToString());
                }
                myXmlTextWriter.WriteEndElement();
            });


            myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
            myXmlTextWriter = null;
            GC.Collect();
            return true;
            #endregion
        }
    }


    internal class XmlEntityInfo
    {
        /// <summary>
        /// Xml文件的存放路径
        /// </summary>
        public string XmlSavePath;

        /// <summary>
        /// 相关存储xml的文件名称
        /// </summary>
        public string XmlFileName;

        /// <summary>
        /// 相关的属性
        /// </summary>
        public List<PropertyInfo> Properties;

        /// <summary>
        /// 主键属性
        /// </summary>
        public PropertyInfo PrimayKey;
    }
}
