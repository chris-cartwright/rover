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

using System.ComponentModel;
using MjpegProcessor;

namespace VDash.Controls
{
    /// <summary>
    /// Interaction logic for VehicleVideo.xaml
    /// </summary>
    public partial class VideoControl
    {
	    private readonly DataModel _dm = DataModel.Instance;
	    private readonly MjpegDecoder mjpeg;

	    private bool streaming;

		public VideoControl()
        {
			DataContext = _dm;
			_dm.PropertyChanged += DataModelOnPropertyChanged;

            InitializeComponent();

			mjpeg = new MjpegDecoder();
			mjpeg.FrameReady += MjpegOnFrameReady;
        }

	    private void DataModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	    {
		    if (e.PropertyName != "VideoFeed")
			{
				return;
			}

			if (streaming)
			{
				mjpeg.StopStream();
			}

			streaming = true;
			mjpeg.ParseStream(_dm.VideoFeed);
	    }

	    private void MjpegOnFrameReady(object sender, FrameReadyEventArgs e)
	    {
		    Image.Source = e.BitmapImage;
	    }
    }
}
