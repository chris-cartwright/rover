using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace VDash
{
	/// <summary>
	/// Interaction logic for KeyBindingsWindow.xaml
	/// </summary>
	public partial class KeyBindingsWindow : Window
	{
		private class DataSource : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			private char _moveForward;
			public char MoveForward
			{
				get { return _moveForward; }
				set { _moveForward = value; Notify("MoveForward"); }
			}

			private char _moveBackward;
			public char MoveBackward
			{
				get { return _moveBackward; }
				set { _moveBackward = value; Notify("MoveBackward"); }
			}

			private char _turnLeft;
			public char TurnLeft
			{
				get { return _turnLeft; }
				set { _turnLeft = value; Notify("TurnLeft"); }
			}

			private char _turnRight;
			public char TurnRight
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

		private DataSource _dataSource;

		public KeyBindingsWindow()
		{
			_dataSource = new DataSource();
			this.DataContext = _dataSource;

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
