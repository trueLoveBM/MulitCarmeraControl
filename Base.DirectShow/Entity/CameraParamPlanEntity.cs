using Base.DirectShow.XmlHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.DirectShow.Entity
{

    /// <summary>
    /// 摄像头参数配置方案数据
    /// 主键唯一 基本无用
    /// 摄像头名称+ 摄像头配置名称唯一 
    /// </summary>
    public class CameraParamPlanEntity : XmlEntity
    {

        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey]
        public string PrimaryKey { get; set; }


        /// <summary>
        /// 摄像头名称
        /// </summary>
        public string CameraName { get; set; }


        /// <summary>
        /// 摄像头配置名称 同一摄像头内名称唯一
        /// </summary>
        public string ParamPlanName { get; set; }


        /// <summary>
        /// 亮度 取值0~255之间
        /// </summary>
        public int Brightness { get; set; }


        /// <summary>
        /// 是否自动调节亮度
        /// </summary>
        public bool AutoBrightness { get; set; }

        /// <summary>
        /// 对比度，取值0~255之间
        /// </summary>
        public int ContrastRatio { get; set; }

        /// <summary>
        /// 是否自动对比度
        /// </summary>
        public bool AutoContrastRatio { get; set; }

        /// <summary>
        /// 饱和度
        /// </summary>
        public int Saturation { get; set; }

        /// <summary>
        /// 是否自动饱和度
        /// </summary>
        public bool AutoSaturation { get; set; }

        /// <summary>
        /// 色调
        /// </summary>
        public int Hue { get; set; }

        /// <summary>
        /// 是否自动色调
        /// </summary>
        public bool AutoHue { get; set; }

        /// <summary>
        /// 清晰度
        /// </summary>
        public int Sharpness { get; set; }

        /// <summary>
        /// 是否自动清晰度
        /// </summary>
        public bool AutoSharpness { get; set; }

        /// <summary>
        /// 伽玛
        /// </summary>
        public int Gamma { get; set; }

        /// <summary>
        /// 自动伽玛
        /// </summary>
        public bool AutoGamma { get; set; }

        /// <summary>
        /// 启用颜色
        /// </summary>
        public bool ColorEnable { get; set; }

        /// <summary>
        /// 白平衡
        /// </summary>
        public int WhiteBalance { get; set; }


        /// <summary>
        /// 自动白平衡
        /// </summary>
        public bool AutoWhiteBalance { get; set; }

        /// <summary>
        /// 背光补偿
        /// </summary>
        public int BacklightCompensation { get; set; }

        /// <summary>
        /// 自动背光补偿
        /// </summary>
        public bool AutoBacklightCompensation { get; set; }

        /// <summary>
        /// 增益
        /// </summary>
        public int Gain { get; set; }

        /// <summary>
        /// 自动增益
        /// </summary>
        public bool AutoGain { get; set; }

        /// <summary>
        /// 默认设置
        /// </summary>
        public bool DefaultSetting { get; set; }


        protected override bool ValidForAdd(object CurrentDatas)
        {
            List<CameraParamPlanEntity> datas = CurrentDatas as List<CameraParamPlanEntity>;
            if (datas == null)
                return true;

            CameraParamPlanEntity oldPlan = datas.Find(m => m.CameraName == this.CameraName && m.ParamPlanName == this.ParamPlanName);

            if (oldPlan != null)
                return false;

            return true;
        }

    }
}
