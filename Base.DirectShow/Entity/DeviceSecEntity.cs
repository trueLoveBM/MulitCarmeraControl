using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.DirectShow.Entity
{
    public class DeviceSecEntity
    {
        /// <summary>
        /// 选择的设备名称
        /// </summary>
        public string VideoDeviceName { get; set; }

        /// <summary>
        /// 选择的音频设备名称
        /// </summary>
        public string AudioInputDeviceName { get; set; }

        /// <summary>
        /// 帧率
        /// </summary>
        public int Frames { get; set; }


        /// <summary>
        /// 分辨率，请保持 XX*XX的格式
        /// </summary>
        public string Resolution { get; set; }


        /// <summary>
        /// 是否设置完毕
        /// </summary>
        public bool SetFisished;
    }
}
