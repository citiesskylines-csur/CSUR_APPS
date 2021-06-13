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
using Microsoft.Win32;

namespace CSUR.Apps
{
    public class Common
    {

        public static bool yn;

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

        public static string getUsername()
        {
            string _userName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return _userName;
        }

        public static string getRoadimporter_Mods()
        {
            string _userName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string destDir = _userName + @"\Colossal Order\Cities_Skylines\Addons\Mods\Roadimporter";
            return destDir;
        } 

        public static bool Roadimpoter_Check()
        {
            Console.WriteLine(getRoadimporter_Mods() + "\\RoadImporter.dll");
            if(File.Exists(getRoadimporter_Mods() + "\\RoadImporter.dll") == true)
            {
                return true;
            }
            else
            {
                return false;
            }
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
            Process[] _blenders_opend = Process.GetProcessesByName("Blender");
            if(_blenders_opend.Length > 0)
            {
                return true;
            }
            else
            {
                Process _c_b = new Process();
                _c_b.StartInfo.FileName = "cmd.exe";
                _c_b.StartInfo.UseShellExecute = false;
                _c_b.StartInfo.RedirectStandardInput = true;
                _c_b.StartInfo.RedirectStandardOutput = true;
                _c_b.StartInfo.RedirectStandardError = true;
                _c_b.StartInfo.CreateNoWindow = true;

                _c_b.Start();

                _c_b.StandardInput.WriteLine("Blender" + "&&exit");

                _c_b.StandardInput.AutoFlush = true;

                _c_b.WaitForExit(2000);
                _c_b.Close();

                Process[] _blenders = Process.GetProcessesByName("Blender");

                if (_blenders.Length > 0)
                {
                    Process _c_b_close = new Process();
                    _c_b_close.StartInfo.FileName = "cmd.exe";
                    _c_b_close.StartInfo.UseShellExecute = false;
                    _c_b_close.StartInfo.RedirectStandardInput = true;
                    _c_b_close.StartInfo.RedirectStandardOutput = true;
                    _c_b_close.StartInfo.RedirectStandardError = true;
                    _c_b_close.StartInfo.CreateNoWindow = true;

                    _c_b_close.Start();

                    _c_b_close.StandardInput.WriteLine("taskkill /f /t /im Blender.exe");

                    _c_b_close.StandardInput.AutoFlush = true;

                    _c_b_close.WaitForExit(2000);
                    _c_b_close.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static string GetEnvironmentVariable()
        {
            RegistryKey regLocalMachine = Registry.CurrentUser;
            RegistryKey regEnvironment = regLocalMachine.OpenSubKey("Environment", true);
            string sPath = regEnvironment.GetValue("path").ToString();//读取path的值
            Console.WriteLine(sPath);
            bool _is_blender = sPath.Contains("Blender");

            if(_is_blender == true)
            {
                return "Blender OK";
            }
            else
            {
                return "Blender NO";
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
        public static string Bar = "";

        public static bool CanStart = false;

        public static bool _f_contain= false;
    }

    public static class Generator_Program
    {
        //复制文件程序
        public static bool CopyDir(string _source, string _dest)
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
        public static string Generator_S(string roadname)
        {
            Process proc = null;

            if (Common.GetCRMPath() != "" && Common.GetCRMPath() != null)
            {
                string Path = Common.GetCRMPath();
            }
            else
            {
                return "CSUR Master error";
            }

            CSUR_Generator_Var.Bar = "15";
            //检查是否有文件残留，如果没有残留则直接生成
            if (Directory.Exists(Common.getRoadimporter_CO() + "\\import") == true)//检查CSL C盘根目录import目录是否存在，如果存在则清理残留
            {
                DirectoryInfo _del_imp_CO = new DirectoryInfo(Common.getRoadimporter_CO() + "\\import");//检查CSL C盘根目录import的文件残留
                _del_imp_CO.Delete(true);
                Directory.CreateDirectory(Common.getRoadimporter_CO() + "\\import");//删除完毕后重建确保程序正确运行
                FileInfo _del_imp_CO_txt = new FileInfo(Common.getRoadimporter_CO() + "\\imports.txt");//imports.txt由生成器自动生成
                _del_imp_CO_txt.Delete();
                if (Directory.Exists(Common.GetCRMPath() + "\\output") == true)//检查CSUR代码仓库残留，有则删除
                {
                    DirectoryInfo _del_CRM_imp = new DirectoryInfo(Common.GetCRMPath() + "\\output");
                    _del_CRM_imp.Delete(true);
                    ///output为自动生成，可以不用程序创建
                    ///import删除完毕后需要重建文件夹
                }
            }
            else
            {
                Directory.CreateDirectory(Common.getRoadimporter_CO() + "\\import");//如果文件夹不存在则创建
            }
            CSUR_Generator_Var.Bar = "45";
            ///生成环境二次确认完毕
            ///开始生成
            string GeneratorPath = Common.GetCRMPath();//生成器运行目录
            proc = new Process();
            proc.StartInfo.WorkingDirectory = GeneratorPath;
            proc.StartInfo.FileName = Common.GetCRMPath() + "\\make.bat";
            proc.StartInfo.Arguments = string.Format(roadname);//this is argument
            //proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.CreateNoWindow = false;
            proc.Start();
            CSUR_Generator_Var.Bar = "65";
            proc.WaitForExit();
            ///通过判断imports.txt有没有生成来进行一级判断
            ///再通过imports.txt的内容能否和RoadCode对应
            ///如果可以，则代表生成成功
            ///反之代表生成失败或者错误
            CSUR_Generator_Var.Bar = "75";
            Console.WriteLine(Directory.Exists(GeneratorPath + "\\output"));
            if (Directory.Exists(GeneratorPath + "\\output") == true)
            {
                Console.WriteLine(File.Exists(GeneratorPath + "\\output\\imports.txt"));
                if (File.Exists(GeneratorPath + "\\output\\imports.txt") == true)
                {
                    if (Directory.Exists(GeneratorPath + "\\import") == true)
                    {
                        DirectoryInfo _old_import = new DirectoryInfo(GeneratorPath + "\\import");
                        _old_import.Delete(true);
                        Directory.Move(GeneratorPath + "\\output", GeneratorPath + "\\import");
                        //将生成的文件移动至CSL C盘根目录Roadimporter文件夹
                        CopyDir(GeneratorPath + "\\import", Common.getRoadimporter_CO() + "\\import");
                        //将imports.txt复制到上一级目录(Roadimporter)
                        FileInfo importtxt = new FileInfo(Common.getRoadimporter_CO() + "\\import\\imports.txt");
                        importtxt.MoveTo(Common.getRoadimporter_CO()+"\\imports.txt");
                        CSUR_Generator_Var.Bar = "100";
                        return "1 True";//全部执行完毕返回true，程序关闭
                    }
                    else
                    {
                        Directory.Move(GeneratorPath + "\\output", GeneratorPath + "\\import");//将生成的文件移动至CSL C盘根目录Roadimporter文件夹
                        CopyDir(GeneratorPath + "\\import", Common.getRoadimporter_CO() + "\\import");
                        //将imports.txt复制到上一级目录(Roadimporter)
                        FileInfo importtxt = new FileInfo(Common.getRoadimporter_CO() + "\\import\\imports.txt");
                        importtxt.MoveTo(Common.getRoadimporter_CO() + "\\imports.txt");
                        CSUR_Generator_Var.Bar = "100";
                        return "1 True";//全部执行完毕返回true，程序关闭
                    }
                    
                }
                else
                {
                    CSUR_Generator_Var.Bar = "100";
                    return "false \n output failed";
                }
            }
            else
            {
                return "false \n output failed";
            }
        }
    }

    public static class Installation
    {
        public static string _instal_callback_mod = "";
        public static string _instal_callback_blender = "";
        public static string _instal_callback_texture = "";
        public static bool _install_status_mod;
        public static bool _install_status_blender;
        public static bool _install_status_texture;
        public static void _install_all()
        {
            if(Directory.Exists(Common.getRoadimporter_Mods()) == true)//先判断C盘的MOD文件夹是否存在
            {
                if (Directory.Exists(Common.GetCRMPath() + "\\Bin") == true)//其次判断代码仓库的bin文件夹是否存在
                {
                    if (File.Exists(Common.GetCRMPath() + "\\Bin\\Roadimporter.dll") == true)//最后判断MOD的文件是否存在
                    {
                        _install_dll();//如果存在进行安装
                        _instal_callback_mod = "Mods installed";
                        _install_status_mod = true;
                    }
                    else
                    {
                        _instal_callback_mod = "Mods install failed \n mod not found \n path:" + Common.GetCRMPath() + "\\Bin\\Roadimporter.dll";//如果不存在返回错误详情
                        _install_status_mod = false;
                    }
                }
                else
                {
                    _instal_callback_mod = "Mods install failed \n Directory not found \n Path:" + Common.GetCRMPath() + "\\Bin";//如果不存在返回错误详情
                    _install_status_mod = false;
                }
            }
            else
            {
                Directory.CreateDirectory(Common.getRoadimporter_Mods());
                if (Directory.Exists(Common.GetCRMPath() + "\\Bin") == true)
                {
                    if (File.Exists(Common.GetCRMPath() + "\\Bin\\Roadimporter.dll") == true)
                    {
                        _install_dll();
                        _instal_callback_mod = "Mods installed";
                        _install_status_mod = true;
                    }
                }
                else
                {
                    _instal_callback_mod = "Mods install failed \n mod not found";//如果不存在返回错误详情
                    _install_status_mod = false;
                }
            }
            if (Common.checkBlender() == true)
            {
                RegistryKey regLocalMachine = Registry.CurrentUser;
                RegistryKey regEnvironment = regLocalMachine.OpenSubKey("Environment", true);
                if (regEnvironment.GetValue("path").ToString().Contains("Blender") == true)
                {
                    _instal_callback_blender = "Blender OK";
                    _install_status_blender = true;
                }
                else
                {
                    _install_blender();
                }
            }
            else
            {
                _instal_callback_blender = "Blender not install first.";
            }
            if (Directory.Exists(Common.getRoadimporter_CO() + "\\Textures") == true)
            {
                _install_status_texture = true;
                _instal_callback_texture = "Texture OK";
            }
            else if (Directory.Exists(Common.getRoadimporter_CO() + "\\Textures") == false)
            {
                _install_textpack();
            }
           
            

        }

        private static  void _install_dll()
        {
            FileInfo _dll = new FileInfo(Common.GetCRMPath() + "\\Bin\\Roadimporter.dll");
            _dll.CopyTo(Common.getRoadimporter_Mods() + "\\Roadimporter.dll");
        }

        private static void _install_blender()
        {
            Process[] _blenders = Process.GetProcessesByName("Blender");

            if (_blenders.Length > 0)
            {
                string _blender_path = _blenders[0].MainModule.FileName;
                RegistryKey regLocalMachine = Registry.CurrentUser;
                RegistryKey regEnvironment = regLocalMachine.OpenSubKey("Environment", true);
                regEnvironment.SetValue("path", _blender_path);//写入path的值
                if (regEnvironment.GetValue("path").ToString().Contains("Blender") == true)
                {
                    _instal_callback_blender = "Blender OK";
                    _install_status_blender = true;
                }
            }
            else
            {
                _instal_callback_blender = "Blender not open";
                _install_status_blender = false;
            }
        }

        private static void _install_textpack()
        {
            if(Directory.Exists(Common.GetCRMPath() + "\\Textures") == true)
            {
                if(Directory.Exists(Common.getRoadimporter_CO()) == true)
                {
                    Generator_Program.CopyDir(Common.GetCRMPath() + "\\Textures", Common.getRoadimporter_CO()+"\\Textures");
                    if(Directory.Exists(Common.getRoadimporter_CO() + "\\Textures") == true)
                    {
                        _install_status_texture = true;
                        _instal_callback_texture = "Texture OK";
                    }
                    else
                    {
                        _install_status_texture = false;
                        _instal_callback_texture = "Texture install failed";
                    }
                }
                else
                {
                    _install_status_texture = false;
                    _instal_callback_texture = "(-1)";
                }
            }
            else
            {
                _install_status_texture = false;
                _instal_callback_texture = "Textures souce not found";
            }
        }
    }
}
