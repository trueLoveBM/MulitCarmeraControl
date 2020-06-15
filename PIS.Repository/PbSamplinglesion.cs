﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace PIS.Repository
{
    /// <summary>PbSamplinglesion</summary>
    /// <remarks></remarks>
    [Serializable]
    [DataObject]
    [Description("")]
    [BindIndex("indexFlagValid", false, "Flag_Valid")]
    [BindIndex("indexPBId", false, "PBId")]
    [BindIndex("indexSamplingStatus", false, "SamplingStatus")]
    [BindIndex("PK_PB_SamplingLesion", true, "SLId")]
    [BindTable("PB_SamplingLesion", Description = "", ConnName = "cell", DbType = DatabaseType.SqlServer)]
    public partial class PbSamplinglesion : IPbSamplinglesion
    {
        #region 属性
        private String _SLId;
        /// <summary></summary>
        [DisplayName("SLId")]
        [Description("")]
        [DataObjectField(true, false, false, 64)]
        [BindColumn("SLId", "", "varchar(64)")]
        public virtual String SLId
        {
            get { return _SLId; }
            set { if (OnPropertyChanging(__.SLId, value)) { _SLId = value; OnPropertyChanged(__.SLId); } }
        }

        private String _PBId;
        /// <summary></summary>
        [DisplayName("PBId")]
        [Description("")]
        [DataObjectField(false, false, false, 64)]
        [BindColumn("PBId", "", "varchar(64)")]
        public virtual String PBId
        {
            get { return _PBId; }
            set { if (OnPropertyChanging(__.PBId, value)) { _PBId = value; OnPropertyChanged(__.PBId); } }
        }

        private String _SamplingType;
        /// <summary></summary>
        [DisplayName("SamplingType")]
        [Description("")]
        [DataObjectField(false, false, true, 20)]
        [BindColumn("SamplingType", "", "varchar(20)")]
        public virtual String SamplingType
        {
            get { return _SamplingType; }
            set { if (OnPropertyChanging(__.SamplingType, value)) { _SamplingType = value; OnPropertyChanged(__.SamplingType); } }
        }

        private String _SamplingFindings;
        /// <summary></summary>
        [DisplayName("SamplingFindings")]
        [Description("")]
        [DataObjectField(false, false, true, 1024)]
        [BindColumn("SamplingFindings", "", "varchar(1024)")]
        public virtual String SamplingFindings
        {
            get { return _SamplingFindings; }
            set { if (OnPropertyChanging(__.SamplingFindings, value)) { _SamplingFindings = value; OnPropertyChanged(__.SamplingFindings); } }
        }

        private String _FlagCapture;
        /// <summary></summary>
        [DisplayName("FlagCapture")]
        [Description("")]
        [DataObjectField(false, false, true, 1)]
        [BindColumn("Flag_Capture", "", "char(1)")]
        public virtual String FlagCapture
        {
            get { return _FlagCapture; }
            set { if (OnPropertyChanging(__.FlagCapture, value)) { _FlagCapture = value; OnPropertyChanged(__.FlagCapture); } }
        }

        private String _SamplingStatus;
        /// <summary></summary>
        [DisplayName("SamplingStatus")]
        [Description("")]
        [DataObjectField(false, false, true, 1)]
        [BindColumn("SamplingStatus", "", "char(1)")]
        public virtual String SamplingStatus
        {
            get { return _SamplingStatus; }
            set { if (OnPropertyChanging(__.SamplingStatus, value)) { _SamplingStatus = value; OnPropertyChanged(__.SamplingStatus); } }
        }

        private String _FlagCutorder;
        /// <summary></summary>
        [DisplayName("FlagCutorder")]
        [Description("")]
        [DataObjectField(false, false, true, 1)]
        [BindColumn("Flag_CutOrder", "", "char(1)")]
        public virtual String FlagCutorder
        {
            get { return _FlagCutorder; }
            set { if (OnPropertyChanging(__.FlagCutorder, value)) { _FlagCutorder = value; OnPropertyChanged(__.FlagCutorder); } }
        }

        private String _FlagPrintedSl;
        /// <summary></summary>
        [DisplayName("FlagPrintedSl")]
        [Description("")]
        [DataObjectField(false, false, true, 1)]
        [BindColumn("Flag_Printed_SL", "", "char(1)")]
        public virtual String FlagPrintedSl
        {
            get { return _FlagPrintedSl; }
            set { if (OnPropertyChanging(__.FlagPrintedSl, value)) { _FlagPrintedSl = value; OnPropertyChanged(__.FlagPrintedSl); } }
        }

        private String _QcFlagSl;
        /// <summary></summary>
        [DisplayName("QcFlagSl")]
        [Description("")]
        [DataObjectField(false, false, true, 1)]
        [BindColumn("QC_Flag_SL", "", "char(1)")]
        public virtual String QcFlagSl
        {
            get { return _QcFlagSl; }
            set { if (OnPropertyChanging(__.QcFlagSl, value)) { _QcFlagSl = value; OnPropertyChanged(__.QcFlagSl); } }
        }

        private String _QcTypeSl;
        /// <summary></summary>
        [DisplayName("QcTypeSl")]
        [Description("")]
        [DataObjectField(false, false, true, 128)]
        [BindColumn("QC_Type_SL", "", "varchar(128)")]
        public virtual String QcTypeSl
        {
            get { return _QcTypeSl; }
            set { if (OnPropertyChanging(__.QcTypeSl, value)) { _QcTypeSl = value; OnPropertyChanged(__.QcTypeSl); } }
        }

        private String _QcDescriptionSl;
        /// <summary></summary>
        [DisplayName("QcDescriptionSl")]
        [Description("")]
        [DataObjectField(false, false, true, 128)]
        [BindColumn("QC_Description_SL", "", "varchar(128)")]
        public virtual String QcDescriptionSl
        {
            get { return _QcDescriptionSl; }
            set { if (OnPropertyChanging(__.QcDescriptionSl, value)) { _QcDescriptionSl = value; OnPropertyChanged(__.QcDescriptionSl); } }
        }

        private Int32 _FlagValid;
        /// <summary></summary>
        [DisplayName("FlagValid")]
        [Description("")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn("Flag_Valid", "", "int")]
        public virtual Int32 FlagValid
        {
            get { return _FlagValid; }
            set { if (OnPropertyChanging(__.FlagValid, value)) { _FlagValid = value; OnPropertyChanged(__.FlagValid); } }
        }

        private String _SamplingFormat;
        /// <summary>取材样式Id（结构化报告样式）</summary>
        [DisplayName("取材样式Id")]
        [Description("取材样式Id（结构化报告样式）")]
        [DataObjectField(false, false, true, 64)]
        [BindColumn("SamplingFormat", "取材样式Id（结构化报告样式）", "varchar(64)")]
        public virtual String SamplingFormat
        {
            get { return _SamplingFormat; }
            set { if (OnPropertyChanging(__.SamplingFormat, value)) { _SamplingFormat = value; OnPropertyChanged(__.SamplingFormat); } }
        }
        #endregion

        #region 获取/设置 字段值
        /// <summary>
        /// 获取/设置 字段值。
        /// 一个索引，基类使用反射实现。
        /// 派生实体类可重写该索引，以避免反射带来的性能损耗
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public override Object this[String name]
        {
            get
            {
                switch (name)
                {
                    case __.SLId : return _SLId;
                    case __.PBId : return _PBId;
                    case __.SamplingType : return _SamplingType;
                    case __.SamplingFindings : return _SamplingFindings;
                    case __.FlagCapture : return _FlagCapture;
                    case __.SamplingStatus : return _SamplingStatus;
                    case __.FlagCutorder : return _FlagCutorder;
                    case __.FlagPrintedSl : return _FlagPrintedSl;
                    case __.QcFlagSl : return _QcFlagSl;
                    case __.QcTypeSl : return _QcTypeSl;
                    case __.QcDescriptionSl : return _QcDescriptionSl;
                    case __.FlagValid : return _FlagValid;
                    case __.SamplingFormat : return _SamplingFormat;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case __.SLId : _SLId = Convert.ToString(value); break;
                    case __.PBId : _PBId = Convert.ToString(value); break;
                    case __.SamplingType : _SamplingType = Convert.ToString(value); break;
                    case __.SamplingFindings : _SamplingFindings = Convert.ToString(value); break;
                    case __.FlagCapture : _FlagCapture = Convert.ToString(value); break;
                    case __.SamplingStatus : _SamplingStatus = Convert.ToString(value); break;
                    case __.FlagCutorder : _FlagCutorder = Convert.ToString(value); break;
                    case __.FlagPrintedSl : _FlagPrintedSl = Convert.ToString(value); break;
                    case __.QcFlagSl : _QcFlagSl = Convert.ToString(value); break;
                    case __.QcTypeSl : _QcTypeSl = Convert.ToString(value); break;
                    case __.QcDescriptionSl : _QcDescriptionSl = Convert.ToString(value); break;
                    case __.FlagValid : _FlagValid = Convert.ToInt32(value); break;
                    case __.SamplingFormat : _SamplingFormat = Convert.ToString(value); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得PbSamplinglesion字段信息的快捷方式</summary>
        public partial class _
        {
            ///<summary></summary>
            public static readonly Field SLId = FindByName(__.SLId);

            ///<summary></summary>
            public static readonly Field PBId = FindByName(__.PBId);

            ///<summary></summary>
            public static readonly Field SamplingType = FindByName(__.SamplingType);

            ///<summary></summary>
            public static readonly Field SamplingFindings = FindByName(__.SamplingFindings);

            ///<summary></summary>
            public static readonly Field FlagCapture = FindByName(__.FlagCapture);

            ///<summary></summary>
            public static readonly Field SamplingStatus = FindByName(__.SamplingStatus);

            ///<summary></summary>
            public static readonly Field FlagCutorder = FindByName(__.FlagCutorder);

            ///<summary></summary>
            public static readonly Field FlagPrintedSl = FindByName(__.FlagPrintedSl);

            ///<summary></summary>
            public static readonly Field QcFlagSl = FindByName(__.QcFlagSl);

            ///<summary></summary>
            public static readonly Field QcTypeSl = FindByName(__.QcTypeSl);

            ///<summary></summary>
            public static readonly Field QcDescriptionSl = FindByName(__.QcDescriptionSl);

            ///<summary></summary>
            public static readonly Field FlagValid = FindByName(__.FlagValid);

            ///<summary>取材样式Id（结构化报告样式）</summary>
            public static readonly Field SamplingFormat = FindByName(__.SamplingFormat);

            static Field FindByName(String name) { return Meta.Table.FindByName(name); }
        }

        /// <summary>取得PbSamplinglesion字段名称的快捷方式</summary>
        partial class __
        {
            ///<summary></summary>
            public const String SLId = "SLId";

            ///<summary></summary>
            public const String PBId = "PBId";

            ///<summary></summary>
            public const String SamplingType = "SamplingType";

            ///<summary></summary>
            public const String SamplingFindings = "SamplingFindings";

            ///<summary></summary>
            public const String FlagCapture = "FlagCapture";

            ///<summary></summary>
            public const String SamplingStatus = "SamplingStatus";

            ///<summary></summary>
            public const String FlagCutorder = "FlagCutorder";

            ///<summary></summary>
            public const String FlagPrintedSl = "FlagPrintedSl";

            ///<summary></summary>
            public const String QcFlagSl = "QcFlagSl";

            ///<summary></summary>
            public const String QcTypeSl = "QcTypeSl";

            ///<summary></summary>
            public const String QcDescriptionSl = "QcDescriptionSl";

            ///<summary></summary>
            public const String FlagValid = "FlagValid";

            ///<summary>取材样式Id（结构化报告样式）</summary>
            public const String SamplingFormat = "SamplingFormat";

        }
        #endregion
    }

    /// <summary>PbSamplinglesion接口</summary>
    /// <remarks></remarks>
    public partial interface IPbSamplinglesion
    {
        #region 属性
        /// <summary></summary>
        String SLId { get; set; }

        /// <summary></summary>
        String PBId { get; set; }

        /// <summary></summary>
        String SamplingType { get; set; }

        /// <summary></summary>
        String SamplingFindings { get; set; }

        /// <summary></summary>
        String FlagCapture { get; set; }

        /// <summary></summary>
        String SamplingStatus { get; set; }

        /// <summary></summary>
        String FlagCutorder { get; set; }

        /// <summary></summary>
        String FlagPrintedSl { get; set; }

        /// <summary></summary>
        String QcFlagSl { get; set; }

        /// <summary></summary>
        String QcTypeSl { get; set; }

        /// <summary></summary>
        String QcDescriptionSl { get; set; }

        /// <summary></summary>
        Int32 FlagValid { get; set; }

        /// <summary>取材样式Id（结构化报告样式）</summary>
        String SamplingFormat { get; set; }
        #endregion

        #region 获取/设置 字段值
        /// <summary>获取/设置 字段值。</summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        Object this[String name] { get; set; }
        #endregion
    }
}