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
    /// <summary>
    /// Interaction logic for ExerciseControl.xaml
    /// </summary>
    public partial class ExerciseControl : Window
    {
        public ExerciseControl()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Set end flag to false
            endFlag = false;

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

            _gesture.GestureRecognized += Gesture_GestureRecognized;

            exerciseData = FileRW.ReadFromFile(ExName);

            // Get all the skeleton data of an exercise
            skelList = exerciseData.GetSkelData();
            frames = skelList.Count;

            // Check if the files have been read correctly
            if (skelList.Count() == 0)
            {
                MessageBox.Show("Не найдены файлы с данными! Убедитесь, что они находятся в папке с программой.", "Отсутствие файлов!", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }

            pointsList.Clear();
            pointsList = exerciseData.GetSkelPointsData();

            exerciseType = exerciseData.GetExerciseType();

            timerForReferenceMovement.Tick += new EventHandler(dispatcherTimer_TickReferenceMovement);
            timerForReferenceMovement.Interval = new TimeSpan(0, 0, 0, 0, 20);
            timerForReferenceMovement.Start();

            String currentDirectory = System.IO.Directory.GetCurrentDirectory();
            try
            {
                this.VideoControlTimeline.Source = new Uri(currentDirectory + "\\ExercisesData\\" + ExName + ".avi", UriKind.Absolute);
                this.VideoControl.LoadedBehavior = MediaState.Manual;
                this.VideoControl.Play();
            }
            catch
            {

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
                endFlag = true;
                this.backgroundRemovedColorStream.Disable();
                this.backgroundRemovedColorStream.BackgroundRemovedFrameReady -= this.BackgroundRemovedFrameReadyHandler;
                this.backgroundRemovedColorStream.Dispose();
                this.backgroundRemovedColorStream = null;

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
                this.backgroundRemovedColorStream.SetTrackedPlayer(nearestSkeleton);
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
                }
            }

        }

        /// <summary>
        /// Finalizes an instance of the class.
        /// This destructor will run only if the Dispose method does not get called.
        /// </summary>
        ~ExerciseControl()
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
        /// Event handler for Kinect sensor's DepthFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // in the middle of shutting down, or lingering events from previous sensor, do nothing here.
            if (null == this.sensor || this.sensor != sender || endFlag)
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

                            List<JointType> jointsWithErrors = _gesture.Update(skel, skelList, exerciseType);

                            this.DrawBonesAndJoints(skel, dc, jointsWithErrors);
                            
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
        }

        /// <summary>
        /// Draws a skeleton's bones and joints for live data
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext, List<JointType> jointsWithErrors)
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

            if (jointsWithErrors.Count > 0)
            {
                framesToShowErrors = 20;
                listWIthCurrentlyFailedJoints.Clear();
                listWIthCurrentlyFailedJoints = jointsWithErrors;
            }

            if (framesToShowErrors > 0)
            {
                Brush drawBrushForFailedJoints = null;
                foreach (var jointWithError in listWIthCurrentlyFailedJoints)
                {
                    //JointThickness = 15;
                    drawBrushForFailedJoints = this.failedJointBrush;
                    drawingContext.DrawEllipse(drawBrushForFailedJoints, null, this.SkeletonPointToScreen(skeleton.Joints[jointWithError].Position), JointThicknessForFailedJoints, JointThicknessForFailedJoints);
                }
                framesToShowErrors--;
                if (framesToShowErrors == 0)
                {
                    listWIthCurrentlyFailedJoints.Clear();
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
