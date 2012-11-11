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
		internal static VehiclePipe Vehicle = new VehiclePipe();
		internal static BroadcastListener Listener = new BroadcastListener();

		public MainWindow()
        {
			InitializeComponent();

			Closed += new EventHandler(MainWindow_Closed);
			try				
			{
				Listener.Start(Convert.ToUInt16(Properties.Settings.Default.ListenPort));
			}
			catch (Exception ex)
			{
				LogControl.Error(ex);
			}

			AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
			{
				LogControl.Error(e.ExceptionObject as Exception);
			};

#if _DEBUG
			Listener.OnBroadcastReceived += delegate(string name, IPEndPoint ep)
			{
				if(!Vehicle.Connected)
					Vehicle.Connect(ep, new Login("pwd"));
			};
#endif
        }

		void MainWindow_Closed(object sender, EventArgs e)
		{
			Vehicle.Shutdown();
			Listener.Shutdown();
		}

		private void ApplicationClose(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}
	}
}
