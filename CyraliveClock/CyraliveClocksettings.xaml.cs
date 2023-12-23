using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static CyraliveClock.GlobalFunction;

namespace CyraliveClock
{
    /// <summary>
    /// CyraliveClocksettings.xaml 的交互逻辑
    /// </summary>
    public partial class CyraliveClocksettings : Window
    {
        public CyraliveClocksettings()
        {
            InitializeComponent();
            object[] all_author = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            author_info.Text = "作者: " + ((AssemblyCompanyAttribute)all_author[0]).Company;
            ver_info.Text = "版本" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            getCyraliveConfig = JsonNode.Parse(File.ReadAllText("CyraliveClock.json"));
            WindowStartupLocation = WindowStartupLocation.Manual;
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    if (window.Left <= SystemParameters.PrimaryScreenWidth / 2)
                    {
                        Left = window.Left + window.Width + 5;
                    }
                    else
                    {
                        Left = window.Left - Width - 5;
                    }
                    if (window.Top <= SystemParameters.PrimaryScreenHeight / 2)
                    {
                        Top = window.Top;
                    }
                    else
                    {
                        Top = window.Top + window.Height - Height;
                    }
                }
            }
            if ((int)getCyraliveConfig["Clock"] != 0)
            {
                CC_style.SelectedIndex = (int)getCyraliveConfig["Clock"];
            }
            if (getCyraliveConfig["WindowXY"].ToString() != "")
            {
                CC_position.IsChecked = true;
            }
            if ((double)getCyraliveConfig["WindowSize"] != 0)
            {
                CC_size.IsChecked = true;
            }
            if (!(bool)getCyraliveConfig["Topmost"])
            {
                CC_topmost.IsChecked = false;
            }
            if ((bool)getCyraliveConfig["Taskbar"])
            {
                CC_taskbar.IsChecked = true;
            }
            if ((bool)getCyraliveConfig["TransparentWindow"])
            {
                CC_transparent_window_bg.IsChecked = true;
            }
        }

        private void CC_style_DropDownClosed(object sender, EventArgs e)
        {
            stylechange = true;
            write_config_file("Clock", CC_style.SelectedIndex);
            CC_size.IsChecked = false;
            write_config_file("WindowSize", 0);
            Application.Current.Shutdown();
            Process process = new Process();
            process.StartInfo.FileName = Assembly.GetExecutingAssembly().GetName().Name + ".exe";
            process.Start();
        }

        private void CC_position_Click(object sender, RoutedEventArgs e)
        {
            if (read_config_file("WindowXY") != "")
            {
                write_config_file("WindowXY", "");
                Cierra_hold_position = false;
            }
            else
            {
                write_config_file("WindowXY", Application.Current.MainWindow.Left + "," + Application.Current.MainWindow.Top);
                Cierra_hold_position = true;
                get_position = Regex.Split(read_config_file("WindowXY"), ",");
            }
        }

        private void CC_size_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToDouble(read_config_file("WindowSize")) != 0)
            {
                write_config_file("WindowSize", 0);
            }
            else if (Convert.ToDouble(read_config_file("WindowSize")) == 0)
            {
                write_config_file("WindowSize", Application.Current.MainWindow.Height);
            }
        }

        private void CC_topmost_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.Topmost)
            {
                Application.Current.MainWindow.Topmost = false;
                write_config_file("Topmost", false);
            }
            else
            {
                Application.Current.MainWindow.Topmost = true;
                write_config_file("Topmost", true);
            }
        }

        private void CC_taskbar_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.ShowInTaskbar)
            {
                Application.Current.MainWindow.ShowInTaskbar = false;
                write_config_file("Taskbar", false);
            }
            else
            {
                Application.Current.MainWindow.ShowInTaskbar = true;
                write_config_file("Taskbar", true);
            }
        }

        private void CC_transparent_window_bg_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.Background == Brushes.Transparent)
            {
                Application.Current.MainWindow.Background = (Brush)new BrushConverter().ConvertFromString("#01FFFFFF");
                write_config_file("TransparentWindow", false);
            }
            else
            {
                Application.Current.MainWindow.Background = Brushes.Transparent;
                write_config_file("TransparentWindow", true);
            }
        }
    }
}
