using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExercisesPerformanceControl
{
    public class JointPosComparisonResultStorage
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
    }
}
