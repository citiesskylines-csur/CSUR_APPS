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

namespace CSUR.Apps
{
    class Main : Formium
    {
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
            //ShowDevTools();
        }

        private void LoginmodeCheck()
        {

        }

        private void Envirment_Check()
        {
            var Envir = JavaScriptValue.CreateObject();

            var StartCheck = JavaScriptValue.CreateObject();

            StartCheck.SetValue("StartCheck", JavaScriptValue.CreateFunction(args =>
            {
                var msg = args.FirstOrDefault(x => x.IsString);

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
                            if (Common.getRoadimporter_Mods() != "" || Common.getRoadimporter_Mods() != null)//检查Addons下的Roadimporter的MOD是否存在
                            {
                                Envir.SetValue("RI_Mods", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateString(Common_Var.Roadimporter_Mods)));
                                await EvaluateJavaScriptAsync("document.getElementById('RI_Mods').innerHTML = 'Installed';");
                                await EvaluateJavaScriptAsync("document.getElementById('RI_Mods').style.color =  '#16E15E';");
                                if (Common.checkBlender() == true)//检查Blender是否安装
                                {
                                    Envir.SetValue("Blender", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateBool(true)));
                                    await EvaluateJavaScriptAsync ("document.getElementById('Blender_in').innerHTML = 'Installed';");//Blender_in
                                    await EvaluateJavaScriptAsync ("document.getElementById('Blender_in').style.color =  '#16E15E';");
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
                                    ExecuteJavaScript(@"Blender_error()");
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
        private void Generator()
        {
            var Roadname_obj = JavaScriptValue.CreateObject();

            Roadname_obj.SetValue("Express",JavaScriptValue.CreateProperty(()=>JavaScriptValue.CreateString(CSUR_Generator_Var.Express)));
            Roadname_obj.SetValue("Compress", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateString(CSUR_Generator_Var.Compress)));
            Roadname_obj.SetValue("Filp", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateString(CSUR_Generator_Var.Filp)));
            Roadname_obj.SetValue("Roadcode", JavaScriptValue.CreateProperty(() => JavaScriptValue.CreateString(CSUR_Generator_Var.RoadeCode)));

            RegisterExternalObjectValue("Roadinfo", Roadname_obj);

            var Generated = JavaScriptValue.CreateObject();
            Generated.SetValue("StartGenerat", JavaScriptValue.CreateFunction(args =>
             {
                 var msg = args.FirstOrDefault(x => x.IsString);

                 var text = msg?.GetString();

                 InvokeIfRequired(() =>
                 {
                     if(CSUR_Generator_Var.RoadeCode != "" && CSUR_Generator_Var.RoadeCode != null)
                     {
                         bool _code_check = CSUR_Generator_Var.RoadeCode.Contains(",");//检查是否是多个模块
                         if (_code_check == true)
                         {
                             string _code_result= CSUR_Generator_Var.RoadeCode.Replace(","," ");//将分隔符替换为空格
                             if (Generator_Program.Generator_S(_code_result) == true)//传参到生成器
                             {
                                 ExecuteJavaScript(@"Generator_success()");
                             }
                             else
                             {
                                 ExecuteJavaScript(@"Generator_error()");
                             }
                             ///注意
                             ///从界面返回的代码必须是经过处理的，含有道路标识的道路模块名称
                             ///注意
                         }
                         else
                         {

                         }
                     }
                 });

                 return JavaScriptValue.CreateString(text);
             }));

            Generated.SetValue("asyncmethod", JavaScriptValue.CreateFunction((args, callback) =>
            {
                Task.Run(async () =>
                {
                    var rnd = new Random(DateTime.Now.Millisecond);

                    var rndValue = rnd.Next(3000, 6000);

                    await Task.Delay(rndValue);
                    callback.Success(JavaScriptValue.CreateString($"Delayed {rndValue} milliseconds"));
                });
            }));
            RegisterExternalObjectValue("Generator", Generated);
        }
    }
}
