using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExercisesPerformanceControl
{
    public class JointPosComparisonResultStorage : IEquatable<JointPosComparisonResultStorage>
    {
        public JointPositionsComparisonResult LeftShoulderLeftElbow;
        public JointPositionsComparisonResult LeftShoulderLeftWrist;
        public JointPositionsComparisonResult LeftElbowLeftWrist;

        public JointPositionsComparisonResult RightShoulderRightElbow;
        public JointPositionsComparisonResult RightShoulderRightWrist;
        public JointPositionsComparisonResult RightElbowRightWrist;

        public JointPositionsComparisonResult ShoulderCenterSpine;
        public JointPositionsComparisonResult SpineHipCenter;
        public JointPositionsComparisonResult ShoulderCenterHipCenter;

        public JointPositionsComparisonResult LeftHipLeftKneeY;
        public JointPositionsComparisonResult LeftHipLeftKneeX;
        public JointPositionsComparisonResult LeftKneeLeftAnkleX;
        public JointPositionsComparisonResult LeftKneeLeftAnkleY;

        public JointPositionsComparisonResult RightHipRightKneeY;
        public JointPositionsComparisonResult RightHipRightKneeX;
        public JointPositionsComparisonResult RightKneeRightAnkleX;
        public JointPositionsComparisonResult RightKneeRightAnkleY;
        public JointPosComparisonResultStorage()
        {
            LeftShoulderLeftElbow = new JointPositionsComparisonResult();
            LeftShoulderLeftWrist = new JointPositionsComparisonResult();
            LeftElbowLeftWrist  = new JointPositionsComparisonResult();

            RightShoulderRightElbow = new JointPositionsComparisonResult();
            RightShoulderRightWrist = new JointPositionsComparisonResult();
            RightElbowRightWrist = new JointPositionsComparisonResult();

            ShoulderCenterSpine = new JointPositionsComparisonResult();
            SpineHipCenter = new JointPositionsComparisonResult();

            LeftHipLeftKneeY = new JointPositionsComparisonResult();
            LeftHipLeftKneeX = new JointPositionsComparisonResult();
            LeftKneeLeftAnkleX = new JointPositionsComparisonResult();
            LeftKneeLeftAnkleY = new JointPositionsComparisonResult();

            RightHipRightKneeY = new JointPositionsComparisonResult();
            RightHipRightKneeX = new JointPositionsComparisonResult();
            RightKneeRightAnkleX = new JointPositionsComparisonResult();
            RightKneeRightAnkleY = new JointPositionsComparisonResult();
        }

        public static List<JointType> CompareJointsPositions(JointPosComparisonResultStorage first, JointPosComparisonResultStorage second)
        {
            List<JointType> jointsWithErrors = new List<JointType>();
            if (first.LeftElbowLeftWrist != second.LeftElbowLeftWrist) jointsWithErrors.Add(JointType.ElbowLeft);
            if (first.LeftHipLeftKneeX != second.LeftHipLeftKneeX) jointsWithErrors.Add(JointType.HipLeft);
            if (first.LeftHipLeftKneeY != second.LeftHipLeftKneeY) jointsWithErrors.Add(JointType.HipLeft);
            if (first.LeftKneeLeftAnkleX != second.LeftKneeLeftAnkleX) jointsWithErrors.Add(JointType.KneeLeft);
            if (first.LeftKneeLeftAnkleY != second.LeftKneeLeftAnkleY) jointsWithErrors.Add(JointType.KneeLeft);
            if (first.LeftShoulderLeftElbow != second.LeftShoulderLeftElbow) jointsWithErrors.Add(JointType.ShoulderLeft);
            if (first.LeftShoulderLeftWrist != second.LeftShoulderLeftWrist) jointsWithErrors.Add(JointType.WristLeft);

            if (first.RightElbowRightWrist != second.RightElbowRightWrist) jointsWithErrors.Add(JointType.ElbowLeft);
            if (first.RightHipRightKneeX != second.RightHipRightKneeX) jointsWithErrors.Add(JointType.HipRight);
            if (first.RightHipRightKneeY != second.RightHipRightKneeY) jointsWithErrors.Add(JointType.HipRight);
            if (first.RightKneeRightAnkleX != second.RightKneeRightAnkleX) jointsWithErrors.Add(JointType.KneeRight);
            if (first.RightKneeRightAnkleY != second.RightKneeRightAnkleY) jointsWithErrors.Add(JointType.KneeRight);
            if (first.RightShoulderRightElbow != second.RightShoulderRightElbow) jointsWithErrors.Add(JointType.ShoulderRight);
            if (first.RightShoulderRightWrist != second.RightShoulderRightWrist) jointsWithErrors.Add(JointType.WristRight);

            if (first.ShoulderCenterHipCenter != second.ShoulderCenterHipCenter) jointsWithErrors.Add(JointType.ShoulderCenter);
            if (first.ShoulderCenterSpine != second.ShoulderCenterSpine) jointsWithErrors.Add(JointType.Spine);
            if (first.SpineHipCenter != second.SpineHipCenter) jointsWithErrors.Add(JointType.HipCenter);

            List<JointType> jointsWithErrorsWithoutDuplicates = jointsWithErrors.Distinct().ToList();
            return jointsWithErrorsWithoutDuplicates;
        }

        public bool Equals(JointPosComparisonResultStorage other)
        {
            if (this.LeftElbowLeftWrist != other.LeftElbowLeftWrist) return false;
            if (this.LeftHipLeftKneeX != other.LeftHipLeftKneeX) return false;
            if (this.LeftHipLeftKneeY != other.LeftHipLeftKneeY) return false;
            if (this.LeftKneeLeftAnkleX != other.LeftKneeLeftAnkleX) return false;
            if (this.LeftKneeLeftAnkleY != other.LeftKneeLeftAnkleY) return false;
            if (this.LeftShoulderLeftElbow != other.LeftShoulderLeftElbow) return false;
            if (this.LeftShoulderLeftWrist != other.LeftShoulderLeftWrist) return false;

            if (this.RightElbowRightWrist != other.RightElbowRightWrist) return false;
            if (this.RightHipRightKneeX != other.RightHipRightKneeX) return false;
            if (this.RightHipRightKneeY != other.RightHipRightKneeY) return false;
            if (this.RightKneeRightAnkleX != other.RightKneeRightAnkleX) return false;
            if (this.RightKneeRightAnkleY != other.RightKneeRightAnkleY) return false;
            if (this.RightShoulderRightElbow != other.RightShoulderRightElbow) return false;
            if (this.RightShoulderRightWrist != other.RightShoulderRightWrist) return false;

            if (this.ShoulderCenterHipCenter != other.ShoulderCenterHipCenter) return false;
            if (this.ShoulderCenterSpine != other.ShoulderCenterSpine) return false;
            if (this.SpineHipCenter != other.SpineHipCenter) return false;

            return true;
        }

        public bool Equals(JointPosComparisonResultStorage other, ExerciseType type)
        {
            if (type == ExerciseType.UpperBody)
            {
                if (this.LeftElbowLeftWrist != other.LeftElbowLeftWrist) return false;
                if (this.LeftShoulderLeftElbow != other.LeftShoulderLeftElbow) return false;
                if (this.LeftShoulderLeftWrist != other.LeftShoulderLeftWrist) return false;
                if (this.RightElbowRightWrist != other.RightElbowRightWrist) return false;
                if (this.RightShoulderRightElbow != other.RightShoulderRightElbow) return false;
                if (this.RightShoulderRightWrist != other.RightShoulderRightWrist) return false;
                if (this.ShoulderCenterHipCenter != other.ShoulderCenterHipCenter) return false;
                if (this.ShoulderCenterSpine != other.ShoulderCenterSpine) return false;
                if (this.SpineHipCenter != other.SpineHipCenter) return false;
            }
            else if (type == ExerciseType.LowerBody)
            {
                if (this.LeftHipLeftKneeX != other.LeftHipLeftKneeX) return false;
                if (this.LeftHipLeftKneeY != other.LeftHipLeftKneeY) return false;
                if (this.LeftKneeLeftAnkleX != other.LeftKneeLeftAnkleX) return false;
                if (this.LeftKneeLeftAnkleY != other.LeftKneeLeftAnkleY) return false;
                if (this.ShoulderCenterHipCenter != other.ShoulderCenterHipCenter) return false;
                if (this.ShoulderCenterSpine != other.ShoulderCenterSpine) return false;
                if (this.SpineHipCenter != other.SpineHipCenter) return false;

            }
            else if (type == ExerciseType.WholeBody)
            {
                if (this.LeftElbowLeftWrist != other.LeftElbowLeftWrist) return false;
                if (this.LeftHipLeftKneeX != other.LeftHipLeftKneeX) return false;
                if (this.LeftHipLeftKneeY != other.LeftHipLeftKneeY) return false;
                if (this.LeftKneeLeftAnkleX != other.LeftKneeLeftAnkleX) return false;
                if (this.LeftKneeLeftAnkleY != other.LeftKneeLeftAnkleY) return false;
                if (this.LeftShoulderLeftElbow != other.LeftShoulderLeftElbow) return false;
                if (this.LeftShoulderLeftWrist != other.LeftShoulderLeftWrist) return false;

                if (this.RightElbowRightWrist != other.RightElbowRightWrist) return false;
                if (this.RightHipRightKneeX != other.RightHipRightKneeX) return false;
                if (this.RightHipRightKneeY != other.RightHipRightKneeY) return false;
                if (this.RightKneeRightAnkleX != other.RightKneeRightAnkleX) return false;
                if (this.RightKneeRightAnkleY != other.RightKneeRightAnkleY) return false;
                if (this.RightShoulderRightElbow != other.RightShoulderRightElbow) return false;
                if (this.RightShoulderRightWrist != other.RightShoulderRightWrist) return false;

                if (this.ShoulderCenterHipCenter != other.ShoulderCenterHipCenter) return false;
                if (this.ShoulderCenterSpine != other.ShoulderCenterSpine) return false;
                if (this.SpineHipCenter != other.SpineHipCenter) return false;
            }

            return true;
        }
    }
}
