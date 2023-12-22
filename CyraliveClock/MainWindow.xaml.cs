using System.Globalization;
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
using System.Timers;
using System.Windows.Threading;
using Timer = System.Timers.Timer;
using System.IO;
using System.Text.Json.Nodes;
using System.Text.Json;
using static CyraliveClock.GlobalFunction;
using System.Text.RegularExpressions;

namespace CyraliveClock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists("CyraliveClock.json"))
            {
                JsonObject CSconfig = new JsonObject
                {
                    { "Clock", 0 },
                    { "Topmost", true },
                    { "Taskbar", false },
                    { "TransparentWindow", false },
                    { "WindowSize", 0 },
                    { "WindowXY", "" }
                };
                StreamWriter streamWriter = File.CreateText("CyraliveClock.json");
                streamWriter.WriteLine(JsonSerializer.Serialize(CSconfig, new JsonSerializerOptions { WriteIndented = true }));
                streamWriter.Close();
            }
            getCyraliveConfig = JsonNode.Parse(File.ReadAllText("CyraliveClock.json"));
            setCierraclockDate();
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
            if ((int)getCyraliveConfig["Clock"] != 0)
            {
                if ((double)getCyraliveConfig["WindowSize"] == 0)
                {
                    Height = 50;
                    Width = 170;
                }
                Cierra_analog_clock.Visibility = Visibility.Hidden;
                Cierra_digital_clock.Visibility = Visibility.Visible;
            }
            if ((double)getCyraliveConfig["WindowSize"] != 0)
            {
                Height = (double)getCyraliveConfig["WindowSize"];
                if ((int)getCyraliveConfig["Clock"] != 0)
                {
                    Width = Height * 3.4;
                }
                else
                {
                    Width = (int)getCyraliveConfig["WindowSize"];
                }
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
            secondHand.Angle = DateTime.Now.Second * 6 + 180;
            minuteHand.Angle = DateTime.Now.Minute * 6 + 180;
            hourHand.Angle = (DateTime.Now.Hour * 30) + (DateTime.Now.Minute * 0.5) + 180;
            Cierra_clock_day.Text = DateTime.Now.Day.ToString();
            Cierra_clock_week.Text = DateTime.Now.ToString("ddd", CultureInfo.CreateSpecificCulture("en_US")).ToUpper();
            Cierra_clock_month.Text = DateTime.Now.ToString("MMM", CultureInfo.CreateSpecificCulture("en_US")).ToUpper();
            if (DateTime.Now.Hour > 17)
            {
                Cierra_clock_sun_moon.Text = "☽";
            }
            Cierra_digital_clock_time.Text = DateTime.Now.ToString("T");
        }

        private void CCclose_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.Close();
            }
        }

        private void CCsettings_Click(object sender, RoutedEventArgs e)
        {
            new CyraliveClocksettings().Show();
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
                        if (Cierra_analog_clock.Visibility == Visibility.Visible)
                        {
                            Height = Height * 1;
                            Width = Height * 1;
                        }
                        else
                        {
                            Height = Width / 3.4;
                            Width = Height * 3.4;
                        }
                        if (WindowState == WindowState.Maximized)
                        {
                            WindowState = WindowState.Normal;
                        }
                        else if (Height > 450 || Width > 450)
                        {
                            if (Cierra_analog_clock.Visibility == Visibility.Visible)
                            {
                                Height = 130;
                                Width = 130;
                            }
                            else
                            {
                                Height = 50;
                                Width = 170;
                            }
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