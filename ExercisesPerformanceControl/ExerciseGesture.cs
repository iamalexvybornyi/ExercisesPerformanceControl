using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExercisesPerformanceControl
{
    public class ExerciseGesture
    {
        readonly int WINDOW_SIZE = 50;

        private static bool isInProgress = false;

        IGestureSegment[] _segments;

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
                    if (_currentSegment == 2)
                    {
                        isInProgress = true;
                    }
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
            else if (result == GesturePartResult.Failed && isInProgress)
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
            else if (isInProgress)
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
            isInProgress = false;
        }
    }
}
