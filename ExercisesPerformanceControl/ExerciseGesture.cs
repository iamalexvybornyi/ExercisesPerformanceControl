using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExercisesPerformanceControl
{
    public class ExerciseGesture
    {
        readonly int WINDOW_SIZE = 50;

        IGestureSegment[] _segments;

        private static GesturePartResult result;

        int _currentSegment = 0;
        int _frameCount = 0;

        public event EventHandler GestureRecognized;

        public ExerciseGesture()
        {
            ExerciseSegment exerciseSegment = new ExerciseSegment();

            _segments = new IGestureSegment[]
            {
                exerciseSegment,
                exerciseSegment,
                exerciseSegment,
                exerciseSegment,
                exerciseSegment,
                exerciseSegment,
                exerciseSegment,
                exerciseSegment,
                exerciseSegment,
                exerciseSegment
            };
        }

        public static GesturePartResult getResult()
        {
            return result;
        }

        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton data.</param>
        public List<JointType> Update(Skeleton skeleton, List<Skeleton> skelList, ExerciseType exType)
        {
            int frames = skelList.Count;
            int frame = frames / _segments.Length;

            GesturePartResult result = _segments[_currentSegment].Update(skeleton, skelList[frame * (_currentSegment + 1) - 1], exType);
            List<JointType> jointsWithErrorsFinalList = new List<JointType>();

            if (result == GesturePartResult.Succeeded)
            {
                if (_currentSegment + 1 < _segments.Length)
                {
                    Console.WriteLine("@@@@@@@@@@@@@@@@@=======SUCCESSFUL SEGMENT " + _currentSegment + "=======@@@@@@@@@@@@@@@@@" );
                    _currentSegment++;
                    _frameCount = 0;
                }
                else
                {
                    if (GestureRecognized != null)
                    {
                        GestureRecognized(this, new EventArgs());
                        Console.WriteLine("@@@@@@@@@@@@@@@@@=======SUCCESSFUL MOVEMENT=======@@@@@@@@@@@@@@@@@");
                        Reset();
                    }
                }
            }
            else if (result == GesturePartResult.Failed)
            {
                Console.WriteLine("@@@@@@@@@@@@@@@@@=======WRONG MOVEMENT=======@@@@@@@@@@@@@@@@@");
                Console.WriteLine("@@@@@@@@@@@@@@@@@=======Error in segment " + _currentSegment + "=======@@@@@@@@@@@@@@@@@");

                JointPosComparisonResultStorage userRes = Calculation.ComparePositionsOfJoints(skeleton);
                JointPosComparisonResultStorage refRes = Calculation.ComparePositionsOfJoints(skelList[frame * (_currentSegment + 1) - 1]);
                List<JointType> jointsWithPosErrors = JointPosComparisonResultStorage.CompareJointsPositions(userRes, refRes, exType);

                Dictionary<JointType, int> jointsWithErrors = AnglesStorage.CompareAngles(
                    Calculation.getAnglesInSkeletonJoints(skeleton),
                    Calculation.getAnglesInSkeletonJoints(skelList[frame * (_currentSegment + 1) - 1]),
                    exType);

                List<JointType> jointsWithAnglesErrors = jointsWithErrors.Keys.ToList();
                List<JointType> resListWithPossibleDuplicates = jointsWithPosErrors.Concat(jointsWithAnglesErrors).ToList();
                jointsWithErrorsFinalList = resListWithPossibleDuplicates.Distinct().ToList();

                Console.WriteLine("The errors are in next joints:");
                foreach (var jointWithError in jointsWithErrorsFinalList)
                {
                    Console.WriteLine(jointWithError);
                }
                Reset();
            }
            else if (result == GesturePartResult.Uncertain)
            {
                jointsWithErrors = AnglesStorage.CompareAngles(
                    Calculation.getAnglesInSkeletonJoints(skeleton),
                    Calculation.getAnglesInSkeletonJoints(skelList[frame * (_currentSegment + 1) - 1]));
                Console.WriteLine("The uncertainty is in the next joints:");
                foreach (KeyValuePair<JointType, int> jointWithError in jointsWithErrors)
                {
                    Console.WriteLine(jointWithError.Key);
                }
            }
            else if (_frameCount == WINDOW_SIZE)
            {
                Console.WriteLine("@@@@@@@@@@@@@@@@@=======TOO SLOW=======@@@@@@@@@@@@@@@@@");
                JointPosComparisonResultStorage userRes = Calculation.ComparePositionsOfJoints(skeleton);
                JointPosComparisonResultStorage refRes = Calculation.ComparePositionsOfJoints(skelList[frame * (_currentSegment + 1) - 1]);
                List<JointType> jointsWithPosErrors = JointPosComparisonResultStorage.CompareJointsPositions(userRes, refRes, exType);

                Dictionary<JointType, int> jointsWithErrors = AnglesStorage.CompareAngles(
                    Calculation.getAnglesInSkeletonJoints(skeleton),
                    Calculation.getAnglesInSkeletonJoints(skelList[frame * (_currentSegment + 1) - 1]),
                    exType);

                List<JointType> jointsWithAnglesErrors = jointsWithErrors.Keys.ToList();
                List<JointType> resListWithPossibleDuplicates = jointsWithPosErrors.Concat(jointsWithAnglesErrors).ToList();
                jointsWithErrorsFinalList = resListWithPossibleDuplicates.Distinct().ToList();

                Console.WriteLine("The errors are in next joints:");
                foreach (var jointWithError in jointsWithErrorsFinalList)
                {
                    Console.WriteLine(jointWithError);
                }
                Reset();
            }
            else
            {
                _frameCount++;
            }

            return jointsWithErrorsFinalList;
        }

        /// <summary>
        /// Resets the current gesture.
        /// </summary>
        public void Reset()
        {
            _currentSegment = 0;
            _frameCount = 0;
        }
    }
}
