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
    }
}
