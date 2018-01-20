/*
 * Class for calculation operations 
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
    /// Class for calculation operations
    /// </summary>
    class Calculation
    {
        /// <summary>
        /// Gets angle in a particular joint
        /// </summary>
        /// <param name="skel">skeleton in which we calculate angle</param>
        /// <param name="type0">type of the first joint</param>
        /// <param name="type1">type of the second join in which we calculate the angle</param>
        /// <param name="type2">type of the third joint</param>
        /// <returns>angle in the type1 joint</returns>
        public static double getAngle(Skeleton skel, JointType type0, JointType type1, JointType type2)
        {
            Microsoft.Xna.Framework.Vector3 crossProduct;
            Microsoft.Xna.Framework.Vector3 vec1;
            Microsoft.Xna.Framework.Vector3 vec2;

            Joint joint0 = skel.Joints[type0];
            Joint joint1 = skel.Joints[type1];
            Joint joint2 = skel.Joints[type2];

            double angle = 0;

            //Get vectors
            vec1 = new Microsoft.Xna.Framework.Vector3(joint0.Position.X - joint1.Position.X, joint0.Position.Y - joint1.Position.Y, 
                joint0.Position.Z - joint1.Position.Z);
            vec2 = new Microsoft.Xna.Framework.Vector3(joint2.Position.X - joint1.Position.X, joint2.Position.Y - joint1.Position.Y, 
                joint2.Position.Z - joint1.Position.Z);
            vec1.Normalize();
            vec2.Normalize();

            //Calculate cross product
            crossProduct = Microsoft.Xna.Framework.Vector3.Cross(vec1, vec2);
            double crossProductLength = crossProduct.Length();

            //Calculate dot product
            double dotProduct = Microsoft.Xna.Framework.Vector3.Dot(vec1, vec2);

            //Convert to radians
            double angleInRad = Math.Atan2(crossProductLength, dotProduct);
            angle = Math.Round(angleInRad * (180 / Math.PI), 2);

            double sin = vec1.X * vec2.Y - vec2.X * vec1.Y;
            double cos = vec1.X * vec2.X + vec1.Y * vec2.Y;

            //Calculate angle
            angle = Math.Atan2(sin, cos) * (180 / Math.PI);

            if (angle < 0)
            {
                angle = 180 + (180 - Math.Abs(angle));
            }
            return angle;
        }

        public static AnglesStorage getAnglesInSkeletonJoints(Skeleton skeleton)
        {
            double angle = 0;
            AnglesStorage anglesStorage = new AnglesStorage();
            // Get angle in right shoulder
            angle = Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderRight, JointType.ElbowRight);
            anglesStorage.angleInRightShoulder = angle;

            // Get angle in left shoulder
            angle = Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft);
            anglesStorage.angleInLeftShoulder = angle;

            // Get angle in right elbow
            angle = Calculation.getAngle(skeleton, JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight);
            anglesStorage.angleInRightElbow = angle;

            // Get angle in left elbow
            angle = Calculation.getAngle(skeleton, JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft);
            anglesStorage.angleInLeftElbow = angle;

            // Get angle in right hip
            angle = Calculation.getAngle(skeleton, JointType.HipCenter, JointType.HipRight, JointType.KneeRight);
            anglesStorage.angleInRightHip = angle;

            // Get angle in left hip
            angle = Calculation.getAngle(skeleton, JointType.HipCenter, JointType.HipLeft, JointType.KneeLeft);
            anglesStorage.angleInLeftHip = angle;

            // Get angle in hip center right
            angle = Calculation.getAngle(skeleton, JointType.Spine, JointType.HipCenter, JointType.HipRight);
            anglesStorage.angleInHipCenterRight = angle;

            // Get angle in hip center left
            angle = Calculation.getAngle(skeleton, JointType.Spine, JointType.HipCenter, JointType.HipLeft);
            anglesStorage.angleInHipCenterLeft = angle;

            // Get angle in spine
            angle = Calculation.getAngle(skeleton, JointType.ShoulderCenter, JointType.Spine, JointType.HipCenter);
            anglesStorage.angleInSpine = angle;

            // Get angle in right knee
            angle = Calculation.getAngle(skeleton, JointType.HipRight, JointType.KneeRight, JointType.AnkleRight);
            anglesStorage.angleInRightKnee = angle;

            // Get angle in left knee
            angle = Calculation.getAngle(skeleton, JointType.HipLeft, JointType.KneeLeft, JointType.AnkleLeft);
            anglesStorage.angleInLeftKnee = angle;

            // Get angle in right ankle
            angle = Calculation.getAngle(skeleton, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight);
            anglesStorage.angleInRightAnkle = angle;

            // Get angle in left ankle
            angle = Calculation.getAngle(skeleton, JointType.KneeLeft, JointType.AnkleLeft, JointType.FootLeft);
            anglesStorage.angleInLeftAnkle = angle;

            return anglesStorage;
        }

        private static JointPositionsComparisonResult CompareTwoJointsPositions(double Joint1, double Joint2)
        {
            JointPositionsComparisonResult res = new JointPositionsComparisonResult();

            if (Joint1 - Joint2 <= Helper.err)
            {
                res = JointPositionsComparisonResult.Equal;
            }
            else if (Joint1 > Joint2)
            {
                res = JointPositionsComparisonResult.More;
            }
            else
            {
                res = JointPositionsComparisonResult.Less;
            }

            return res;
        }

        public static  JointPosComparisonResultStorage ComparePositionsOfJoints(Skeleton skeleton)
        {
            JointPosComparisonResultStorage res = new JointPosComparisonResultStorage();

            double YPosWristRight = skeleton.Joints[JointType.WristRight].Position.Y;
            double YPosElbowRight = skeleton.Joints[JointType.ElbowRight].Position.Y;
            double YPosWristLeft = skeleton.Joints[JointType.WristLeft].Position.Y;
            double YPosElbowLeft = skeleton.Joints[JointType.ElbowLeft].Position.Y;
            double YPosShoulderRight = skeleton.Joints[JointType.ShoulderRight].Position.Y;
            double YPosShoulderLeft = skeleton.Joints[JointType.ShoulderLeft].Position.Y;

            double XPosHipLeft = skeleton.Joints[JointType.HipLeft].Position.X;
            double YPosHipLeft = skeleton.Joints[JointType.HipLeft].Position.Y;
            double XPosKneeLeft = skeleton.Joints[JointType.KneeLeft].Position.X;
            double YPosKneeLeft = skeleton.Joints[JointType.KneeLeft].Position.Y;
            double XPosAnkleLeft = skeleton.Joints[JointType.AnkleLeft].Position.X;
            double YPosAnkleLeft = skeleton.Joints[JointType.AnkleLeft].Position.Y;

            double XPosHipRight = skeleton.Joints[JointType.HipRight].Position.X;
            double YPosHipRight = skeleton.Joints[JointType.HipRight].Position.Y;
            double XPosKneeRight = skeleton.Joints[JointType.KneeRight].Position.X;
            double YPosKneeRight = skeleton.Joints[JointType.KneeRight].Position.Y;
            double XPosAnkleRight = skeleton.Joints[JointType.AnkleRight].Position.X;
            double YPosAnkleRight = skeleton.Joints[JointType.AnkleRight].Position.Y;

            double YPosShoulderCenter = skeleton.Joints[JointType.ShoulderCenter].Position.Y;
            double YPosHipCenter = skeleton.Joints[JointType.HipCenter].Position.Y;
            double YPosSpine = skeleton.Joints[JointType.Spine].Position.Y;

            res.RightElbowRightWrist = CompareTwoJointsPositions(YPosElbowRight, YPosWristRight);
            res.RightShoulderRightElbow = CompareTwoJointsPositions(YPosShoulderRight, YPosElbowRight);
            res.RightShoulderRightWrist = CompareTwoJointsPositions(YPosShoulderRight, YPosWristRight);


            res.LeftElbowLeftWrist = CompareTwoJointsPositions(YPosElbowLeft, YPosWristLeft);
            res.LeftShoulderLeftElbow = CompareTwoJointsPositions(YPosShoulderLeft, YPosElbowLeft);
            res.LeftShoulderLeftWrist = CompareTwoJointsPositions(YPosShoulderLeft, YPosWristLeft);

            res.LeftHipLeftKneeX = CompareTwoJointsPositions(XPosHipLeft, XPosKneeLeft);
            res.LeftHipLeftKneeY = CompareTwoJointsPositions(YPosHipLeft, YPosKneeLeft);
            res.LeftKneeLeftAnkleX = CompareTwoJointsPositions(XPosKneeLeft, XPosAnkleLeft);
            res.LeftKneeLeftAnkleY = CompareTwoJointsPositions(YPosKneeLeft, YPosKneeLeft);

            res.RightHipRightKneeX = CompareTwoJointsPositions(XPosHipRight, XPosKneeRight);
            res.RightHipRightKneeY = CompareTwoJointsPositions(YPosHipRight, YPosKneeRight);
            res.RightKneeRightAnkleX = CompareTwoJointsPositions(XPosKneeRight, XPosAnkleRight);
            res.RightKneeRightAnkleY = CompareTwoJointsPositions(YPosKneeRight, YPosKneeRight);

            res.ShoulderCenterHipCenter = CompareTwoJointsPositions(YPosShoulderCenter, YPosHipCenter);
            res.ShoulderCenterSpine = CompareTwoJointsPositions(YPosShoulderCenter, YPosSpine);
            res.SpineHipCenter = CompareTwoJointsPositions(YPosSpine, YPosHipCenter);

            return res;
        }

        /// <summary>
        /// Gets percent of correctness of the repitiion
        /// </summary>
        /// <param name="boolList">list with the info about movement form in every frame of the repetition</param>
        /// <param name="frames">amount of frames</param>
        /// <returns>percent of correctness</returns>
        public static double getPercentOfCorrectness(List<bool> boolList, int frames)
        {
            int tmpCount = boolList.Count();
            double percentOfCorrectness = 0;

            // Count the amount of frames with the bad and good form of movement 
            if (tmpCount >= frames - 1)
            {
                int countOfFalse = 0;
                int countOfTrue = 0;
                for (int i = 0; i < tmpCount; i++)
                {
                    // Form is bad
                    if (boolList[i] == false)
                    {
                        countOfFalse++;
                    }
                    // Form is good
                    else
                    {
                        countOfTrue++;
                    }
                }

                // Get percent of correctness
                percentOfCorrectness = (Convert.ToDouble(countOfTrue) / Convert.ToDouble(tmpCount)) * 100;
            }

            return percentOfCorrectness;
        }
    }
}
