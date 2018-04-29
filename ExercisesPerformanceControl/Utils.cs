using Microsoft.Kinect.Toolkit.BackgroundRemoval;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExercisesPerformanceControl
{
    public static class Utils
    {
        public static System.Drawing.Bitmap BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
        {
            System.Drawing.Bitmap bmp;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
                enc.Save(outStream);
                bmp = new System.Drawing.Bitmap(outStream);
            }
            return bmp;
        }

        public static WriteableBitmap WriteableBitmapFromBitmap(System.Drawing.Bitmap bitmap)
        {
            System.Windows.Media.Imaging.BitmapSource bitmapSource = 
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            System.Windows.Media.Imaging.WriteableBitmap writeableBitmap = new System.Windows.Media.Imaging.WriteableBitmap(bitmapSource);
            return writeableBitmap;
        }

        public static void CreateThumbnailPNG(string filename, BitmapSource image5)
        {
            if (filename != string.Empty)
            {
                using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(image5));
                    encoder5.Save(stream5);
                }
            }
        }

        public static void CreateThumbnailJPG(string filename, BitmapSource image5)
        {
            if (filename != string.Empty)
            {
                using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                {
                    JpegBitmapEncoder encoder5 = new JpegBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(image5));
                    encoder5.Save(stream5);
                }
            }
        }

        public static MyBackgroundRemovedColourFrame GetWriteableBackgroundRemovedFrame(BackgroundRemovedColorFrame inputFrame)
        {
            MyBackgroundRemovedColourFrame myFrame = new MyBackgroundRemovedColourFrame(inputFrame);
            byte[] tmpPixelData = new byte[inputFrame.GetRawPixelData().Length];
            myFrame.height = inputFrame.Height;
            myFrame.width = inputFrame.Width;
            inputFrame.GetRawPixelData().CopyTo(myFrame.pixelData, 0);

            return myFrame;
        }
    }
}
