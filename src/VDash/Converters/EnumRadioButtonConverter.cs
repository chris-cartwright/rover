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
using System.Windows;
using System.Windows.Data;

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
