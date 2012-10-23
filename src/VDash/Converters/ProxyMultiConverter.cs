using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VDash.Converters
{
	/// <summary>
	/// Sets up a proxy to another IMultiConverter. Useful if one would want to use a nested class as a converter.
	/// Set the Type parameter when creating the resource then use as normal.
	/// 
	/// Example: <converters:ProxyMultiConverter x:Key="visibleConverter" Type="{x:Type me:LogControl+LogVisibleConverter}" />
	/// </summary>
	public class ProxyMultiConverter : IMultiValueConverter
	{
		private Type _type;
		public Type Type
		{
			get { return _type; }
			set
			{
				if (value == _type)
					return;

				if (value.GetInterface("IMultiValueConverter") == null)
					throw new ArgumentException(String.Format("Type {0} doesn't support IMultiValueConverter.", value.FullName), "Type");

				_type = value;
			}
		}

		private IMultiValueConverter _converter;

		private void CreateConverter()
		{
			if (_converter != null)
				return;

			if (_type == null)
				throw new InvalidOperationException("Cannot user converter without a type.");

			_converter = Activator.CreateInstance(_type) as IMultiValueConverter;
		}

		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			CreateConverter();
			return _converter.Convert(values, targetType, parameter, culture);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			return _converter.ConvertBack(value, targetTypes, parameter, culture);
		}
	}
}
