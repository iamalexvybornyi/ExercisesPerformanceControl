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
            AnglesStorage anglesInUserSkeleton = Calculation.getAnglesInSkeletonJoints(skeleton);
            AnglesStorage anglesInReferenceSkeleton = Calculation.getAnglesInSkeletonJoints(referenceSkeleton);

            JointPosComparisonResultStorage userRes = Calculation.ComparePositionsOfJoints(skeleton);
            JointPosComparisonResultStorage refRes = Calculation.ComparePositionsOfJoints(referenceSkeleton);

            if (refRes.LeftElbowLeftWrist == userRes.LeftElbowLeftWrist &&
                refRes.LeftShoulderLeftElbow == userRes.LeftShoulderLeftElbow &&
                refRes.RightElbowRightWrist == userRes.RightElbowRightWrist &&
                refRes.RightShoulderRightElbow == userRes.RightShoulderRightElbow &&
                refRes.ShoulderCenterHipCenter == userRes.ShoulderCenterHipCenter &&
                refRes.ShoulderCenterSpine == userRes.ShoulderCenterSpine &&
                refRes.ShoulderCenterSpine == userRes.ShoulderCenterSpine &&
                refRes.LeftHipLeftKneeX == userRes.LeftHipLeftKneeX &&
                refRes.LeftHipLeftKneeY == userRes.LeftHipLeftKneeY &&
                refRes.LeftKneeLeftAnkleX == userRes.LeftKneeLeftAnkleX &&
                refRes.LeftKneeLeftAnkleY == userRes.LeftKneeLeftAnkleY &&
                refRes.RightHipRightKneeX == userRes.RightHipRightKneeX &&
                refRes.RightHipRightKneeY == userRes.RightHipRightKneeY &&
                refRes.RightKneeRightAnkleX == userRes.RightKneeRightAnkleX &&
                refRes.RightKneeRightAnkleY == userRes.RightKneeRightAnkleY)
            {
                var ER = Math.Abs(anglesInUserSkeleton.angleInRightElbow - anglesInReferenceSkeleton.angleInRightElbow);
                var EL = Math.Abs(anglesInUserSkeleton.angleInLeftElbow - anglesInReferenceSkeleton.angleInLeftElbow);
                var ShR = Math.Abs(anglesInUserSkeleton.angleInRightShoulder - anglesInReferenceSkeleton.angleInRightShoulder);
                var ShL = Math.Abs(anglesInUserSkeleton.angleInLeftShoulder - anglesInReferenceSkeleton.angleInLeftShoulder);

                var KnL = Math.Abs(anglesInUserSkeleton.angleInLeftKnee - anglesInReferenceSkeleton.angleInLeftKnee);
                var KnR = Math.Abs(anglesInUserSkeleton.angleInRightKnee - anglesInReferenceSkeleton.angleInRightKnee);

                var HipCL = Math.Abs(anglesInUserSkeleton.angleInHipCenterLeft - anglesInReferenceSkeleton.angleInHipCenterLeft);
                var HipCR = Math.Abs(anglesInUserSkeleton.angleInHipCenterRight - anglesInReferenceSkeleton.angleInHipCenterRight);

                var HipR = Math.Abs(anglesInUserSkeleton.angleInRightHip - anglesInReferenceSkeleton.angleInRightHip);
                var HipL = Math.Abs(anglesInUserSkeleton.angleInLeftHip - anglesInReferenceSkeleton.angleInLeftHip);

                Console.WriteLine(anglesInReferenceSkeleton.angleInLeftShoulder);
                Console.WriteLine(anglesInUserSkeleton.angleInLeftShoulder);

                //Console.WriteLine("ER = " + ER.ToString() + "; El = " + EL.ToString() + "; ShL = " + ShL.ToString() + "; ShR = " + ShR.ToString());
                // Angles
                if (ER <= Helper.error &&
                    EL <= Helper.error &&
                    ShR <= Helper.error &&
                    ShL <= Helper.error &&
                    KnL <= Helper.error &&
                    KnR <= Helper.error &&
                    HipCL <= Helper.error &&
                    HipCR <= Helper.error && 
                    HipR <= Helper.error &&
                    HipL <= Helper.error)
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
