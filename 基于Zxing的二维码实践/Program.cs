using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace 基于Zxing的二维码实践
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new 演示主窗体());
        }
    }
}
