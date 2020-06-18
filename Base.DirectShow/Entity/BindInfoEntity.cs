using Base.DirectShow.Utils;
using Base.DirectShow.XmlHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.DirectShow.Entity
{
    /// <summary>
    /// 摄像头及音频使用记录数据
    /// </summary>
    public class BindInfoEntity : XmlEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// 摄像头名称
        /// </summary>
        public string CameraName { get; set; }

        /// <summary>
        /// 音频名称
        /// </summary>
        public string AudioName { get; set; }

        /// <summary>
        /// 使用的分辨率
        /// </summary>
        public string Resolution { get; set; }

        /// <summary>
        /// 使用的配置方案
        /// </summary>
        public string SettingName { get; set; }
    }
}
