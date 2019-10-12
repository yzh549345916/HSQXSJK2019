using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace _2019HSQXSJK
{
    /// <summary>
    /// 数据恢复.xaml 的交互逻辑
    /// </summary>
    public partial class 数据恢复 : UserControl
    {
        StringToDisplay callDuration = new StringToDisplay();
        public 数据恢复()
        {
            InitializeComponent();
            sDatePic.SelectableDateEnd = DateTime.Now;
            eDatePic.SelectableDateEnd = DateTime.Now;
            t1.DataContext = callDuration;
        }

        private void RadPathButton_Click(object sender, RoutedEventArgs e)
        {
            if(dataCom.SelectedItems.Count>0&& sDatePic.SelectedValue.HasValue&& eDatePic.SelectedValue.HasValue && sDatePic.SelectedValue <= eDatePic.SelectedValue)
            {
                System.Collections.IList lists = dataCom.SelectedItems;
                数据库处理 sjkcl = new 数据库处理();
                DateTime sDateTime = Convert.ToDateTime(sDatePic.SelectedValue), eDateTime = Convert.ToDateTime(eDatePic.SelectedValue);
                foreach (资料种类 item in lists)
                {
                    if(item.ID==1)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddMinutes(9);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.分钟其他资料入库(dateTime.AddMinutes(-9), dateTime);
                                dateTime = dateTime.AddMinutes(10);
                                callDuration.Text = str + callDuration.Text;

                            }
                        });
                    }
                    else if (item.ID == 2)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddMinutes(9);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.分钟降水量入库(dateTime.AddMinutes(-9), dateTime);
                                dateTime = dateTime.AddMinutes(10);
                                callDuration.Text = str + callDuration.Text;

                            }
                        });
                    }
                    else if (item.ID == 3)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddMinutes(9);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.分钟常规气压入库(dateTime.AddMinutes(-9), dateTime);
                                str = sjkcl.分钟其他气压入库(dateTime.AddMinutes(-9), dateTime)+ str;
                                dateTime = dateTime.AddMinutes(10);
                                callDuration.Text = str + callDuration.Text;

                            }
                        });
                    }
                    else if (item.ID == 4)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddMinutes(9);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.分钟常规湿度入库(dateTime.AddMinutes(-9), dateTime);
                                str = sjkcl.分钟其他湿度入库(dateTime.AddMinutes(-9), dateTime) + str;
                                dateTime = dateTime.AddMinutes(10);
                                callDuration.Text = str + callDuration.Text;

                            }
                        });
                    }
                    else if (item.ID == 5)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddMinutes(9);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.分钟常规温度入库(dateTime.AddMinutes(-9), dateTime);
                                str = sjkcl.分钟其他温度入库(dateTime.AddMinutes(-9), dateTime) + str;
                                dateTime = dateTime.AddMinutes(10);
                                callDuration.Text = str + callDuration.Text;

                            }
                        });
                    }
                    else if (item.ID == 6)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddMinutes(9);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.分钟其他风入库(dateTime.AddMinutes(-9), dateTime);
                                dateTime = dateTime.AddMinutes(10);
                                callDuration.Text = str + callDuration.Text;
                            }
                        });
                    }
                    else if (item.ID == 7)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddHours(5);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.小时其他资料入库(dateTime.AddHours(-4), dateTime);
                                dateTime = dateTime.AddHours(5);
                                callDuration.Text = str + callDuration.Text;
                            }
                        });
                    }
                    else if (item.ID == 8)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddHours(5);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.小时降水量入库(dateTime.AddHours(-4), dateTime);
                                dateTime = dateTime.AddHours(5);
                                callDuration.Text = str + callDuration.Text;
                            }
                        });
                    }
                    else if (item.ID == 9)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddHours(5);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.小时气压入库(dateTime.AddHours(-4), dateTime);
                                dateTime = dateTime.AddHours(5);
                                callDuration.Text = str + callDuration.Text;
                            }
                        });
                    }
                    else if (item.ID == 10)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddHours(5);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.小时湿度入库(dateTime.AddHours(-4), dateTime);
                                dateTime = dateTime.AddHours(5);
                                callDuration.Text = str + callDuration.Text;
                            }
                        });
                    }
                    else if (item.ID == 11)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddHours(5);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.小时温度入库(dateTime.AddHours(-4), dateTime);
                                dateTime = dateTime.AddHours(5);
                                callDuration.Text = str + callDuration.Text;
                            }
                        });
                    }
                    else if (item.ID == 12)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            DateTime dateTime = sDateTime.AddHours(5);
                            while (dateTime.CompareTo(eDateTime) <= 0)
                            {
                                string str = sjkcl.小时风入库(dateTime.AddHours(-4), dateTime);
                                dateTime = dateTime.AddHours(5);
                                callDuration.Text = str + callDuration.Text;
                            }
                        });
                    }
                }
            }
            else
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = "参数输入错误，请重新输入选项"
                });
            }
            
        }
    }
}
