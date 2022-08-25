using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mvvm.Core {
    public static class Helpers {
        public static void SetPropertyValue(string propertyName, object value, object obj) {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            if(propertyInfo != null) {
                propertyInfo.SetValue(obj, value, null);
            }
        }
        public static object GetPropertyValue(string propertyName, object obj) {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
    }
}
