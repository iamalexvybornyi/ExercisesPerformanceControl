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
    class AnglesStorage
    {
        /// <summary>
        /// List for right shoulder
        /// <summary>
        public List<double> anglesListForRightShoulder;

        /// <summary>
        /// List for left shoulder
        /// <summary>
        public List<double> anglesListForLeftShoulder;

        /// <summary>
        /// List for right elbow
        /// <summary>
        public List<double> anglesListForRightElbow;

        /// <summary>
        /// List for left elbow
        /// <summary>
        public List<double> anglesListForLeftElbow;

        /// <summary>
        /// List for right hip
        /// <summary>
        public List<double> anglesListForRightHip;

        /// <summary>
        /// List for left hip
        /// <summary>
        public List<double> anglesListForLeftHip;

        /// <summary>
        /// List for hip center right
        /// <summary>
        public List<double> anglesListForHipCenterRight;

        /// <summary>
        /// List for hip center left
        /// <summary>
        public List<double> anglesListForHipCenterLeft;

        /// <summary>
        /// List for spine
        /// <summary>
        public List<double> anglesListForSpine;

        /// <summary>
        /// List for right knee
        /// <summary>
        public List<double> anglesListForRightKnee;

        /// <summary>
        /// List for left knee
        /// <summary>
        public List<double> anglesListForLeftKnee;

        /// <summary>
        /// List for right ankle
        /// <summary>
        public List<double> anglesListForRightAnkle;

        /// <summary>
        /// List for left ankle
        /// <summary>
        public List<double> anglesListForLeftAnkle;

        /// <summary>
        /// Initializes a new instance of the AnglesStorage class.
        /// </summary>
        public AnglesStorage()
        {
            anglesListForRightShoulder = new List<double>();
            anglesListForLeftShoulder = new List<double>();
            anglesListForRightElbow = new List<double>();
            anglesListForLeftElbow = new List<double>();
            anglesListForRightHip = new List<double>();
            anglesListForLeftHip = new List<double>();
            anglesListForHipCenterRight = new List<double>();
            anglesListForHipCenterLeft = new List<double>();
            anglesListForSpine = new List<double>();
            anglesListForRightKnee = new List<double>();
            anglesListForLeftKnee = new List<double>();
            anglesListForRightAnkle = new List<double>();
            anglesListForLeftAnkle = new List<double>();
        }
    }
}
