﻿/*
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
using System.Windows.Input;

namespace VDash
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window
	{
		public string AboutText { get; set; }

		public AboutWindow()
		{
			AboutText = @"
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
				along with VDash.  If not, see <http://www.gnu.org/licenses/>.";

			InitializeComponent();
		}

		private void WindowClose(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}
	}
}
