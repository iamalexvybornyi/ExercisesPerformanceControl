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

namespace ExercisesPerformanceControl
{
    /// <summary>
    /// Interaction logic for ExerciseControl.xaml
    /// </summary>
    public partial class ExerciseControl : Window
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
        /// Var for the name of the exercise
        /// </summary>
        public string ExName = "";

        DispatcherTimer timerForReferenceMovement = new DispatcherTimer();
        DispatcherTimer timerForUserData = new DispatcherTimer();

        List<List<Point>> pointsList = new List<List<Point>>();
        List<List<Point>> pointsListUser = new List<List<Point>>();

        static HandRaiseGesture _gesture = new HandRaiseGesture();


        public ExerciseControl()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing live data
            this.drawingGroupForLiveData = new DrawingGroup();

            // Create the drawing group we'll use for drawing recorded data
            this.drawingGroupForRecordedData = new DrawingGroup();

            // Create an image source that we can use in our image control for live data
            this.imageSourceForLiveData = new DrawingImage(this.drawingGroupForLiveData);

            // Create an image source that we can use in our image control for recorded data
            this.imageSourceForRecordedData = new DrawingImage(this.drawingGroupForRecordedData);

            // Set text in the textbox responsible for amount of reps
            this.Reps.Text = "Количество повторений: " + Convert.ToString(numberOfReps);

            // Display the drawing using our image control for live data
            ImageForLiveData.Source = this.imageSourceForLiveData;

            // Display the drawing using our image control for recorded data
            ImageForRecordedData.Source = this.imageSourceForRecordedData;


            // Look through all Kinect sensors and start the first connected one.
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

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

