using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace _2019HSQXSJK
{
    class 数据恢复视图: 数据恢复ViewMode, IDataErrorInfo
    {
        private ICommand _Reserve;
        public ICommand Reserve
        {
            get
            {
                return this._Reserve;
            }
        }

        private void ReserveAction(object parameter)
        {
            this._DepartureDateChanged = true;
            this._ReturnDateChanged = true;
            this.OnPropertyChanged("DepartureDate");
            this.OnPropertyChanged("ReturnDate");
        }


        private DateTime _DisplayDateStart;
        public DateTime DisplayDateStart
        {
            get
            {
                return this._DisplayDateStart;
            }
        }

        private DateTime _DisplayDateEnd;
        public DateTime DisplayDateEnd
        {
            get
            {
                return this._DisplayDateEnd;
            }
        }

        private bool _DepartureDateChanged;
        private bool _ReturnDateChanged;

        private DateTime? _DepartureDate;
        public DateTime? DepartureDate
        {
            get
            {
                return this._DepartureDate;
            }
            set
            {
                if (this._DepartureDate.HasValue != value.HasValue || (this._DepartureDate.HasValue == value.HasValue && (!value.HasValue || (this._DepartureDate.Value != value.Value))))
                {
                    this._DepartureDate = value;
                    this._DepartureDateChanged = true;
                    this.OnPropertyChanged("DepartureDate");
                    if (this._ReturnDateChanged)
                    {
                        this.OnPropertyChanged("ReturnDate");
                    }
                }
            }
        }

        private DateTime? _ReturnDate;
        public DateTime? ReturnDate
        {
            get
            {
                return this._ReturnDate;
            }
            set
            {
                if (this._ReturnDate.HasValue != value.HasValue || (this._ReturnDate.HasValue == value.HasValue && (!value.HasValue || (this._ReturnDate.Value != value.Value))))
                {
                    this._ReturnDate = value;
                    this._ReturnDateChanged = true;
                    this.OnPropertyChanged("ReturnDate");
                    if (this._DepartureDateChanged)
                    {
                        this.OnPropertyChanged("DepartureDate");
                    }
                }
            }
        }

        private string ValidateDepartureDate()
        {
            if (this._DepartureDateChanged)
            {
                if (!this.DepartureDate.HasValue)
                {
                    return "Departure date can not be empty.";
                }
                else if (DateTime.Now.AddDays(7).CompareTo(this.DepartureDate.Value) == -1)
                {
                    return "Departure can not be further than 7 days from today.";
                }
                else if (DateTime.Now.CompareTo(this.DepartureDate.Value) == 0)
                {
                    return "Departure can not be today.";
                }
                else if (DateTime.Now.CompareTo(this.DepartureDate.Value) == 1)
                {
                    return "Departure can not be in the past.";
                }
                else if (this.ReturnDate.HasValue && this.ReturnDate.Value.CompareTo(this.DepartureDate.Value) != 1)
                {
                    return "Departure date should be before return date.";
                }
            }
            return null;
        }

        private string ValidateReturnDate()
        {
            if (this._ReturnDateChanged)
            {
                if (!this.ReturnDate.HasValue)
                {
                    return "Return date can not be empty";
                }
                else if (this.DepartureDate.HasValue && this.ReturnDate.Value.CompareTo(this.DepartureDate.Value) != 1)
                {
                    return "Return date should be after the departure date.";
                }
                else if (DateTime.Now.CompareTo(this.ReturnDate.Value) == 1)
                {
                    return "Return can not be in the past.";
                }
                else if (DateTime.Now.AddDays(31).CompareTo(this.ReturnDate.Value) == -1)
                {
                    return "Retrun date should be within 31 days from today.";
                }
            }
            return null;
        }

        public string Error
        {
            get
            {
                return ValidateDepartureDate() ?? ValidateReturnDate();
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "DepartureDate": return this.ValidateDepartureDate();
                    case "ReturnDate": return this.ValidateReturnDate();
                }
                return null;
            }
        }
    }
}
