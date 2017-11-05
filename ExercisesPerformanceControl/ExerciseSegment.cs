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
        GesturePartResult Update(Skeleton skeleton, Skeleton referenceSkeleton);
    }

    public class Helper
    {
        public static double err = 0.2;
        public static int error = 20;
    }

    public class ExerciseSegment : IGestureSegment
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

            if (refRes.Equals(userRes))
            {
                if (anglesInReferenceSkeleton.Equals(anglesInUserSkeleton))
                {
                    return GesturePartResult.Succeeded;
                }

                return GesturePartResult.Uncertain;
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
