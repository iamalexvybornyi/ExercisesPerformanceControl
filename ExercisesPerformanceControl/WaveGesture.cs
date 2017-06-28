using Microsoft.Kinect;
using System;

namespace ExercisesPerformanceControl
{
    public class HandRaiseGesture
    {
        readonly int WINDOW_SIZE = 150;

        IGestureSegment[] _segments;

        int _currentSegment = 0;
        int _frameCount = 0;

        public event EventHandler GestureRecognized;

        public HandRaiseGesture()
        {
            HandRaise1 handRaiseSegment1 = new HandRaise1();
            HandRaise2 handRaiseSegment2 = new HandRaise2();
            HandRaise3 handRaiseSegment3 = new HandRaise3();
            HandRaise4 handRaiseSegment4 = new HandRaise4();
            HandRaise5 handRaiseSegment5 = new HandRaise5();

            _segments = new IGestureSegment[]
            {
                handRaiseSegment1,
                handRaiseSegment2,
                handRaiseSegment3,
                handRaiseSegment4,
                handRaiseSegment5,
                handRaiseSegment4,
                handRaiseSegment3,
                handRaiseSegment2,
                handRaiseSegment1
            };
        }

        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton data.</param>
        public void Update(Skeleton skeleton)
        {
            GesturePartResult result = _segments[_currentSegment].Update(skeleton);

            if (result == GesturePartResult.Succeeded)
            {
                if (_currentSegment + 1 < _segments.Length)
                {
                    _currentSegment++;
                    _frameCount = 0;
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
