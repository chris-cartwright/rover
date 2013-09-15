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
using System.Windows.Controls;

namespace VDash.Controls
{
    /// <summary>
    /// Interaction logic for VehicleKeyState.xaml
    /// </summary>
    public partial class KeyStateControl : UserControl
    {
		DataModel dm = DataModel.GetInstance();
        double timeOffset = 0.0;
        DateTime currentTime = DateTime.Now;
        DateTime lastCmdTime;
        List<Record> records = new List<Record>();
        public KeyStateControl()
        {
			this.DataContext = dm;

			dm.PropertyChanged += new PropertyChangedEventHandler(dm_PropertyChanged);

			InitializeComponent();
        }

		void dm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
            // if you are recording
            if (cbRecord.IsChecked.Value)
            {
                lastCmdTime = currentTime;
                currentTime = DateTime.Now;
                Record rec = new Record();
                PropertyInfo pi = dm.GetType().GetProperty(e.PropertyName);
                rec.setValue(pi.GetValue(dm, null));
                rec.setName(e.PropertyName);
                if (timeOffset == 0.0)
                {
                    rec.setTimeOffset(timeOffset);
                    timeOffset = 1.0;  //just random number so that it doesn't enter this if after the first run
                }
                else
                {
                    TimeSpan diff = currentTime.Subtract(lastCmdTime);
                    timeOffset = diff.Seconds + diff.Milliseconds;
                }
                records.Add(rec);
            }

			if (e.PropertyName == "Speed")
			{
				if (dm.Speed > 0)
				{
					label2.Content = "Moving Forward";
				}
				else if (dm.Speed < 0)
				{
					label2.Content = "Moving Backwards";
				}
				else
				{
					label2.Content = "Stopped";
				}
			}
			else if (e.PropertyName == "Turn")
			{
				if (dm.Turn == DataModel.TurnDirection.Left)
				{
					label4.Content = "Turning Left";
				}
				else if (dm.Turn == DataModel.TurnDirection.Right)
				{
					label4.Content = "Turning Right";
				}
				else
				{
					label4.Content = "Straight";
				}
			}
		}

        private void cbRecord_Checked(object sender, RoutedEventArgs e)
        {
            btnSaveRecord.IsEnabled = true;
        }

        private void btnSaveRecord_Click(object sender, RoutedEventArgs e)
        {
            //write the Record List to an xml file
        }

		//private void OnKeyDownHandler(object sender, KeyEventArgs e)
		//{
		//    DataModel dm = DataModel.GetInstance();

		//    if (dm.Speed > 0)
		//    {
		//        label2.Content = "Moving Forward";
		//    }
		//    else if (dm.Speed < 0)
		//    {
		//        label2.Content = "Moving Backwards";
		//    }
		//    else 
		//    {
		//        label2.Content = "Stopped";
		//    }

		//    if (e.Key.ToString().Equals(Properties.Settings.Default.KeyForward.ToString(), StringComparison.InvariantCultureIgnoreCase))
		//    {
		//        textBox1.Text = "";
		//    }
		//    else if (e.Key.ToString().Equals(Properties.Settings.Default.KeyBackward.ToString(), StringComparison.InvariantCultureIgnoreCase))
		//    {               
		//        textBox1.Text = "";
		//    }
		//    else if (e.Key.ToString().Equals(Properties.Settings.Default.KeyLeft.ToString(), StringComparison.InvariantCultureIgnoreCase))
		//    {
		//        label2.Content = "Turning Left";
		//        textBox1.Text = "";
		//    }
		//    else if (e.Key.ToString().Equals(Properties.Settings.Default.KeyRight.ToString(), StringComparison.InvariantCultureIgnoreCase))
		//    {
		//        label2.Content = "Turning Right";
		//        textBox1.Text = "";
		//    }
		//    else
		//    {
		//        label2.Content = "Invalid Movement Key";
		//        textBox1.Text = "";
		//    }
		//}
    }
}
