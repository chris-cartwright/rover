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
    public partial class VehicleKeyState : UserControl
    {
        public VehicleKeyState()
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
