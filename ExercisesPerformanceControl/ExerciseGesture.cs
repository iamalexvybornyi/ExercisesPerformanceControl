using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace ExercisesPerformanceControl
{
    public class ExerciseGesture
    {
        readonly int WINDOW_SIZE = 50;

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
        public Dictionary<JointType, int> Update(Skeleton skeleton, List<Skeleton> skelList)
        {
            int frames = skelList.Count;
            int frame = frames / _segments.Length;

            GesturePartResult result = _segments[_currentSegment].Update(skeleton, skelList[frame * (_currentSegment + 1) - 1]);
            Dictionary<JointType, int> jointsWithErrors = new Dictionary<JointType, int>();

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
                jointsWithErrors = AnglesStorage.CompareAngles(
                    Calculation.getAnglesInSkeletonJoints(skeleton),
                    Calculation.getAnglesInSkeletonJoints(skelList[frame * (_currentSegment + 1) - 1]));
                Console.WriteLine("The errors are in next joints:");
                foreach (KeyValuePair<JointType, int> jointWithError in jointsWithErrors)
                {
                    Console.WriteLine(jointWithError.Key);
                }
                Reset();
            }
            else if (_frameCount == WINDOW_SIZE)
            {
                Console.WriteLine("@@@@@@@@@@@@@@@@@=======TOO SLOW=======@@@@@@@@@@@@@@@@@");
                Reset();
            }
            else
            {
                _frameCount++;
            }

            return jointsWithErrors;
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
