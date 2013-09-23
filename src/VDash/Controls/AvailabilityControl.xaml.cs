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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using Aspects;
using Aspects.Annotations;
using VDash.Properties;
using VehicleLib;
using VehicleLib.Events;

namespace VDash.Controls
{
	/// <summary>
	///     Interaction logic for VehicleAvailability.xaml
	/// </summary>
	public partial class AvailabilityControl
	{
		private class DataSource : NotifyPropertyChanged
		{
			private readonly DataModel _dm = DataModel.GetInstance();

			private ObservableCollection<IPAddress> _ipAddresses;
			private IPAddress _listenAddress;

			[Notify]
			public ObservableCollection<Vehicle> Vehicles { get; set; }

			[Notify]
			public ObservableCollection<IPAddress> IpAddresses
			{
				[UsedImplicitly]
				get { return _ipAddresses; }
				set
				{
					_ipAddresses = value;

					if (_listenAddress == null && value.Count > 0)
						ListenAddress = value[0];
				}
			}

			[Notify]
			public IPAddress ListenAddress
			{
				[UsedImplicitly]
				get { return _listenAddress; }
				set
				{
					if (Equals(_listenAddress, value))
						return;

					_listenAddress = value;

					var ep = new IPEndPoint(value, Settings.Default.ListenPort);
					_dm.Listener.Shutdown();
					_dm.Listener.Start(ep);
					LogControl.Info(String.Format("Starting listener on {0}", ep));

					Vehicles = new ObservableCollection<Vehicle>();
				}
			}

			public DataSource()
			{
				Vehicles = new ObservableCollection<Vehicle>();
			}
		}

		private class Vehicle : NotifyPropertyChanged
		{
			public Broadcast Broadcast { get; private set; }

			[Notify]
			public bool Connected { get; set; }

			public Vehicle(Broadcast bcast)
			{
				Broadcast = bcast;
				Connected = false;
			}

			public override string ToString()
			{
				return Broadcast.Name;
			}
		}

		private readonly DataModel _dm = DataModel.GetInstance();
		private readonly DataSource _ds;
		private VehicleLogin _login;
		private string _password;
		private Vehicle _selVehicle;

		/// <summary>
		///     Returns all IP addresses associated with this machine.
		///     Filters out IPv6 and 169.254.* addresses.
		/// </summary>
		/// <returns>List of associated IP addresses.</returns>
		private static IPAddress[] GetIpAddresses()
		{
			var ips = new List<IPAddress>();
			foreach (
				NetworkInterface iface in
					NetworkInterface.GetAllNetworkInterfaces().Where(iface => iface.OperationalStatus != OperationalStatus.Down))
			{
				ips.AddRange(from uni in iface.GetIPProperties().UnicastAddresses
							 where uni.Address.AddressFamily != AddressFamily.InterNetworkV6
							 where uni.IsDnsEligible
							 select uni.Address);
			}

			ips.Add(new IPAddress(new byte[] { 127, 0, 0, 1 }));

			return ips.ToArray();
		}

		private bool VehiclesContains(string name)
		{
			return _ds.Vehicles.Any(v => v.Broadcast.Name.Equals(name));
		}

		private void buttonConnect_Click(object sender, RoutedEventArgs e)
		{
			if (ListViewVehicles.SelectedItems.Count < 1)
			{
				MessageBox.Show("Please select a vehicle to connect to.");
				return;
			}

			_selVehicle = (Vehicle)ListViewVehicles.SelectedItems[0];

			_login = new VehicleLogin() { Owner = MainWindow.Self };

			if (!_dm.Vehicle.Connected)
				_login.Closing += login_Closing;

			_login.ShowDialog();
		}

		private void login_Closing(object sender, CancelEventArgs e)
		{
			_password = _login.Password;
			LogControl.Info("Attempting connection to " + _selVehicle.Broadcast.Name);
			_dm.Vehicle.Connect(_selVehicle.Broadcast, new Login(_password));
		}

		public AvailabilityControl()
		{
			_ds = new DataSource();
			DataContext = _ds;

			_ds.IpAddresses = new ObservableCollection<IPAddress>();

			foreach (IPAddress addr in GetIpAddresses())
				_ds.IpAddresses.Add(addr);

			_dm.Listener.OnBroadcastReceived += delegate(Broadcast bcast)
			{
				if (!VehiclesContains(bcast.Name))
					DataModel.Invoke(() => _ds.Vehicles.Add(new Vehicle(bcast)));
			};

			LoginSuccessEvent.Invoked += evnt => _selVehicle.Connected = true;

			InitializeComponent();
		}
	}
}
