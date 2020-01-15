using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge;
using AForge.Video;
using AForge.Vision;
using AForge.Vision.Motion;
using System.Drawing;
using System.Windows.Forms;
namespace LFM_CAM_FACE
{
    class sensormovimento
    {
    public MotionDetector motionDetector = null;
        private static sensormovimento _instance;

        public static sensormovimento Instance()
        {
            if (_instance == null)
            {
                _instance = new sensormovimento();
            }
            return _instance;
        }

        public sensormovimento()
        {
           
        }
     
       
    }
}
