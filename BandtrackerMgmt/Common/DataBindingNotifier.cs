using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BandtrackerMgmt
{
    public class DataBindingNotifier : INotifyPropertyChanged
    {
        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        protected bool SetField<T>(ref T p_field, T p_value, [CallerMemberName] string p_property = null)
        {
            if (EqualityComparer<T>.Default.Equals(p_field, p_value))
                return false;

            p_field = p_value;
            RaisePropertyChanged(p_property);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
