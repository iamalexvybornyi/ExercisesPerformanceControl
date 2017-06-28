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
            return angle;
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
