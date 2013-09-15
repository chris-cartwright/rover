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

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace VDash
{
	/// <summary>
	/// Interaction logic for KeyBindingsWindow.xaml
	/// </summary>
	public partial class KeyBindingsWindow
	{
		private class DataSource : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			private string _moveForward;
			public string MoveForward
			{
				get { return _moveForward; }
				set { _moveForward = value; Notify("MoveForward"); }
			}

			private string _moveBackward;
			public string MoveBackward
			{
				get { return _moveBackward; }
				set { _moveBackward = value; Notify("MoveBackward"); }
			}

			private string _turnLeft;
			public string TurnLeft
			{
				get { return _turnLeft; }
				set { _turnLeft = value; Notify("TurnLeft"); }
			}

			private string _turnRight;
			public string TurnRight
			{
				get { return _turnRight; }
				set { _turnRight = value; Notify("TurnRight"); }
			}

			private void Notify(string name)
			{
				if (PropertyChanged == null)
					return;

				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		private readonly DataSource _dataSource;

		public KeyBindingsWindow()
		{
			_dataSource = new DataSource();
			DataContext = _dataSource;

			_dataSource.MoveForward = Properties.Settings.Default.KeyForward;
			_dataSource.MoveBackward = Properties.Settings.Default.KeyBackward;
			_dataSource.TurnLeft = Properties.Settings.Default.KeyLeft;
			_dataSource.TurnRight = Properties.Settings.Default.KeyRight;

			InitializeComponent();
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			Properties.Settings.Default.KeyForward = _dataSource.MoveForward;
			Properties.Settings.Default.KeyBackward = _dataSource.MoveBackward;
			Properties.Settings.Default.KeyLeft = _dataSource.TurnLeft;
			Properties.Settings.Default.KeyRight = _dataSource.TurnRight;

			Close();
		}

		private void WindowClose(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}
	}
}
