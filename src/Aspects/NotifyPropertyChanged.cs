/*
    Copyright (C) 2013 Christopher Cartwright
    
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
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace Aspects
{
	/// <summary>
	/// Base class used by NotifyAttribute
	/// </summary>
	public class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	/// <summary>
	/// Signifies that the associated property should fire PropertyChanged
	/// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class NotifyAttribute : LocationInterceptionAspect
	{
		private MethodInfo _event;

		public bool IgnoreDuplicate;

		public override bool CompileTimeValidate(PostSharp.Reflection.LocationInfo locationInfo)
		{
			if (!base.CompileTimeValidate(locationInfo))
				return false;

			PropertyInfo property = locationInfo.PropertyInfo;
			Type type = property.DeclaringType;
			if (type == null || !typeof(NotifyPropertyChanged).IsAssignableFrom(type))
			{
				Statics.Message.Write(locationInfo.PropertyInfo, SeverityType.Error, "VD0001", property);
				return false;
			}

			_event = type.GetMethod("OnPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			return true;
		}

		public override void OnSetValue(LocationInterceptionArgs args)
		{
			object old = args.GetCurrentValue();

			base.OnSetValue(args);
			
			if (args.Value.Equals(old) && IgnoreDuplicate)
				return;

			_event.Invoke(args.Instance, new object[] { args.LocationName });
		}
	}
}
