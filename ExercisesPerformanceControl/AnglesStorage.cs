/*
 * Class for angles storage 
 * Copyright (C) 2016 Vybornyi Alexander  (iamalexvybornyi@gmail.com/cahek2605@mail.ru)
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExercisesPerformanceControl
{
    /// <summary>
    /// Class for angles storage
    /// </summary>
    class AnglesStorage : IEquatable<AnglesStorage>
    {
        /// <summary>
        /// Angle in right shoulder
        /// <summary>
        public double angleInRightShoulder;

        /// <summary>
        /// Angle in left shoulder
        /// <summary>
        public double angleInLeftShoulder;

        /// <summary>
        /// Angle in right elbow
        /// <summary>
        public double angleInRightElbow;

        /// <summary>
        /// Angle in left elbow
        /// <summary>
        public double angleInLeftElbow;

        /// <summary>
        /// Angle in right hip
        /// <summary>
        public double angleInRightHip;

        /// <summary>
        /// Angle in left hip
        /// <summary>
        public double angleInLeftHip;

        /// <summary>
        /// Angle in hip center right
        /// <summary>
        public double angleInHipCenterRight;

        /// <summary>
        /// Angle in hip center left
        /// <summary>
        public double angleInHipCenterLeft;

        /// <summary>
        /// Angle in spine
        /// <summary>
        public double angleInSpine;

        /// <summary>
        /// Angle in right knee
        /// <summary>
        public double angleInRightKnee;

        /// <summary>
        /// Angle in left knee
        /// <summary>
        public double angleInLeftKnee;

        /// <summary>
        /// Angle in right ankle
        /// <summary>
        public double angleInRightAnkle;

        /// <summary>
        /// Angle in left ankle
        /// <summary>
        public double angleInLeftAnkle;

        /// <summary>
        /// Initializes a new instance of the AnglesStorage class.
        /// </summary>
        public AnglesStorage()
        {
            angleInRightShoulder = new double();
            angleInLeftShoulder = new double();
            angleInRightElbow = new double();
            angleInLeftElbow = new double();
            angleInRightHip = new double();
            angleInLeftHip = new double();
            angleInHipCenterRight = new double();
            angleInHipCenterLeft = new double();
            angleInSpine = new double();
            angleInRightKnee = new double();
            angleInLeftKnee = new double();
            angleInRightAnkle = new double();
            angleInLeftAnkle = new double();
        }

        public bool Equals(AnglesStorage other)
        {
            var ER = Math.Abs(this.angleInRightElbow - other.angleInRightElbow);
            var EL = Math.Abs(this.angleInLeftElbow - other.angleInLeftElbow);
            var ShR = Math.Abs(this.angleInRightShoulder - other.angleInRightShoulder);
            var ShL = Math.Abs(this.angleInLeftShoulder - other.angleInLeftShoulder);

            var KnL = Math.Abs(this.angleInLeftKnee - other.angleInLeftKnee);
            var KnR = Math.Abs(this.angleInRightKnee - other.angleInRightKnee);

            var HipCL = Math.Abs(this.angleInHipCenterLeft - other.angleInHipCenterLeft);
            var HipCR = Math.Abs(this.angleInHipCenterRight - other.angleInHipCenterRight);

            var HipR = Math.Abs(this.angleInRightHip - other.angleInRightHip);
            var HipL = Math.Abs(this.angleInLeftHip - other.angleInLeftHip);

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
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
