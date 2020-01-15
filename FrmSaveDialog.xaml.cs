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
    /// Lógica interna para FrmSaveDialog.xaml
    /// </summary>
    public partial class FrmSaveDialog : Window
    {
        public FrmSaveDialog()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Variaveis.IdentificationNumber = textBox1.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            textBox1.Text = Variaveis.IdentificationNumber;
        }
    }
}
