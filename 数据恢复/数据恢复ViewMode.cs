using System.Collections.ObjectModel;

namespace _2019HSQXSJK
{
    class 数据恢复ViewMode
    {
        private ObservableCollection<资料种类> agencies;

        public ObservableCollection<资料种类> Agencies
        {
            get
            {
                if (agencies == null)
                {
                    agencies = new ObservableCollection<资料种类>();
                    agencies.Add(new 资料种类("Exotic Liquids", "(171) 555-2222"));
                    agencies.Add(new 资料种类("New Orleans Cajun Delights", "(100) 555-4822"));
                    agencies.Add(new 资料种类("Grandma Kelly's Homestead", "(313) 555-5735"));
                    agencies.Add(new 资料种类("Tokyo Traders", "(03) 3555-5011"));
                    agencies.Add(new 资料种类("Cooperativa de Quesos 'Las Cabras'", "(98) 598 76 54"));
                    agencies.Add(new 资料种类("Pavlova, Ltd.", "(03) 444-2343"));
                    agencies.Add(new 资料种类("Specialty Biscuits, Ltd.", "(161) 555-4448"));
                    agencies.Add(new 资料种类("PB Knäckebröd AB", "031-987 65 43"));
                }
                return agencies;
            }
        }
    }
}
