using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Windows.Threading;
namespace LFM_CAM_FACE
{
    class CameraCaptura
    {
        private readonly RecognizerEngine recoEngine;
        private Capture camera;
        private CascadeClassifier faceDetected;
        
        private static CameraCaptura _instance = null;
        private Image<Gray, byte> rosto = null;
        private readonly DispatcherTimer timer = null;
        private int a = 0;
        private CameraCaptura()
        { 
            recoEngine = new RecognizerEngine(Directory.GetCurrentDirectory() + @"\facesDB.db", Directory.GetCurrentDirectory() + @"\RecognizerEngineData.YAML");
 faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalface_alt_tree.xml");
       
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);

        }
        public static CameraCaptura Instance()
        {
            if (_instance == null)
            {
                _instance = new CameraCaptura();
            }
            return _instance;
        }
        public void Iniciar(int i)
        {
            if (camera != null)
            {
                camera.Dispose();
                timer.Stop();
                camera = new Capture(i);
                timer.Start();
            }
            else
            {
                camera = new Capture(i);
                timer.Start();
                Treinar();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
           
            Reconhecerrostocamera();
        }
        public void Reconhecerrostocamera()
        {
            faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalface_alt_tree.xml");
            string txt = "";
            Image<Bgr, byte> imageFrame = camera.QueryFrame().ToImage<Bgr, Byte>();
            Variaveis.Principal().imageBox1.Image =imageFrame;
            IDBAccess dataStore = new DBAccess("facesDB.db");
            using (imageFrame)
            {
                if (imageFrame != null)
                {
                    var grayframe = imageFrame.Convert<Gray, byte>();
                    System.Drawing.Rectangle[] faces = faceDetected.DetectMultiScale(grayframe, 1.2, 10, Size.Empty);
                    foreach (System.Drawing.Rectangle face in faces)
                    {
                       
                        imageFrame.Draw(face, new Bgr(Color.BurlyWood), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them
                        rosto = imageFrame.GetSubRect(face).Convert<Gray, byte>();
                        if (File.Exists(Directory.GetCurrentDirectory() + @"\RecognizerEngineData.YAML"))
                        {
                            txt += dataStore.GetUsername(recoEngine.RecognizeUser(imageFrame.GetSubRect(face).Convert<Gray, byte>())) + " ";
                        }
                        else
                        {
                            txt += "Train the recognizer engine first !";
                        }
                    }
                 
                    if (faces.GetLength(0) > 0)
                    {
                        imageFrame.Draw(faces[0], new Bgr(System.Drawing.Color.Red), 3);
                    }  Variaveis.Principal().imageBox2.Image = rosto;
                Variaveis.Principal().textBox1.Text = txt;
                Variaveis.Principal().imageBox1.Image = imageFrame;
                }
              
            }
            Variaveis.Principal().imageBox2.Image = rosto;
            if (rosto == null)
            {
                a++;
                if (a == 1) { faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalface_alt2.xml"); }
                if (a == 2) { faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalcatface.xml"); }
                if (a == 3) { faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalface_alt.xml"); }
                if (a == 4) { faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalface_alt_tree.xml"); }
                if (a == 5) { faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalcatface_extended.xml"); }
                if (a == 6)
                {
                    faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalface_alt_tree.xml");
                    a = 0;
                }
             
            }
        }
        public void Salvar()
        {
            var faceToSave = rosto;
            timer.Stop();
            Byte[] file;
            IDBAccess dataStore = new DBAccess("facesDB.db");
            var frmSaveDialog = new FrmSaveDialog();
            if (frmSaveDialog.ShowDialog().Value == true)
            {
                if (Variaveis.IdentificationNumber.Trim() != String.Empty)
                {
                    var username = Variaveis.IdentificationNumber.Trim().ToLower();
                    var filePath = Directory.GetCurrentDirectory() + @"\" + username + ".bmp";
                    faceToSave.Save(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            file = reader.ReadBytes((int)stream.Length);
                        }
                    }
                    dataStore.SaveFace(username, file);
                    Variaveis.Principal().textBox2.Text = "Save Result " + Variaveis.IdentificationNumber;
                }
                Treinar();
            }
            timer.Start();
        }
        public void Treinar()
        {
            if (recoEngine.TrainRecognizer())
            {
                Variaveis.Principal().textBox2.Text = "Done";
            }
        }
        public void Parar()
        {
            if (camera != null)
            {
                camera.Dispose();
                timer.Stop();
            }
        }
        public void Buscarimagemarquivo()
        {
            a = 0;
            rosto = null;
            Parar();
            using (OpenFileDialog ofd = new OpenFileDialog { Multiselect = false, Filter = "JPEG|*.jpg" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Variaveis.Principal().textBox1.Text = ofd.FileName;
                    Imagembuscar(ofd.FileName);
                }
            }


        }
        int ih = 0;
        int iw = 0;

        public void Redimensionarimagem(int wid,int hei,int tama)
        {
            if (wid > hei)
            {
                if (wid > tama)
                {
                    ih = hei / (wid / tama);
                    iw = tama;
                }
            }
            if (wid < hei)
            {
                if (hei > tama)
                {
                    ih = tama;
                    iw = wid / (hei / tama);
                }
            }
        }
        public void Imagembuscar(string arquivo)
        {
            string txt = "";
            Graphics graphics;
            string nome = "";
            Image<Bgr, byte> faceImage = new Image<Bgr, byte>(arquivo);
            Redimensionarimagem(faceImage.Width, faceImage.Height,600);
        
            Variaveis.Principal().imageBox1.Image = new Image<Bgr, byte>(arquivo).Resize(iw, ih, Emgu.CV.CvEnum.Inter.Linear);
            Bitmap bitma = faceImage.ToBitmap();
            IDBAccess dataStore = new DBAccess("facesDB.db");
            using (faceImage)
            {
                if (faceImage != null)
                {
                    Rectangle[] faces = faceDetected.DetectMultiScale(faceImage, 1.1, 15); 
                    foreach (Rectangle face in faces)
                    {
                       
                        using (graphics = Graphics.FromImage(bitma))
                        {
                            using (Pen pen = new Pen(Color.Red, 4))
                            {
                                graphics.DrawRectangle(pen, face);
                            }
                        }
                        rosto = faceImage.GetSubRect(face).Convert<Gray, byte>();
                        if (File.Exists(Directory.GetCurrentDirectory() + @"\RecognizerEngineData.YAML"))
                        { nome= dataStore.GetUsername(recoEngine.RecognizeUser(faceImage.GetSubRect(face).Convert<Gray, byte>())) + " ";
                        
                          
                        }
                        else
                        {
                            nome += "Train the recognizer engine first !";
                        }
                      
                        if (rosto != null)
                        {

                            Variaveis.Principal().imageBox2.Image = rosto;
                            Bitmap bitmap = rosto.ToBitmap();
                          
                            Salvarimagem(nome);
                            Redimensionarimagem(rosto.Width, rosto.Height,200);
                            Variaveis.Principal().imageBox2.Image = rosto.Resize(iw, ih, Emgu.CV.CvEnum.Inter.Linear);
                        }
                        Variaveis.Principal().textBox1.Text = nome;
                        Redimensionarimagem(bitma.Width, bitma.Height,600);
                        Variaveis.Principal().imageBox1.Image = new Image<Bgr, byte>(bitma).Resize(iw, ih, Emgu.CV.CvEnum.Inter.Linear);
                        txt += nome;
                    }
                }
                Variaveis.Principal().textBox1.Text = txt;
            }
            if (rosto != null)
            {
                Redimensionarimagem(rosto.Width, rosto.Height,200);
                Variaveis.Principal().imageBox2.Image = rosto.Resize(iw, ih, Emgu.CV.CvEnum.Inter.Linear);
            }

            if (rosto == null)
            {
                a++;
                if (a == 1) { faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalface_alt2.xml"); }
                if (a == 2) { faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalcatface.xml"); }
                if (a == 3) { faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalface_alt.xml"); }
                if (a == 4) { faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalface_alt_tree.xml"); }
                if (a == 5) { faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalcatface_extended.xml"); }
                if (a == 6)
                {
                    faceDetected = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\haarcascade_frontalface_alt_tree.xml");
                    a = 7;
                }
                if (a < 7)
                {
                    Imagembuscar(arquivo);
                }
            }
        }
        public void Salvarimagem(string nome)
        {
            Variaveis.IdentificationNumber = nome;
               var faceToSave = rosto;
            Byte[] file;
            IDBAccess dataStore = new DBAccess("facesDB.db");
            var frmSaveDialog = new FrmSaveDialog();
            if (frmSaveDialog.ShowDialog().Value == true)
            {
                if (Variaveis.IdentificationNumber.Trim() != String.Empty)
                {
                    var username = Variaveis.IdentificationNumber.Trim().ToLower();
                    var filePath = Directory.GetCurrentDirectory() + @"\" + username + ".bmp";

                    faceToSave.Save(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            file = reader.ReadBytes((int)stream.Length);
                        }
                    }
                    dataStore.SaveFace(username, file);
                    Variaveis.Principal().textBox2.Text = "Save Result " + Variaveis.IdentificationNumber;
                }
                Treinar();
            }

        }
    }
}
