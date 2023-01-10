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
    
    public partial class MainWindow : Window
    {
        [DllImport(@"D:\Program Files (x86)\Studia\Język Asemblera\Projekt\repo\Project_JA\x64\Debug\Project_JAAsm.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static int countUpColors(byte[] bitmapBytes, int width, int heigth, int blurSize, int x, int y);

        [DllImport(@"D:\Program Files (x86)\Studia\Język Asemblera\Projekt\repo\Project_JA\x64\Debug\Project_JAAsm.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static int returnCountUpGreen();

        [DllImport(@"D:\Program Files (x86)\Studia\Język Asemblera\Projekt\repo\Project_JA\x64\Debug\Project_JAAsm.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static int returnCountUpRed();

        public MainWindow()
        {
            InitializeComponent();

            amountOfThreads.Value = Environment.ProcessorCount;
        }

        private static ManualResetEvent resetEvent = new ManualResetEvent(false);

        public Bitmap bitmap;
        public Bitmap handleBitmap;
        public int threads = Environment.ProcessorCount;
        public Int32 blurSize;
        public int radius = 0;
        bool isButtonUploadClicked = false;

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            bitmap = new Bitmap(handleBitmap);

            var watch = System.Diagnostics.Stopwatch.StartNew();

            bitmap = Blur();

            watch.Stop();
          
            timeText.Text = watch.ElapsedMilliseconds.ToString() + "ms";

            BitmapImage bitmapImageAfter = chamgeBitmapToBitmapImage(bitmap);
            imageAfter.Source = bitmapImageAfter;
        }
       private static void MyMethod(byte[] bitmapBytes)
        {
            //Thread thread = Thread.CurrentThread;
            //string message = $"Background: {thread.IsBackground}, Thread Pool: {thread.IsThreadPoolThread}, Thread ID: {thread.ManagedThreadId}";
            //Trace.WriteLine(message);
        }

        private Bitmap Blur()
        {
            if (buttonSaveTimes.IsChecked == true)
            {
                if (buttonASM.IsChecked == true)
                {
                    return blurASMSaveTimes();
                }
                else if (buttonCsharp.IsChecked == true)
                {
                    return blurWithThreadsSaveTimes();
                }
                else
                {
                    return bitmap;
                }
            }
            else
            {
                if (buttonASM.IsChecked == true)
                {
                    return blurASM();
                }
                else if (buttonCsharp.IsChecked == true)
                {
                    return blurWithThreads();
                }
                else
                {
                    return bitmap;
                }
            }
        }

        private Bitmap blurASM()
        {
            Bitmap image = bitmap;
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //Przechowujemy adres pamięci do początku danych obrazu
            IntPtr ptr = data.Scan0;
            //Obliczamy rozmiar tablicy bajtów
            int bytes = data.Stride * data.Height;
            //Inicjalizujemy wartość tablicy bajtów o podanym rozmiarze
            byte[] bitmapBytes = new byte[bytes];

            Marshal.Copy(ptr, bitmapBytes, 0, bytes);


            Parallel.For(0, data.Height, new ParallelOptions { MaxDegreeOfParallelism = threads }, y =>
            {
                for (int x = 0; x < data.Width; x++)
                {
                    // Sprawdzamy czy x i y znajdują się obszarze okręgu o promieniu wybranym wcześniej przez użytkownika
                    if (Math.Pow((x - data.Width / 2), 2) + Math.Pow((y - data.Height / 2), 2) > Math.Pow(radius, 2))
                    {
                        int i = y * Math.Abs(data.Stride) + x * 4;

                        int blue = countUpColors(bitmapBytes, data.Width, data.Height, blurSize, x, y);
                        int green = returnCountUpGreen();
                        int red = returnCountUpRed();
                    
                        //Dzielimy sumę wartości składowych przez liczbę pikseli wokół danego piksela, aby obliczyć średnią
                        int counter = (int)Math.Pow(blurSize * 2 + 1, 2);
                        blue /= counter;
                        green /= counter;
                        red /= counter;

                        bitmapBytes[i] = (byte)blue;
                        bitmapBytes[i + 1] = (byte)green;
                        bitmapBytes[i + 2] = (byte)red;
                    }
                }
            });
            Marshal.Copy(bitmapBytes, 0, ptr, bytes);
            image.UnlockBits(data);
            return image;
        }

        private Bitmap blurWithThreads()
        {
                Bitmap image = bitmap;
                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                //Przechowujemy adres pamięci do początku danych obrazu
                IntPtr ptr = data.Scan0;
                //Obliczamy rozmiar tablicy bajtów
                int bytes = data.Stride * data.Height;
                //Inicjalizujemy wartość tablicy bajtów o podanym rozmiarze
                byte[] bitmapBytes = new byte[bytes];

                Marshal.Copy(ptr, bitmapBytes, 0, bytes);

                Parallel.For(0, data.Height, new ParallelOptions { MaxDegreeOfParallelism = threads }, y =>
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        // Sprawdzamy czy x i y znajdują się obszarze okręgu o promieniu wybranym wcześniej przez użytkownika
                        if (Math.Pow((x - data.Width / 2), 2) + Math.Pow((y - data.Height / 2), 2) > Math.Pow(radius, 2))
                        {
                            // Pobieramy adres bajtu dla danego piksela
                            int i = y * Math.Abs(data.Stride) + x * 4;

                            // Tworzymy zmienne do przechowywania wartości składowych RGB dla danego piksela oraz wartość zmiennej wielkości macierzy
                            int blue = 0, green = 0, red = 0, counter = 0;

                            // Przechodzimy po pikselach wokół danego piksela
                            for (int dy = -blurSize; dy <= blurSize; dy++)
                            {
                                for (int dx = -blurSize; dx <= blurSize; dx++)
                                {
                                    //Tworzymy zmienne które odpowiadają za pozycje w macierzy 
                                    int posX = x + dx;
                                    int posY = y + dy;
                                    if (posX >= 0 && posX < data.Width && posY >= 0 && posY < data.Height)
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
                });
                Marshal.Copy(bitmapBytes, 0, ptr, bytes);
                image.UnlockBits(data);
            return image;
        }

        private Bitmap blurASMSaveTimes()
        {
            for (int k = 2; k <= 64; k *= 2)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                threads = k;

                Bitmap image = bitmap;
                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                IntPtr ptr = data.Scan0;

                int bytes = data.Stride * data.Height;
                byte[] bitmapBytes = new byte[bytes];

                Marshal.Copy(ptr, bitmapBytes, 0, bytes);


                Parallel.For(0, data.Height, new ParallelOptions { MaxDegreeOfParallelism = threads }, y =>
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        int i = y * Math.Abs(data.Stride) + x * 4;

                        int blue = countUpColors(bitmapBytes, data.Width, data.Height, blurSize, x, y);
                        int green = returnCountUpGreen();
                        int red = returnCountUpRed();

                        // Sprawdzamy czy x i y znajdują się obszarze okręgu o promieniu wybranym wcześniej przez użytkownika
                        if (Math.Pow((x - data.Width / 2), 2) + Math.Pow((y - data.Height / 2), 2) > Math.Pow(radius, 2))
                        {
                            //Dzielimy sumę wartości składowych przez liczbę pikseli wokół danego piksela, aby obliczyć średnią
                            int counter = (int)Math.Pow(blurSize * 2 + 1, 2);
                            blue /= counter;
                            green /= counter;
                            red /= counter;

                            bitmapBytes[i] = (byte)blue;
                            bitmapBytes[i + 1] = (byte)green;
                            bitmapBytes[i + 2] = (byte)red;
                        }

                    }
                });
                Marshal.Copy(bitmapBytes, 0, ptr, bytes);

                image.UnlockBits(data);


                watch.Stop();
                string path = @"D:\Program Files (x86)\Studia\Język Asemblera\Projekt\repo\Project_JA\Project_JA\times_save.txt";
                string appendText = "Ilość wątków: " + threads.ToString() + Environment.NewLine;
                File.AppendAllText(path, appendText);
                appendText = "Czas: " + watch.ElapsedMilliseconds.ToString() + Environment.NewLine;
                File.AppendAllText(path, appendText);
            }
            return bitmap;
        }

        private Bitmap blurWithThreadsSaveTimes()
        {
            for (int k = 2; k <= 64; k *= 2)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                threads = k;
                Bitmap image = bitmap;
                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                IntPtr ptr = data.Scan0;

                int bytes = data.Stride * data.Height;
                byte[] bitmapBytes = new byte[bytes];

                Marshal.Copy(ptr, bitmapBytes, 0, bytes);

                Parallel.For(0, data.Height, new ParallelOptions { MaxDegreeOfParallelism = threads }, y =>
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        // Pobieramy adres bajtu dla danego piksela
                        int i = y * Math.Abs(data.Stride) + x * 4;

                        // Tworzymy zmienne do przechowywania wartości składowych RGB dla danego piksela oraz wartość zmiennej wielkości macierzy
                        int blue = 0, green = 0, red = 0, counter = 0;

                        // Przechodzimy po pikselach wokół danego piksela
                        for (int dy = -blurSize; dy <= blurSize; dy++)
                        {
                            for (int dx = -blurSize; dx <= blurSize; dx++)
                            {
                                //Tworzymy zmienne które odpowiadają za pozycje w macierzy 
                                int posX = x + dx;
                                int posY = y + dy;
                                if (posX >= 0 && posX < data.Width && posY >= 0 && posY < data.Height)
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

                        // Sprawdzamy czy x i y znajdują się obszarze okręgu o promieniu wybranym wcześniej przez użytkownika
                        if (Math.Pow((x - data.Width / 2), 2) + Math.Pow((y - data.Height / 2), 2) > Math.Pow(radius, 2))
                        {
                            // Dzielimy sumę wartości składowych przez liczbę pikseli wokół danego piksela, aby obliczyć średnią
                            blue /= counter;
                            green /= counter;
                            red /= counter;

                            bitmapBytes[i] = (byte)blue;
                            bitmapBytes[i + 1] = (byte)green;
                            bitmapBytes[i + 2] = (byte)red;
                        }
                    }
                });
                Marshal.Copy(bitmapBytes, 0, ptr, bytes);

                image.UnlockBits(data);
                watch.Stop();
                string path = @"times_save.txt";
                string appendText = "Ilość wątków: " + threads.ToString() + Environment.NewLine;
                File.AppendAllText(path, appendText);
                appendText = "Czas: " + watch.ElapsedMilliseconds.ToString() + Environment.NewLine;
                File.AppendAllText(path, appendText);
            }
            return bitmap;
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
                    handleBitmap = new Bitmap(openFileDialog.FileName);
                    BitmapImage bitmapImageBefore = new BitmapImage(new Uri(openFileDialog.FileName));
                    imageBefore.Source = bitmapImageBefore;
                    imageAfter.Source = new BitmapImage();

                    isButtonUploadClicked = true;

                    if (buttonASM.IsChecked == true || buttonCsharp.IsChecked == true)
                    {
                        runButton.IsEnabled = true;
                    }
                    saveButton.IsEnabled = false;

                    sizeOfRadius.IsEnabled = true;
                    sizeOfRadius.Maximum = (bitmap.Width-10)/2;
                    sizeOfRadiusTextBox.IsEnabled = true;
                }
            }
        }

        private BitmapImage chamgeBitmapToBitmapImage(Bitmap image)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Jpeg);
            stream.Position = 0;

            // utwórz obiekt BitmapImage z danych strumienia
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            spinner.Visibility = Visibility.Hidden;
            saveButton.IsEnabled = true;

            return bitmapImage;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            bitmap.Save("D:\\Pobrane\\blur3.png");
            MessageBox.Show("Your photo has been saved!", "Success");
        }

        private void amountOfThreads_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            threads = (int)amountOfThreads.Value;
        }

        private void blurSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            blurSize = (Int32)sizeOfBlur.Value;
        }

        private void radius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            radius = (int)sizeOfRadius.Value;
        }

        private void buttonCsharp_Checked(object sender, RoutedEventArgs e)
        {
            if (buttonASM.IsChecked == true)
            {
                buttonASM.IsChecked = !buttonASM.IsChecked;
            }
            if(isButtonUploadClicked)
            {
                runButton.IsEnabled = true;
            }
        }

        private void buttonASM_Checked(object sender, RoutedEventArgs e)
        {
            if(buttonCsharp.IsChecked == true)
            {
                buttonCsharp.IsChecked = !buttonCsharp.IsChecked;
            }
            if (isButtonUploadClicked)
            {
                runButton.IsEnabled = true;
            }
        }

        private void buttonSaveTimes_Checked(object sender, RoutedEventArgs e)
        {
            

        }
    }
}
