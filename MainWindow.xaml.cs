using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Navigation;

namespace _2019HSQXSJK
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RadWindow
    {
        System.Timers.Timer timer = new System.Timers.Timer(60000);
        string adminCodes = "";
        StringToDisplay callDuration = new StringToDisplay();
        public MainWindow()
        {

            try
            {
                LocalizationManager.Manager = new LocalizationManager()
                {
                    ResourceManager = GridViewResources.ResourceManager
                };
                InitializeComponent();
                t1.DataContext = callDuration;
                Settheme settheme1 = new Settheme();
                StyleManager.SetTheme(this, GetMyTheme("Crystal"));
                settheme1.setTheme(settheme1.setLightOrDark("Crystal"));
                StyleManager.ApplicationTheme = GetMyTheme("Crystal");
                timer.Elapsed += new System.Timers.ElapsedEventHandler(refreshTime);
                timer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；  
                timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                数据库处理 sjkcl = new 数据库处理();
                adminCodes = sjkcl.获取实况站点范围(0, 0, 0);
                callDuration.Text= $"{DateTime.Now}  启动呼和浩特市气象资料数据库";
                
            }
            catch(Exception e)
            {
                callDuration.Text = $"{DateTime.Now}  启动失败:{e.Message}";
                
            }

            //rkcs();


        }
        public void refreshTime(object source, System.Timers.ElapsedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                DateTime dateTime1 = DateTime.Now;
                string strLS=PreMinuteRk(dateTime1);
                strLS= TEMMinuteRk(dateTime1)+ strLS;
                strLS = PRSMinuteRk(dateTime1) + strLS;
                strLS = RHUMinuteRk(dateTime1) + strLS;
                if(strLS.Trim().Length>0)
                {
                    callDuration.Text = strLS + callDuration.Text;
                }
            });

            DateTime dateTime = DateTime.Now;
            if (dateTime.Minute %5==1)
            {
                Task.Factory.StartNew(() =>
                {
                    string[] szLS = callDuration.Text.Split('\n');
                    if(szLS.Length>=3000)
                    {
                        string sls = "";
                        for(int i=0;i<2000;i++)
                        {
                            sls += szLS[i] + '\n';
                        }
                        callDuration.Text = sls;
                    }
                    DateTime dateTime1 = DateTime.Now;
                    string strLS = TEMMinuteRk_5minutes(dateTime1);
                    strLS = PRSMinuteRk_5minutes(dateTime1) + strLS;
                    strLS = RHUMinuteRk_5minutes(dateTime1) + strLS;
                    strLS = WindMinuteRk_5minutes(dateTime1) + strLS;
                    strLS = OtherMinuteRk_5minutes(dateTime1) + strLS;
                    if (strLS.Trim().Length > 0)
                    {
                        callDuration.Text = strLS + callDuration.Text;
                    }

                });


            }
            if(dateTime.Minute==5|| dateTime.Minute == 10)
            {
                Task.Factory.StartNew(() =>
                {
                    DateTime dateTime1 = DateTime.Now;
                    string strLS = HourSK(dateTime1);
                    if (strLS.Trim().Length > 0)
                    {
                        callDuration.Text = strLS + callDuration.Text;
                    }
                });
            }
                //每天00:21恢复前三天的数据
            if (dateTime.Hour==0&& dateTime.Minute==21)
            {
                Task.Factory.StartNew(() =>
                {
                   // HFRk();
                });
            }
        }

        
        public void cs()
        {

            数据库处理 sjkcl = new 数据库处理();
            DateTime dateTime = Convert.ToDateTime("2019-08-20 12:00:00");
            while (dateTime.CompareTo(DateTime.Now) <= 0)
            {
                string str = sjkcl.分钟降水量入库(dateTime.AddMinutes(-4), dateTime);
                dateTime = dateTime.AddMinutes(5);
                callDuration.Text = str + callDuration.Text;

            }
        }
        public void temcs()
        {
            数据库处理 sjkcl = new 数据库处理();
            DateTime dateTime = Convert.ToDateTime("2019-08-08 23:00:00");
            while (dateTime.CompareTo(DateTime.Now) <= 0)
            {
                string str = sjkcl.分钟常规温度入库(dateTime.AddMinutes(-9), dateTime);
                sjkcl.分钟其他温度入库(dateTime.AddMinutes(-9), dateTime);
                dateTime = dateTime.AddMinutes(10);
                callDuration.Text = str + callDuration.Text;
                Thread.Sleep(100);

            }
        }
        public void PRScs()
        {
            数据库处理 sjkcl = new 数据库处理();
            DateTime dateTime = Convert.ToDateTime("2019-07-01 00:00:00");
            while (dateTime.CompareTo(DateTime.Now) <= 0)
            {
                string str = sjkcl.分钟常规气压入库(dateTime.AddMinutes(-9), dateTime);
                sjkcl.分钟其他气压入库(dateTime.AddMinutes(-9), dateTime);
                dateTime = dateTime.AddMinutes(10);
                callDuration.Text = str + callDuration.Text;
                Thread.Sleep(100);
            }
        }
        public void Windcs()
        {
            数据库处理 sjkcl = new 数据库处理();
            DateTime dateTime = Convert.ToDateTime("2019-07-01 00:00:00");
            while (dateTime.CompareTo(DateTime.Now) <= 0)
            {
                string str = sjkcl.分钟其他风入库(dateTime.AddMinutes(-9), dateTime);
                dateTime = dateTime.AddMinutes(10);
                callDuration.Text = str + callDuration.Text;
                Thread.Sleep(100);
            }
        }
        public void Othercs()
        {
            数据库处理 sjkcl = new 数据库处理();
            DateTime dateTime = Convert.ToDateTime("2019-07-01 00:00:00");
            while (dateTime.CompareTo(DateTime.Now) <= 0)
            {
                string str = sjkcl.分钟其他资料入库(dateTime.AddMinutes(-9), dateTime);
                dateTime = dateTime.AddMinutes(10);
                callDuration.Text = str + callDuration.Text;
                Thread.Sleep(100);
            }
        }
        public void RHUcs()
        {
            数据库处理 sjkcl = new 数据库处理();
            DateTime dateTime = Convert.ToDateTime("2019-07-01 00:00:00");
            while (dateTime.CompareTo(DateTime.Now) <= 0)
            {
                callDuration.Text = sjkcl.分钟常规湿度入库(dateTime.AddMinutes(-9), dateTime) + callDuration.Text;
                callDuration.Text = sjkcl.分钟其他湿度入库(dateTime.AddMinutes(-9), dateTime) + callDuration.Text;
                dateTime = dateTime.AddMinutes(10);
                Thread.Sleep(100);
            }
        }
        public string PreMinuteRk(DateTime dateTime)
        {
           
            try
            {
                数据库处理 sjkcl = new 数据库处理(adminCodes);
                return sjkcl.分钟降水量入库(dateTime.AddMinutes(-4), dateTime);
            }
            catch
            {
                return "";
            }
        }
        public string TEMMinuteRk(DateTime dateTime)
        {
            try
            {
                数据库处理 sjkcl = new 数据库处理(adminCodes);
                return sjkcl.分钟常规温度入库(dateTime.AddMinutes(-5), dateTime);
            }
            catch
            {
                return "";
            }
        }
        public string TEMMinuteRk_5minutes(DateTime dateTime)
        {
            try
            {
                数据库处理 sjkcl = new 数据库处理(adminCodes);
                return sjkcl.分钟其他温度入库(dateTime.AddMinutes(-11), dateTime);
            }
            catch
            {
                return "";
            }
           
        }
        public string RHUMinuteRk(DateTime dateTime)
        {
            try
            {
                数据库处理 sjkcl = new 数据库处理(adminCodes);
                return sjkcl.分钟常规湿度入库(dateTime.AddMinutes(-5), dateTime);
            }
            catch 
            {
                return "";
            }
        }
        public string RHUMinuteRk_5minutes(DateTime dateTime)
        {
            try
            {
                数据库处理 sjkcl = new 数据库处理(adminCodes);
                return sjkcl.分钟其他湿度入库(dateTime.AddMinutes(-11), dateTime);
            }
            catch
            {
                return "";
            }

        }
        public string PRSMinuteRk(DateTime dateTime)
        {
            try
            {
                数据库处理 sjkcl = new 数据库处理(adminCodes);
                return sjkcl.分钟常规气压入库(dateTime.AddMinutes(-5), dateTime);
            }
            catch
            {
                return "";
            }


        }
        public string PRSMinuteRk_5minutes(DateTime dateTime)
        {
            try
            {
                数据库处理 sjkcl = new 数据库处理(adminCodes);
                return sjkcl.分钟其他气压入库(dateTime.AddMinutes(-11), dateTime);
            }
            catch
            {
                return "";
            }

        }
        public string WindMinuteRk_5minutes(DateTime dateTime)
        {
            try
            {
                数据库处理 sjkcl = new 数据库处理(adminCodes);
                return sjkcl.分钟其他风入库(dateTime.AddMinutes(-11), dateTime);
            }
            catch
            {
                return "";
            }

        }
        public string OtherMinuteRk_5minutes(DateTime dateTime)
        {
            try
            {
                数据库处理 sjkcl = new 数据库处理(adminCodes);
                return sjkcl.分钟其他资料入库(dateTime.AddMinutes(-11), dateTime);
            }
            catch
            {
                return "";
            }
        }
        private string HourSK(DateTime dateTime)
        {
            try
            {
                数据库处理 sjkcl = new 数据库处理(adminCodes);
               
                string strfh = "";
                try
                {
                    strfh=sjkcl.小时降水量入库(dateTime.AddHours(-2), dateTime)+ strfh;
                }
                catch { }
                try
                {
                    strfh = sjkcl.小时温度入库(dateTime.AddHours(-2), dateTime) + strfh;
                }
                catch { }
                try
                {
                    strfh = sjkcl.小时气压入库(dateTime.AddHours(-2), dateTime) + strfh;
                }
                catch { }
                try
                {
                    strfh = sjkcl.小时湿度入库(dateTime.AddHours(-2), dateTime) + strfh;
                }
                catch { }
                try
                {
                    strfh = sjkcl.小时风入库(dateTime.AddHours(-2), dateTime) + strfh;
                }
                catch { }

                try
                {
                    strfh = sjkcl.小时其他资料入库(dateTime.AddHours(-2), dateTime) + strfh;
                }
                catch { }

                return strfh;
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// 恢复前三天的数据
        /// </summary>
        public void HFRk()
        {
            数据库处理 sjkcl = new 数据库处理(adminCodes);
            DateTime sdate = DateTime.Now.Date.AddDays(-3), edate = DateTime.Now.Date;
            DateTime dateTime = sdate;
            while (dateTime.CompareTo(edate) <= 0)
            {
                try
                {
                    sjkcl.分钟降水量入库(dateTime.AddMinutes(-9), dateTime);
                    sjkcl.分钟常规温度入库(dateTime.AddMinutes(-9), dateTime);
                    sjkcl.分钟其他温度入库(dateTime.AddMinutes(-9), dateTime);
                    sjkcl.分钟常规气压入库(dateTime.AddMinutes(-9), dateTime);
                    sjkcl.分钟其他气压入库(dateTime.AddMinutes(-9), dateTime);
                    sjkcl.分钟其他风入库(dateTime.AddMinutes(-9), dateTime);
                    sjkcl.分钟常规湿度入库(dateTime.AddMinutes(-9), dateTime);
                    sjkcl.分钟其他湿度入库(dateTime.AddMinutes(-9), dateTime);
                    sjkcl.分钟其他资料入库(dateTime.AddMinutes(-9), dateTime);
                    dateTime = dateTime.AddMinutes(10);
                }
                catch
                {
                }
            }
        }
        /// <summary>
        /// 重载OnClosing()函数实现--使用户无法通过点击右上角的关闭按钮来关闭窗口
        /// </summary>
        /// <param name="e"></param>
        //protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        //{
        //    e.Cancel = true;
        //}

        private void T1_TextChanged_1(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                t1.TextChanged -= T1_TextChanged_1;
                string[] szls = t1.Text.Split('\n');
                if (szls.Length > 50000)
                {
                    string data = "";
                    for (int i = 0; i < 40000; i++)
                    {
                        data += szls[40000 - i] + '\n';
                    }
                    t1.Dispatcher.BeginInvoke(
                           new Action(
                               delegate
                               {
                                   t1.Text = data;
                                   //将光标移至文本框最后
                                   t1.Focus();
                                   t1.CaretIndex = (t1.Text.Length);
                               }
                           ));
                }

            }
            catch
            {
                t1.Clear();
            }
            finally
            {
                t1.TextChanged += T1_TextChanged_1;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(cs);
            thread.Start();
        }
        public Theme GetMyTheme(string name)
        {
            string myName = name.ToLower();
            if (myName.Contains("crystal"))
            {
                return new CrystalTheme();
            }
            else if (myName.Contains("fluent"))
            {
                return new FluentTheme();
            }
            else if (myName.Contains("material"))
            {
                return new MaterialTheme();
            }
            else if (myName.Contains("office2016touch"))
            {
                return new Office2016TouchTheme();
            }
            else if (myName.Contains("office2016"))
            {
                return new Office2016Theme();
            }
            else if (myName.Contains("green"))
            {
                return new GreenTheme();
            }
            else if (myName.Contains("office2013"))
            {
                return new Office2013Theme();
            }
            else if (myName.Contains("visualstudio2013"))
            {
                return new VisualStudio2013Theme();
            }
            else if (myName.Contains("windows8touch"))
            {
                return new Windows8TouchTheme();
            }
            else if (myName.Contains("windows8"))
            {
                return new Windows8Theme();
            }
            else if (myName.Contains("office_black"))
            {
                return new Office_BlackTheme();
            }
            else if (myName.Contains("office_blue"))
            {
                return new Office_BlueTheme();
            }
            else if (myName.Contains("office_silver"))
            {
                return new Office_SilverTheme();
            }
            else if (myName.Contains("summer"))
            {
                return new SummerTheme();
            }
            else if (myName.Contains("vista"))
            {
                return new VistaTheme();
            }
            else if (myName.Contains("transparent"))
            {
                return new TransparentTheme();
            }
            else if (myName.Contains("windows7"))
            {
                return new Windows7Theme();
            }
            else if (myName.Contains("expression_dark"))
            {
                return new Expression_DarkTheme();
            }
            return new CrystalTheme();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(temcs);
            thread.Start();
           
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                PRScs();
            });
        }

        private void RadButton_Click_2(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Windcs();
            });
        }

        private void RadButton_Click_3(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                RHUcs();
            });
            
        }

        private void RadButton_Click_4(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Othercs();
            });
            
        }

        private void RadButton_Click_5(object sender, RoutedEventArgs e)
        {
            RadWindow settingsDialog = new RadWindow();
            settingsDialog.Content = new 数据恢复();
            settingsDialog.ResizeMode = ResizeMode.CanResize;
            settingsDialog.Header = "数据恢复";
            settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settingsDialog.HideMinimizeButton = false;
            settingsDialog.HideMaximizeButton = false;
            settingsDialog.CanClose = true;
            settingsDialog.Show();
            RadWindowInteropHelper.SetShowInTaskbar(settingsDialog, true);
            //Task.Factory.StartNew(() =>
            //{
            //    DateTime dateTime = DateTime.Now;
            //    string strLS=HourSK(dateTime);
            //    if (strLS.Trim().Length > 0)
            //    {
            //        callDuration.Text = strLS + callDuration.Text;
            //    }
            //});

            //string myData = cIMISS.CIMISS_SK_Hour_byTimeRangeAndRegion_SURF_CHN_MUL_HOR(DateTime.Now.AddHours(-2), DateTime.Now, adminCodes, "EVP_Big,GST,GST_Max,GST_Max_Otime,GST_Min,GST_Min_OTime,GST_5cm,GST_10cm,GST_15cm,GST_20cm,GST_40Cm,GST_80cm,GST_160cm,GST_320cm,LGST,LGST_Max,LGST_Max_OTime,LGST_Min,LGST_Min_OTime,VIS_HOR_1MI,VIS_HOR_10MI,VIS_Min,VIS_Min_OTime,VIS,WEP_Now");
        }
    }
}
