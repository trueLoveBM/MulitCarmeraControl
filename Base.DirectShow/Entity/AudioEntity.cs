using Base.DirectShow.Utils;
using Base.DirectShow.XmlHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.DirectShow.Entity
{
    /// <summary>
    /// 音频的信息
    /// </summary>
    public class AudioEntity : XmlEntity
    {
        /// <summary>
        /// 摄像头名称
        /// </summary>
        [PrimaryKey]
        public string Name { get; set; }

        /// <summary>
        /// 摄像头状态
        /// 0:空闲
        /// 1:正在使用中
        /// 2:禁用
        /// 4:不可用
        /// </summary>
        public int Status { get; set; }
    }
}
