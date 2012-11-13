/*
    Copyright (C) 2012 Christopher Cartwright
    Copyright (C) 2012 Richard Payne
    Copyright (C) 2012 Andrew Hill
    Copyright (C) 2012 David Shirley
    
    This file is part of VDash.

    VDash is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    VDash is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with VDash.  If not, see <http://www.gnu.org/licenses/>.
*/

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
		private string _name;
		public string ClassName
		{
			get { return _name; }
			set
			{
				if (value == _name)
					return;

				_type = System.Type.GetType("VDash." + value, true);
				if (_type.GetInterface("IMultiValueConverter") == null)
					throw new ArgumentException(String.Format("Type {0} doesn't support IMultiValueConverter.", _type.FullName), "Type");

				_name = value;
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
