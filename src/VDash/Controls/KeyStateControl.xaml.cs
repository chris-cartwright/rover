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
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using VDash.Properties;

namespace VDash.Controls
{
	/// <summary>
	/// Interaction logic for VehicleKeyState.xaml
	/// </summary>
	public partial class KeyStateControl
	{
		private class KeyHandler
		{
			private readonly Timer _speedTimer;

			private short _speed;
			private DataModel.TurnDirection _turn;

			private bool _forward;
			private bool _reverse;
			private bool _left;
			private bool _right;

			public bool Forward
			{
				get => _forward;
				set
				{
					if (_forward && value)
					{
						return;
					}

					_forward = value;
					UpdateSpeed();
				}
			}

			public bool Reverse
			{
				get => _reverse;
				set
				{
					if (_reverse && value)
					{
						return;
					}

					_reverse = value;
					UpdateSpeed();
				}
			}

			public bool Left
			{
				get => _left;
				set
				{
					if (_left && value)
					{
						return;
					}

					_left = value;
					UpdateTurn();
				}
			}

			public bool Right
			{
				get => _right;
				set
				{
					if (_right && value)
					{
						return;
					}

					_right = value;
					UpdateTurn();
				}
			}

			public bool Ramping { get; set; }

			private void UpdateSpeed()
			{
				if (_forward && _reverse)
				{
					_speed = 0;
				}
				else if (_forward)
				{
					_speed += 10;
				}
				else if (_reverse)
				{
					_speed -= 10;
				}
				else
				{
					_speed = 0;
				}

				DataModel.Instance.Speed = _speed;
				_speedTimer.Enabled = _speed != 0;
			}

			private void UpdateTurn()
			{
				if (_left && _right)
				{
					_turn = DataModel.TurnDirection.None;
				}
				else if(_left)
				{
					_turn = DataModel.TurnDirection.Left;
				}
				else if(_right)
				{
					_turn = DataModel.TurnDirection.Right;
				}
				else
				{
					_turn = DataModel.TurnDirection.None;
				}

				DataModel.Instance.Turn = _turn;
			}

			public KeyHandler()
			{
				_speedTimer = new Timer(250);
				_speedTimer.Elapsed += (sender, args) =>
				{
					if (Ramping)
					{
						UpdateSpeed();
					}
				};
			}

			public void Stop()
			{
				_speed = 0;
				_turn = DataModel.TurnDirection.None;
				DataModel.Instance.Speed = 0;
				DataModel.Instance.Turn = DataModel.TurnDirection.None;
			}
		}

		private readonly DataModel _dm = DataModel.Instance;
		private readonly List<Record> _records = new List<Record>();
		private readonly KeyHandler _keys;

		private double _timeOffset;
		private DateTime _currentTime = DateTime.Now;
		private DateTime _lastCmdTime;

		public KeyStateControl()
		{
			DataContext = _dm;
			_dm.PropertyChanged += dm_PropertyChanged;

			_keys = new KeyHandler();

			InitializeComponent();

			Loaded += KeyStateControl_Loaded;
		}

		void KeyStateControl_Loaded(object sender, RoutedEventArgs e)
		{
			MainWindow.Self.PreviewKeyDown += OnPreviewKeyDown;
			MainWindow.Self.PreviewKeyUp += OnPreviewKeyUp;
		}

		private void OnPreviewKeyUp(object sender, KeyEventArgs e)
		{
			var key = e.Key.ToString().ToLower();
			if (key == Settings.Default.KeyForward)
			{
				_keys.Forward = false;
			}

			if (key == Settings.Default.KeyBackward)
			{
				_keys.Reverse = false;
			}

			if (key == Settings.Default.KeyLeft)
			{
				_keys.Left = false;
			}

			if (key == Settings.Default.KeyRight)
			{
				_keys.Right = false;
			}

			if (key == Settings.Default.KeyRamping)
			{
				_keys.Ramping = false;
			}
		}

		private void OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			var key = e.Key.ToString().ToLower();
			if (key == Settings.Default.KeyForward)
			{
				_keys.Forward = true;
			}

			if (key == Settings.Default.KeyBackward)
			{
				_keys.Reverse = true;
			}

			if (key == Settings.Default.KeyLeft)
			{
				_keys.Left = true;
			}

			if (key == Settings.Default.KeyRight)
			{
				_keys.Right = true;
			}

			if (key == Settings.Default.KeyStop)
			{
				_keys.Stop();
			}

			if (key == Settings.Default.KeyRamping)
			{
				_keys.Ramping = true;
			}
		}

		void dm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// if you are recording
			if (CbRecord.IsChecked != null && CbRecord.IsChecked.Value)
			{
				_lastCmdTime = _currentTime;
				_currentTime = DateTime.Now;
				var rec = new Record();
				var pi = _dm.GetType().GetProperty(e.PropertyName);
				rec.Value = pi.GetValue(_dm, null);
				rec.Name = e.PropertyName;
				if (_timeOffset < float.Epsilon)
				{
					rec.TimeOffset = _timeOffset;
					_timeOffset = 1.0;  //just random number so that it doesn't enter this if after the first run
				}
				else
				{
					var diff = _currentTime.Subtract(_lastCmdTime);
					_timeOffset = diff.Seconds + diff.Milliseconds;
				}

				_records.Add(rec);
			}

			switch (e.PropertyName)
			{
			case "Speed":
				if (_dm.Speed > 0)
				{
					LabelState.Content = "Moving Forward";
				}
				else if (_dm.Speed < 0)
				{
					LabelState.Content = "Moving Backwards";
				}
				else
				{
					LabelState.Content = "Stopped";
				}

				break;

			case "Turn":
				switch (_dm.Turn)
				{
				case DataModel.TurnDirection.Left:
					LabelTurn.Content = "Turning Left";
					break;
				case DataModel.TurnDirection.Right:
					LabelTurn.Content = "Turning Right";
					break;
				default:
					LabelTurn.Content = "Straight";
					break;
				}

				break;
			}
		}

		private void cbRecord_Checked(object sender, RoutedEventArgs e)
		{
			BtnSaveRecord.IsEnabled = true;
		}

		private void btnSaveRecord_Click(object sender, RoutedEventArgs e)
		{
			//write the Record List to an xml file
		}
	}
}
