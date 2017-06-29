using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExercisesPerformanceControl
{
    class JointPosComparisonResultStorage
    {
        public JointPositionsComparisonResult LeftShoulderLeftElbow;
        public JointPositionsComparisonResult LeftShoulderLeftWrist;
        public JointPositionsComparisonResult LeftElbowLeftWrist;

        public JointPositionsComparisonResult RightShoulderRightElbow;
        public JointPositionsComparisonResult RightShoulderRightWrist;
        public JointPositionsComparisonResult RightElbowRightWrist;

        public JointPositionsComparisonResult ShoulderCenterSpine;
        public JointPositionsComparisonResult SpineHipCenter;

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
        }
    }
}
