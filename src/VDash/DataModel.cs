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
using VehicleLib;
using VehicleLib.States;
using System.ComponentModel;
using System.Net;
using System.Windows.Controls;
using VehicleLib.Errors;

namespace VDash
{
	/// <summary>
	/// Represents the ViewModel for the car.
	/// Any global information pertaining to the vehicle should be included.
	/// </summary>
	internal class DataModel : INotifyPropertyChanged
	{
		private static DataModel _inst;

		/// <summary>
		/// Returns the static instance of this.
		/// Constructs object if required.
		/// </summary>
		/// <returns>Static instance</returns>
		public static DataModel GetInstance()
		{
			if (_inst == null)
				_inst = new DataModel();

			return _inst;
		}

		/// <summary>
		/// Represents the current state of the front wheels
		/// </summary>
		public enum TurnDirection { Left, None, Right };

		/// <summary>
		/// Fired when a property changes.
		/// Should cover all properties except:
		///		Vehicle
		///		Listener
		///	It just would not make sense to notify on updates to these objects.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		public VehiclePipe Vehicle = new VehiclePipe();
		public BroadcastListener Listener = new BroadcastListener();

		private TurnDirection _turn;

		/// <summary>
		/// Sets the direction of turn for the vehicle.
		/// </summary>
		public TurnDirection Turn
		{
			get { return _turn; }
			set
			{
				_turn = value;
				if (_turn < TurnDirection.Left)
					_turn = TurnDirection.Left;
				else if (_turn > TurnDirection.Right)
					_turn = TurnDirection.Right;

				LogControl.Debug("Turn set: " + value);

				Notify("Turn");

				if (Vehicle.Connected)
				{
					if (_turn == TurnDirection.Left)
						Vehicle.Send(new LeftTurnState());
					else if (_turn == TurnDirection.Right)
						Vehicle.Send(new RightTurnState());
					else
						Vehicle.Send(new TurnState());
				}
			}
		}

		private short _speed;

		/// <summary>
		/// Controls the speed of the vehicle.
		/// Accepts a range of +100 to -100.
		/// </summary>
		public short Speed
		{
			get { return _speed; }
			set
			{
				if (value < -100)
					value = -100;

				if (value > 100)
					value = 100;

				LogControl.Debug("Speed set: " + _speed);

				_speed = value;
				Notify("Speed");

				if (Vehicle.Connected)
				{
					Vehicle.Send(new MoveState(0, 0, _speed));
					//if (_speed < 0)
					//    Vehicle.Send(new BackwardMoveState((short)(_speed)));
					//else
					//    Vehicle.Send(new ForwardMoveState((short)_speed));
				}
			}
		}

		private string _key;
		/// <summary>
		/// Reports Last Key pressed 
		/// </summary>
		public string Key {
			get { return _key; }
			set {
				LogControl.Debug("Key pressed: " + value);

				_key = value;
				Notify("Key");
			}
		}
		
		private void dm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Key")
			{
				if (Key == Properties.Settings.Default.KeyForward)
					Speed += 10;
				else if (Key == Properties.Settings.Default.KeyBackward)
					Speed -= 10;
				else if (Key == Properties.Settings.Default.KeyLeft)
					Turn--;
				else if (Key == Properties.Settings.Default.KeyRight)
					Turn++;
				else if (Key == Properties.Settings.Default.KeyStop)
				{
					Speed = 0;
					Turn = DataModel.TurnDirection.None;
				}
			}
		}


		/// <summary>
		/// Constructor. Sets default values for properties.
		/// </summary>
		private DataModel()
		{
			_turn = TurnDirection.None;

			PropertyChanged += new PropertyChangedEventHandler(dm_PropertyChanged);

			Vehicle.OnException += delegate(Exception ex)
			{
				MainWindow.Invoke(() => LogControl.Error(ex));
			};

			Vehicle.OnError += delegate(Error err)
			{
				MainWindow.Invoke(() => LogControl.Error(err.ToString()));
			};

			LogControl.Info("Broadcast listener starting");
			Listener.Start(Convert.ToUInt16(Properties.Settings.Default.ListenPort));

#if DEBUG
			Listener.OnBroadcastReceived += delegate(string name, IPEndPoint ep)
			{
				if (!Vehicle.Connected)
				{
					LogControl.Debug("Attempting connection to " + ep.Address);
					Vehicle.Connect(ep, new Login("pwd"));
				}
			};
#endif
		}

		/// <summary>
		/// Shuts down all network connections and cleans up an resources.
		/// Should only be called on application exit. Behaviour is underfined if
		/// properties are accessed after this has been called.
		/// </summary>
		public void Shutdown()
		{
			// Kill Listener first so a reconnect isn't attempted on shutdown
			Listener.Shutdown();
			Vehicle.Shutdown();
		}

		/// <summary>
		/// Notifies any listeners that a property has changed.
		/// </summary>
		/// <param name="name">Name of the property that changed.</param>
		private void Notify(string name)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}
