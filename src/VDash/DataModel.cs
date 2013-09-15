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
using System.Timers;
using System.Windows;
using Aspects;
using VDash.Controls;
using VDash.Properties;
using VehicleLib;
using VehicleLib.Events;
using VehicleLib.Queries;
using VehicleLib.Sensors;
using VehicleLib.States;

namespace VDash
{
	/// <summary>
	///     Represents the ViewModel for the car.
	///     Any global information pertaining to the vehicle should be included.
	/// </summary>
	internal class DataModel : NotifyPropertyChanged
	{
		/// <summary>
		///     Represents the current state of the front wheels
		/// </summary>
		public enum TurnDirection
		{
			Left,
			None,
			Right
		};

		private static DataModel _inst;

		/// <summary>
		///     Returns the static instance of this.
		///     Constructs object if required.
		/// </summary>
		/// <returns>Static instance</returns>
		public static DataModel GetInstance()
		{
			return _inst ?? (_inst = new DataModel());
		}

		/// <summary>
		///     Used to run an action on the GUI thread
		/// </summary>
		/// <param name="action">Action to run</param>
		public static void Invoke(Action action)
		{
			Application.Current.Dispatcher.BeginInvoke(action);
		}

		/// <summary>
		///     Used to update sensors
		/// </summary>
		private readonly Timer _timer;

		private ushort _headlights;
		private string _key;
		private short _speed;
		private TurnDirection _turn;
		private Uri _videoFeed;

		public VehiclePipe Vehicle { get; private set; }
		public BroadcastListener Listener { get; private set; }

		/// <summary>
		///     Sets the direction of turn for the vehicle.
		/// </summary>
		[Notify]
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

				if (!Vehicle.Connected)
					return;

				switch (_turn)
				{
				case TurnDirection.Left:
					Vehicle.Send(new LeftTurnState());
					break;
				case TurnDirection.Right:
					Vehicle.Send(new RightTurnState());
					break;
				default:
					Vehicle.Send(new TurnState());
					break;
				}
			}
		}

		/// <summary>
		///     Controls the speed of the vehicle.
		///     Accepts a range of +100 to -100.
		/// </summary>
		[Notify]
		public short Speed
		{
			get { return _speed; }
			set
			{
				if (value < -100)
					value = -100;

				if (value > 100)
					value = 100;

				LogControl.Debug("Speed set: " + value);

				_speed = value;

				if (Vehicle.Connected)
					Vehicle.Send(new MoveState(0, 0, _speed));
			}
		}

		/// <summary>
		///     Controls the brightness of the headlights.
		///     Set to 0 to turn off.
		/// </summary>
		[Notify]
		public ushort Headlights
		{
			get { return _headlights; }
			set
			{
				if (value > 100)
					value = 100;

				LogControl.Debug("Headlights set: " + value);

				_headlights = value;

				if (Vehicle.Connected)
					Vehicle.Send(new HeadLightState(_headlights));
			}
		}

		/// <summary>
		///     Minimum voltage for the battery
		/// </summary>
		[Notify]
		public float BatteryMin { get; private set; }

		/// <summary>
		///     Maximum voltage for the battery
		/// </summary>
		[Notify]
		public float BatteryMax { get; private set; }

		/// <summary>
		///     Current voltage of the battery
		/// </summary>
		[Notify]
		public float BatteryCurrent { get; private set; }

		/// <summary>
		///     Reports Last Key pressed
		/// </summary>
		[Notify]
		public string Key
		{
			get { return _key; }
			set
			{
				LogControl.Debug("Key pressed: " + value);

				_key = value;
			}
		}

		/// <summary>
		///     Reports when Video feed Uri is set
		/// </summary>
		[Notify]
		public Uri VideoFeed
		{
			get { return _videoFeed; }
			set
			{
				_videoFeed = value;
				LogControl.Debug("Video feed Uri set: " + value);
			}
		}

		/// <summary>
		///     Constructor. Sets default values for properties.
		/// </summary>
		private DataModel()
		{
			Vehicle = new VehiclePipe();
			Listener = new BroadcastListener();

			_turn = TurnDirection.None;

			PropertyChanged += dm_PropertyChanged;
			Vehicle.OnException += ex => Invoke(() => LogControl.Error(ex));
			Vehicle.OnError += err => Invoke(() => LogControl.Error(err.ToString()));
			Vehicle.OnDisconnect += () => _timer.Stop();

			LoginSuccessEvent.Invoked += delegate
			{
				_timer.Start();

				Vehicle.Send(new VoltageQuery("battery", sensor => Invoke(() => BatteryUpdated(sensor))));
			};

			_timer = new Timer(30000);
			_timer.Elapsed += delegate
			{
				if (!Vehicle.Connected)
					return;

				Vehicle.Send(new VoltageQuery("battery", sensor => Invoke(() => BatteryUpdated(sensor))));
			};
		}

		/// <summary>
		///     Event handler to responging to internal property changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Key")
				return;

			if (Key == Settings.Default.KeyForward)
				Speed += 10;
			else if (Key == Settings.Default.KeyBackward)
				Speed -= 10;
			else if (Key == Settings.Default.KeyLeft)
				Turn--;
			else if (Key == Settings.Default.KeyRight)
				Turn++;
			else if (Key == Settings.Default.KeyStop)
			{
				Speed = 0;
				Turn = TurnDirection.None;
			}
		}

		/// <summary>
		///     Shuts down all network connections and cleans up an resources.
		///     Should only be called on application exit. Behaviour is underfined if
		///     properties are accessed after this has been called.
		/// </summary>
		public void Shutdown()
		{
			// Kill Listener first so a reconnect isn't attempted on shutdown
			Listener.Shutdown();
			Vehicle.Shutdown();
		}

		/// <summary>
		///     Handles the incoming battery sensor information and updates Battery*
		/// </summary>
		/// <param name="sensor">Sensor information from vehicle</param>
		private void BatteryUpdated(Sensor sensor)
		{
			var vs = (VoltageSensor) sensor;

			BatteryCurrent = vs.Current;
			BatteryMax = vs.Max;
			BatteryMin = vs.Min;
		}
	}
}
