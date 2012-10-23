using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace VDash.Converters
{
	/// <summary>
	/// Converts a radio button to an enum value.
	/// 
	/// Usage: {Binding [enum instance],Converter={StaticResource [converter name]},ConverterParameter=[enum value]}
	/// </summary>
	public class EnumRadioButtonConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string param = parameter as string;
			if (param == null)
				return DependencyProperty.UnsetValue;

			Type type = value.GetType();
			if(!Enum.IsDefined(type, value))
				return DependencyProperty.UnsetValue;

			return Enum.Parse(type, param).Equals(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string param = parameter as string;
			bool bv = (bool)value;

			if (param == null || !bv)
			{
				try
				{
					return Enum.Parse(targetType, "0");
				}
				catch (Exception)
				{
					return DependencyProperty.UnsetValue;
				}
			}

			return Enum.Parse(targetType, param);
		}
	}
}
