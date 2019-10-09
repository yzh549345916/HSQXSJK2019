using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2019HSQXSJK
{
    class 资料种类
    {
        private string name;
        private string phone;

        public 资料种类()
        {
        }

        public 资料种类(string name, string phone)
        {
            Name = name;
            Phone = phone;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Phone
        {
            get
            {
                return this.phone;
            }
            set
            {
                this.phone = value;
            }
        }


        public override string ToString()
        {
            return string.Format("[名称 : {0}]", this.Name);
        }
    }
}
