using System.Windows;
using AForge.Video.DirectShow;
using AForge.Video;
using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using AForge.Vision.Motion;

namespace LFM_CAM_FACE
{

    public partial class MainWindow : Window
    { 
     
        int i = 0;

        public MainWindow()
        {
       
            InitializeComponent();
            imageBox1.Width = (int)primeirohost.Width;
            imageBox1.Height = (int)primeirohost.Height;
            imageBox2.Width = (int)segundohost.Width;
            imageBox2.Height = (int)segundohost.Height;
            main = this;
           
        }

 
        internal static MainWindow main;
        internal string Status
        {
            get { return textBox1_Copy.Content.ToString(); }
            set { Dispatcher.Invoke(new Action(() => { textBox1_Copy.Content = value; })); }
        }
        internal System.Drawing.Image Statusa
        {
            get { return picturebox1.Image; }
            set { Dispatcher.Invoke(new Action(() => { picturebox1.Image = value; })); }
        }
        private void Iniciar_Click(object sender, RoutedEventArgs e)
        {
          
            CameraCaptura.Instance().Iniciar(i);
        }


        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptura.Instance().Salvar();
        }

        private void Treinar_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptura.Instance().Treinar();
        }

        private void Parar_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptura.Instance().Parar();
        }

        private void Imagem_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptura.Instance().Buscarimagemarquivo();
        }

        private void Salvar_imagem_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptura.Instance().Salvarimagem(Variaveis.IdentificationNumber); ;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cameras.Instance().CloseVideoSource();
        }
        private void btnselecionarcamer_Click(object sender, RoutedEventArgs e)
        {
            cameras.Instance().selecionarcameras();
        }
     
       
    }
}




