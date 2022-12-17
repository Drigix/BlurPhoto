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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

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
        public Int32 blurSize = 3;

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            bitmap = Blur();

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

        private Bitmap Blur()
        {
            Bitmap image = bitmap;
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            IntPtr ptr = data.Scan0;

            int bytes = data.Stride * data.Height;
            byte[] bitmapBytes = new byte[bytes];

            Marshal.Copy(ptr, bitmapBytes, 0, bytes);

            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    // Pobieramy adres bajtu dla danego piksela
                    int i = y * Math.Abs(data.Stride) + x * 4;

                    // Tworzymy zmienne do przechowywania wartości składowych RGB dla danego piksela oraz wartość zmiennej wielkości macierzy
                    double blue = 0, green = 0, red = 0, counter = 0;

                    // Przechodzimy po pikselach wokół danego piksela
                    for (int dy = -blurSize; dy <= blurSize; dy++)
                    {
                        for(int dx = -blurSize; dx <= blurSize; dx++)
                        {
                            //Tworzymy zmienne które odpowiadają za pozycje w macierzy 
                            int posX = x + dx;
                            int posY = y + dy;
                            if(posX >= 0 && posX < data.Width && posY >= 0 && posY < data.Height)
                            {
                                // Pobieramy adres bajtu dla piksela wokół danego piksela
                                int j = posY * Math.Abs(data.Stride) + posX * 4;

                                // Pobieramy wartości składowych RGB dla tego piksela
                                blue += bitmapBytes[j];
                                green += bitmapBytes[j + 1];
                                red += bitmapBytes[j + 2];
                            }
                            counter++;
                        }
                    }
                    // Dzielimy sumę wartości składowych przez liczbę pikseli wokół danego piksela, aby obliczyć średnią
                    blue /= counter;
                    green /= counter;
                    red /= counter;

                    bitmapBytes[i] = (byte)blue;
                    bitmapBytes[i + 1] = (byte)green;
                    bitmapBytes[i + 2] = (byte)red;
                }
            }
            Marshal.Copy(bitmapBytes, 0, ptr, bytes);

            image.UnlockBits(data);
            return image;
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
