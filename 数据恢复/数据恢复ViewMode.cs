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
                    数据库处理 sjkcl = new 数据库处理();
                    agencies=sjkcl.获取数据库表信息();
                }
                return agencies;
            }
        }
    }
}
