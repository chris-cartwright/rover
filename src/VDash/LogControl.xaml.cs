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
	/// Interaction logic for LogControl.xaml
	/// </summary>
	public partial class LogControl : UserControl
	{
		public static void Error(string msg)
		{
			//
		}

		public static void Error(Exception ex)
		{
			//
		}

		public static void Warning(string msg)
		{
			//
		}

		public static void Info(string msg)
		{
			//
		}

		public static void Debug(string msg)
		{
			//
		}

		public LogControl()
		{
			InitializeComponent();
		}
	}
}
