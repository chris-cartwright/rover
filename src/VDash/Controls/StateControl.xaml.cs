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
    /// Interaction logic for VehicleStateControl.xaml
    /// </summary>
    public partial class StateControl
    {
		public StateControl()
		{
			DataContext = DataModel.Instance;

			InitializeComponent();
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			DataModel dm = DataModel.Instance;
			dm.Speed = 0;
			dm.Turn = DataModel.TurnDirection.None;
		}
    }
}
