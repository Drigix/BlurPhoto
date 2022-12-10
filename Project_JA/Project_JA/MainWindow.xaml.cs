using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Threading;
using System.Diagnostics;

namespace Project_JA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private static ManualResetEvent resetEvent = new ManualResetEvent(false);

        public Bitmap bitmap;
        public Int32 blurSize = 10;

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            bitmap = Blur2();

            watch.Stop();

            timeText.Text = watch.ElapsedMilliseconds.ToString();

            bitmap.Save("D:\\Pobrane\\blur3.png");

            BitmapImage bitmapImageAfter = new BitmapImage(new Uri("D:\\Pobrane\\blur3.png"));

            imageAfter.Source = bitmapImageAfter;
        }


       private void test()
       {
            for (int i = 0; i < 10; i++)
            {
                ThreadPool.QueueUserWorkItem(
                 new WaitCallback(delegate (object state) {
                     MyMethod(1);
                }), null);
            }
       }

       private static void MyMethod(int x)
        {
            Thread thread = Thread.CurrentThread;
            string message = $"Background: {thread.IsBackground}, Thread Pool: {thread.IsThreadPoolThread}, Thread ID: {thread.ManagedThreadId}";
            Trace.WriteLine(message);
        }

        private Bitmap Blur2()
        {
            Bitmap image = bitmap;
            Bitmap blurred = new Bitmap(image.Width, image.Height);
            Int32 tempBlurSize = blurSize;

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            int tempX = 0;
            int tempY = 0;


            //    ManualResetEvent resetEvent = new ManualResetEvent(false);
            //    var list = new List<int>();
            //    for (int i = 0; i < 2; i++) list.Add(i);


            //for (int i = 0; i < 1; i++)
            //    {
            //        for (int j = 0; j < 1; j++)
            //        {
            //            Rectangle rectangle = new Rectangle(tempX, tempY, image.Width, image.Height);
            //            ThreadPool.SetMinThreads(1, 0);
            //            ThreadPool.SetMaxThreads(1, 0);
            //            Bitmap tempBlurred = (Bitmap)blurred.Clone();
            //             Bitmap tempImage = (Bitmap)bitmap.Clone();
            //            ThreadPool.QueueUserWorkItem(
            //                new WaitCallback(delegate (object state)
            //                {
            //                    Thread thread = Thread.CurrentThread;
            //                    string message = $"Background: {thread.IsBackground}, Thread Pool: {thread.IsThreadPoolThread}, Thread ID: {thread.ManagedThreadId}";
            //                    Trace.WriteLine(message);
            //                    for (int xx = rectangle.X; xx < rectangle.Width; xx++)
            //                    {
            //                        for (int yy = rectangle.Y; yy < rectangle.Height; yy++)
            //                        {
            //                            int avgR = 0, avgG = 0, avgB = 0;
            //                            int blurPixelCount = 0;

            //                        // average the color of the red, green and blue for each pixel in the
            //                        // blur size while making sure you don't go outside the image bounds
            //                        for (int x = xx; (x < xx + tempBlurSize && x < tempImage.Width); x++)
            //                            {
            //                                for (int y = yy; (y < yy + tempBlurSize && y < tempImage.Height); y++)
            //                                {

            //                                    System.Drawing.Color pixel = tempBlurred.GetPixel(x, y);
            //                                //System.Drawing.Color pixel = blurred.GetPixel(Math.Abs(x), Math.Abs(y));

            //                                    avgR += pixel.R;
            //                                    avgG += pixel.G;
            //                                    avgB += pixel.B;

            //                                    blurPixelCount++;
            //                                }
            //                            }
            //                            if (blurPixelCount > 0)
            //                            {
            //                                avgR = avgR / blurPixelCount;
            //                                avgG = avgG / blurPixelCount;
            //                                avgB = avgB / blurPixelCount;
            //                            }


            //                        // now that we know the average for the blur size, set each pixel to that color
            //                        for (int x = xx; x < xx + tempBlurSize && x < tempImage.Width && x < rectangle.Width; x++)
            //                                for (int y = yy; y < yy + tempBlurSize && y < tempImage.Height && y < rectangle.Height; y++)
            //                                {
            //                                    if (Math.Pow((x - tempImage.Width / 2), 2) + Math.Pow((y - tempImage.Height / 2), 2) > 6000)
            //                                    {
            //                                        tempBlurred.SetPixel(x, y, System.Drawing.Color.FromArgb(avgR, avgG, avgB));
            //                                    }
            //                                }
            //                        }
            //                    }
            //                    resetEvent.Set();
            //                }), list[i]);
            //            tempY += image.Height / 2;
            //        }
            //        tempY = 0;
            //        tempX += image.Width / 2;
            //    }
            //   resetEvent.WaitOne();
            //blurred = tempBlurred;
            //WaitHandle.WaitAll(events.ToArray());
            Rectangle rectangle = new Rectangle(0, 0, image.Width, image.Height);
            for (int xx = rectangle.X; xx < rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    //blur size while making sure you don't go outside the image bounds
                    //for (int x = xx; (x < xx + tempBlurSize && x < image.Width); x++)
                    //{
                    //    for (int y = yy; (y < yy + tempBlurSize && y < image.Height); y++)
                    //    {
                    //        if (Math.Pow((x - image.Width / 2), 2) + Math.Pow((y - image.Height / 2), 2) > 10000)
                    //        {
                    //            System.Drawing.Color pixel = blurred.GetPixel(x, y);

                    //            avgR += pixel.R;
                    //            avgG += pixel.G;
                    //            avgB += pixel.B;

                    //            blurPixelCount++;
                    //        }

                    //    }
                    //}

                    for (int y = yy; (y < yy + tempBlurSize && y < image.Height); y++)
                    {
                        for (int x = xx; (x < xx + tempBlurSize && x < image.Width); x++)
                        {
                            if (Math.Pow((x - image.Width / 2), 2) + Math.Pow((y - image.Height / 2), 2) > 10000)
                            {
                                System.Drawing.Color pixel = blurred.GetPixel(x, y);
                                //System.Drawing.Color pixel = blurred.GetPixel(Math.Abs(x), Math.Abs(y));

                                avgR += pixel.R;
                                avgG += pixel.G;
                                avgB += pixel.B;

                                blurPixelCount++;
                            }

                        }
                    }
                    if (blurPixelCount > 0)
                        {
                            avgR = avgR / blurPixelCount;
                            avgG = avgG / blurPixelCount;
                            avgB = avgB / blurPixelCount;
                        }


                    // now that we know the average for the blur size, set each pixel to that color

                    for (int x = xx; x < xx + tempBlurSize && x < image.Width && x < rectangle.Width; x++)
                        for (int y = yy; y < yy + tempBlurSize && y < image.Height && y < rectangle.Height; y++)
                        {
                            if (Math.Pow((x - image.Width / 2), 2) + Math.Pow((y - image.Height / 2), 2) > 10000)
                            {
                                image.SetPixel(x, y, System.Drawing.Color.FromArgb(avgR, avgG, avgB));
                            }
                        }

                    //for (int y = yy; y < yy + tempBlurSize && y < image.Height && y < rectangle.Height; y++)
                    //    for (int x = xx; x < xx + tempBlurSize && x < image.Width && x < rectangle.Width; x++)
                    //    {
                    //        if (Math.Pow((x - image.Width / 2), 2) + Math.Pow((y - image.Height / 2), 2) > 10000)
                    //        {
                    //            image.SetPixel(x, y, System.Drawing.Color.FromArgb(avgR, avgG, avgB));
                    //        }
                    //    }
                }
            }
            return image;
        }

        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        private void uploadButton_Click(object sender, RoutedEventArgs e)
        {
            //BitmapImage bitmapImageBefore = new BitmapImage(new Uri("D:\\Pobrane\\blurphoto.png"));

            //imageBefore.Source = bitmapImageBefore;
            var fileContent = string.Empty;
            var filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Image Files (*.bmp;*.png;*.jpg)|*.bmp;*.png;*.jpg";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;


            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                    bitmap = new Bitmap(openFileDialog.FileName);
                    BitmapImage bitmapImageBefore = new BitmapImage(new Uri(openFileDialog.FileName));
                    imageBefore.Source = bitmapImageBefore;
                    runButton.IsEnabled = true;
                }
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            test();
        }

    }
}
