using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetDimension.NanUI.EmbeddedFileResource;
using NetDimension.NanUI.JavaScript;
using System.Reflection;
using NetDimension.NanUI;
using NetDimension.NanUI.HostWindow;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Threading;

namespace CSUR.Apps
{
    class Main : Formium
    {
        System.Timers.Timer getprocess = new System.Timers.Timer(); //创建获取进度时钟

        System.Timers.Timer getInsall_envir_dll = new System.Timers.Timer();//创建安装Roadimporter.dll回调的时钟

        string Bar;

        string _Generat_result;

        bool _expecterror = false;


        public override HostWindowType WindowType => HostWindowType.Borderless;

        public override string StartUrl => "http://csur.app.local/index.html";
        public Main()
        {
            Size = new Size(1600, 900);
            MinimumSize = new Size(1600, 900);
            MaximumSize = new Size(1600, 900);

            Icon = Properties.Resources.logoicon;

            BorderlessWindowProperties.BorderEffect = BorderEffect.RoundCorner;
            BorderlessWindowProperties.ShadowEffect = ShadowEffect.DropShadow;
            BorderlessWindowProperties.ShadowColor = Color.DimGray;

            CustomMaskPanel();

            StartPosition = FormStartPosition.CenterScreen;

            Title = "CSUR Apps";
        }

        private void CustomMaskPanel()
        {
            var label = new Label
            {
                Text = "CSUR Apps",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.None,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Light", 60.0f, GraphicsUnit.Point)
            };

            label.Width = Width;
            label.Height = Height / 2;
            label.Location = new Point(0, (Height - label.Height) / 2);
            label.BackColor = Color.Transparent;


            //Mask.Content.Add(label);

            //Mask.BackColor = Color.FromArgb(247,247,247);

            var loaderlogo = new PictureBox
            {
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Size = new Size(1600, 900),
                Image = Properties.Resources.CSURlogo_3_whit_mid,
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent
            };

            Mask.Content.Add(loaderlogo);

            Mask.Image = Properties.Resources.Artboard1_bg;
            //Mask.Content.Add(loaderbg);
        }


        protected override void OnReady()
        {
            
            Generator();
            Envirment_Check();
            BeforCheck();
            Install_envir();
            //ShowDevTools();
        }

