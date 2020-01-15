using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging;
namespace LFM_CAM_FACE
{
    public class cameras
    {
        VideoCaptureDevice videoSource;
        AsyncVideoSource asyncVideoSource = null;
        MotionDetector detector = new MotionDetector(new SimpleBackgroundModelingDetector(), new BlobCountingObjectsProcessing());
        private float motionAlarmLevel = 0.2f;

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();


        private static cameras _instance;

        public static cameras Instance()
        {
            if (_instance == null)
            {
                _instance = new cameras();
            }
            return _instance;
        }

        public cameras()
        {
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (asyncVideoSource != null)
            {
                if (asyncVideoSource.IsRunning == true) { Variaveis.Camerafechada = false; }
                if (asyncVideoSource.IsRunning == false) { Variaveis.Camerafechada = true; }
            }
        }

        public void selecionarcameras()
        {
            try
            {
                VideoCaptureDeviceForm videoCaptureDeviceForm = new VideoCaptureDeviceForm();
                videoCaptureDeviceForm.Owner = Variaveis.Principal();
                videoCaptureDeviceForm.ShowDialog();
                if (videoCaptureDeviceForm.DialogResult == true)
                {
                    videoSource = new VideoCaptureDevice(videoCaptureDeviceForm.VideoDevice);
                    OpenVideoSource(videoSource);
                }
                else if (videoCaptureDeviceForm.DialogResult == false)
                {
                    System.Windows.Forms.MessageBox.Show("User clicked Cancel");
                }
            }
            catch { }
        }
        private void OpenVideoSource(VideoCaptureDevice source)
        {
            Variaveis.Principal().Cursor = System.Windows.Input.Cursors.Wait;

            CloseVideoSource();
            asyncVideoSource = new AsyncVideoSource(source);
            asyncVideoSource.NewFrame += AsyncVideoSource_NewFrame;
            asyncVideoSource.Start();
            videoSource = source;
            Variaveis.Principal().Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void AsyncVideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            lock (this)
            {
                if (detector != null)
                {
                    Bitmap temp = (Bitmap)eventArgs.Frame.Clone();

                    Bitmap templ = (Bitmap)eventArgs.Frame.Clone();

                    float motionLevel = detector.ProcessFrame(templ);
                    MainWindow.main.Status = motionLevel.ToString();

                    if (motionLevel > motionAlarmLevel)
                    {  
                        BlobCounter blobCounter1 = new BlobCounter();
                        blobCounter1.ProcessImage(temp);
                        Rectangle[] rects = blobCounter1.GetObjectsRectangles();
                        foreach (Rectangle recs in rects)
                            if (rects.Length > 0)
                            {
                                if (recs.Width > 100)
                                {
                                    if (recs.Height > 100)
                                    { 
                                        Graphics g = Graphics.FromImage(temp);
                                        using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 3))
                                        {
                                            g.DrawRectangle(pen, recs);
                                            var rosto = temp.Clone(recs, temp.PixelFormat);
                                            MainWindow.main.Statusa = rosto;
                                        }
                                        g.Dispose();
                                    }
                                }

                            }
                    }

                }
            }
        }

        public void CloseVideoSource()
        {

            Variaveis.Principal().Cursor = System.Windows.Input.Cursors.Wait;
            if (asyncVideoSource != null)
            {
                // 
                asyncVideoSource.NewFrame -= AsyncVideoSource_NewFrame;
                asyncVideoSource.SignalToStop();
                if (asyncVideoSource.IsRunning == true)
                {
                    var esperawindow = new Esperawindow();
                    esperawindow.Owner = Variaveis.Principal();
                    esperawindow.ShowDialog();
                }
                for (int i = 0; (i < 50) && (asyncVideoSource.IsRunning == true); i++)
                {
                    if (asyncVideoSource.IsRunning != true) { break; }


                    if (asyncVideoSource.IsRunning == true) { MessageBox.Show("OK"); }
                }
                if (asyncVideoSource.IsRunning == true) { asyncVideoSource.Stop(); }

                if (detector != null) { detector.Reset(); }
            }

            Variaveis.Principal().Cursor = System.Windows.Input.Cursors.Arrow;
        }





    }
}
