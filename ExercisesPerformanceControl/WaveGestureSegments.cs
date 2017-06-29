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
        GesturePartResult Update(Skeleton skeleton, Skeleton referenceSkeleton);
    }

    public class Helper
    {
        public static double err = 0.2;
        public static int error = 20;
    }

    public class HandRaise1 : IGestureSegment
    {
        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>A GesturePartResult based on whether the gesture part has been completed.</returns>
        public GesturePartResult Update(Skeleton skeleton, Skeleton referenceSkeleton)
        {
            double Sh = 140;
            double El = 170;

            AnglesStorage anglesInUserSkeleton = Calculation.getAnglesInSkeletonJoints(skeleton);
            AnglesStorage anglesInReferenceSkeleton = Calculation.getAnglesInSkeletonJoints(referenceSkeleton);

            JointPosComparisonResultStorage userRes = Calculation.ComparePositionsOfJoints(skeleton);
            JointPosComparisonResultStorage refRes = Calculation.ComparePositionsOfJoints(referenceSkeleton);

            if (refRes.LeftElbowLeftWrist == userRes.LeftElbowLeftWrist &&
                refRes.LeftShoulderLeftElbow == userRes.LeftShoulderLeftElbow &&
                refRes.RightElbowRightWrist == userRes.RightElbowRightWrist &&
                refRes.RightShoulderRightElbow == userRes.RightShoulderRightElbow)
            {
                var ER = Math.Abs(anglesInUserSkeleton.angleInRightElbow - anglesInReferenceSkeleton.angleInRightElbow);
                var EL = Math.Abs(anglesInUserSkeleton.angleInLeftElbow - anglesInReferenceSkeleton.angleInLeftElbow);
                var ShR = Math.Abs(anglesInUserSkeleton.angleInRightShoulder - anglesInReferenceSkeleton.angleInRightShoulder);
                var ShL = Math.Abs(anglesInUserSkeleton.angleInLeftShoulder - anglesInReferenceSkeleton.angleInLeftShoulder);

                Console.WriteLine(anglesInReferenceSkeleton.angleInLeftShoulder);
                Console.WriteLine(anglesInUserSkeleton.angleInLeftShoulder);

                //Console.WriteLine("ER = " + ER.ToString() + "; El = " + EL.ToString() + "; ShL = " + ShL.ToString() + "; ShR = " + ShR.ToString());
                // Angles
                if (ER <= Helper.error &&
                    EL <= Helper.error &&
                    ShR <= Helper.error &&
                    ShL <= Helper.error)
                {
                    //Console.WriteLine("1");
                    //Console.WriteLine("ER = " + ER.ToString() + "; El = " + EL.ToString() + "; ShL = " + ShL.ToString() + "; ShR = " + ShR.ToString());
                    Console.WriteLine("Success");
                    Console.WriteLine(anglesInReferenceSkeleton.angleInLeftShoulder);
                    Console.WriteLine(anglesInUserSkeleton.angleInLeftShoulder);
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

            Console.WriteLine("1 failed");
            // Hand dropped
            return GesturePartResult.Failed;
        }
    }
	
}
