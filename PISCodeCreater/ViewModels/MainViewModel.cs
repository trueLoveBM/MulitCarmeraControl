using EzQrCode;
using PIS.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PISCodeCreater.ViewModels
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : Caliburn.Micro.Screen
    {

        #region 字段

        /// <summary>
        /// 每页的数量
        /// </summary>
        private int _pageSize = 10;

        /// <summary>
        /// 当前页码
        /// </summary>
        private long _currentPageIndex = 0;
        #endregion


        #region 界面绑定数据源
        private ObservableCollection<PbSamplinglesion> _datas;

        /// <summary>
        /// 列表数据源
        /// </summary>
        public ObservableCollection<PbSamplinglesion> Datas
        {
            get { return _datas; }
            set
            {
                _datas = value;
                NotifyOfPropertyChange(nameof(Datas));
            }
        }


        private ObservableCollection<long> _pages;

        /// <summary>
        /// 页码
        /// </summary>
        public ObservableCollection<long> Pages
        {
            get { return _pages; }
            set
            {
                _pages = value;
                NotifyOfPropertyChange(nameof(Pages));
            }
        }


        public long CurrentPage
        {
            get { return _currentPageIndex; }
            set
            {
                _currentPageIndex = value;
                NotifyOfPropertyChange(nameof(CurrentPage));
            }
        }


        private ObservableCollection<string> _codeSize;

        /// <summary>
        /// 二维码的可选尺寸
        /// </summary>
        public ObservableCollection<string> CodeSize
        {
            get { return _codeSize; }
            set
            {
                _codeSize = value;
                NotifyOfPropertyChange(nameof(CodeSize));
            }
        }


        private string _selectCodeSize;
        /// <summary>
        /// 当前选择的尺寸
        /// </summary>
        public string SelectCodeSize
        {
            get { return _selectCodeSize; }
            set
            {
                _selectCodeSize = value;
                NotifyOfPropertyChange(nameof(SelectCodeSize));
            }
        }

        private bool _mergeCode = true;

        /// <summary>
        /// 是否合并二维码
        /// </summary>
        public bool MergeCode
        {
            get { return _mergeCode; }
            set
            {
                _mergeCode = value;
                NotifyOfPropertyChange(nameof(MergeCode));
            }
        }


        private string _outputDir = @"D:\\Test";

        /// <summary>
        /// 输出目录
        /// </summary>
        public string OutputDir
        {
            get { return _outputDir; }
            set
            {
                _outputDir = value;
                NotifyOfPropertyChange(nameof(OutputDir));
            }
        }


        #endregion



        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            //二维码可选尺寸
            CodeSize = new ObservableCollection<string>();
            CodeSize.Add("500*500");
            CodeSize.Add("100*100");
            CodeSize.Add("50*50");
            CodeSize.Add("10*10");
            CodeSize.Add("5*5");

            SelectCodeSize = CodeSize.First();

            //加载标本数据
            var list = PbSamplinglesion.Search(_currentPageIndex, _pageSize, out long PageCount);
            Datas = new ObservableCollection<PbSamplinglesion>(list);


            //生成页码
            List<long> p = new List<long>();
            for (long i = 0; i < PageCount; i++)
            {
                p.Add(i);
            }
            Pages = new ObservableCollection<long>(p);
        }


        public void PageChanged()
        {
            //加载标本数据
            var list = PbSamplinglesion.Search(_currentPageIndex, _pageSize, out long PageCount);
            Datas.Clear();
            foreach (var item in list)
            {
                Datas.Add(item);
            }
        }


        public void CreateQrCode()
        {
            string[] Size = SelectCodeSize.Split("*");
            int width = int.Parse(Size[0]);
            int height = int.Parse(Size[1]);

            if (!MergeCode)
            {
                foreach (var item in Datas)
                {
                    string CodeContent = item.SLId + "_" + item.PBId;
                    string savePath = _outputDir + $"\\{item.SLId}.png";
                    QrCodeWriter.CreateQrCode(CodeContent, savePath, ImageFormat.Png, "utf-8", width, height);
                }
            }
            else
            {
                //将生成的二维码放在一张图上
                //暂定一行放五个二维码
                int column = 5;
               
                List<Bitmap> codeList = new List<Bitmap>();
                foreach (var item in Datas)
                {
                    string CodeContent = item.SLId + "_" + item.PBId;
                    Bitmap code = QrCodeWriter.CreateQrCode(CodeContent, ImageFormat.Png, "utf-8", width, height);
                    codeList.Add(code);
                }

                Bitmap newBitmap = MergeBitmaps(codeList, column);
                string savePath = _outputDir + $"\\testMeger.png";
                newBitmap.Save(savePath, ImageFormat.Png);
                newBitmap.Dispose();
                newBitmap = null;

                codeList.ForEach(X => X.Dispose());
                codeList.Clear();
                codeList = null;
            }
            MessageBox.Show("生成成功");

        }

        public void ChangeOutputDir()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OutputDir = dialog.SelectedPath;
            }
        }


        /// <summary>
        /// 合并二维码到一张图上
        /// </summary>
        /// <param name="bitmaps"></param>
        /// <param name="Columns"></param>
        private Bitmap MergeBitmaps(List<Bitmap> bitmaps, int Columns)
        {
            //总宽度
            int width = (bitmaps.First().Width + 20) * Columns;
            //总高度
            int height = (bitmaps.Count / Columns + bitmaps.Count % Columns) * (bitmaps.First().Height + 20);

            Bitmap bitmap = new Bitmap(width, height);

            int rowIndex = -1;
            for (int i = 0; i < bitmaps.Count; i++)
            {
                //第几列
                int ColumnIndex = i % Columns;
                //第几行
                if (ColumnIndex == 0)
                    rowIndex++;


                //计算此二维码排放七点
                int o_x = ColumnIndex * (bitmaps.First().Width + 10) + 10;
                int o_y = rowIndex * (bitmaps.First().Height + 10) + 10;


                for (int X = 0; X < bitmaps[i].Width - 1; X++)
                {
                    for (int Y = 0; Y < bitmaps[i].Height - 1; Y++)
                    {
                        var c = bitmaps[i].GetPixel(X, Y);
                        bitmap.SetPixel(o_x, o_y + Y, c);
                    }
                    o_x++;
                }


            }

            return bitmap;
        }
    }
}
