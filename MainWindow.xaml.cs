﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices.ComTypes;
using QB_WPF.Internals;
using WpfAnimatedGif;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Media.Animation;

namespace QB_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Serializable]
    public class LocationData
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class LocationFileManager
    {
        public static void SaveLocationToFile(int x, int y, string filePath)
        {
            LocationData data = new LocationData { X = x, Y = y };

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, data);
                }
                Console.WriteLine("Data saved to file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }

        public static LocationData ReadLocationFromFile(string filePath)
        {
            LocationData data = null;

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    IFormatter formatter = new BinaryFormatter();
                    data = (LocationData)formatter.Deserialize(fileStream);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading data: " + ex.Message);
            }

            return data;
        }
    }

    [Serializable]
    public class AppSettings
    {
        public bool IsMuted { get; set; }
    }

    public class SettingsManager
    {
        public static void SaveMuteStatus(bool isMuted, string filePath)
        {
            AppSettings settings = new AppSettings { IsMuted = isMuted };

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, settings);
                }
                Console.WriteLine("Mute status saved to file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving mute status: " + ex.Message);
            }
        }

        public static bool ReadMuteStatus(string filePath)
        {
            AppSettings settings = null;

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    IFormatter formatter = new BinaryFormatter();
                    settings = (AppSettings)formatter.Deserialize(fileStream);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading mute status: " + ex.Message);
            }

            return settings != null ? settings.IsMuted : false;
        }
    }

    public partial class MainWindow : Window
    {
        string datapath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\QB-desktop\\";
        public MainWindow()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            Directory.CreateDirectory(datapath);
            WriteResourceToFile("QB_res.zip", datapath + "QB_res.zip");
            using (var unzip = new Unzip(datapath + "QB_res.zip"))
            {
                // extract all files from zip archive to a directory
                unzip.ExtractToDirectory(datapath);
            }
            File.Delete(datapath + "QB_res.zip");

            InitializeComponent();

            string filePath = datapath+"location.dat";
            LocationData locationData = LocationFileManager.ReadLocationFromFile(filePath);
            if (locationData != null)
            {
                int x = locationData.X, y = locationData.Y;
                this.Top = x; this.Left = y;
            }

            filePath = datapath+"size.dat";
            locationData = LocationFileManager.ReadLocationFromFile(filePath);
            if (locationData != null)
            {
                int x = locationData.X, y = locationData.Y;
                this.Width = x; this.Height = y;
            }

            filePath = datapath + "mute.dat";
            bool mute = SettingsManager.ReadMuteStatus(filePath);
            player.IsMuted = mute;
            chkMute.IsChecked = mute;
        }

        private Thread fadeWin { get; set; }
        bool fading = false;
        bool winMoved = false;
        //bool muted = false;

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            winMoved = false;
            fading = false;
            fadeWin.Abort();
            this.DragMove();
        }

        List<int> lastPlayedList = new List<int>();
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (winMoved == false)
            {
                if (GridResize.Visibility == Visibility.Visible)
                {
                    GridResize.Visibility = Visibility.Hidden;
                    ResizeMode = ResizeMode.CanResize;
                }
                else
                {
                    string[] fileArray = Directory.GetFiles(datapath + "expression\\");
                    Random rnd = new Random();
                    int i;
                    while (true)
                    {
                        i = rnd.Next(0, fileArray.Length);
                        if (!lastPlayedList.Contains(i))
                        {
                            lastPlayedList.Add(i);
                            if (lastPlayedList.Count > 5)
                            {
                                lastPlayedList.RemoveAt(0);
                            }
                            break;
                        }
                    }
                    string videoFile = fileArray[i];
                    player.Source = new Uri(videoFile);
                    player.Visibility = Visibility.Visible;
                    player.Play();

                }
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            winMoved = true;
        }

        public void fadeTimeout()
        {
            Thread.Sleep(1000);
            for (double i = 1; i >= 0.3; i -= 0.035)
            {
                Dispatcher.BeginInvoke(new ThreadStart(() => {
                    img.Opacity = i;
                }));
                Thread.Sleep(10);
            }
            Dispatcher.BeginInvoke(new ThreadStart(() => {
                var windowHwnd = new WindowInteropHelper(this).Handle;
                //WindowsServices.SetWindowExTransparent(windowHwnd);
            }));
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!fading)
            {
                fadeWin = new Thread(fadeTimeout);
                //fadeWin.Start();
                fading = true;
            }
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            fading = false;
            fadeWin.Abort();
            img.Opacity = 1;
        }

        private void exit(object sender, RoutedEventArgs e)
        {
            string datapath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\QB-desktop\\";

            string filePath = datapath + "location.dat";
            int x = (int)this.Top, y = (int)this.Left;
            LocationFileManager.SaveLocationToFile(x, y, filePath);

            filePath = datapath + "size.dat";
            x = (int)this.Width; y= (int)this.Height;
            LocationFileManager.SaveLocationToFile(x, y, filePath);

            filePath = datapath + "mute.dat";
            bool mute = player.IsMuted;
            SettingsManager.SaveMuteStatus(mute, filePath);

            Application.Current.Shutdown();
        }

        private void mute(object sender, RoutedEventArgs e)
        {
            player.IsMuted = chkMute.IsChecked;
        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            player.Close();
            player.Visibility = Visibility.Hidden;
            img.Visibility = Visibility.Visible;
        }

        public void WriteResourceToFile(string resourceName, string fileName)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("QB_WPF.QB_res." + resourceName);
            FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            for (int i = 0; i < stream.Length; i++)
                fileStream.WriteByte((byte)stream.ReadByte());
            fileStream.Close();
        }

        private void MenuItem_GotFocus(object sender, RoutedEventArgs e)
        {
            if (idleList.IsSubmenuOpen == false)
            {
                idleList.Items.Clear();
                string[] fileArray = Directory.GetFiles(datapath + "idle\\");
                foreach (string file in fileArray)
                {
                    MenuItem newMenuItem = new MenuItem();
                    newMenuItem.Header = file.Substring(file.LastIndexOf("\\") + 1, file.LastIndexOf(".") - file.LastIndexOf("\\") - 1);
                    newMenuItem.Tag = file;
                    newMenuItem.Click += changeIdleImg;
                    idleList.Items.Add(newMenuItem);
                }
            }
        }

        private void changeIdleImg(object sender, EventArgs e)
        {
            string path = ((MenuItem)sender).Tag.ToString();
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(path);
            image.EndInit();
            ImageBehavior.SetAnimatedSource(img, image);
        }

        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            img.Visibility = Visibility.Hidden;
        }

        private void resize(object sender, RoutedEventArgs e)
        {
            ResizeMode = ResizeMode.CanResizeWithGrip;
            GridResize.Visibility = Visibility.Visible;
        }

        private void img_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
        }

        private void openFolder(object sender, RoutedEventArgs e)
        {
            Process.Start(datapath);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://space.bilibili.com/252913909");
            Process.Start("https://alexsd06.github.io/");
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/haojiezhe12345/Kyubey-desktop");
            Process.Start("https://github.com/alexsd06");
        }
    }

    public static class WindowsServices
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        public static void SetWindowExNotTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
        }
    }
}
