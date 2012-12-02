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

using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using VehicleLib;
using System.Collections.ObjectModel;
using System;

namespace VDash
{
    /// <summary>
    /// Interaction logic for VehicleAvailability.xaml
    /// </summary>
    public partial class AvailabilityControl : UserControl
    {
		private class Vehicle {
			private string _name;
			private IPEndPoint _ip;
			private bool _connected;

			public string Name
			{
				get { return _name; }
				set { _name = value; Notify("Name"); }
				
			}
			public IPEndPoint Ip
			{
				get { return _ip; }
				set { _ip = value; Notify("Ip"); }
			}
			public bool Connected
			{
				get { return _connected; }
				set { _connected = value; Notify("Connected"); }
			}
			public event PropertyChangedEventHandler PropertyChanged;

			public Vehicle(string name, IPEndPoint ip)
			{
				Name = name;
				Ip = ip;
				Connected = false;
			}

			public Vehicle(string name, IPEndPoint ip, bool connected)
			{
				Name = name;
				Ip = ip;
				Connected = connected;
			}

			public override string ToString()
			{
				return Name.ToString();
			}

			private void Notify(string name)
			{
				if (PropertyChanged == null)
					return;

				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}

		}// end class Vehicle

		private class DataSource : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			private DataModel _dm = DataModel.GetInstance();

			private ObservableCollection<Vehicle> _vehicles;
			public ObservableCollection<Vehicle> Vehicles
			{
				get { return _vehicles; }
				set
				{
					_vehicles = value;
					Notify("Vehicles");
				}
			}

			private IPAddress[] _ipAddresses;
			public IPAddress[] IPAddresses
			{
				get { return _ipAddresses; }
				set
				{
					_ipAddresses = value;
					Notify("IPAddresses");

					if (_listenAddress == null && value.Length > 0)
						ListenAddress = value[0];
				}
			}

			private IPAddress _listenAddress;
			public IPAddress ListenAddress
			{
				get { return _listenAddress; }
				set
				{
					if (_listenAddress == value)
						return;

					_listenAddress = value;

					IPEndPoint ep = new IPEndPoint(value, Properties.Settings.Default.ListenPort);
					_dm.Listener.Shutdown();
					_dm.Listener.Start(ep);
					LogControl.Info(String.Format("Starting listener on {0}", ep));

					Vehicles = new ObservableCollection<Vehicle>();
				}
			}

			public DataSource()
			{
				_vehicles = new ObservableCollection<Vehicle>();
			}

			private void Notify(string name)
			{
				if (PropertyChanged == null)
					return;

				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		private VehicleLogin _login;
		private DataModel _dm = DataModel.GetInstance();
		private string _password;
		private Vehicle _selVehicle;
		private DataSource _ds;

		public AvailabilityControl()
        {
			_ds = new DataSource();
			DataContext = _ds;
			_ds.IPAddresses = GetIPAddresses();

			_dm.Listener.OnBroadcastReceived += delegate(string name, IPEndPoint ep)
			{
				if (!VehiclesContains(name))
					MainWindow.Invoke(() => _ds.Vehicles.Add(new Vehicle(name, ep)));
			};
			
            InitializeComponent();
        }

		private bool VehiclesContains(string name)
		{
			foreach (Vehicle v in _ds.Vehicles)
			{
				if (v.Name.Equals(name)) {
					return true;
				}
			}

			return false;
		}

		private void buttonConnect_Click(object sender, RoutedEventArgs e)
		{
			if (listViewVehicles.SelectedItems.Count < 1)
			{
				MessageBox.Show("Please select a vehicle to connect to.");
				return;
			}

			var selItem = (Vehicle)listViewVehicles.SelectedItems[0];
			_selVehicle = selItem;

			_login = new VehicleLogin(_selVehicle.Name);
			_login.Owner = MainWindow.Self;
				
			if (!_dm.Vehicle.Connected)
				_login.Closing += new CancelEventHandler(login_Closing);

			_login.ShowDialog();			
		}

		private void login_Closing(object sender, CancelEventArgs e)
		{
			_password = _login.Password;
			LogControl.Info("Attempting connection to " + _selVehicle.Name);
			_dm.Vehicle.Connect(_selVehicle.Ip, new Login(_password));
		}

		/// <summary>
		/// Returns all IP addresses associated with this machine.
		/// Filters out IPv6 and 169.254.* addresses.
		/// </summary>
		/// <returns>List of associated IP addresses.</returns>
		private IPAddress[] GetIPAddresses()
		{
			List<IPAddress> ips = new List<IPAddress>();
			foreach (NetworkInterface iface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (iface.OperationalStatus == OperationalStatus.Down)
					continue;

				foreach (UnicastIPAddressInformation uni in iface.GetIPProperties().UnicastAddresses)
				{
					if (uni.Address.AddressFamily == AddressFamily.InterNetworkV6)
						continue;

					// Filter out 169.254.*
					if (!uni.IsDnsEligible)
						continue;

					ips.Add(uni.Address);
				}
			}

			ips.Add(new IPAddress(new byte[] { 127, 0, 0, 1 }));

			return ips.ToArray();
		}
    }
}
