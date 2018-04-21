using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExercisesPerformanceControl
{
    [Serializable]
    public class SerializableExercise
    {
        private List<Skeleton> skelData { get; set; }
        private List<List<Point>> skelPointsData { get; set; }
        private ExerciseType exType { get; set; }

        public SerializableExercise(List<Skeleton> inputSkelData, List<List<Point>> inputSkelPointsData, ExerciseType inputExType)
        {
            this.skelData = inputSkelData;
            this.skelPointsData = inputSkelPointsData;
            this.exType = inputExType;
        }

        public SerializableExercise()
        {
            this.skelData = new List<Skeleton>();
            this.skelPointsData = new List<List<Point>>();
            this.exType = 0;
        }

        public List<Skeleton> GetSkelData()
        {
            return skelData;
        }

        public List<List<Point>> GetSkelPointsData()
        {
            return skelPointsData;
        }

        public ExerciseType GetExerciseType()
        {
            return exType;
        }

        public static void WriteToFile(SerializableExercise exercise, String fileLocation)
        {
            Stream stream = null;

            // Write skeleton data to file using serialization
            try
            {
                BinaryFormatter bFormatter = new BinaryFormatter();
                stream = new FileStream(fileLocation, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                bFormatter.Serialize(stream, exercise);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public static SerializableExercise ReadFromFile(String exText)
        {
            SerializableExercise Motion = null;
            Stream stream = null;
            String fileLocation = @"ExercisesData\" + exText + ".txt";

            // Read file using deserialization
            try
            {
                Motion = new SerializableExercise();
                BinaryFormatter bFormatter = new BinaryFormatter();
                stream = File.Open(fileLocation, FileMode.Open);
                Motion = (SerializableExercise)bFormatter.Deserialize(stream);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            if (stream != null)
            {
                stream.Close();
            }

            return Motion;
        }
    }
}
