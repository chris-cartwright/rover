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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace VDash
{
	public class ObservableSquareArray<T> : INotifyCollectionChanged, IEnumerable<ICollection<T>>
	{
		public class ItemChangedEventArgs
		{
			public int Row { get; private set; }
			public int Column { get; private set; }
			public T Value { get; private set; }

			public ItemChangedEventArgs(int column, int row, T value)
			{
				Row = row;
				Column = column;
				Value = value;
			}
		}

		public delegate void NotifyItemChangedEventHandler(object sender, ItemChangedEventArgs e);

		public event NotifyItemChangedEventHandler ItemChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private T[,] _collection;
		private int _rows;
		private int _columns;

		public int Rows
		{
			get => _rows;
			set
			{
				if (_rows == value)
				{
					return;
				}

				_rows = value;
				_collection = new T[Columns, Rows];
				Notify(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		public int Columns
		{
			get => _columns;
			set
			{
				if (_columns == value)
				{
					return;
				}

				_columns = value;
				_collection = new T[Columns, Rows];
				Notify(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		public int Count => Rows * Columns;

		public T this[int x, int y]
		{
			get => _collection[x, y];
			set
			{
				var old = _collection[x, y];
				_collection[x, y] = value;
				Notify(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, old));

				ItemChanged?.Invoke(this, new ItemChangedEventArgs(x, y, value));
			}
		}

		private void Iterate(Action<int, int> action)
		{
			for (var x = 0; x < Columns; x++)
			{
				for (var y = 0; y < Rows; y++)
				{
					action(x, y);
				}
			}
		}

		private void Notify(NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged?.Invoke(this, e);
		}

		public ObservableSquareArray()
		{
			_collection = new T[0, 0];
		}

		public ObservableSquareArray(int rows, int columns)
			: this()
		{
			Rows = rows;
			Columns = columns;
		}

		public ObservableSquareArray(int rows, int columns, T defaultValue)
			: this(rows, columns)
		{
			Iterate((x, y) => _collection[x, y] = defaultValue);
		}

		public void SetAll(T value)
		{
			Iterate((x, y) => _collection[x, y] = value);
			Notify(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void Clear()
		{
			SetAll(default(T));
		}

		public IEnumerator<ICollection<T>> GetEnumerator()
		{
			for (var x = 0; x < Columns; x++)
			{
				var row = new List<T>();
				for (var y = 0; y < Rows; y++)
				{
					row.Add(_collection[x, y]);
				}

				yield return row;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
