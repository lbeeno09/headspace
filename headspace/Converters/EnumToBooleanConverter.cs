using headspace.Utilities;
using Microsoft.UI.Xaml.Data;
using System;

namespace headspace.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(parameter is not string enumString)
            {
                throw new ArgumentException("Exception: EnumToBooleanConverter parameter must be an EnumName");
            }
            if(!Enum.IsDefined(typeof(AppTheme), value))
            {
                throw new ArgumentException("Exception: EnumToBooleanConverter must be an Enum");
            }

            var enumValue = Enum.Parse(typeof(AppTheme), enumString);
            return enumValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if(parameter is not string enumString)
            {
                throw new ArgumentException("Exception: EnumToBooleanConverter parameter must be an EnumName");
            }

            return Enum.Parse(typeof(AppTheme), enumString);
        }
    }
}