                // Allocate space to put the depth, color, and skeleton data we'll receive
                if (null == this.skeletons)
                {
                    this.skeletons = new Skeleton[this.sensor.SkeletonStream.FrameSkeletonArrayLength];
                }

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
                //MessageBox.Show("Kinect не подключен или работает некорректно! Проверьте подключение и попробуйте еще раз.", "Состояние Kinect", MessageBoxButton.OK, MessageBoxImage.Warning);
                //this.Close();
            }

            _gesture.GestureRecognized += Gesture_GestureRecognized;

            ExText = "Ex" + ExName;

            // Get all the skeleton data of an exercise
            skelList = FileRW.ReadSkelDataFromFile(ExText);

            string userInput = "Input" + ExName;
            skelListUser = FileRW.ReadSkelDataFromFile(userInput);

            frames = skelList.Count;
            framesUserData = skelListUser.Count;

            // Check if the files were read correctly
            if (skelList.Count() == 0)
            {
                MessageBox.Show("Не найдены файлы с данными! Убедитесь, что они находятся в папке с программой.", "Отсутствие файлов!", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
            else
            {
                //foreach (var skel in skelList)
                //{
                //    List<Point> tmpList = new List<Point>();
                //    foreach (Joint joint in skel.Joints)
                //    {
                //        tmpList.Add(SkeletonPointToScreen(joint.Position));
                //    }
                //    pointsList.Add(tmpList);
                //}

                //string fileLocation = ExText + ".pnt";
                //FileRW.WritePointsDataToFile(pointsList, fileLocation);

                //foreach (var skel in skelListUser)
                //{
                //    List<Point> tmpList = new List<Point>();
                //    foreach (Joint joint in skel.Joints)
                //    {
                //        tmpList.Add(SkeletonPointToScreen(joint.Position));
                //    }
                //    pointsList.Add(tmpList);
                //}

                //string fileLocationUser = userInput + ".pnt";
                //FileRW.WritePointsDataToFile(pointsList, fileLocationUser);
            }

            pointsList.Clear();
            pointsList = FileRW.ReadPointsDataFromFile(ExText);

            pointsListUser.Clear();
            pointsListUser = FileRW.ReadPointsDataFromFile(userInput);

            timerForReferenceMovement.Tick += new EventHandler(dispatcherTimer_TickReferenceMovement);
            timerForReferenceMovement.Interval = new TimeSpan(0, 0, 0, 0, 20);

            timerForUserData.Tick += new EventHandler(dispatcherTimer_TickUserMovement);
            timerForUserData.Interval = new TimeSpan(0, 0, 0, 0, 20);

            timerForReferenceMovement.Start();
            if (sensor == null)
            {
                timerForUserData.Start();
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
                //endFlag = true;
                //this.backgroundRemovedColorStream.Disable();
                //this.backgroundRemovedColorStream.BackgroundRemovedFrameReady -= this.BackgroundRemovedFrameReadyHandler;
                //this.backgroundRemovedColorStream.Dispose();
                //this.backgroundRemovedColorStream = null;

                this.sensor.AllFramesReady -= this.SensorAllFramesReady;
                this.sensor.SkeletonFrameReady -= this.SensorSkeletonFrameReady;
                this.sensor.DepthStream.Disable();
                this.sensor.ColorStream.Disable();
                this.sensor.SkeletonStream.Disable();

                this.sensor.Stop();
            }

            timerForReferenceMovement.Stop();
            timerForUserData.Stop();
        }

        private void dispatcherTimer_TickReferenceMovement(object sender, EventArgs e)
        {
            //=================Draw skelelton for recorded data=======================================
            using (DrawingContext dc2 = this.drawingGroupForRecordedData.Open())
            {
                // Draw a transparent background to set the render size
                dc2.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skelList.Count != 0)
                {
                    RenderClippedEdges(skelList[index], dc2);

                    if (skelList[index].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        this.DrawBonesAndJointsForRecordedData(skelList[index], dc2);
                    }
                    else if (skelList[index].TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        dc2.DrawEllipse(
                        this.centerPointBrush,
                        null,
                        pointsList[index][0],
                        BodyCenterThickness,
                        BodyCenterThickness);
                    }

                    // Increase index of the recorded exercise current frame
                    index++;
                    if (index >= frames)
                        index = 0;
                }

                // prevent drawing outside of our render area
                this.drawingGroupForRecordedData.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));

            }
        }

        private void dispatcherTimer_TickUserMovement(object sender, EventArgs e)
        {
            //=================Draw skelelton for recorded data=======================================
            using (DrawingContext dc2 = this.drawingGroupForLiveData.Open())
            {
                // Draw a transparent background to set the render size
                dc2.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skelListUser.Count != 0)
                {
                    RenderClippedEdges(skelListUser[indexUserData], dc2);

                    if (skelListUser[indexUserData].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        this.DrawBonesAndJointsForRecordedDataUser(skelListUser[indexUserData], dc2);
                        _gesture.Update(skelListUser[indexUserData], skelList);
                    }
                    else if (skelListUser[indexUserData].TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        dc2.DrawEllipse(
                        this.centerPointBrush,
                        null,
                        pointsListUser[indexUserData][0],
                        BodyCenterThickness,
                        BodyCenterThickness);
                    }

                    // Increase index of the recorded exercise current frame
                    indexUserData++;
                    if (indexUserData >= framesUserData)
                        indexUserData = 0;
                }

                // prevent drawing outside of our render area
                this.drawingGroupForLiveData.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));

            }
        }

        private void Gesture_GestureRecognized(object sender, EventArgs e)
        {
            numberOfReps++;
            this.Reps.Text = "Количество повторений: " + Convert.ToString(numberOfReps);
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
                using (var skeletonFrame = e.OpenSkeletonFrame())
                {
                    if (null != skeletonFrame)
                    {
                        skeletonFrame.CopySkeletonDataTo(this.skeletons);
                    }
                }

                this.ChooseSkeleton();
            }
            catch (InvalidOperationException)
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

                            _gesture.Update(skel, skelList);

                            // All these commented lines below were used to record exercises. That's why they're still here.
                            
                            //Write skeleton data of an exercise to file
                            //if (skelListForRecording.Count() < 250)
                            //{
                            //    skelListForRecording.Add(skel);
                            //}
                            //else if (Written == false)
                            //{
                            //    string fileLocation = "Input8.txt";
                            //    FileRW.WriteSkelDataToFile(skelListForRecording, fileLocation);
                            //    Written = true;
                            //}
                            
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

            int counter = 0;
            foreach (var point in pointsList[index])
            {
                Brush drawBrush = this.trackedJointBrush;
                drawingContext.DrawEllipse(drawBrush, null, point, JointThickness, JointThickness);

                if (counter == 4)
                {
                    double angle = Calculation.getAngle(skelList[index], JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft);
                    FormattedText formattedText = new FormattedText(String.Format("{0:0.00}", angle), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 32, Brushes.Black);
                    drawingContext.DrawText(formattedText, point);
                }

                JointThickness = 3;
                counter++;
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints for recorded data
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJointsForRecordedDataUser(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter, 3, 2);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, 2, 4);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, 2, 8);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine, 2, 1);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.Spine, JointType.HipCenter, 1, 0);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft, 0, 12);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight, 0, 16);

            // Left Arm
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, 4, 5);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, 5, 6);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, 6, 7);

            // Right Arm
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, 8, 9);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, 9, 10);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.WristRight, JointType.HandRight, 10, 11);

            // Left Leg
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft, 12, 13);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft, 13, 14);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft, 14, 15);

            // Right Leg
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight, 16, 17);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight, 17, 18);
            this.DrawBoneForRecordedDataUser(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight, 18, 19);

            int counter = 0;
            foreach (var point in pointsListUser[indexUserData])
            {
                Brush drawBrush = this.trackedJointBrush;
                drawingContext.DrawEllipse(drawBrush, null, point, JointThickness, JointThickness);
                JointThickness = 3;

                if (counter == 4)
                {
                    double angle = Calculation.getAngle(skelListUser[indexUserData], JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft);
                    FormattedText formattedText = new FormattedText(String.Format("{0:0.00}", angle), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 32, Brushes.Black);
                    drawingContext.DrawText(formattedText, point);
                }

                counter++;
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

                    if (joint.JointType == JointType.ShoulderLeft)
                    {
                        double angle = Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft);
                        FormattedText formattedText = new FormattedText(String.Format("{0:0.00}", angle), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 32, Brushes.Black);
                        drawingContext.DrawText(formattedText, this.SkeletonPointToScreen(joint.Position));
                    }
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

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBoneForRecordedDataUser(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1, int joint0Index, int joint1Index)
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

            drawingContext.DrawLine(drawPen, pointsListUser[indexUserData][joint0Index], pointsListUser[indexUserData][joint1Index]);
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

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
