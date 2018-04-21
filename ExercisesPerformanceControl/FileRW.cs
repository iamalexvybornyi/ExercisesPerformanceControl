/*
 * Class for writing data to file/reading data from file
 * Copyright (C) 2016 Vybornyi Alexander  (iamalexvybornyi@gmail.com/cahek2605@mail.ru)
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.Kinect.Toolkit.BackgroundRemoval;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ExercisesPerformanceControl
{
    /// <summary>
    /// Class for writing data to file/reading data from file
    /// </summary>
    class FileRW
    {
        /// <summary>
        /// Write skeleton data to file
        /// </summary>
        /// <param name="skel">List with a skeleton data</param>
        /// <param name="fileLocation">Location of the output file including the name</param>
        public static void WriteSkelDataToFile(List<Skeleton> skel, string fileLocation)
        {
            Stream stream = null;
            
            // Write skeleton data to file using serialization
            try
            {
                BinaryFormatter bFormatter = new BinaryFormatter();
                stream = new FileStream(fileLocation, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                bFormatter.Serialize(stream, skel);
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

        public static void WritePointsDataToFile(List<List<Point>> skel, string fileLocation)
        {
            Stream stream = null;

            // Write skeleton data to file using serialization
            try
            {
                BinaryFormatter bFormatter = new BinaryFormatter();
                stream = new FileStream(fileLocation, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                bFormatter.Serialize(stream, skel);
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

        /// <summary>
        /// //Read skeleton data of an exercise
        /// </summary>
        /// <param name="ExText">Name of the file with the data of an exercise without format</param>
        /// <returns>Motion i.e. list of the skelton data</returns>
        public static List<Skeleton> ReadSkelDataFromFile(string ExText)
        {
            List<Skeleton> Motion = null;
            Stream stream = null;
            String fileLocation = @"ExercisesData\" + ExText + ".txt";

            // Read file using deserialization
            try
            {
                Motion = new List<Skeleton>();
                BinaryFormatter bFormatter = new BinaryFormatter();
                stream = File.Open(fileLocation, FileMode.Open);
                Motion = (List<Skeleton>)bFormatter.Deserialize(stream);
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

        public static List<List<Point>> ReadPointsDataFromFile(string ExText)
        {
            List<List<Point>> Motion = null;
            Stream stream = null;
            String fileLocation = @"ExercisesData\" + ExText + ".pnt";

            // Read file using deserialization
            try
            {
                Motion = new List<List<Point>>();
                BinaryFormatter bFormatter = new BinaryFormatter();
                stream = File.Open(fileLocation, FileMode.Open);
                Motion = (List<List<Point>>)bFormatter.Deserialize(stream);
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
            String fileLocation = @"ExercisesData\" + exText + ".xrs";

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

        /*
        /// <summary>
        ///  Write BackgroundRemoved data to file
        /// </summary>
        /// <param name="silhouette">List with a Silhouette (BackgroundRemoved) data</param>
        /// <param name="fileLocation">Location of the output file including the name</param>
        public static void WriteSilhouetteDataToFile(List<MyBackgroundRemovedColourFrame> silhouette, string fileLocation)
        {
            Stream stream = null;

            // Write Silhouette data to file using serialization
            try
            {
                BinaryFormatter bFormatter = new BinaryFormatter();
                stream = new FileStream(fileLocation, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                bFormatter.Serialize(stream, silhouette);
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

        /// <summary>
        /// Read BackgroundRemoved data from file
        /// </summary>
        /// <param name="ExText">Name of the file with the data of an exercise without format</param>
        /// <returns>Motion i.e. list of the Silhouette data</returns>
        public static List<MyBackgroundRemovedColourFrame> ReadSilhouetteDataFromFile(string ExText)
        {
            List<MyBackgroundRemovedColourFrame> Motion = null;
            Stream stream = null;
            String fileLocation = ExText + ".sil";

            // Read file using deserialization
            try
            {
                Motion = new List<MyBackgroundRemovedColourFrame>();
                BinaryFormatter bFormatter = new BinaryFormatter();
                stream = File.Open(fileLocation, FileMode.Open);
                Motion = (List<MyBackgroundRemovedColourFrame>)bFormatter.Deserialize(stream);
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
        */
    }

}
