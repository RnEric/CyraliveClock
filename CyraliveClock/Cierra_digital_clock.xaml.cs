using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Timer = System.Timers.Timer;
using static CyraliveClock.GlobalFunction;
using System.IO;
using System.Text.RegularExpressions;

namespace CyraliveClock
{
    /// <summary>
    /// Cierra_digital_clock.xaml 的交互逻辑
    /// </summary>
    public partial class Cierra_digital_clock : Window
    {
        public Cierra_digital_clock()
        {
            InitializeComponent();
            setCierraclockDate();
            getCyraliveConfig = JsonNode.Parse(File.ReadAllText("CyraliveClock.json"));
            WindowStartupLocation = WindowStartupLocation.Manual;
            if (getCyraliveConfig["WindowXY"].ToString() != "")
            {
                get_position = Regex.Split(getCyraliveConfig["WindowXY"].ToString(), ",");
                Top = Convert.ToDouble(get_position[1]);
                Left = Convert.ToDouble(get_position[0]);
                Cierra_hold_position = true;
            }
            else
            {
                Left = SystemParameters.PrimaryScreenWidth - Width - SystemParameters.PrimaryScreenWidth * 0.05;
                Top = SystemParameters.PrimaryScreenHeight - Height - SystemParameters.PrimaryScreenHeight * 0.15;
            }
            if ((double)getCyraliveConfig["WindowSize"] != 0)
            {
                Height = (double)getCyraliveConfig["WindowSize"];
                Width = (double)getCyraliveConfig["WindowSize"] * 3.4;
            }
            if (!(bool)getCyraliveConfig["Topmost"])
            {
                Topmost = false;
            }
            if ((bool)getCyraliveConfig["Taskbar"])
            {
                ShowInTaskbar = true;
            }
            if ((bool)getCyraliveConfig["TransparentWindow"])
            {
                Background = Brushes.Transparent;
            }
            Timer timer = new Timer(1000);
            timer.Elapsed += (s, e) =>
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, () =>
                {
                    setCierraclockDate();
                });
            };
            timer.Enabled = true;
        }

        void setCierraclockDate()
        {
            Cierra_digital_clock_time.Text = DateTime.Now.ToString("T");
        }

        private void CCsettings_Click(object sender, RoutedEventArgs e)
        {
            new CyraliveClocksettings().Show();
        }

        private void CCclose_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.SizeAll;
            DragMove();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += (s, e2) =>
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, () =>
                {
                    if (Mouse.LeftButton == MouseButtonState.Released)
                    {
                        timer.Stop();
                        Cursor = Cursors.Arrow;
                        if (Cierra_hold_position)
                        {
                            if (Left != Convert.ToDouble(get_position[0]) || Top != Convert.ToDouble(get_position[1]))
                            {
                                write_config_file("WindowXY", Left + "," + Top);
                            }
                        }
                    }
                });
            };
            timer.Enabled = true;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += (s, e2) =>
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, () =>
                {
                    if (!stylechange)
                    {
                        Height = Height * 1;
                        Width = Height * 3.4;
                        if (WindowState == WindowState.Maximized)
                        {
                            WindowState = WindowState.Normal;
                        }
                        else if (Height > 150 || Width > 510)
                        {
                            Height = 50;
                            Width = 170;
                        }
                        if (Convert.ToDouble(read_config_file("WindowSize")) != 0)
                        {
                            write_config_file("WindowSize", Height);
                        }
                    }
                    else
                    {
                        stylechange = false;
                    }
                });
            };
            timer.Enabled = true;
        }
    }
}
