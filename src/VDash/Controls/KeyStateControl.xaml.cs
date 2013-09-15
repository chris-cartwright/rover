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
using System.Reflection;
using System.Windows;

namespace VDash.Controls
{
    /// <summary>
    /// Interaction logic for VehicleKeyState.xaml
    /// </summary>
    public partial class KeyStateControl
    {
	    private readonly DataModel _dm = DataModel.GetInstance();
		private readonly List<Record> _records = new List<Record>();

		private double _timeOffset;
		private DateTime _currentTime = DateTime.Now;
		private DateTime _lastCmdTime;

        public KeyStateControl()
        {
			DataContext = _dm;
			_dm.PropertyChanged += dm_PropertyChanged;

			InitializeComponent();
        }

		void dm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// if you are recording
            if (CbRecord.IsChecked != null && CbRecord.IsChecked.Value)
            {
                _lastCmdTime = _currentTime;
                _currentTime = DateTime.Now;
                Record rec = new Record();
                PropertyInfo pi = _dm.GetType().GetProperty(e.PropertyName);
                rec.Value = pi.GetValue(_dm, null);
                rec.Name = e.PropertyName;
                if (_timeOffset < Single.Epsilon)
                {
                    rec.TimeOffset = _timeOffset;
                    _timeOffset = 1.0;  //just random number so that it doesn't enter this if after the first run
                }
                else
                {
                    TimeSpan diff = _currentTime.Subtract(_lastCmdTime);
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
