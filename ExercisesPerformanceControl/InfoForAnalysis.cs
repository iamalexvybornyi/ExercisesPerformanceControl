/*
 * Class to store info about movement correctness in every frame
 * Copyright (C) 2016 Vybornyi Alexander  (iamalexvybornyi@gmail.com/cahek2605@mail.ru)
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace ExercisesPerformanceControl
{
    /// <summary>
    /// Class to store info about movement correctness in every frame
    /// </summary>
    class InfoForAnalysis
    {
        /// <summary>
        /// List for right shoulder
        /// </summary>
        public List<bool> boolListForRightShoulder;

        /// <summary>
        /// List for left shoulder
        /// </summary>
        public List<bool> boolListForLeftShoulder;

        /// <summary>
        /// List for right elbow
        /// </summary>
        public List<bool> boolListForRightElbow;

        /// <summary>
        /// List for left elbow
        /// </summary>
        public List<bool> boolListForLeftElbow;

        /// <summary>
        /// List for right hip
        /// </summary>
        public List<bool> boolListForRightHip;

        /// <summary>
        /// List for left hip
        /// </summary>
        public List<bool> boolListForLeftHip;

        /// <summary>
        /// List for Hip Center Right
        /// </summary>
        public List<bool> boolListForHipCenterRight;

        /// <summary>
        /// List for Hip Center Left
        /// </summary>
        public List<bool> boolListForHipCenterLeft;

        /// <summary>
        /// List for spine
        /// </summary>
        public List<bool> boolListForSpine;

        /// <summary>
        /// List for right knee
        /// </summary>
        public List<bool> boolListForRightKnee;

        /// <summary>
        /// List for left knee
        /// </summary>
        public List<bool> boolListForLeftKnee;

        /// <summary>
        /// List for right ankle
        /// </summary>
        public List<bool> boolListForRightAnkle;

        /// <summary>
        /// List for left ankle
        /// </summary>
        public List<bool> boolListForLeftAnkle;

        /// <summary>
        /// Initializes a new instance of the InfoForAnalysis class.
        /// </summary>
        public InfoForAnalysis()
        {
            boolListForRightShoulder = new List<bool>();
            boolListForLeftShoulder = new List<bool>();
            boolListForRightElbow = new List<bool>();
            boolListForLeftElbow = new List<bool>();
            boolListForRightHip = new List<bool>();
            boolListForLeftHip = new List<bool>();
            boolListForHipCenterRight = new List<bool>();
            boolListForHipCenterLeft = new List<bool>();
            boolListForSpine = new List<bool>();
            boolListForRightKnee = new List<bool>();
            boolListForLeftKnee = new List<bool>();
            boolListForRightAnkle = new List<bool>();
            boolListForLeftAnkle = new List<bool>();
        }
    }
}
