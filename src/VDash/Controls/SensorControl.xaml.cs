/*
    Copyright (C) 2012 Christopher Cartwright
    
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

namespace VDash.Controls
{
	/// <summary>
	/// Interaction logic for SensorControl.xaml
	/// </summary>
	public partial class SensorControl
	{
		private class DataSource : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			private float _percent;
			public float Percent
			{
				get => _percent;
				set { _percent = value; Notify("Percent"); }
			}

			private float _current;
			public float Current
			{
				get => _current;
				set { _current = value; Notify("Current"); }
			}

			public DataSource()
			{
				var dm = DataModel.Instance;
				dm.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
				{
					if (e.PropertyName != "BatteryCurrent")
					{
						return;
					}

					Percent = (float)Math.Round((dm.BatteryCurrent - dm.BatteryMin) / (dm.BatteryMax - dm.BatteryMin) * 100, 1);
					Current = dm.BatteryCurrent;
				};
			}

			private void Notify(string name)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(name));
				}
			}
		}

		private readonly DataSource _ds;

		public SensorControl()
		{
			_ds = new DataSource();
			DataContext = _ds;

			InitializeComponent();
		}
	}
}
