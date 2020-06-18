using Base.DirectShow.Utils;
using Base.DirectShow.XmlHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.DirectShow.Entity
{
    /// <summary>
    /// 摄像头的相关信息
    /// </summary>
    public class CameraEntity : XmlEntity
    {
        /// <summary>
        /// 摄像头名称
        /// </summary>
        [PrimaryKey]
        public string Name { get; set; }

        /// <summary>
        /// 该摄像头支持的分辨率
        /// </summary>
        public string Resolution { get; set; }

        /// <summary>
        /// 摄像头状态
        /// 0:空闲
        /// 1:已被占用
        /// 2:禁用
        /// 3:不可用
        /// </summary>
        public int Status { get; set; }


        public override string ToString()
        {
            string stuff = "";
            switch (Status)
            {
                case 0: stuff = "(空闲)"; break;
                case 1: stuff = "(已被占用)"; break;
                case 2: stuff = "(禁用)"; break;
                case 3: stuff = "(不可用)"; break;
            }
            return Name + stuff;
        }
    }
}
