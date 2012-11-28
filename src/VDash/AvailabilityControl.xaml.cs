/*
    Copyright (C) 2012 Christopher Cartwright
    Copyright (C) 2012 Richard Payne
    Copyright (C) 2012 Andrew Hill
    Copyright (C) 2012 David Shirley
	Copyright (C) 2012 Brent Cornwall
    
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
using System.Windows.Shapes;
using System.Net;
using System.ComponentModel;
using VehicleLib;

namespace VDash
{
    /// <summary>
    /// Interaction logic for VehicleAvailability.xaml
    /// </summary>
    public partial class AvailabilityControl : UserControl
    {
		DataModel dm = DataModel.GetInstance();
		string _password;
		Vehicle _selVehicle;

		private class Vehicle {
			public string Name;
			public IPEndPoint Ip;

			public Vehicle(string name, IPEndPoint ip)
			{
				Name = name;
				Ip = ip;
			}

			public override string ToString()
			{
				return Ip.Address.ToString();
			}

		}
		private Dictionary<string, Vehicle> _vehicles;
		private VehicleLogin login = null;

		private Dictionary<string, Vehicle> Vehicles
		{
			get { return _vehicles; }
			set { _vehicles = value; }
		}

		public AvailabilityControl()
        {
			_vehicles = new Dictionary<string, Vehicle>();
			dm.Listener.OnBroadcastReceived += delegate(string name, IPEndPoint ep)
			{
				Vehicle hasKey;
				_vehicles.TryGetValue(name, out hasKey);
				if (hasKey != null)
				{
					_vehicles.Add(name, new Vehicle (name, ep));
				}
			};

			Resources["Vehicles"] = _vehicles;

            InitializeComponent();
        }

		private void buttonConnect_Click(object sender, RoutedEventArgs e)
		{
			if (listBoxVehicles.SelectedItems.Count < 1)
			{
				MessageBox.Show("Select a vehicle to connect to");
				// message to select a vehicle?
				return;
			}
			var selItem = (KeyValuePair<string, Vehicle>)listBoxVehicles.SelectedItems[0];
			_selVehicle = selItem.Value;

			login = new VehicleLogin(_selVehicle.Name);
			login.Owner = MainWindow.Self;
				
			if (!dm.Vehicle.Connected)
				login.Closing += new CancelEventHandler(login_Closing);

			login.ShowDialog();			
		}

		void login_Closing(object sender, CancelEventArgs e)
		{
			_password = login.Password;
			LogControl.Debug("Attempting connection to " + _selVehicle.Name);
			dm.Vehicle.Connect(_selVehicle.Ip, new Login(_password));
		}
    }
}
