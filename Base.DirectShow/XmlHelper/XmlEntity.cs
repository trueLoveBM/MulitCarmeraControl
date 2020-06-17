using Base.DirectShow.XmlHelper;
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
    /// <summary>
    /// XML存储的基类
    /// 在基类中实现了将实体按照XML方式进行存储，读取的方法
    /// </summary>
    public class XmlEntity
    {
        private XmlEntityInfo _EntityInfo;

        /// <summary>
        /// 构造函数
        /// </summary>
        public XmlEntity()
        {
            Type type = this.GetType();
            _EntityInfo = XmlHelper.GetXmlEntityInfo(type);
        }


        public void Save()
        {
            //首先读取当前数据
            List<XmlEntity> result = findAll();

            //判断当前数据中是否存在此数据，存在则更换此数据，不存在则追加至集合结尾
            object saveDataPrimaryKey = _EntityInfo.PrimayKey.GetValue(this, null);

            if (result != null && result.Count > 0)
            {
                int findIndex = -1;
                for (int i = 0; i < result.Count; i++)
                {
                    var item = result[i];
                    object permaryKeyValue = _EntityInfo.PrimayKey.GetValue(item, null);
                    if (saveDataPrimaryKey.ToString() == permaryKeyValue.ToString())
                    {
                        findIndex = i;
                        break;
                    }
                }
                if (findIndex != -1)
                {
                    result.RemoveAt(findIndex);
                    result.Insert(findIndex, this);
                }
                else
                    result.Add(this);
            }
            else
            {
                result = new List<XmlEntity>();
                result.Add(this);
            }



            #region  通过 XmlTextWriter将配置持久化到文件中
            XmlTextWriter myXmlTextWriter = new XmlTextWriter(_EntityInfo.XmlSavePath, null);

            //使用 Formatting 属性指定希望将 XML 设定为何种格式。 这样，子元素就可以通过使用 Indentation 和 IndentChar 属性来缩进。
            myXmlTextWriter.Formatting = Formatting.Indented;
            myXmlTextWriter.WriteStartDocument(true);

            myXmlTextWriter.WriteStartElement("XmlDatas");
            myXmlTextWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            myXmlTextWriter.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            myXmlTextWriter.WriteStartElement(_EntityInfo.XmlFileName + "s");


            //反射获取到当前类的所有属性
            Type type = this.GetType();

            result.ForEach(m =>
            {
                myXmlTextWriter.WriteStartElement(_EntityInfo.XmlFileName);
                //循环所有属性
                foreach (PropertyInfo item in _EntityInfo.Properties)
                {
                    if (item.Name == nameof(_EntityInfo))
                        continue;

                    object value = item.GetValue(m, null);
                    myXmlTextWriter.WriteElementString(item.Name, value?.ToString());
                }
                myXmlTextWriter.WriteEndElement();
            });


            myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
            myXmlTextWriter = null;
            GC.Collect();
            #endregion
            //重新加密这个文件
            //Base64Helper.Base64Encode4txtFile(XmlSavePath);

        }

        public static T FindByPrimaryKey<T>() where T : XmlEntity
        {
            return null;
        }

        public static List<T> FindAll<T>() where T : XmlEntity
        {
            Type type = typeof(T);
            //获取当前类的名称,用作持久化Xml文件的名称
            string FileName = type.Name;

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
            string SavePath = System.Environment.CurrentDirectory + "/" + XmlDb + "/" + FileName + ".xml";

            var Properties = type.GetProperties()?.ToList();


            List<T> result = new List<T>();


            if (!File.Exists(SavePath))
            {
                return null;
            }

            //先解密这个文件
            //Base64Helper.Base64Decode4txtFile(XmlSavePath);

            XmlTextReader reader = new XmlTextReader(SavePath);
            T obj = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == FileName)
                    {
                        obj = System.Activator.CreateInstance<T>();
                        result.Add(obj);
                    }
                    else
                    {
                        //读取到属性
                        string propertyName = reader.Name;
                        var ProertyInfo = Properties.Find(m => m.Name == propertyName);
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
        public static bool SaveList<T>() where T : XmlEntity
        {
            throw new NotImplementedException();
        }

        private List<XmlEntity> findAll()
        {
            List<XmlEntity> result = new List<XmlEntity>();

            #region 读取文件
            if (!File.Exists(_EntityInfo.XmlSavePath))
            {
                return null;
            }

            //先解密这个文件
            //Base64Helper.Base64Decode4txtFile(XmlSavePath);
            Type type = this.GetType();
            XmlTextReader reader = new XmlTextReader(_EntityInfo.XmlSavePath);
            XmlEntity obj = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == _EntityInfo.XmlFileName)
                    {
                        obj = System.Activator.CreateInstance(type) as XmlEntity;
                        result.Add(obj);
                    }
                    else
                    {
                        //读取到属性
                        string propertyName = reader.Name;
                        var ProertyInfo = _EntityInfo.Properties.Find(m => m.Name == propertyName);
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
            #endregion
        }

    }
}
