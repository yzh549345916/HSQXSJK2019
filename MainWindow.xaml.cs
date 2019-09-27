using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace _2019HSQXSJK
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RadWindow
    {
        System.Timers.Timer timer = new System.Timers.Timer(60000);
        string adminCodes = "";
        public MainWindow()
        {

            try
            {
                LocalizationManager.Manager = new LocalizationManager()
                {
                    ResourceManager = GridViewResources.ResourceManager
                };
                InitializeComponent();

                Settheme settheme1 = new Settheme();
                StyleManager.SetTheme(this, GetMyTheme("Crystal"));
                settheme1.setTheme(settheme1.setLightOrDark("Crystal"));
                StyleManager.ApplicationTheme = GetMyTheme("Crystal");
                timer.Elapsed += new System.Timers.ElapsedEventHandler(refreshTime);
                timer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；  
                timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                数据库处理 sjkcl = new 数据库处理();
                adminCodes = sjkcl.获取实况站点范围(0, 0, 0);
                t1.Dispatcher.BeginInvoke(new Action(delegate
                {
                    t1.Text += $"{DateTime.Now}  启动呼和浩特市气象资料数据库\n";
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }), DispatcherPriority.Normal);
            }
            catch(Exception e)
            {
                t1.Dispatcher.BeginInvoke(new Action(delegate
                {
                    t1.Text += $"{DateTime.Now}  启动失败:{e.Message}\n";
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }), DispatcherPriority.Normal);
            }

            //rkcs();


        }
        public void refreshTime(object source, System.Timers.ElapsedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                PreMinuteRk();
                
            });
            Task.Factory.StartNew(() =>
            {
                TEMMinuteRk();

            });
            Task.Factory.StartNew(() =>
            {
                PRSMinuteRk();

            });
            DateTime dateTime = DateTime.Now;
            if (dateTime.Minute %5==1)
            {
                Task.Factory.StartNew(() =>
                {
                    TEMMinuteRk_5minutes();
                    PRSMinuteRk_5minutes();
                });
            }
                //每天00:21恢复前三天的数据
            if (dateTime.Hour==0&& dateTime.Minute==21)
            {
                Task.Factory.StartNew(() =>
                {
                    HFRk();
                });
            }
        }

        
        public void cs()
        {
            数据库处理 sjkcl = new 数据库处理();
            DateTime dateTime = Convert.ToDateTime("2019-09-27 00:20:00");
            while (dateTime.CompareTo(DateTime.Now) <= 0)
            {
                string str = sjkcl.分钟降水量入库(dateTime.AddMinutes(-4), dateTime);
                dateTime = dateTime.AddMinutes(5);
                t1.Dispatcher.BeginInvoke(new Action(delegate
                {
                    t1.Text += str;
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }), DispatcherPriority.Normal);
            }
        }
        public void temcs()
        {
            数据库处理 sjkcl = new 数据库处理();
            DateTime dateTime = Convert.ToDateTime("2019-07-05 13:40:00");
            while (dateTime.CompareTo(DateTime.Now) <= 0)
            {
                string str = sjkcl.分钟常规温度入库(dateTime.AddMinutes(-9), dateTime);
                sjkcl.分钟其他温度入库(dateTime.AddMinutes(-9), dateTime);
                dateTime = dateTime.AddMinutes(10);
                t1.Dispatcher.BeginInvoke(new Action(delegate
                {
                    t1.Text += str;
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }), DispatcherPriority.Normal);
            }
        }
        public void PRScs()
        {
            数据库处理 sjkcl = new 数据库处理();
            DateTime dateTime = Convert.ToDateTime("2019-07-01 10:00:00");
            while (dateTime.CompareTo(DateTime.Now) <= 0)
            {
                string str = sjkcl.分钟常规气压入库(dateTime.AddMinutes(-9), dateTime);
                sjkcl.分钟其他气压入库(dateTime.AddMinutes(-9), dateTime);
                dateTime = dateTime.AddMinutes(10);
                t1.Dispatcher.BeginInvoke(new Action(delegate
                {
                    t1.Text += str;
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }), DispatcherPriority.Normal);
            }
        }
        public void PreMinuteRk()
        {
            数据库处理 sjkcl = new 数据库处理(adminCodes);
            string str = sjkcl.分钟降水量入库(DateTime.Now.AddMinutes(-4), DateTime.Now);
            if(str.Length>0)
            {
                t1.Dispatcher.BeginInvoke(new Action(delegate
                {
                    t1.Text += str;
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }), DispatcherPriority.Normal);
            }
        }
        public void TEMMinuteRk()
        {
            数据库处理 sjkcl = new 数据库处理(adminCodes);
            string str = sjkcl.分钟常规温度入库(DateTime.Now.AddMinutes(-5), DateTime.Now);
            
            if (str.Length > 0)
            {
                t1.Dispatcher.BeginInvoke(new Action(delegate
                {
                    t1.Text += str;
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }), DispatcherPriority.Normal);
            }
        }
        public void TEMMinuteRk_5minutes()
        {
            数据库处理 sjkcl = new 数据库处理(adminCodes);
            string str=sjkcl.分钟其他温度入库(DateTime.Now.AddMinutes(-11), DateTime.Now);
            if (str.Length > 0)
            {
                t1.Dispatcher.BeginInvoke(new Action(delegate
                {
                    t1.Text += str;
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }), DispatcherPriority.Normal);
            }
        }
        public void PRSMinuteRk()
        {
            数据库处理 sjkcl = new 数据库处理(adminCodes);
            string str = sjkcl.分钟常规气压入库(DateTime.Now.AddMinutes(-5), DateTime.Now);

            if (str.Length > 0)
            {
                t1.Dispatcher.BeginInvoke(new Action(delegate
                {
                    t1.Text += str;
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }), DispatcherPriority.Normal);
            }
        }
        public void PRSMinuteRk_5minutes()
        {
            数据库处理 sjkcl = new 数据库处理(adminCodes);
            string str = sjkcl.分钟其他气压入库(DateTime.Now.AddMinutes(-11), DateTime.Now);
            if (str.Length > 0)
            {
                t1.Dispatcher.BeginInvoke(new Action(delegate
                {
                    t1.Text += str;
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }), DispatcherPriority.Normal);
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
            Task.Factory.StartNew(() =>
            {
                cs();
            });
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
            Task.Factory.StartNew(() =>
            {
                temcs();
            });
            
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                PRScs();
            });
        }
    }
}
