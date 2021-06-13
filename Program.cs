using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetDimension.NanUI;
using NetDimension.NanUI.HostWindow;
using NetDimension.NanUI.EmbeddedFileResource;
using NetDimension.NanUI.JavaScript;
using System.Reflection;
using System.IO;

namespace CSUR.Apps
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
            WinFormium.CreateRuntimeBuilder(env =>
            {
                env.CustomCefSettings(settings =>
                {
                    //在此处设置CEF参数
                });

                env.CustomCefCommandLineArguments(command =>
                {
                    //在此设置CEF命令行
                });

            },app=> {
                //启动指定窗体
                app.UseDebuggingMode();
                app.UseEmbeddedFileResource("http","csur.app.local","CSURWebs");

                //app.UseEmbeddedFileResource("http", "csur1.app.local", "test");

                app.UseMainWindow(context => new Main());
            })
                .Build()
                .Run()
                ;
        }
    }
}
