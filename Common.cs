using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using NetDimension.NanUI.JavaScript;
using NetDimension.NanUI;
using NetDimension.NanUI.HostWindow;
using NetDimension.NanUI.EmbeddedFileResource;

namespace CSUR.Apps
{
    public class Common
    {
        public static void TestShow()
        {
            MessageBox.Show("Test");
        }
        public static string getRoadimporter_CO()
        {
            string _userName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string destDir = _userName + @"\Colossal Order\Cities_Skylines\RoadImporter";
            return destDir;
        }

        public static string getRoadimporter_Mods()
        {
            string _userName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string destDir = _userName + @"\Colossal Order\Cities_Skylines\Addons\Mods\RoadImporter";
            return destDir;
        }

        public delegate void CopyDirCallback(bool _error, bool _finish);

        public static void CopyDir(string _source, string _dest, CopyDirCallback _callback)
        {
            Thread _thread = new Thread(new ThreadStart(delegate ()
            {
                try
                {
                    if (!CopyDir(_source, _dest))
                    {
                        _callback(true, true);
                    }
                    else
                    {
                        _callback(false, true);
                    }
                }
                catch (Exception)
                {
                    _callback(true, true);
                };
            }));

            _thread.Start();
        }

        private static bool CopyDir(string _source, string _dest)
        {
            bool retValue = false;

            try
            {
                _source = _source.EndsWith(@"\") ? _source : _source + @"\";
                _dest = _dest.EndsWith(@"\") ? _dest : _dest + @"\";

                if (Directory.Exists(_source))
                {
                    if (Directory.Exists(_dest) == false)
                    {
                        Directory.CreateDirectory(_dest);
                    }

                    foreach (string _file in Directory.GetFiles(_source))
                    {
                        FileInfo _fileInfo = new FileInfo(_file);
                        _fileInfo.CopyTo(_dest + _fileInfo.Name, true);
                    }

                    foreach (string _dir in Directory.GetDirectories(_source))
                    {
                        DirectoryInfo _dirInfo = new DirectoryInfo(_dir);
                        if (CopyDir(_dir, _dest + _dirInfo.Name) == false)
                        {
                            retValue = false;
                        }
                    }

                    retValue = true;
                }
            }
            catch (Exception) { };

            return retValue;
        }

        ///判断CSUR代码仓库是否存在
        
        public static bool CheckCSURMaster()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\CSUR-master") == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //判断贴图包是否安装到CSL目录下
        public static bool CheckTexturesPack()
        {
            Console.WriteLine(getRoadimporter_CO() + "\\textures");
            if(Directory.Exists(getRoadimporter_CO() + "\\textures") == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断Blender是否安装
        /// </summary>
        /// <returns></returns>
        public static bool checkBlender()
        {
            if( Directory.Exists(@"C:\Users\PhantomCTS\AppData\Roaming\Blender Foundation\Blender") == true)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public static string GetCRMPath()
        {
            return Directory.GetCurrentDirectory() + "\\CSUR-master";
        }


    }

    public static class Common_Var
    {
        /// <summary>
        /// 常用变量
        /// </summary>
        public static bool Blender;
        public static string CRM_path = "";
        public static  string CEM_path = "";
        public static  string CSL_path = "";
        public static string CRM_ver = "";
        public static string CEM_ver = "";
        public static bool CRM;
        public static bool Ri_mods;
        public static  string Roadimporter_CO = "";
        public static  string Roadimporter_Mods = "";
        public static string w_dir = Directory.GetCurrentDirectory();
        public static string config = Directory.GetCurrentDirectory() + "\\config.txt";
    }

    public static class CSUR_Generator_Var
    {
        public static string Task = "./make.bat";
        public static string RoadeCode = "";
        public static string Compress = "";
        public static string Express = "";
        public static string Filp = "";

        public static bool CanStart = false;
    }

    public static class Generator_Program
    {
        //复制文件程序
        private static bool CopyDir(string _source, string _dest)
        {
            bool retValue = false;

            try
            {
                _source = _source.EndsWith(@"\") ? _source : _source + @"\";
                _dest = _dest.EndsWith(@"\") ? _dest : _dest + @"\";

                if (Directory.Exists(_source))
                {
                    if (Directory.Exists(_dest) == false)
                    {
                        Directory.CreateDirectory(_dest);
                    }

                    foreach (string _file in Directory.GetFiles(_source))
                    {
                        FileInfo _fileInfo = new FileInfo(_file);
                        _fileInfo.CopyTo(_dest + _fileInfo.Name, true);
                    }

                    foreach (string _dir in Directory.GetDirectories(_source))
                    {
                        DirectoryInfo _dirInfo = new DirectoryInfo(_dir);
                        if (CopyDir(_dir, _dest + _dirInfo.Name) == false)
                        {
                            retValue = false;
                        }
                    }

                    retValue = true;
                }
            }
            catch (Exception) { };

            return retValue;
        }
        //生成器程序
        public static bool Generator_S(string roadname)
        {
            Process proc = null;
            bool locals = false;

            if (Common.GetCRMPath() != "" && Common.GetCRMPath() != null)
            {
                locals = true;
            }
            else
                locals = false;

            try
            {
                if (locals == true)
                {
                    string Path = Common.GetCRMPath();
                }
                //检查是否有文件残留，如果没有残留则直接生成
                if (Directory.Exists(Common.getRoadimporter_CO() + "\\import") == true)//检查CSL C盘根目录import目录是否存在，如果存在则清理残留
                {
                    DirectoryInfo _del_imp_CO = new DirectoryInfo(Common.getRoadimporter_CO() + "\\import");//检查CSL C盘根目录import的文件残留
                    _del_imp_CO.Delete(true);
                    Directory.CreateDirectory(Common.getRoadimporter_CO() + "\\import");//删除完毕后重建确保程序正确运行
                    FileInfo _del_imp_CO_txt = new FileInfo(Common.getRoadimporter_CO() + "\\import.txt");//import.txt由生成器自动生成
                    _del_imp_CO_txt.Delete();
                    if (Directory.Exists(Common.GetCRMPath() + "\\output") == true)//检查CSUR代码仓库残留，有则删除
                    {
                        DirectoryInfo _del_CRM_imp = new DirectoryInfo(Common.GetCRMPath() + "\\ouput");
                        _del_CRM_imp.Delete(true);
                        ///output为自动生成，可以不用程序创建
                        ///import删除完毕后需要重建文件夹
                    }
                }
                else
                {
                    Directory.CreateDirectory(Common.getRoadimporter_CO() + "\\import");//如果文件夹不存在则创建
                }
                ///生成环境二次确认完毕
                ///开始生成
                string GeneratorPath = Common.GetCRMPath() ;//生成器运行目录
                proc = new Process();
                proc.StartInfo.WorkingDirectory = GeneratorPath;
                proc.StartInfo.FileName = "make.bat";
                proc.StartInfo.Arguments = string.Format(roadname);//this is argument
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                StreamReader _g_back = proc.StandardOutput;
                proc.WaitForExit();
                string _g_result = _g_back.ReadToEnd().Trim();
                _g_back.Close();
                bool _g_contain = _g_result.Contains("ValueError");
                if (_g_contain == false)
                {
                    Directory.Move(GeneratorPath + "\\ouput", GeneratorPath + "\\import");//将生成的文件移动至CSL C盘根目录Roadimporter文件夹
                    CopyDir(GeneratorPath + "\\import", Common.getRoadimporter_CO() + "\\import");
                    //将import.txt复制到上一级目录(Roadimporter)
                    FileInfo importtxt = new FileInfo(Common.getRoadimporter_CO() + "\\import\\import.txt");
                    importtxt.MoveTo(Common.getRoadimporter_CO());
                    return true;//全部执行完毕返回true，程序关闭
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
                return false;
            }
        }
    }
}
