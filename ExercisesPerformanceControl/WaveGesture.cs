using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace ExercisesPerformanceControl
{
    public class HandRaiseGesture
    {
        readonly int WINDOW_SIZE = 50;

        IGestureSegment[] _segments;

        int _currentSegment = 0;
        int _frameCount = 0;

        public event EventHandler GestureRecognized;

        public HandRaiseGesture()
        {
            HandRaise1 handRaiseSegment = new HandRaise1();

            _segments = new IGestureSegment[]
            {
                handRaiseSegment,
                handRaiseSegment,
                handRaiseSegment,
                handRaiseSegment,
                handRaiseSegment,
                handRaiseSegment,
                handRaiseSegment,
                handRaiseSegment,
                handRaiseSegment,
                handRaiseSegment
            };
        }

        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton data.</param>
        public void Update(Skeleton skeleton, List<Skeleton> skelList)
        {
            int frames = skelList.Count;
            int frame = frames / 10;

            GesturePartResult result = _segments[_currentSegment].Update(skeleton, skelList[frame * (_currentSegment + 1) - 1]);

            if (result == GesturePartResult.Succeeded)
            {
                if (_currentSegment + 1 < _segments.Length)
                {
                    _currentSegment++;
                    _frameCount = 0;
                    Console.WriteLine(_currentSegment);
                }
                else
                {
                    if (GestureRecognized != null)
                    {
                        GestureRecognized(this, new EventArgs());
                        Reset();
                    }
                }
            }
            else if (result == GesturePartResult.Failed || _frameCount == WINDOW_SIZE)
            {
                Reset();
            }
            else
            {
                _frameCount++;
            }
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
