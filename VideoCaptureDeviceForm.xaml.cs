using System;
using System.Windows;
using System.Windows.Controls;

using AForge.Video.DirectShow;
namespace LFM_CAM_FACE
{
    /// <summary>
    /// Lógica interna para VideoCaptureDeviceForm.xaml
    /// </summary>
    public partial class VideoCaptureDeviceForm : Window
    {
        FilterInfoCollection videoDevices;
        private string device;
        private int deviceint;
    
        public string VideoDevice
        {
            get { return device; }
        }

        public int Deviceint { get => deviceint; set => deviceint = value; }

        public VideoCaptureDeviceForm()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                    throw new ApplicationException();
                foreach (FilterInfo device in videoDevices)
                {
                    devicesCombo.Items.Add(device.Name);
                }
            }
            catch (ApplicationException)
            {
                devicesCombo.Items.Add("No local capture devices");
                devicesCombo.IsEnabled = false;
                okButton.IsEnabled = false;
            }

            devicesCombo.SelectedIndex = 0;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            device = videoDevices[devicesCombo.SelectedIndex].MonikerString;
            DialogResult = true;
        }

        private void devicesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            device = videoDevices[devicesCombo.SelectedIndex].MonikerString;
            Deviceint = devicesCombo.SelectedIndex;
        }
    }
}
