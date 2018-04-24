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
    /// <summary>
    /// Interaction logic for ExerciseRecording.xaml
    /// </summary>
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



        public ExerciseRecording()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.OkBtn.IsEnabled = false;
            this.SaveBtn.IsEnabled = false;
            this.StopBtn.IsEnabled = false;
            // Create the drawing group we'll use for drawing live data
            this.drawingGroupForLiveData = new DrawingGroup();

            // Create the drawing group we'll use for drawing recorded data
            this.drawingGroupForRecordedData = new DrawingGroup();

            // Create an image source that we can use in our image control for live data
            this.imageSourceForLiveData = new DrawingImage(this.drawingGroupForLiveData);

            // Create an image source that we can use in our image control for recorded data
            this.imageSourceForRecordedData = new DrawingImage(this.drawingGroupForRecordedData);

            // Display the drawing using our image control for live data
            ImageForLiveData.Source = this.imageSourceForLiveData;


            // Look through all Kinect sensors and start the first connected one.
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            // Set type of the exercise
            exerciseType = (ExerciseType)Enum.Parse(typeof(ExerciseType), ((ComboBoxItem)this.ExerciseTypeComboBox.SelectedItem).Name);

            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handlers
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                this.sensor.AllFramesReady += this.SensorAllFramesReady;

                // Turn on the depth stream
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

                // Turn on the color stream
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Turn on the backgroundRemoved color stream
                this.backgroundRemovedColorStream = new BackgroundRemovedColorStream(this.sensor);
                this.backgroundRemovedColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution320x240Fps30);

                // Allocate space to put the depth, color, and skeleton data we'll receive
                if (null == this.skeletons)
                {
                    this.skeletons = new Skeleton[this.sensor.SkeletonStream.FrameSkeletonArrayLength];
                }

                this.backgroundRemovedColorStream.BackgroundRemovedFrameReady += this.BackgroundRemovedFrameReadyHandler;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                MessageBox.Show("Kinect не подключен или работает некорректно! Проверьте подключение и попробуйте еще раз.", "Состояние Kinect", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Disable all the streams and event handlers
            if (null != this.sensor)
            {
                this.sensor.AllFramesReady -= this.SensorAllFramesReady;
                this.sensor.SkeletonFrameReady -= this.SensorSkeletonFrameReady;
                this.backgroundRemovedColorStream.BackgroundRemovedFrameReady -= this.BackgroundRemovedFrameReadyHandler;
                this.backgroundRemovedColorStream.Disable();
                this.backgroundRemovedColorStream.Dispose();
                this.backgroundRemovedColorStream = null;
                this.sensor.DepthStream.Disable();
                this.sensor.ColorStream.Disable();
                this.sensor.SkeletonStream.Disable();

                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Finalizes an instance of the class.
        /// This destructor will run only if the Dispose method does not get called.
        /// </summary>
        ~ExerciseRecording()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Dispose the allocated frame buffers and reconstruction.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // This object will be cleaned up by the Dispose method.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees all memory associated with the FusionImageFrame.
        /// </summary>
        /// <param name="disposing">Whether the function was called from Dispose.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (null != this.backgroundRemovedColorStream)
                {
                    this.backgroundRemovedColorStream.Dispose();
                    this.backgroundRemovedColorStream = null;
                    GC.SuppressFinalize(this);
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Handle the background removed color frame ready event. The frame obtained from the background removed
        /// color stream is in RGBA format.
        /// </summary>
        /// <param name="sender">object that sends the event</param>
        /// <param name="e">argument of the event</param>
        private void BackgroundRemovedFrameReadyHandler(object sender, BackgroundRemovedColorFrameReadyEventArgs e)
        {
            using (var backgroundRemovedFrame = e.OpenBackgroundRemovedColorFrame())
            {
                if (backgroundRemovedFrame != null)
                {
                    if (null == this.foregroundBitmap || this.foregroundBitmap.PixelWidth != backgroundRemovedFrame.Width
                        || this.foregroundBitmap.PixelHeight != backgroundRemovedFrame.Height)
                    {
                        this.foregroundBitmap = new WriteableBitmap(backgroundRemovedFrame.Width, backgroundRemovedFrame.Height, 96.0, 96.0, PixelFormats.Bgra32, null);

                        // Set the image we display to point to the bitmap where we'll put the image data
                        this.ImageForLiveDataWithRemovedBackground.Source = this.foregroundBitmap;
                    }

                    // Write the pixel data into our bitmap
                    this.foregroundBitmap.WritePixels(
                        new Int32Rect(0, 0, this.foregroundBitmap.PixelWidth, this.foregroundBitmap.PixelHeight),
                        backgroundRemovedFrame.GetRawPixelData(),
                        this.foregroundBitmap.PixelWidth * sizeof(int),
                        0);

                    if (Written == false && (this.skeletons.Any(skel => skel.TrackingState == SkeletonTrackingState.Tracked)))
                    {
                        MyBackgroundRemovedColourFrame MyFrame = new MyBackgroundRemovedColourFrame(backgroundRemovedFrame);
                        byte[] tmpPixelData = new byte[backgroundRemovedFrame.GetRawPixelData().Length];
                        MyFrame.height = backgroundRemovedFrame.Height;
                        MyFrame.width = backgroundRemovedFrame.Width;
                        backgroundRemovedFrame.GetRawPixelData().CopyTo(MyFrame.pixelData, 0);
                        bitmapListForRecording.Add(MyFrame);
                    }
                }
            }

        }

        /// <summary>
        /// Use the sticky skeleton logic to choose a player that we want to set as foreground. This means if the app
        /// is tracking a player already, we keep tracking the player until it leaves the sight of the camera, 
        /// and then pick the closest player to be tracked as foreground.
        /// </summary>
        private void ChooseSkeleton()
        {
            var isTrackedSkeltonVisible = false;
            var nearestDistance = float.MaxValue;
            var nearestSkeleton = 0;

            foreach (var skel in this.skeletons)
            {
                if (null == skel)
                {
                    continue;
                }

                if (skel.TrackingState != SkeletonTrackingState.Tracked)
                {
                    continue;
                }

                if (skel.TrackingId == this.currentlyTrackedSkeletonId)
                {
                    isTrackedSkeltonVisible = true;
                    break;
                }

                if (skel.Position.Z < nearestDistance)
                {
                    nearestDistance = skel.Position.Z;
                    nearestSkeleton = skel.TrackingId;
                }
            }

            if (!isTrackedSkeltonVisible && nearestSkeleton != 0)
            {
                this.currentlyTrackedSkeletonId = nearestSkeleton;
                this.backgroundRemovedColorStream.SetTrackedPlayer(nearestSkeleton);
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's DepthFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // in the middle of shutting down, or lingering events from previous sensor, do nothing here.
            if (null == this.sensor || this.sensor != sender)
            {
                return;
            }

            try
            {
                using (var depthFrame = e.OpenDepthImageFrame())
                {
                    if (null != depthFrame)
                    {
                        this.backgroundRemovedColorStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
                    }
                }

                using (var colorFrame = e.OpenColorImageFrame())
                {
                    if (null != colorFrame)
                    {
                        this.backgroundRemovedColorStream.ProcessColor(colorFrame.GetRawPixelData(), colorFrame.Timestamp);
                    }
                }

                using (var skeletonFrame = e.OpenSkeletonFrame())
                {
                    if (null != skeletonFrame)
                    {
                        skeletonFrame.CopySkeletonDataTo(this.skeletons);
                        this.backgroundRemovedColorStream.ProcessSkeleton(this.skeletons, skeletonFrame.Timestamp);
                    }
                }

                this.ChooseSkeleton();
            }
            catch
            {
                // Ignore the exception. 
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            //Draw skeleton for live data
            using (DrawingContext dc = this.drawingGroupForLiveData.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked && currentlyTrackedSkeletonId == skel.TrackingId)
                        {
                            if (skelIsTracked == false)
                            {
                                skelIsTracked = true;
                            }

                            this.DrawBonesAndJoints(skel, dc);

                            // All these commented lines below were used to record exercises. That's why they're still here.

                            //Write skeleton data of an exercise to file
                            if (Written == false)
                            {
                                skelListForRecording.Add(skel);

                                List<Point> tmpList = new List<Point>();
                                foreach (Joint joint in skel.Joints)
                                {
                                    tmpList.Add(SkeletonPointToScreen(joint.Position));
                                }
                                pointsList.Add(tmpList);
                            }

                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly && currentlyTrackedSkeletonId == skel.TrackingId)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroupForLiveData.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints for recorded data
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                    JointThickness = 3;
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawPen = new Pen(Brushes.Black, 10);

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            this.StopBtn.IsEnabled = true;
            this.StartBtn.IsEnabled = false;
            Written = false;
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            this.StopBtn.IsEnabled = false;
            this.StartBtn.IsEnabled = false;
            this.OkBtn.IsEnabled = true;
            this.SaveBtn.IsEnabled = true;

            if (((skelListForRecording.Count - bitmapListForRecording.Count) <= 5) && skelListForRecording.Count >= 30)
            {
                Written = true;
                int bitmapFrames = bitmapListForRecording.Count;
                int skelFrames = skelListForRecording.Count;

                if (skelFrames > bitmapFrames)
                {
                    skelListForRecording.RemoveRange(bitmapFrames - 1, (skelFrames - 1) - (bitmapFrames - 1));
                }
                else if (bitmapFrames > skelFrames)
                {
                    bitmapListForRecording.RemoveRange(skelFrames - 1, (bitmapFrames - 1) - (skelFrames - 1));
                }
                //AmountOfFramesLabel.Content += skelListForRecording.Count.ToString();
                StartingFrameTextbox.Text = "0";
                EndingFrameTextbox.Text = (skelListForRecording.Count - 1).ToString();
                timer.Start();

                if (null != this.sensor)
                {
                    //this.backgroundRemovedColorStream.Disable();
                    //this.backgroundRemovedColorStream.BackgroundRemovedFrameReady -= this.BackgroundRemovedFrameReadyHandler;
                    //this.backgroundRemovedColorStream.Dispose();
                    //this.backgroundRemovedColorStream = null;

                    //this.sensor.AllFramesReady -= this.SensorAllFramesReady;
                    //this.sensor.SkeletonFrameReady -= this.SensorSkeletonFrameReady;
                    //this.sensor.DepthStream.Disable();
                    //this.sensor.ColorStream.Disable();
                    //this.sensor.SkeletonStream.Disable();

                    this.sensor.Stop();
                }
            }
            else
            {
                MessageBox.Show("При записи произошел сбой, попробуйте еще раз.");
                skelListForRecording.Clear();
                pointsList.Clear();
                bitmapListForRecording.Clear();
                Written = false;
                this.StopBtn.IsEnabled = false;
                this.StartBtn.IsEnabled = true;
                this.OkBtn.IsEnabled = false;
                this.SaveBtn.IsEnabled = false;
            }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            index = Convert.ToInt16(StartingFrameTextbox.Text);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string fileLocation = @"ExercisesData\" + NameOfTheExTextbox.Text;

            int start = Convert.ToInt16(StartingFrameTextbox.Text);
            int end = Convert.ToInt16(EndingFrameTextbox.Text);

            if (end - start >= 30)
            {
                skelListForRecording.RemoveRange(end, skelListForRecording.Count - end);
                skelListForRecording.RemoveRange(0, start);

                pointsList.RemoveRange(end, pointsList.Count - end);
                pointsList.RemoveRange(0, start);

                bitmapListForRecording.RemoveRange(end, bitmapListForRecording.Count - end);
                bitmapListForRecording.RemoveRange(0, start);

                //AmountOfFramesLabel.Content += skelListForRecording.Count.ToString();
                StartingFrameTextbox.Text = "0";
                EndingFrameTextbox.Text = (skelListForRecording.Count - 1).ToString();
                index = 0;

                //FileRW.WritePointsDataToFile(pointsList, fileLocation + ".pnt");
                //FileRW.WriteSkelDataToFile(skelListForRecording, fileLocation + ".txt");

                exerciseType = (ExerciseType)Enum.Parse(typeof(ExerciseType), ((ComboBoxItem)this.ExerciseTypeComboBox.SelectedItem).Name);
                SerializableExercise serEx = new SerializableExercise(skelListForRecording, pointsList, exerciseType);
                FileRW.WriteToFile(serEx, fileLocation + ".xrs");

                try
                {
                    int counter = 0;
                    foreach (var backRemovedFrame in bitmapListForRecording)
                    {
                        WriteableBitmap tmpBitmap = new WriteableBitmap(backRemovedFrame.width, backRemovedFrame.height, 96.0, 96.0, PixelFormats.Bgra32, null);
                        tmpBitmap.WritePixels(
                                new Int32Rect(0, 0, tmpBitmap.PixelWidth, tmpBitmap.PixelHeight),
                                backRemovedFrame.pixelData,
                                tmpBitmap.PixelWidth * sizeof(int),
                                0);

                        Utils.CreateThumbnailPNG(@"ExercisesData\Pics\" + NameOfTheExTextbox.Text + counter.ToString() + ".png", tmpBitmap);
                        counter++;
                    }

                    using (VideoFileWriter writer = new VideoFileWriter())
                    {
                        writer.Open(fileLocation + ".avi", 640, 480, 30, VideoCodec.MPEG4);
                        var files = new DirectoryInfo(@"ExercisesData\Pics").GetFiles().OrderBy(f => f.LastWriteTime).ToList();
                        foreach (var file in files)
                        {
                            var bitmap = System.Drawing.Bitmap.FromFile(file.FullName) as System.Drawing.Bitmap;
                            bitmap.MakeTransparent(bitmap.GetPixel(0, 0));
                            writer.WriteVideoFrame(bitmap);
                        }
                        writer.Close();
                    }
                }
                catch
                {
                    MessageBox.Show("Видео не было сохранено!");
                }

                timer.Stop();
                this.Close();

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                try
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(@"ExercisesData\Pics");
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }
                catch { }
            }
            else
            {
                MessageBox.Show("Количество кадров в упражнении слишком маленькое, попробуйте еще раз.");
                this.StopBtn.IsEnabled = false;
                this.StartBtn.IsEnabled = true;
                this.OkBtn.IsEnabled = false;
                this.SaveBtn.IsEnabled = false;
                skelListForRecording.Clear();
                pointsList.Clear();
                bitmapListForRecording.Clear();
                Written = false;
                timer.Stop();
                // Look through all Kinect sensors and start the first connected one.
                foreach (var potentialSensor in KinectSensor.KinectSensors)
                {
                    if (potentialSensor.Status == KinectStatus.Connected)
                    {
                        this.sensor = potentialSensor;
                        break;
                    }
                }

                // Set type of the exercise
                exerciseType = (ExerciseType)Enum.Parse(typeof(ExerciseType), ((ComboBoxItem)this.ExerciseTypeComboBox.SelectedItem).Name);

                timer.Tick += new EventHandler(dispatcherTimer_Tick);
                timer.Interval = new TimeSpan(0, 0, 0, 0, 20);

                if (null != this.sensor)
                {
                    // Turn on the skeleton stream to receive skeleton frames
                    this.sensor.SkeletonStream.Enable();

                    // Add an event handlers
                    this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                    this.sensor.AllFramesReady += this.SensorAllFramesReady;

                    // Turn on the depth stream
                    this.sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

                    // Turn on the color stream
                    this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                    // Turn on the backgroundRemoved color stream
                    this.backgroundRemovedColorStream = new BackgroundRemovedColorStream(this.sensor);
                    this.backgroundRemovedColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution320x240Fps30);

                    // Allocate space to put the depth, color, and skeleton data we'll receive
                    if (null == this.skeletons)
                    {
                        this.skeletons = new Skeleton[this.sensor.SkeletonStream.FrameSkeletonArrayLength];
                    }

                    this.backgroundRemovedColorStream.BackgroundRemovedFrameReady += this.BackgroundRemovedFrameReadyHandler;

                    // Start the sensor!
                    try
                    {
                        this.sensor.Start();
                    }
                    catch (IOException)
                    {
                        this.sensor = null;
                    }
                }
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //=================Draw skelelton for recorded data=======================================
            using (DrawingContext dc2 = this.drawingGroupForLiveData.Open())
            {
                // Draw a transparent background to set the render size
                dc2.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skelListForRecording.Count != 0)
                {
                    // Increase index of the recorded exercise current frame
                    index++;

                    try
                    {
                        int startingIndex = Convert.ToInt16(StartingFrameTextbox.Text);
                        int endingFrame = Convert.ToInt16(EndingFrameTextbox.Text);

                        if (index >= Convert.ToInt16(EndingFrameTextbox.Text))
                        {
                            index = Convert.ToInt16(StartingFrameTextbox.Text);
                            if (index >= endingFrame)
                            {
                                index = 0;
                                StartingFrameTextbox.Text = index.ToString();
                            }
                        }

                        if (endingFrame > bitmapListForRecording.Count())
                        {
                            EndingFrameTextbox.Text = bitmapListForRecording.Count().ToString();
                        }
                    }
                    catch
                    {
                        if (index >= bitmapListForRecording.Count())
                        {
                            index = 0;
                        }
                    }

                    RenderClippedEdges(skelListForRecording[index], dc2);

                    if (skelListForRecording[index].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        this.DrawBonesAndJointsForRecordedData(skelListForRecording[index], dc2);
                    }
                    else if (skelListForRecording[index].TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        dc2.DrawEllipse(
                        this.centerPointBrush,
                        null,
                        pointsList[index][0],
                        BodyCenterThickness,
                        BodyCenterThickness);
                    }

                    this.foregroundBitmap.WritePixels(
                        new Int32Rect(0, 0, this.foregroundBitmap.PixelWidth, this.foregroundBitmap.PixelHeight),
                        bitmapListForRecording[index].pixelData,
                        this.foregroundBitmap.PixelWidth * sizeof(int),
                        0);

                    // Set the image we display to point to the bitmap where we'll put the image data
                    //this.ImageForLiveDataWithRemovedBackground.Source = this.foregroundBitmap;
                }

                // prevent drawing outside of our render area
                this.drawingGroupForLiveData.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));

            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints for recorded data
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJointsForRecordedData(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter, 3, 2);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, 2, 4);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, 2, 8);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine, 2, 1);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.Spine, JointType.HipCenter, 1, 0);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft, 0, 12);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight, 0, 16);

            // Left Arm
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, 4, 5);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, 5, 6);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, 6, 7);

            // Right Arm
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, 8, 9);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, 9, 10);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.WristRight, JointType.HandRight, 10, 11);

            // Left Leg
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft, 12, 13);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft, 13, 14);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft, 14, 15);

            // Right Leg
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight, 16, 17);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight, 17, 18);
            this.DrawBoneForRecordedData(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight, 18, 19);

            foreach (var point in pointsList[index])
            {
                Brush drawBrush = this.trackedJointBrush;
                drawingContext.DrawEllipse(drawBrush, null, point, JointThickness, JointThickness);

                JointThickness = 3;
            }
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBoneForRecordedData(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1, int joint0Index, int joint1Index)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, pointsList[index][joint0Index], pointsList[index][joint1Index]);
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
