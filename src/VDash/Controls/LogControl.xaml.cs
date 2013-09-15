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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace VDash.Controls
{
	/// <summary>
	/// Hosts all log output. Static functions are available to insert logs into the control.
	/// Any log submitted will be posted to every instance of LogControl.
	/// </summary>
	public partial class LogControl : UserControl
	{
		/// <summary>
		/// Log level of the message.
		/// </summary>
		private enum MessageType { All, Error, Warning, Info, Debug };

		/// <summary>
		/// Represents each item in the ListBox.
		/// </summary>
		private class LogItem
		{
			public MessageType Type { get; set; }
			public string Message { get; set; }
			public DateTime Time { get; private set; }

			public LogItem()
			{
				Time = DateTime.Now;
			}

			public override string ToString()
			{
				return Message;
			}
		}

		/// <summary>
		/// DataContext for this Window.
		/// </summary>
		private class DataSource : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			private ObservableCollection<LogItem> _logs;
			public ObservableCollection<LogItem> Logs
			{
				get { return _logs; }
				set { _logs = value; Notify("Logs"); }
			}

			private MessageType _type = MessageType.All;
			public MessageType Type
			{
				get { return _type; }
				set { _type = value; Notify("Type"); }
			}

			public DataSource()
			{
				Logs = new ObservableCollection<LogItem>();
			}

			private void Notify(string name)
			{
				if (PropertyChanged == null)
					return;

				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		/// <summary>
		/// Only used to specify whether or not a ListBoxItem should be shown.
		/// Would be private if it could be.
		/// </summary>
		internal class LogVisibleConverter : IMultiValueConverter
		{
			public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				if (values.Length != 2)
					return DependencyProperty.UnsetValue;

				int setting = (int)values[0];
				int self = (int)values[1];

				return (setting == (int)MessageType.All || setting == self ? Visibility.Visible : Visibility.Collapsed);
			}

			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
			{
				return null;
			}
		}

		/// <summary>
		/// Used by each instance of LogControl to display the newly submitted log.
		/// </summary>
		private static event Action<MessageType, string> OnLogReceived;

		private static void SendOnLogReceived(MessageType type, string message)
		{
			if (OnLogReceived == null)
				return;

			DataModel.Invoke(() => OnLogReceived(type, message));
		}

		public static void Error(string msg)
		{
			SendOnLogReceived(MessageType.Error, msg);
		}

		public static void Error(Exception ex)
		{
			Error(ex.Message);
		}

		public static void Warning(string msg)
		{
			SendOnLogReceived(MessageType.Error, msg);
		}

		public static void Info(string msg)
		{
			SendOnLogReceived(MessageType.Info, msg);
		}

		public static void Debug(string msg)
		{
			SendOnLogReceived(MessageType.Debug, msg);
		}

		private DataSource ds;

		public LogControl()
		{
			ds = new DataSource();
			this.DataContext = ds;

			InitializeComponent();

			OnLogReceived += LogReceived;
		}

		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			ds.Logs.Clear();
		}

		private void LogReceived(MessageType type, string message)
		{
			ScrollViewer sv = GetScrollViewer();
			bool scroll = false;
			if(sv != null)
				scroll = sv.VerticalOffset == sv.ScrollableHeight;

			ds.Logs.Add(new LogItem() { Type = type, Message = message });
			if (ds.Logs.Count > 100)
				ds.Logs.RemoveAt(0);

			if (scroll)
				sv.ScrollToBottom();
		}

		private ScrollViewer GetScrollViewer()
		{
			try
			{
				int idx = 0;
				object obj = null;

				do
				{
					obj = VisualTreeHelper.GetChild(listBoxLogs, idx);
					idx++;
				}
				while (obj != null && obj.GetType() != typeof(Border));

				if (obj == null)
					return null;

				return (ScrollViewer)((Border)obj).Child;
			}
			catch (ArgumentOutOfRangeException)
			{
				// Cycled through all children
				return null;
			}
		}
	}
}
