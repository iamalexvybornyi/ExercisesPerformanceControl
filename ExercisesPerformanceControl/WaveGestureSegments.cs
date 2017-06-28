using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace ExercisesPerformanceControl
{
    /// <summary>
    /// Represents a single gesture segment which uses relative positioning of body parts to detect a gesture.
    /// </summary>
    public interface IGestureSegment
    {
        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>A GesturePartResult based on whether the gesture part has been completed.</returns>
        /// 
        GesturePartResult Update(Skeleton skeleton);
    }

    public class Helper
    {
        public static double err = 0.2;
        public static int error = 15;
    }

    public class HandRaise1 : IGestureSegment
    {
        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>A GesturePartResult based on whether the gesture part has been completed.</returns>
        public GesturePartResult Update(Skeleton skeleton)
        {
            double Sh = 140;
            double El = 170;
            if (Math.Abs(skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.HandLeft].Position.Y) < Helper.err)
            {
                double YPosHandRight = skeleton.Joints[JointType.HandRight].Position.Y;
                double YPosElbowRight = skeleton.Joints[JointType.ElbowRight].Position.Y;
                double YPosHandLeft = skeleton.Joints[JointType.HandLeft].Position.Y;
                double YPosElbowLeft = skeleton.Joints[JointType.ElbowLeft].Position.Y;
                double YPosShoulderRight = skeleton.Joints[JointType.ShoulderRight].Position.Y;
                double YPosShoulderLeft = skeleton.Joints[JointType.ShoulderLeft].Position.Y;

                // Hands below elbows
                if (YPosHandRight < YPosElbowRight && YPosHandLeft < YPosElbowLeft && 
                    YPosElbowRight < YPosShoulderRight && YPosElbowLeft < YPosShoulderLeft)
                {
                    var ER = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.HandRight, JointType.ElbowRight, JointType.ShoulderRight)) - El);
                    var EL = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.HandLeft, JointType.ElbowLeft, JointType.ShoulderLeft)) - El);
                    var ShR = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderRight, JointType.ElbowRight)) - Sh);
                    var ShL = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft)) - Sh);
                    Console.WriteLine("ER = " + ER.ToString() + "; El = " + EL.ToString() + "; ShL = " + ShL.ToString() + "; ShR = " + ShR.ToString());
                    // Angles
                    if (ER <= Helper.error &&
                        EL <= Helper.error &&
                        ShR <= Helper.error &&
                        ShL <= Helper.error)
                    {
                        Console.WriteLine("1");
                        return GesturePartResult.Succeeded;
                    }
                    else
                    {
                        return GesturePartResult.Uncertain;
                    }
                }
                else
                {
                    return GesturePartResult.Uncertain;
                }
            }

            Console.WriteLine("1 failed");
            // Hand dropped
            return GesturePartResult.Failed;
        }
    }
	
	public class HandRaise2 : IGestureSegment
    {
        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>A GesturePartResult based on whether the gesture part has been completed.</returns>
        public GesturePartResult Update(Skeleton skeleton)
        {
            double Sh = 150;
            double El = 170;
            if (Math.Abs(skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.HandLeft].Position.Y) < Helper.err)
            {
                // Hands below elbows
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ElbowRight].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ElbowLeft].Position.Y &&
                skeleton.Joints[JointType.ElbowRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y &&
                skeleton.Joints[JointType.ElbowLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y)
                {
                    var ER = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.HandRight, JointType.ElbowRight, JointType.ShoulderRight)) - El);
                    var EL = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.HandLeft, JointType.ElbowLeft, JointType.ShoulderLeft)) - El);
                    var ShR = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderRight, JointType.ElbowRight)) - Sh);
                    var ShL = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft)) - Sh);
                    Console.WriteLine("ER = " + ER.ToString() + "; El = " + EL.ToString() + "; ShL = " + ShL.ToString() + "; ShR = " + ShR.ToString());
                    // Angles
                    if (ER <= Helper.error &&
                        EL <= Helper.error &&
                        ShR <= Helper.error &&
                        ShL <= Helper.error)
                    {
                        Console.WriteLine("2");
                        return GesturePartResult.Succeeded;
                    }
                    else
                    {
                        return GesturePartResult.Uncertain;
                    }
                }
                else
                {
                    return GesturePartResult.Uncertain;
                }
            }
            Console.WriteLine("2 failed");
            // Hand dropped
            return GesturePartResult.Failed;
        }
    }
	
	public class HandRaise3 : IGestureSegment
    {
        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>A GesturePartResult based on whether the gesture part has been completed.</returns>
        public GesturePartResult Update(Skeleton skeleton)
        {
            double Sh = 180;
            double El = 170;
            if (Math.Abs(skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.HandLeft].Position.Y) < Helper.err)
            {
                // Hands below elbows
                if (Math.Abs(skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ElbowRight].Position.Y) <= Helper.error &&
                Math.Abs(skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.ElbowLeft].Position.Y) <= Helper.error &&
                Math.Abs(skeleton.Joints[JointType.ElbowRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y) <= Helper.error &&
                Math.Abs(skeleton.Joints[JointType.ElbowLeft].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y) <= Helper.error)
                {
                    var ER = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.HandRight, JointType.ElbowRight, JointType.ShoulderRight)) - El);
                    var EL = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.HandLeft, JointType.ElbowLeft, JointType.ShoulderLeft)) - El);
                    var ShR = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderRight, JointType.ElbowRight)) - Sh);
                    var ShL = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft)) - Sh);
                    Console.WriteLine("ER = " + ER.ToString() + "; El = " + EL.ToString() + "; ShL = " + ShL.ToString() + "; ShR = " + ShR.ToString());
                    // Angles
                    if (ER <= Helper.error &&
                        EL <= Helper.error &&
                        ShR <= Helper.error &&
                        ShL <= Helper.error)
                    {
                        Console.WriteLine("3");
                        return GesturePartResult.Succeeded;
                    }
                    else
                    {
                        return GesturePartResult.Uncertain;
                    }
                }
                else
                {
                    return GesturePartResult.Uncertain;
                }
            }

            Console.WriteLine("3 failed");
            // Hand dropped
            return GesturePartResult.Failed;
        }
    }

    public class HandRaise4 : IGestureSegment
    {
        public int error = 15;
        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>A GesturePartResult based on whether the gesture part has been completed.</returns>
        public GesturePartResult Update(Skeleton skeleton)
        {
            double Sh = 140;
            double El = 170;
            if (Math.Abs(skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.HandLeft].Position.Y) < Helper.err)
            {
                // Hands below elbows
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y &&
                skeleton.Joints[JointType.ElbowRight].Position.Y > skeleton.Joints[JointType.ShoulderRight].Position.Y &&
                skeleton.Joints[JointType.ElbowLeft].Position.Y > skeleton.Joints[JointType.ShoulderLeft].Position.Y)
                {
                    var ER = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.HandRight, JointType.ElbowRight, JointType.ShoulderRight)) - El);
                    var EL = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.HandLeft, JointType.ElbowLeft, JointType.ShoulderLeft)) - El);
                    var ShR = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderRight, JointType.ElbowRight)) - Sh);
                    var ShL = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft)) - Sh);
                    Console.WriteLine("ER = " + ER.ToString() + "; El = " + EL.ToString() + "; ShL = " + ShL.ToString() + "; ShR = " + ShR.ToString());
                    // Angles
                    if (ER <= Helper.error &&
                        EL <= Helper.error &&
                        ShR <= Helper.error &&
                        ShL <= Helper.error)
                    {
                        Console.WriteLine("4");
                        return GesturePartResult.Succeeded;
                    }
                    else
                    {
                        return GesturePartResult.Uncertain;
                    }
                }
                else
                {
                    return GesturePartResult.Uncertain;
                }
            }

            Console.WriteLine("4 failed");
            // Hand dropped
            return GesturePartResult.Failed;
        }
    }
	
	public class HandRaise5 : IGestureSegment
    {
        public int error = 15;
        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>A GesturePartResult based on whether the gesture part has been completed.</returns>
        public GesturePartResult Update(Skeleton skeleton)
        {
            double Sh = 115;
            double El = 170;
            if (Math.Abs(skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.HandLeft].Position.Y) < Helper.err)
            {
                // Hands below elbows
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y &&
                skeleton.Joints[JointType.ElbowRight].Position.Y > skeleton.Joints[JointType.ShoulderRight].Position.Y &&
                skeleton.Joints[JointType.ElbowLeft].Position.Y > skeleton.Joints[JointType.ShoulderLeft].Position.Y)
                {
                    var ER = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.HandRight, JointType.ElbowRight, JointType.ShoulderRight)) - El);
                    var EL = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.HandLeft, JointType.ElbowLeft, JointType.ShoulderLeft)) - El);
                    var ShR = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderRight, JointType.ElbowRight)) - Sh);
                    var ShL = Math.Abs(Math.Abs(Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft)) - Sh);
                    Console.WriteLine("ER = " + ER.ToString() + "; El = " + EL.ToString() + "; ShL = " + ShL.ToString() + "; ShR = " + ShR.ToString());
                    // Angles
                    if (ER <= Helper.error &&
                        EL <= Helper.error &&
                        ShR <= Helper.error &&
                        ShL <= Helper.error)
                    {
                        Console.WriteLine("5");
                        return GesturePartResult.Succeeded;
                    }
                    else
                    {
                        return GesturePartResult.Uncertain;
                    }
                }
                else
                {
                    return GesturePartResult.Uncertain;
                }
            }

            Console.WriteLine("5 failed");
            // Hand dropped
            return GesturePartResult.Failed;
        }
    }
}
