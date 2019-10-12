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
        private int _id;
        public 资料种类()
        {
        }

        public 资料种类(string name, string phone,int id)
        {
            Name = name;
            SJKName = phone;
            ID = id;
        }
        public 资料种类(string name)
        {
            Name = name;
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

        public string SJKName
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
        public int ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }


        public override string ToString()
        {
            return this.Name;
        }
    }
}
