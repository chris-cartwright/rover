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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using VehicleLib;
using System.Net;
using System.ComponentModel;

namespace VDash
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static Window Self { get; private set; }

		public static void Invoke(Action act)
		{
			if (Self == null)
				return;

			Self.Dispatcher.Invoke(act);
		}

		public MainWindow()
        {
			if (Self != null)
				throw new Exception("Cannot initialize more than one MainWindow.");

			Self = this;

			InitializeComponent();

			Closed += new EventHandler(delegate(object sender, EventArgs e)
			{
				DataModel.GetInstance().Shutdown();
			});

			AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
			{
				LogControl.Error(e.ExceptionObject as Exception);
			};

			this.PreviewKeyDown += new KeyEventHandler(MainWindow_KeyDown);
			this.ContentRendered += (s, e) => Focus();
        }

		void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			string k = e.Key.ToString().ToLower();

			DataModel dm = DataModel.GetInstance();

			if (k == Properties.Settings.Default.KeyForward)
				dm.Speed += 10;
			else if (k == Properties.Settings.Default.KeyBackward)
				dm.Speed -= 10;
			else if (k == Properties.Settings.Default.KeyLeft)
				dm.Turn--;
			else if (k == Properties.Settings.Default.KeyRight)
				dm.Turn++;
			else if (k == Properties.Settings.Default.KeyStop)
			{
				dm.Speed = 0;
				dm.Turn = DataModel.TurnDirection.None;
			}
		}

		private void ApplicationClose(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}
	}
}
