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

using System.Windows;

namespace VDash.Controls
{
    /// <summary>
    /// Interaction logic for MenuControl.xaml
    /// </summary>
    public partial class MenuControl
    {
        public MenuControl()
        {
            InitializeComponent();
        }

		private void KeyBindings_Click(object sender, RoutedEventArgs e)
		{
			var kbw = new KeyBindingsWindow {Owner = Window.GetWindow(this)};
			kbw.Show();
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			var aw = new AboutWindow {Owner = Window.GetWindow(this)};
			aw.Show();
		}

		private void Help_Click(object sender, RoutedEventArgs e)
		{
			var hw = new HelpWindow {Owner = Window.GetWindow(this)};
			hw.Show();
		}
    }
}
