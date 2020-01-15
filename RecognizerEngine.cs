using System;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
namespace LFM_CAM_FACE
{
    class RecognizerEngine
    {
        private Emgu.CV.Face.EigenFaceRecognizer faceRecognizer;
        private DBAccess dbAccess;
        private String recognizerFilePath;

        public RecognizerEngine(String databasePath, String recognizerFilePath)
        {
            this.recognizerFilePath = recognizerFilePath;
            dbAccess = new DBAccess(databasePath);
            faceRecognizer = new Emgu.CV.Face.EigenFaceRecognizer(80, double.PositiveInfinity);
        }

        public bool TrainRecognizer()
        {
            var allFaces = dbAccess.CallFaces("ALL_USERS");
            if (allFaces != null)
            {
                var faceImages = new Image<Gray, byte>[allFaces.Count];
                var faceLabels = new int[allFaces.Count];
                for (int i = 0; i < allFaces.Count; i++)
                {
                    Stream stream = new MemoryStream();
                    stream.Write(allFaces[i].Image, 0, allFaces[i].Image.Length);
                    Image<Gray, byte> faceImage = new Image<Gray, byte>(new Bitmap(stream));
                    faceImages[i] = faceImage.Resize(100, 100, Inter.Cubic);
                    faceLabels[i] = allFaces[i].UserId;
                }
                faceRecognizer.Train(faceImages, faceLabels);
                faceRecognizer.Save(recognizerFilePath);
            }
            return true;

        }

        public void LoadRecognizerData()
        {
            faceRecognizer.Load(recognizerFilePath);
        }

        public int RecognizeUser(Image<Gray, byte> userImage)
        {
            faceRecognizer.Load(recognizerFilePath);

            var result = faceRecognizer.Predict(userImage.Resize(100, 100, Inter.Cubic));
            Variaveis. res = result.Distance;

            return result.Label;
        }
    }
}
