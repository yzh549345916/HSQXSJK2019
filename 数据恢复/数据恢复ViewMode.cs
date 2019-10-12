using System.Collections.ObjectModel;
using Telerik.Windows.Controls;

namespace _2019HSQXSJK
{
    class 数据恢复ViewMode: ViewModelBase
    {
        private ObservableCollection<资料种类> agencies;

        public ObservableCollection<资料种类> Agencies
        {
            get
            {
                if (agencies == null)
                {
                    agencies = new ObservableCollection<资料种类>();
                    agencies.Add(new 资料种类("小时降水量"));
                    agencies.Add(new 资料种类("小时气温"));
                    agencies.Add(new 资料种类("小时气压"));
                    agencies.Add(new 资料种类("小时相对湿度"));
                    agencies.Add(new 资料种类("小时风"));
                    agencies.Add(new 资料种类("小时其他资料"));
                    agencies.Add(new 资料种类("分钟降水量"));
                    agencies.Add(new 资料种类("分钟气温"));
                    agencies.Add(new 资料种类("分钟气压"));
                    agencies.Add(new 资料种类("分钟相对湿度"));
                    agencies.Add(new 资料种类("分钟风"));
                    agencies.Add(new 资料种类("分钟其他资料"));
                }
                return agencies;
            }
        }
    }
}
