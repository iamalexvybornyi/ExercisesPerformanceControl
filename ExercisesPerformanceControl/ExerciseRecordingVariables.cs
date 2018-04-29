using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Windows.Threading;
using Microsoft.Kinect.Toolkit.BackgroundRemoval;
using AForge.Video.FFMPEG;
using System.Text.RegularExpressions;

namespace ExercisesPerformanceControl
{
    public partial class ExerciseRecording : Window
    {
        /// <summary>
        /// Intermediate storage for the skeleton data received from the sensor
        /// </summary>
        private Skeleton[] skeletons;

        /// <summary>
        /// Id of the currently tracked skeleton
        /// </summary>
        private int currentlyTrackedSkeletonId;

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap foregroundBitmap;

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;


        /// <summary>
        /// Flag with info about whether the skeleton is tracked or not
        /// </summary>
        bool skelIsTracked = false;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Name of an exercise
        /// </summary>
        public string ExText = "";

        /// <summary>
        /// List for skeleton data
        /// </summary>
        private List<Skeleton> skelList = new List<Skeleton>();

        private List<Skeleton> skelListUser = new List<Skeleton>();

        /// <summary>
        /// Skeleton data list to record a movement
        /// </summary>
        private List<Skeleton> skelListForRecording = new List<Skeleton>();

        /// <summary>
        /// Skeleton data list to record a movement
        /// </summary>
        private List<MyBackgroundRemovedColourFrame> bitmapListForRecording = new List<MyBackgroundRemovedColourFrame>();

        /// <summary>
        /// Flag with info about whether the Silhouette data was recorded or not
        /// </summary>
        bool silWritten = false;

        /// <summary>
        /// Foreground bitmap for recorded data
        /// </summary>
        private WriteableBitmap foregroundBitmapForRec;

        /// <summary>
        /// List for angles
        /// </summary>
        private List<List<double>> anglesLists = new List<List<double>>();

        /// <summary>
        /// Amount of counted reps
        /// </summary>
        int numberOfReps = 0;

        /// <summary>
        /// Flag with an info about whether recording was finished or not
        /// </summary>
        private bool Written = true;

        /// <summary>
        /// Index of the exercise's current frame
        /// </summary>
        int index = 0;
        int indexUserData = 0;

        /// <summary>
        /// Total amount of frames in the exercise
        /// </summary>
        public int frames = 149;
        public int framesUserData = 150;

        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 10);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 10);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton live data
        /// </summary>
        private DrawingGroup drawingGroupForLiveData;

        /// <summary>
        /// Drawing group for skeleton recorded data
        /// </summary>
        private DrawingGroup drawingGroupForRecordedData;

        /// <summary>
        /// Drawing image that we will display for live data
        /// </summary>
        private DrawingImage imageSourceForLiveData;

        /// <summary>
        /// Drawing image that we will display for recorded data
        /// </summary>
        private DrawingImage imageSourceForRecordedData;

        /// <summary>
        /// Library which does background 
        /// </summary>
        private BackgroundRemovedColorStream backgroundRemovedColorStream;

        /// <summary>
        /// Track whether Dispose has been called
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Var for the name of the exercise
        /// </summary>
        public string ExName = "";

        ExerciseType exerciseType;

        DispatcherTimer timer = new DispatcherTimer();

        List<List<Point>> pointsList = new List<List<Point>>();
        List<List<Point>> pointsListUser = new List<List<Point>>();

        static ExerciseGesture _gesture = new ExerciseGesture();
    }
}
