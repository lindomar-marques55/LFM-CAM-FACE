using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LFM_CAM_FACE
{

    /// <summary>
    /// Lógica interna para Esperawindow.xaml
    /// </summary>
    public partial class Esperawindow : Window
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public Esperawindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
       
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Variaveis.Camerafechada == true)
            {
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
   gifplayer.Play();
        }

        private void gifplayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            gifplayer.Stop();
            gifplayer.Position = new TimeSpan(0, 0, 1);
            gifplayer.Play();
        }
    }
}
