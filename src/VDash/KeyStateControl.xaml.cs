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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VDash
{
    /// <summary>
    /// Interaction logic for VehicleKeyState.xaml
    /// </summary>
    public partial class KeyStateControl : UserControl
    {
        public KeyStateControl()
        {
            InitializeComponent();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().Equals(Properties.Settings.Default.KeyForward.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                label2.Content = "Moving Forward";
                textBox1.Text = "";
            }
            else if (e.Key.ToString().Equals(Properties.Settings.Default.KeyBackward.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                label2.Content = "Moving Backwards";
                textBox1.Text = "";
            }
            else if (e.Key.ToString().Equals(Properties.Settings.Default.KeyLeft.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                label2.Content = "Turning Left";
                textBox1.Text = "";
            }
            else if (e.Key.ToString().Equals(Properties.Settings.Default.KeyRight.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                label2.Content = "Turning Right";
                textBox1.Text = "";
            }
            else
            {
                label2.Content = "Invalid Movement Key";
                textBox1.Text = "";
            }
        }
    }
}
