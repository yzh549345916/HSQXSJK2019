using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Navigation;
using Timer = System.Timers.Timer;

namespace _2019HSQXSJK
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RadWindow
    {
        private Timer timer = new Timer(60000);
        private string adminCodes = "";
        private StringToDisplay callDuration = new StringToDisplay();
        private bool sfrkbs = true, znsfrkbs = true;

        public MainWindow()
        {
            try
            {
                LocalizationManager.Manager = new LocalizationManager
                {
                    ResourceManager = GridViewResources.ResourceManager
                };
                InitializeComponent();
                t1.DataContext = callDuration;
                Settheme settheme1 = new Settheme();
                StyleManager.SetTheme(this, GetMyTheme("Crystal"));
                settheme1.setTheme(settheme1.setLightOrDark("Crystal"));
                StyleManager.ApplicationTheme = GetMyTheme("Crystal");
                timer.Elapsed += refreshTime;
                timer.AutoReset = true; //设置是执行一次（false）还是一直执行(true)；  
                timer.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件；
                数据库处理 sjkcl = new 数据库处理();
                adminCodes = sjkcl.获取实况站点范围(0, 0, 0);
                callDuration.Text = $"{DateTime.Now}  启动呼和浩特市气象资料数据库";
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\基本信息.xml");
                bool sfrk = true;
                bool znls = true;
                try
                {
                    sfrk = Convert.ToBoolean(util.Read("SFRK"));
                    sfrkbs = sfrk;
                    znls = Convert.ToBoolean(util.Read("ZNSFRK"));
                    znsfrkbs = znls;
                }
                catch
                {
                }

                switch1.IsChecked = sfrk;
                switch2.IsChecked = znls;
                switch1.Content = sfrk ? "是" : "否";
                switch2.Content = znls ? "是" : "否";
            }
            catch (Exception e)
            {
                callDuration.Text = $"{DateTime.Now}  启动失败:{e.Message}";
            }
        }

        public void refreshTime(object source, ElapsedEventArgs e)
        {
            DateTime dateTimenow = DateTime.Now;
            if (sfrkbs)
            {
                Task.Factory.StartNew(() =>
                {
                    DateTime dateTime1 = DateTime.Now;
                    string strLS = PreMinuteRk(dateTime1);
                    strLS = TEMMinuteRk(dateTime1) + strLS;
                    strLS = PRSMinuteRk(dateTime1) + strLS;
                    strLS = RHUMinuteRk(dateTime1) + strLS;
                    if (strLS.Trim().Length > 0)
                    {
                        callDuration.Text = strLS + callDuration.Text;
                    }
                });

                DateTime dateTime = DateTime.Now;
                if (dateTime.Minute % 5 == 1)
                {
                    Task.Factory.StartNew(() =>
                    {
                        string[] szLS = callDuration.Text.Split('\n');
                        if (szLS.Length >= 3000)
                        {
                            string sls = "";
                            for (int i = 0; i < 2000; i++)
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

                if (dateTime.Minute == 5 || dateTime.Minute == 10)
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

                
            }
            if(znsfrkbs)
            {
                //每天00:21恢复前三天的数据
                if (dateTimenow.Hour == 0 && dateTimenow.Minute == 21)
                {
                    Task.Factory.StartNew(() =>
                    {
                         HFRk();
                    });
                }
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
                    strfh = sjkcl.小时降水量入库(dateTime.AddHours(-2), dateTime) + strfh;
                }
                catch
                {
                }

                try
                {
                    strfh = sjkcl.小时温度入库(dateTime.AddHours(-2), dateTime) + strfh;
                }
                catch
                {
                }

                try
                {
                    strfh = sjkcl.小时气压入库(dateTime.AddHours(-2), dateTime) + strfh;
                }
                catch
                {
                }

                try
                {
                    strfh = sjkcl.小时湿度入库(dateTime.AddHours(-2), dateTime) + strfh;
                }
                catch
                {
                }

                try
                {
                    strfh = sjkcl.小时风入库(dateTime.AddHours(-2), dateTime) + strfh;
                }
                catch
                {
                }

                try
                {
                    strfh = sjkcl.小时其他资料入库(dateTime.AddHours(-2), dateTime) + strfh;
                }
                catch
                {
                }

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
            DateTime sDateTime = DateTime.Now.Date.AddDays(-3), eDateTime = DateTime.Now.Date;
            ObservableCollection<资料种类> sjbg = sjkcl.获取数据库表信息();
            foreach (资料种类 item in sjbg)
            {
                if (item.ID == 1)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddMinutes(9);
                        int hour = -1;
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            if (myhour != hour)
                            {
                                hour = myhour;
                                List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour), dateTime.Date.AddHours(myhour));
                                List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                                if (mylists1.Count == 0 || mylists2.Count == 0)
                                {
                                    rkbs = true;
                                }
                                else if (mylists1[0].个数 <= 1 || mylists1[0].个数 < mylists2[0].个数 / 2)
                                {
                                    rkbs = true;
                                }
                                else
                                {
                                    rkbs = false;
                                }
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.分钟其他资料入库(dateTime.AddMinutes(-9), dateTime);
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddMinutes(10);
                        }
                    });
                }

                if (item.ID == 2)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddMinutes(9);
                        int hour = -1;
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            if (myhour != hour)
                            {
                                hour = myhour;
                                List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour), dateTime.Date.AddHours(myhour));
                                List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                                if (mylists1.Count == 0 || mylists2.Count == 0)
                                {
                                    rkbs = true;
                                }
                                else if (mylists1[0].个数 <= 1 || mylists1[0].个数 < mylists2[0].个数 / 2)
                                {
                                    rkbs = true;
                                }
                                else
                                {
                                    rkbs = false;
                                }
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.分钟降水量入库(dateTime.AddMinutes(-9), dateTime);
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddMinutes(10);
                        }
                    });
                }

                if (item.ID == 3)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddMinutes(9);
                        int hour = -1;
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            if (myhour != hour)
                            {
                                hour = myhour;
                                List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour), dateTime.Date.AddHours(myhour));
                                List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                                if (mylists1.Count == 0 || mylists2.Count == 0)
                                {
                                    rkbs = true;
                                }
                                else if (mylists1[0].个数 <= 1 || mylists1[0].个数 < mylists2[0].个数 / 2)
                                {
                                    rkbs = true;
                                }
                                else
                                {
                                    rkbs = false;
                                }
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.分钟常规气压入库(dateTime.AddMinutes(-9), dateTime);
                                str = sjkcl.分钟其他气压入库(dateTime.AddMinutes(-9), dateTime) + str;
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddMinutes(10);
                        }
                    });
                }

                if (item.ID == 4)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddMinutes(9);
                        int hour = -1;
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            if (myhour != hour)
                            {
                                hour = myhour;
                                List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour), dateTime.Date.AddHours(myhour));
                                List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                                if (mylists1.Count == 0 || mylists2.Count == 0)
                                {
                                    rkbs = true;
                                }
                                else if (mylists1[0].个数 <= 1 || mylists1[0].个数 < mylists2[0].个数 / 2)
                                {
                                    rkbs = true;
                                }
                                else
                                {
                                    rkbs = false;
                                }
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.分钟常规湿度入库(dateTime.AddMinutes(-9), dateTime);
                                str = sjkcl.分钟其他湿度入库(dateTime.AddMinutes(-9), dateTime) + str;
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddMinutes(10);
                        }
                    });
                }

                if (item.ID == 5)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddMinutes(9);
                        int hour = -1;
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            if (myhour != hour)
                            {
                                hour = myhour;
                                List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour), dateTime.Date.AddHours(myhour));
                                List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                                if (mylists1.Count == 0 || mylists2.Count == 0)
                                {
                                    rkbs = true;
                                }
                                else if (mylists1[0].个数 <= 1 || mylists1[0].个数 < mylists2[0].个数 / 2)
                                {
                                    rkbs = true;
                                }
                                else
                                {
                                    rkbs = false;
                                }
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.分钟常规温度入库(dateTime.AddMinutes(-9), dateTime);
                                str = sjkcl.分钟其他温度入库(dateTime.AddMinutes(-9), dateTime) + str;
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddMinutes(10);
                        }
                    });
                }

                if (item.ID == 6)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddMinutes(9);
                        int hour = -1;
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            if (myhour != hour)
                            {
                                hour = myhour;
                                List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour), dateTime.Date.AddHours(myhour));
                                List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                                if (mylists1.Count == 0 || mylists2.Count == 0)
                                {
                                    rkbs = true;
                                }
                                else if (mylists1[0].个数 <= 1 || mylists1[0].个数 < mylists2[0].个数 / 2)
                                {
                                    rkbs = true;
                                }
                                else
                                {
                                    rkbs = false;
                                }
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.分钟其他风入库(dateTime.AddMinutes(-9), dateTime);
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddMinutes(10);
                        }
                    });
                }
                if (item.ID == 7)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddHours(5);
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour-4), dateTime.Date.AddHours(myhour));
                            List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour-4).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                            if (mylists1.Count == 0 || mylists2.Count == 0)
                            {
                                
                                rkbs = true;
                            }
                            
                            else if (mylists1.Sum(y => y.个数) <= 10 || mylists1.Sum(y => y.个数) < mylists2.Sum(y => y.个数) / 10)
                            {
                                rkbs = true;
                            }
                            else
                            {
                                rkbs = false;
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.小时其他资料入库(dateTime.AddHours(-4), dateTime);
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddHours(5);
                        }
                    });
                }
                if (item.ID == 8)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddHours(5);
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour - 4), dateTime.Date.AddHours(myhour));
                            List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour - 4).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                            if (mylists1.Count == 0 || mylists2.Count == 0)
                            {

                                rkbs = true;
                            }

                            else if (mylists1.Sum(y => y.个数) <= 10 || mylists1.Sum(y => y.个数) < mylists2.Sum(y => y.个数) / 10)
                            {
                                rkbs = true;
                            }
                            else
                            {
                                rkbs = false;
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.小时降水量入库(dateTime.AddHours(-4), dateTime);
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddHours(5);
                        }
                    });
                }
                if (item.ID == 9)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddHours(5);
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour - 4), dateTime.Date.AddHours(myhour));
                            List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour - 4).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                            if (mylists1.Count == 0 || mylists2.Count == 0)
                            {

                                rkbs = true;
                            }

                            else if (mylists1.Sum(y => y.个数) <= 10 || mylists1.Sum(y => y.个数) < mylists2.Sum(y => y.个数) / 10)
                            {
                                rkbs = true;
                            }
                            else
                            {
                                rkbs = false;
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.小时气压入库(dateTime.AddHours(-4), dateTime);
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddHours(5);
                        }
                    });
                }
                if (item.ID == 10)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddHours(5);
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour - 4), dateTime.Date.AddHours(myhour));
                            List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour - 4).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                            if (mylists1.Count == 0 || mylists2.Count == 0)
                            {

                                rkbs = true;
                            }

                            else if (mylists1.Sum(y => y.个数) <= 10 || mylists1.Sum(y => y.个数) < mylists2.Sum(y => y.个数) / 10)
                            {
                                rkbs = true;
                            }
                            else
                            {
                                rkbs = false;
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.小时湿度入库(dateTime.AddHours(-4), dateTime);
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddHours(5);
                        }
                    });
                }
                if (item.ID == 11)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddHours(5);
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour - 4), dateTime.Date.AddHours(myhour));
                            List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour - 4).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                            if (mylists1.Count == 0 || mylists2.Count == 0)
                            {

                                rkbs = true;
                            }

                            else if (mylists1.Sum(y => y.个数) <= 10 || mylists1.Sum(y => y.个数) < mylists2.Sum(y => y.个数) / 10)
                            {
                                rkbs = true;
                            }
                            else
                            {
                                rkbs = false;
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.小时温度入库(dateTime.AddHours(-4), dateTime);
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddHours(5);
                        }
                    });
                }
                if (item.ID == 12)
                {
                    Task.Factory.StartNew(() =>
                    {
                        DateTime dateTime = sDateTime.AddHours(5);
                        bool rkbs = false;
                        while (dateTime.CompareTo(eDateTime) <= 0)
                        {
                            //判断资料是否缺失
                            int myhour = dateTime.Hour;
                            List<数据库处理.入库个数统计信息> mylists1 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour - 4), dateTime.Date.AddHours(myhour));
                            List<数据库处理.入库个数统计信息> mylists2 = sjkcl.获取入库个数统计信息(item.SJKName, dateTime.Date.AddHours(myhour - 4).AddDays(-15), dateTime.Date.AddHours(myhour).AddDays(-15));
                            if (mylists1.Count == 0 || mylists2.Count == 0)
                            {

                                rkbs = true;
                            }

                            else if (mylists1.Sum(y => y.个数) <= 10 || mylists1.Sum(y => y.个数) < mylists2.Sum(y => y.个数) / 10)
                            {
                                rkbs = true;
                            }
                            else
                            {
                                rkbs = false;
                            }

                            if (rkbs)
                            {
                                string str = sjkcl.小时风入库(dateTime.AddHours(-4), dateTime);
                                callDuration.Text = str + callDuration.Text;
                            }

                            dateTime = dateTime.AddHours(5);
                        }
                    });
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
        private void T1_TextChanged_1(object sender, TextChangedEventArgs e)
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

                    t1.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        t1.Text = data;
                        //将光标移至文本框最后
                        t1.Focus();
                        t1.CaretIndex = t1.Text.Length;
                    }));
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


        public Theme GetMyTheme(string name)
        {
            string myName = name.ToLower();
            if (myName.Contains("crystal"))
            {
                return new CrystalTheme();
            }

            if (myName.Contains("fluent"))
            {
                return new FluentTheme();
            }

            if (myName.Contains("material"))
            {
                return new MaterialTheme();
            }

            if (myName.Contains("office2016touch"))
            {
                return new Office2016TouchTheme();
            }

            if (myName.Contains("office2016"))
            {
                return new Office2016Theme();
            }

            if (myName.Contains("green"))
            {
                return new GreenTheme();
            }

            if (myName.Contains("office2013"))
            {
                return new Office2013Theme();
            }

            if (myName.Contains("visualstudio2013"))
            {
                return new VisualStudio2013Theme();
            }

            if (myName.Contains("windows8touch"))
            {
                return new Windows8TouchTheme();
            }

            if (myName.Contains("windows8"))
            {
                return new Windows8Theme();
            }

            if (myName.Contains("office_black"))
            {
                return new Office_BlackTheme();
            }

            if (myName.Contains("office_blue"))
            {
                return new Office_BlueTheme();
            }

            if (myName.Contains("office_silver"))
            {
                return new Office_SilverTheme();
            }

            if (myName.Contains("summer"))
            {
                return new SummerTheme();
            }

            if (myName.Contains("vista"))
            {
                return new VistaTheme();
            }

            if (myName.Contains("transparent"))
            {
                return new TransparentTheme();
            }

            if (myName.Contains("windows7"))
            {
                return new Windows7Theme();
            }

            if (myName.Contains("expression_dark"))
            {
                return new Expression_DarkTheme();
            }

            return new CrystalTheme();
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
        }

        private void RadToggleSwitchButton_Checked(object sender, RoutedEventArgs e)
        {
            switch1.Content = "是";
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\基本信息.xml");
            util.Write(switch1.IsChecked.ToString(), "SFRK");
            sfrkbs = true;
        }

        private void switch2_Checked(object sender, RoutedEventArgs e)
        {
            switch2.Content = "是";
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\基本信息.xml");
            util.Write(switch2.IsChecked.ToString(), "ZNSFRK");
            znsfrkbs = true;
        }

        private void switch2_Unchecked(object sender, RoutedEventArgs e)
        {
            switch2.Content = "否";
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\基本信息.xml");
            util.Write(switch2.IsChecked.ToString(), "ZNSFRK");
            znsfrkbs = false;
        }

        private void RadToggleSwitchButton_Unchecked(object sender, RoutedEventArgs e)
        {
            switch1.Content = "否";
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\基本信息.xml");
            util.Write(switch1.IsChecked.ToString(), "SFRK");
            sfrkbs = false;
        }


    }
}