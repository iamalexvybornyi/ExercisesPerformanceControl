using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Reflection;

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
        GesturePartResult Update(Skeleton skeleton, Skeleton referenceSkeleton, ExerciseType exType);
    }

    public class Helper
    {
        public static double err = 0.1;
        public static int error = 15;
    }

    public class ExerciseSegment : IGestureSegment
    {
        bool isInProgress = false;
        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>A GesturePartResult based on whether the gesture part has been completed.</returns>
        public GesturePartResult Update(Skeleton skeleton, Skeleton referenceSkeleton, ExerciseType exType)
        {
            AnglesStorage anglesInUserSkeleton = Calculation.getAnglesInSkeletonJoints(skeleton);
            AnglesStorage anglesInReferenceSkeleton = Calculation.getAnglesInSkeletonJoints(referenceSkeleton);

            JointPosComparisonResultStorage userRes = Calculation.ComparePositionsOfJoints(skeleton);
            JointPosComparisonResultStorage refRes = Calculation.ComparePositionsOfJoints(referenceSkeleton);

            if (refRes.Equals(userRes, exType))
            {
                isInProgress = true;
                if (anglesInReferenceSkeleton.Equals(anglesInUserSkeleton, exType))
                {
                    isInProgress = false;
                    //Console.WriteLine("Success");
                    //Console.WriteLine("======================");
                    return GesturePartResult.Succeeded;
                }

                //Console.WriteLine("Uncertain, in progress");
                return GesturePartResult.Uncertain;
            }
            else if (!refRes.Equals(userRes, exType) && isInProgress)
            {
                isInProgress = false;
                //Console.WriteLine("FAIL");
                //Console.WriteLine("======================");
                return GesturePartResult.Failed;
            }
            else
            {
                isInProgress = false;
                //Console.WriteLine("Starting point of the segment");
                return GesturePartResult.Uncertain;
            }
        }
    }
	
}