        private void Envirment_Check()
        {
            var Envir = JavaScriptValue.CreateObject();

            var StartCheck = JavaScriptValue.CreateObject();

            StartCheck.SetValue("StartCheck", JavaScriptValue.CreateFunction(args =>
            {
                InvokeIfRequired(async () =>
                {
                    if (File.Exists(Common_Var.config) == false)
                    {
                        if (Common.getRoadimporter_CO() != "" || Common.getRoadimporter_CO() != null)//检查天际线C盘根目录
                        {
                            Envir.SetValue("RI_CO", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateString(Common_Var.Roadimporter_CO)));
                            await EvaluateJavaScriptAsync("document.getElementById('Path').innerHTML = 'Installed';");
                            await EvaluateJavaScriptAsync("document.getElementById('Path').style.color =  '#16E15E';");
                            Common_Var.CSL_path = Common.getRoadimporter_CO();
                            //await EvaluateJavaScriptAsync("document.getElementById('Path_img').className = 'lni lni-checkmark';");
                            if (Common.Roadimpoter_Check() == true)//检查Addons下的Roadimporter的MOD是否存在
                            {
                                Envir.SetValue("RI_Mods", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateString(Common_Var.Roadimporter_Mods)));
                                await EvaluateJavaScriptAsync("document.getElementById('RI_Mods').innerHTML = 'Installed';");
                                await EvaluateJavaScriptAsync("document.getElementById('RI_Mods').style.color =  '#16E15E';");
                                if (Common.GetEnvironmentVariable() == "Blender OK")//检测Blender是否配置了环境变量
                                {
                                    if(Common.checkBlender() == true)//检查Blender是否安装
                                    {
                                        Envir.SetValue("Blender", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateBool(true)));
                                        await EvaluateJavaScriptAsync("document.getElementById('Blender_in').innerHTML = 'Installed';");//Blender_in
                                        await EvaluateJavaScriptAsync("document.getElementById('Blender_in').style.color =  '#16E15E';");
                                    }
                                    else
                                    {
                                        await EvaluateJavaScriptAsync("document.getElementById('Blender_in').innerHTML = 'None';");
                                        await EvaluateJavaScriptAsync("document.getElementById('Blender_in').style.color =  '#A71D2A';");
                                        ExecuteJavaScript(@"Blender_error()");
                                    }
                                    if (Common.CheckCSURMaster() == true)//检查CSUR代码仓库是否安装
                                    {
                                        Envir.SetValue("CRM", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateBool(true)));
                                        await EvaluateJavaScriptAsync ("document.getElementById('CRM').innerHTML = 'Installed';");
                                        await EvaluateJavaScriptAsync("document.getElementById('CRM').style.color =  '#16E15E';");
                                        if (Common.CheckTexturesPack() == true)//检查贴图包是否安装（位于C盘根目录）
                                        {
                                            await EvaluateJavaScriptAsync("document.getElementById('TP_iNS').innerHTML = 'Installed';");
                                            await EvaluateJavaScriptAsync("document.getElementById('TP_iNS').style.color =  '#16E15E';");
                                            string[] configs =//全部检测通过写出config
                                            {
                                                Common.getRoadimporter_CO(),
                                                 Common.getRoadimporter_Mods(),
                                                 "Blender:True",
                                                 "CSURMaster:True",
                                                 "TexturesPack:True"
                                            };
                                            File.WriteAllLines(Common_Var.w_dir + "\\config.txt", configs);
                                            ExecuteJavaScript(@"Check_noti_group()");
                                            CSUR_Generator_Var.CanStart = true;
                                        }
                                        else
                                        {
                                            await EvaluateJavaScriptAsync("document.getElementById('TP_iNS').innerHTML = 'None';");
                                            await EvaluateJavaScriptAsync("document.getElementById('TP_iNS').style.color =  '#A71D2A';");
                                            ExecuteJavaScript(@"TPINS_error()");
                                        }
                                        
                                    }
                                    else
                                    {
                                        Envir.SetValue("CRM", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateBool(false)));
                                        Common_Var.CRM_path = Common.GetCRMPath();
                                        await EvaluateJavaScriptAsync("document.getElementById('CRM').innerHTML = 'None';");
                                        await EvaluateJavaScriptAsync("document.getElementById('CRM').style.color =  '#A71D2A';");
                                        ExecuteJavaScript(@"CRM_error()");
                                    } 
                                }
                                else
                                {
                                    Envir.SetValue("Blender", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateBool(false)));
                                    await EvaluateJavaScriptAsync ("document.getElementById('Blender_in').innerHTML = 'None';");
                                    await EvaluateJavaScriptAsync("document.getElementById('Blender_in').style.color =  '#A71D2A';");
                                    ExecuteJavaScript(@"Blender_path_error()");
                                }
                            }
                            else
                            {
                                Envir.SetValue("RI_Mods", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateString("None")));
                                await EvaluateJavaScriptAsync("document.getElementById('RI_Mods').innerHTML = 'None';");
                                await EvaluateJavaScriptAsync("document.getElementById('RI_Mods').style.color =  '#A71D2A';");
                                ExecuteJavaScript(@"RIMods_error()");
                            }
                        }
                        else
                        {
                            Envir.SetValue("RI_CO", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateString("None")));
                            await EvaluateJavaScriptAsync("document.getElementById('Path').innerHTML = 'None';");
                            await EvaluateJavaScriptAsync("document.getElementById('Path').style.color =  '#A71D2A';");
                            ExecuteJavaScript(@"Path_error()");
                        }
                    }
                    else
                    {
                        string _CONFIG =  File.ReadAllText(Common_Var.config);
                        string[] _CONGIF_ARRAY = _CONFIG.Split('\n');
                        if (_CONGIF_ARRAY[3].Trim() == "CSURMaster:True")
                        {
                            await EvaluateJavaScriptAsync("document.getElementById('CRM').innerHTML = 'Installed';");
                            await EvaluateJavaScriptAsync("document.getElementById('CRM').style.color =  '#16E15E';");
                            Common_Var.CRM = true;
                            Common_Var.CRM_path = Common.GetCRMPath();
                        }
                        else
                        {
                            Common_Var.CRM = false;
                            await EvaluateJavaScriptAsync("document.getElementById('CRM').innerHTML = 'None';");
                            await EvaluateJavaScriptAsync("document.getElementById('CRM').style.color =  '#A71D2A';");
                            ExecuteJavaScript(@"CRM_error()");
                        }
                        
                        if (_CONGIF_ARRAY[0] != "")
                        {
                            Common_Var.Roadimporter_CO = _CONGIF_ARRAY[0].Trim();
                            await EvaluateJavaScriptAsync("document.getElementById('Path').innerHTML = 'Installed';");
                            await EvaluateJavaScriptAsync("document.getElementById('Path').style.color =  '#16E15E';");
                            Common_Var.CSL_path = Common.getRoadimporter_CO();//\Colossal Order\Cities_Skylines\RoadImporter
                        }
                        else
                        {
                            await EvaluateJavaScriptAsync("document.getElementById('Path').innerHTML = 'None';");
                            await EvaluateJavaScriptAsync("document.getElementById('Path').style.color =  '#A71D2A';");
                            ExecuteJavaScript(@"Path_error()");
                        }
                        
                        if(_CONGIF_ARRAY[1] != "")
                        {
                            Common_Var.Roadimporter_Mods = _CONGIF_ARRAY[1].Trim();
                            await EvaluateJavaScriptAsync("document.getElementById('RI_Mods').innerHTML = 'Installed';");
                            await EvaluateJavaScriptAsync("document.getElementById('RI_Mods').style.color =  '#16E15E';");
                            Common_Var.Ri_mods = true;
                        }
                        else
                        {
                            await EvaluateJavaScriptAsync("document.getElementById('RI_Mods').innerHTML = 'None';");
                            await EvaluateJavaScriptAsync("document.getElementById('RI_Mods').style.color =  '#A71D2A';");
                            ExecuteJavaScript(@"RIMods_error()");
                        }
                        if(_CONGIF_ARRAY[2].Trim() == "Blender:True")
                        {
                            Common_Var.Blender = true;
                            await EvaluateJavaScriptAsync("document.getElementById('Blender_in').innerHTML = 'Installed';");//Blender_in
                            await EvaluateJavaScriptAsync("document.getElementById('Blender_in').style.color =  '#16E15E';");
                        }
                        else
                        {
                            Common_Var.Blender = false;
                            await EvaluateJavaScriptAsync("document.getElementById('Blender_in').innerHTML = 'None';");
                            ExecuteJavaScript(@"Blender_error()");
                        }
                        if (_CONGIF_ARRAY[4].Trim() == "TexturesPack:True")
                        {
                            await EvaluateJavaScriptAsync("document.getElementById('TP_iNS').innerHTML = 'Installed';");
                            await EvaluateJavaScriptAsync("document.getElementById('TP_iNS').style.color =  '#16E15E';");
                        }
                        else
                        {
                            await EvaluateJavaScriptAsync("document.getElementById('TP_iNS').innerHTML = 'None';");
                            await EvaluateJavaScriptAsync("document.getElementById('TP_iNS').style.color =  '#A71D2A';");
                            ExecuteJavaScript(@"TPINS_error()");
                        }

                        if (_CONGIF_ARRAY[1].Trim() != "" && _CONGIF_ARRAY[2].Trim() == "Blender:True" && _CONGIF_ARRAY[3].Trim() == "CSURMaster:True" && _CONGIF_ARRAY[4].Trim() == "TexturesPack:True" && _CONGIF_ARRAY[0].Trim() != "")
                        {
                            //await EvaluateJavaScriptAsync("document.getElementById('installation').style.disable =  'none';");
                            ExecuteJavaScript(@"Check_noti_group()");
                        }
                    }
                });

                return JavaScriptValue.CreateString("Checked");
            }));

            RegisterExternalObjectValue("StartCheck", StartCheck);
        }

        private void BeforCheck()
        {
            /***var b_Check = JavaScriptValue.CreateObject();

            b_Check.SetValue("BeforeCheck", JavaScriptValue.CreateFunction(args => {
            var msg = args.FirstOrDefault(x => x.IsString);

                InvokeIfRequired(async () =>
                {
                    string _CONFIG = File.ReadAllText(Common_Var.config);
                    string[] _CONGIF_ARRAY = _CONFIG.Split('\n');
                    if (_CONGIF_ARRAY[1].Trim() != "" && _CONGIF_ARRAY[2].Trim() == "Blender:True" && _CONGIF_ARRAY[3].Trim() == "CSURMaster:True" && _CONGIF_ARRAY[4].Trim() == "TexturesPack:True" && _CONGIF_ARRAY[0].Trim() != "")
                    {
                        await EvaluateJavaScriptAsync(@"Check_noti_group()");
                        CSUR_Generator_Var.CanStart = true;
                    }
                    else
                    {
                        await EvaluateJavaScriptAsync(@"Before_error()");
                    }
                });
                return JavaScriptValue.CreateString("Checked");
        }))***/
            //RegisterExternalObjectValue("BeforeCheck",b_Check);
            if (CSUR_Generator_Var.CanStart == true)
            {

            }
            else
            {
                ExecuteJavaScript(@"Before_error()");
            }
        }
        private void GetProcessBar(object source, ElapsedEventArgs e)
        {
            Bar = CSUR_Generator_Var.Bar;
            ExecuteJavaScript(@"document.getElementById('processbar').style.width =  '" + Bar + "%';");
            if (Bar == "100")
            {
                getprocess.Stop();
            }
            else if (_expecterror == true)
            {
                getprocess.Stop();
                ExecuteJavaScript(@"document.getElementById('processbar').style.width =  '0%';");
            }
        }

        private async void get_install_envir_dll(object source, ElapsedEventArgs e)
        {
            if (Installation._instal_callback_mod != "" && Installation._instal_callback_blender != "" && Installation._instal_callback_texture != "")
            {
                if(Installation._install_status_mod == true)
                {
                    await EvaluateJavaScriptAsync(@"mod_instal_success()");
                }
                if(Installation._install_status_blender == true)
                {
                    await EvaluateJavaScriptAsync(@"blender_instal_success()");
                }
                if(Installation._install_status_texture == true)
                {
                    await EvaluateJavaScriptAsync(@"texture_instal_success()");
                }
                if(Installation._install_status_mod == true && Installation._install_status_blender == true && Installation._install_status_texture == true)
                {
                    await EvaluateJavaScriptAsync(@"load_modal_close()");
                }
                if (Installation._install_status_mod == false)
                {
                    await EvaluateJavaScriptAsync(@"mod_instal_error('" + Installation._instal_callback_mod + "')");
                    await EvaluateJavaScriptAsync(@"document.getElementById('RI_Mods').innerHTML = 'Install Failed';");
                }
                if (Installation._install_status_blender == false)
                {
                    await EvaluateJavaScriptAsync(@"blender_instal_error('" + Installation._instal_callback_blender + "')");
                    await EvaluateJavaScriptAsync(@"document.getElementById('Blender_in').innerHTML = 'Install Failed';");
                }
                if (Installation._install_status_texture == false)
                {
                    await EvaluateJavaScriptAsync(@"blender_instal_error('" + Installation._instal_callback_texture + "')");
                    await EvaluateJavaScriptAsync(@"document.getElementById('TP_iNS').innerHTML = 'Install Failed';");
                }
                getInsall_envir_dll.Stop();
            }
        }
        private void Generator()
        {
            var Roadname_obj = JavaScriptValue.CreateObject();

            

            Roadname_obj.SetValue("Express",JavaScriptValue.CreateProperty(()=>JavaScriptValue.CreateString(CSUR_Generator_Var.Express)));
            Roadname_obj.SetValue("Compress", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateString(CSUR_Generator_Var.Compress)));
            Roadname_obj.SetValue("Filp", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateString(CSUR_Generator_Var.Filp)));

            Roadname_obj.SetValue("Roadname", JavaScriptValue.CreateFunction(args =>
             {
                 var _get_n = args[0].GetString();
                 if(_get_n != "" && _get_n != null)
                 {
                     CSUR_Generator_Var.RoadeCode = _get_n;

                     CSUR_Generator_Var.CanStart = true;

                     return JavaScriptValue.CreateString(_get_n);
                 }
                 return JavaScriptValue.CreateString("failed");
             }));


            RegisterExternalObjectValue("Roadinfo", Roadname_obj);

            var Generated = JavaScriptValue.CreateObject();
            Generated.SetValue("Generat", JavaScriptValue.CreateFunction(args =>
             {
                
                 InvokeIfRequired(() =>
                 {
                     getprocess.Interval = 300;//0.3秒获取一次进度
                     getprocess.Elapsed += new ElapsedEventHandler(GetProcessBar); // 到时间后执行
                     getprocess.AutoReset = true; // 是否一直执行
                     getprocess.Enabled = true;
                     getprocess.Start();
                     Bar = "0";
                     if(CSUR_Generator_Var.CanStart == true)
                     {
                         if (CSUR_Generator_Var.RoadeCode != "" && CSUR_Generator_Var.RoadeCode != null)
                         {
                             //ExecuteJavaScript(@"document.getElementsById('Gen_Start_1').className += 'spinner - border text - light';");
                             bool _code_check = CSUR_Generator_Var.RoadeCode.Contains(",");//检查是否是多个模块
                             if (_code_check == true)
                             {
                                 string _code_result = CSUR_Generator_Var.RoadeCode.Replace(",", " ");//将分隔符替换为空格

                                  _Generat_result = Generator_Program.Generator_S(_code_result);//传参到生成器

                                 Console.WriteLine(_Generat_result);

                                 if (_Generat_result == "1 True")//传参到生成器
                                 {
                                     ExecuteJavaScript(@"Generator_success()");
                                     Bar = "100";
                                     CSUR_Generator_Var.CanStart = false;
                                 }
                                 else if (_Generat_result == "false \n output failed")
                                 {
                                     Console.WriteLine("false \n output failed");
                                     ExecuteJavaScript(@"output_error()");
                                     _expecterror = true;
                                     CSUR_Generator_Var.CanStart = false;
                                 }
                                 else if (_Generat_result == "failed \n Generator error")
                                 {
                                     Console.WriteLine("failed \n Generator error");
                                     ExecuteJavaScript(@"Generator_error()");
                                     _expecterror = true;
                                     CSUR_Generator_Var.CanStart = false;
                                 }
                                 else if (_Generat_result == "Roadcode error")
                                 {
                                     Console.WriteLine("Roadcode error");
                                     ExecuteJavaScript(@"Roadcode_error()");
                                     _expecterror = true;
                                     CSUR_Generator_Var.CanStart = false;
                                 }
                                 ///注意
                                 ///从界面返回的代码必须是经过处理的，含有道路标识的道路模块名称
                                 ///注意
                             }
                             else
                             {
                                 _Generat_result = Generator_Program.Generator_S(CSUR_Generator_Var.RoadeCode);
                                 if (_Generat_result == "1 True")//传参到生成器
                                 {
                                     ExecuteJavaScript(@"Generator_success()");
                                     CSUR_Generator_Var.CanStart = false;
                                 }
                                 else if (_Generat_result == "false \n output failed")
                                 {
                                     Console.WriteLine("false \n output failed");
                                     ExecuteJavaScript(@"output_error()");
                                     CSUR_Generator_Var.CanStart = false;
                                 }
                                 else if (_Generat_result == "failed \n Generator error")
                                 {
                                     Console.WriteLine("failed \n Generator error");
                                     ExecuteJavaScript(@"Generator_error()");
                                     CSUR_Generator_Var.CanStart = false;
                                 }
                                 else if (_Generat_result == "Roadcode error")
                                 {
                                     Console.WriteLine("Roadcode error");
                                     ExecuteJavaScript(@"Roadcode_error()");
                                     CSUR_Generator_Var.CanStart = false;
                                 }
                             }
                         }
                         else
                         {
                             Console.WriteLine("RoadCode empty");
                             ExecuteJavaScript(@"Generator_error()");
                             CSUR_Generator_Var.CanStart = false;
                         }
                     }
                     else
                     {
                         ExecuteJavaScript(@"Start_error()");
                     }
                 });

                 return JavaScriptValue.CreateString("OK");
             }));
            RegisterExternalObjectValue("Generat", Generated);
        }

        private void Install_envir()
        {
            var envir_install = JavaScriptValue.CreateObject();

            envir_install.SetValue("installation",JavaScriptValue.CreateFunction(args => 
            {
                InvokeIfRequired(async() => 
                {
                    //getInsall_envir_dll.Interval = 300;
                    ///getInsall_envir_dll.Elapsed += new ElapsedEventHandler(get_install_envir_dll);
                    //getInsall_envir_dll.AutoReset = true;
                    //getInsall_envir_dll.Enabled = false;
                    //getInsall_envir_dll.Start();
                    try
                    {
                        Thread t1_instal;
                        t1_instal = new Thread(Installation._install_all);
                        t1_instal.Start();
                        //t1_instal.Join();
                        getInsall_envir_dll.Interval = 1100;
                        getInsall_envir_dll.Elapsed += new ElapsedEventHandler(get_install_envir_dll);
                        getInsall_envir_dll.AutoReset = true;
                        getInsall_envir_dll.Enabled = true;
                    }
                    catch(ApplicationException e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                });
                return JavaScriptValue.CreateString("OK");
            }));
            RegisterExternalObjectValue("Envir_Installation", envir_install);
        }
    }
}
