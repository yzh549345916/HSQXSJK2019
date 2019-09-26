using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace _2019HSQXSJK
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon = null;
        public MainWindow()
        {
            InitializeComponent();

            //rkcs();


        }

        public void rkcs()
        {
            数据库处理 sjkcl = new 数据库处理();
            DateTime dateTime = Convert.ToDateTime("2019-07-01 13:50:00");
            sjkcl.统计信息入库(dateTime.Date, DateTime.Now);
            sjkcl = null;
        }
        public void cs()
        {
            数据库处理 sjkcl = new 数据库处理();
            DateTime dateTime = Convert.ToDateTime("2019-09-26 15:20:00");
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
        /// <summary>
        /// 重载OnClosing()函数实现--使用户无法通过点击右上角的关闭按钮来关闭窗口
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

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
    }
}
