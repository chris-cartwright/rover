using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VDash.Converters
{
	[ValueConversion(typeof(DateTime), typeof(string))]
	public class DateTimeToTimeStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value as DateTime? == null)
				return "";

			DateTime dt = (DateTime)value;
			return String.Format("[{0}:{1}:{2}] ", dt.Hour, dt.Minute, dt.Second);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}
