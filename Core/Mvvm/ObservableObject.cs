using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvvm.Core {
    public class ObservableObject : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChangedEvent(string propertyName) {
            var handler = PropertyChanged;
            if(handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public void SetPropertyValue<T>(string propertyName, ref T propertyValueHolder, T newValue, Action<T> action = null) {
            if(object.Equals(propertyValueHolder, newValue))
                return;
            propertyValueHolder = newValue;
            RaisePropertyChangedEvent(propertyName);
            if(action != null)
                action.Invoke(newValue);
        }
    }
}
