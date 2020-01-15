using System.Windows;
namespace LFM_CAM_FACE
{
    public static class Variaveis
    {
        public static MainWindow maina = null;
        public static MainWindow Principal()
        {

            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    maina = (window as MainWindow);
                }
            }
            return maina;
        }
        private static string identificationNumber = string.Empty;
        public static string IdentificationNumber { get => identificationNumber; set => identificationNumber = value; }
        public static double res = 0;

        public static bool Camerafechada = false;

    }
}
