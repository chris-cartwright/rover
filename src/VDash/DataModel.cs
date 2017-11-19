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
using VDash.Controls;
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
	internal class DataModel : DataModelBase
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
		/// <value>Static instance</value>
		public static DataModel Instance => _inst ?? (_inst = new DataModel());

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
		private short _speed;
		private TurnDirection _turn;
		private Uri _videoFeed;
		private string _text;
		private float _batteryMin;
		private float _batteryMax;
		private float _batteryCurrent;
		private string _key;

		public VehiclePipe Vehicle { get; private set; }
		public BroadcastListener Listener { get; private set; }

		/// <summary>
		///     Sets the direction of turn for the vehicle.
		/// </summary>
		public TurnDirection Turn
		{
			get => _turn;
			set
			{
				SetField(ref _turn, value);
				if (_turn < TurnDirection.Left)
				{
					_turn = TurnDirection.Left;
				}
				else if (_turn > TurnDirection.Right)
				{
					_turn = TurnDirection.Right;
				}

				LogControl.Debug("Turn set: " + value);

				if (!Vehicle.Connected)
				{
					return;
				}

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
		public short Speed
		{
			get => _speed;
			set
			{
				if (value < -100)
				{
					value = -100;
				}

				if (value > 100)
				{
					value = 100;
				}

				LogControl.Debug("Speed set: " + value);

				SetField(ref _speed, value);

				if (Vehicle.Connected)
				{
					Vehicle.Send(new MoveState(0, 0, _speed));
				}
			}
		}

		/// <summary>
		///     Controls the brightness of the headlights.
		///     Set to 0 to turn off.
		/// </summary>
		public ushort Headlights
		{
			get => _headlights;
			set
			{
				if (value > 100)
				{
					value = 100;
				}

				SetField(ref _headlights, value);
			}
		}

		/// <summary>
		///     Minimum voltage for the battery
		/// </summary>
		public float BatteryMin
		{
			get => _batteryMin;
			private set => SetField(ref _batteryMin, value);
		}

		/// <summary>
		///     Maximum voltage for the battery
		/// </summary>
		public float BatteryMax
		{
			get => _batteryMax;
			private set => SetField(ref _batteryMax, value);
		}

		/// <summary>
		///     Current voltage of the battery
		/// </summary>
		public float BatteryCurrent
		{
			get => _batteryCurrent;
			private set => SetField(ref _batteryCurrent, value);
		}

		/// <summary>
		///     Reports Last Key pressed
		/// </summary>
		public string Key
		{
			get => _key;
			set => SetField(ref _key, value);
		}

		/// <summary>
		///     Reports when Video feed Uri is set
		/// </summary>
		public Uri VideoFeed
		{
			get => _videoFeed;
			set
			{
				SetField(ref _videoFeed, value);
				LogControl.Debug("Video feed Uri set: " + value);
			}
		}

		public string ScreenText
		{
			get => _text;
			set
			{
				if (value.Length != 32)
				{
					throw new FormatException("Length of string must be 32 characters");
				}

				SetField(ref _text, value);
				if (Vehicle.Connected)
				{
					Vehicle.Send(new ScreenState(_text));
				}
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
			Vehicle.OnConnect += bcast => VideoFeed = new Uri(bcast.VideoFeed);
			Vehicle.OnDisconnect += () => _timer.Stop();

			LoginSuccessEvent.Invoked += delegate
			{
				LogControl.Info("Connected.");

				_timer.Start();

				Vehicle.Send(new VoltageQuery("battery", sensor => Invoke(() => BatteryUpdated(sensor))));
			};

			_timer = new Timer(30000);
			_timer.Elapsed += delegate
			{
				if (!Vehicle.Connected)
				{
					return;
				}

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
			switch (e.PropertyName)
			{
				case "Headlights":
					LogControl.Debug("Headlights set: " + Headlights);
					if (Vehicle.Connected)
					{
						Vehicle.Send(new HeadLightState(Headlights));
					}

					break;
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
			var vs = (VoltageSensor)sensor;

			BatteryCurrent = vs.Current;
			BatteryMax = vs.Max;
			BatteryMin = vs.Min;
		}
	}
}
