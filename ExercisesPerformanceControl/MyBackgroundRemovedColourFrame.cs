using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect.Toolkit.BackgroundRemoval;

namespace ExercisesPerformanceControl
{
    /// <summary>
    /// Class for recording BackgroundRemovedColour data
    /// </summary>
    [Serializable]
    public class MyBackgroundRemovedColourFrame
    {
        /// <summary>
        /// height
        /// </summary>
        public int height;

        /// <summary>
        /// width
        /// </summary>
        public int width;

        /// <summary>
        /// pixel data
        /// </summary>
        public byte[] pixelData;

        /// <summary>
        /// Initializes a new instance of the MyBackgroundRemovedColourFrame class.
        /// </summary>
        public MyBackgroundRemovedColourFrame(BackgroundRemovedColorFrame Frame)
        {
            height = Frame.Height;
            width = Frame.Width;
            pixelData = new byte[Frame.GetRawPixelData().Length];
        }

        /// <summary>
        /// Initializes a new instance of the MyBackgroundRemovedColourFrame class.
        /// </summary>
        public MyBackgroundRemovedColourFrame()
        {
            height = 0;
            width = 0;
            pixelData = null;
        }
    }
}
