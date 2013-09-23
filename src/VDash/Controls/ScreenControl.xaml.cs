/*
Copyright (C) 2013 Christopher Cartwright
    
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
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Aspects;
using TextBox = System.Windows.Controls.TextBox;

namespace VDash.Controls
{
	/// <summary>
	/// Interaction logic for ScreenControl.xaml
	/// </summary>
	public partial class ScreenControl
	{
		private class ArrayTextBox : TextBox
		{
			public int Column { get; set; }
			public int Row { get; set; }
		}

		private class DataSource : NotifyPropertyChanged
		{
			[Notify]
			public ObservableSquareArray<string> Screen { get; private set; }

			public DataSource()
			{
				Screen = new ObservableSquareArray<string>(MaxRows, MaxColumns);

				// ReSharper disable once ExplicitCallerInfoArgument
				Screen.ItemChanged += (sender, args) => OnPropertyChanged("Screen");
			}
		}

		public const short MaxRows = 2;
		public const short MaxColumns = 16;

		private readonly DataSource _ds;
		private readonly ArrayTextBox[,] _boxes;

		// Used to prevent cyclic calls when updating ds.Screen
		private bool _setting;

		public ScreenControl()
		{
			_ds = new DataSource();
			DataContext = _ds;
			_boxes = new ArrayTextBox[MaxColumns, MaxRows];

			InitializeComponent();

			Loaded += OnLoaded;
			_ds.Screen.ItemChanged += Screen_ItemChanged;
			_ds.Screen.CollectionChanged += ScreenOnCollectionChanged;
		}

		private void Screen_ItemChanged(object sender, ObservableSquareArray<string>.ItemChangedEventArgs e)
		{
			if (_setting)
				return;

			_boxes[e.Column, e.Row].Text = e.Value;
		}

		private void OnTextChanged(object o, TextChangedEventArgs e)
		{
			ArrayTextBox tb = (ArrayTextBox)o;
			_setting = true;
			_ds.Screen[tb.Column, tb.Row] = tb.Text;
			_setting = false;

			if (String.IsNullOrEmpty(tb.Text))
				return;

			int idx = PanelCharacter.Children.IndexOf(tb);
			if (idx + 1 >= PanelCharacter.Children.Count)
				return;

			PanelCharacter.Children[idx + 1].Focus();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			for (short x = 0; x < MaxColumns; x++)
			{
				for (short y = 0; y < MaxRows; y++)
				{
					_boxes[x, y] = new ArrayTextBox()
					{
						Width = PanelCharacter.ActualWidth / MaxColumns,
						Height = PanelCharacter.ActualHeight / MaxRows,
						HorizontalAlignment = HorizontalAlignment.Left,
						VerticalAlignment = VerticalAlignment.Top,
						HorizontalContentAlignment = HorizontalAlignment.Center,
						VerticalContentAlignment = VerticalAlignment.Center,
						MaxLength = 1,
						Column = x,
						Row = y
					};

					_boxes[x, y].TextChanged += OnTextChanged;
					_boxes[x, y].GotFocus += delegate(object o, RoutedEventArgs args) { ((TextBox)o).Text = ""; };

					PanelCharacter.Children.Add(_boxes[x, y]);
				}
			}
		}

		private void ScreenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action != NotifyCollectionChangedAction.Reset)
				return;

			for (int x = 0; x < MaxColumns; x++)
			{
				for (int y = 0; y < MaxRows; y++)
				{
					_boxes[x, y].Text = _ds.Screen[x, y];
				}
			}
		}

		private void ButtonClear_Click(object sender, RoutedEventArgs e)
		{
			_ds.Screen.SetAll(null);
		}

		private void ButtonSet_Click(object sender, RoutedEventArgs e)
		{
			string text = "";
			for (int x = 0; x < MaxColumns; x++)
			{
				for (int y = 0; y < MaxRows; y++)
					text += String.IsNullOrEmpty(_ds.Screen[x, y]) ? " " : _ds.Screen[x, y];
			}

			DataModel.Instance.ScreenText = text;
		}
	}
}
