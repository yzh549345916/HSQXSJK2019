﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2019HSQXSJK
{
    public class StringToDisplay : INotifyPropertyChanged

    {

        private string text;

        public string Text

        {

            get { return text; }

            set

            {

                if (text != value)

                {
                    text = value;

                    PropertyChanged(this, new PropertyChangedEventArgs("Text"));

                }

            }

        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

    }

}
