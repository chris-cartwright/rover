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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using VDash.Controls;

namespace VDash
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		DataModel dm = DataModel.GetInstance();
		public static Window Self { get; private set; }

		public MainWindow()
        {
			if (Self != null)
				throw new Exception("Cannot initialize more than one MainWindow.");

			Self = this;

			InitializeComponent();

			Closed += new EventHandler(delegate(object sender, EventArgs e)
			{
				dm.Shutdown();
			});

			AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
			{
				LogControl.Error(e.ExceptionObject as Exception);
			};

			this.PreviewKeyDown += (s, e) => dm.Key = e.Key.ToString().ToLower();
			this.ContentRendered += (s, e) => Focus();
			this.Closing += new CancelEventHandler(MainWindow_Closing);
        }

		void MainWindow_Closing(object sender, CancelEventArgs e)
		{
			if (dm.Vehicle.Connected)
			{
				dm.Vehicle.Disconnect();
			}
		}

		private void ApplicationClose(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}
	}
}
