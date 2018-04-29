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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.IO;
using System.Windows.Threading;
using System.Globalization;
using Microsoft.Kinect.Toolkit.BackgroundRemoval;

namespace ExercisesPerformanceControl
{
    public partial class ExerciseControl : Window
    {
        /// <summary>
        /// Intermediate storage for the skeleton data received from the sensor
        /// </summary>
        public Skeleton[] skeletons;

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
        /// Track whether Dispose has been called
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Library which does background 
        /// </summary>
        private BackgroundRemovedColorStream backgroundRemovedColorStream;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Flag that shows the end of the work with this window
        /// </summary>
        bool endFlag;


        /// <summary>
        /// Flag with info about whether the skeleton is tracked or not
        /// </summary>
        bool skelIsTracked = false;

        /// <summary>
        /// List for skeleton data
        /// </summary>
        private List<Skeleton> skelList = new List<Skeleton>();

        private List<Skeleton> skelListUser = new List<Skeleton>();

        List<JointType> listWIthCurrentlyFailedJoints = new List<JointType>();

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
        private bool Written = false;

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
        /// Thickness of drawn joint lines for joints with errors
        /// </summary>
        private double JointThicknessForFailedJoints = 15;

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
        /// Brush used for drawing joints that are currently with errors
        /// </summary>        
        private readonly Brush failedJointBrush = Brushes.Red;

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
        /// Name of the exercise
        /// </summary>
        public string ExName = "";

        DispatcherTimer timerForReferenceMovement = new DispatcherTimer();
        DispatcherTimer timerForUserData = new DispatcherTimer();

        List<List<Point>> pointsList = new List<List<Point>>();

        static ExerciseGesture _gesture = new ExerciseGesture();

        int framesToShowErrors = 0;

        SerializableExercise exerciseData;

        ExerciseType exerciseType;
    }
}
