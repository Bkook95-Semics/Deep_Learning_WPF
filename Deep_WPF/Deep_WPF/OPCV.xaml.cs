using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;

namespace Deep_WPF
{
    class obj
    {
        public double X
        {
            get;
            set;
        }
        public double Y
        {
            get;
            set;
        }
        public double W
        {
            get;
            set;
        }
        public double H
        {
            get;
            set;
        }
    }
    /// <summary>
    /// OPCV.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OPCV : UserControl
    {
        string fm = "*";
        string SliderValue;
        List<obj> DetectList = new List<obj>();
        string CurrentFN;
        string[] fileEntries;
        string[] fileNames;

        public OPCV()
        {
            InitializeComponent();
        }

        private void Folderdlg_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WinForms.FolderBrowserDialog();
            dialog.Description = "이미지 디렉토리를 지정하세요.";

            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                FPath.Text = dialog.SelectedPath;

                fileEntries = Directory.GetFiles(dialog.SelectedPath, fm);
                fileNames = new string[fileEntries.Length];
                for (int i = 0; i < fileEntries.Length; i++)
                {
                    fileNames[i] = System.IO.Path.GetFileName(fileEntries[i]);
                }
                List<FileInfo> fileList = new List<FileInfo>();
                foreach (string file in fileNames)
                {
                    fileList.Add(new FileInfo(file));
                }

                ImgList.ItemsSource = fileList;

            }
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            if (FPath.Text == " ") return;

            ComboBoxItem item = e.OriginalSource as ComboBoxItem;


            if (item != null)
            {
                fm = item.Content.ToString();

                fileEntries = Directory.GetFiles(FPath.Text, fm);
                fileNames = new string[fileEntries.Length];

                for (int i = 0; i < fileEntries.Length; i++)
                {
                    fileNames[i] = System.IO.Path.GetFileName(fileEntries[i]);
                }

                List<FileInfo> fileList = new List<FileInfo>();
                foreach (string file in fileNames)
                {
                    fileList.Add(new FileInfo(file));
                }

                ImgList.ItemsSource = fileList;
            }
        }

        private void ImgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int pos;
            if (ImgList != null && ImgList.SelectedItem != null)
            {
                if (DetectList != null)
                    DetectList.Clear();

                // var OriImageMat = new Mat(ImgList.SelectedItem.ToString());
                var ThrImageMat = new Mat();
                pos = ImgList.SelectedIndex;
                //CurrentFN = ImgList.SelectedItem.ToString();
                CurrentFN = fileEntries[pos];
                Mat OriImageMat = new Mat(CurrentFN);
                Mat GrayImage = OriImageMat.CvtColor(ColorConversionCodes.BGR2GRAY);

                //int slideVal;
                //int.TryParse(SliderValue, out slideVal);

                Cv2.Threshold(GrayImage, ThrImageMat, int.Parse(SliderValue), 255, ThresholdTypes.Binary);

                int[] mask = {1,1,1,
                               1,1,1,
                               1,1,1};

                Mat kernel = new Mat(3, 3, MatType.CV_32F, mask);

                Cv2.MorphologyEx(ThrImageMat, ThrImageMat, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(ThrImageMat, ThrImageMat, MorphTypes.Close, kernel);
                Cv2.BitwiseNot(ThrImageMat, ThrImageMat);

                var label = new MatOfInt();
                var stats = new MatOfInt();
                var centroids = new MatOfDouble();

                var nLabels = Cv2.ConnectedComponentsWithStats(ThrImageMat, label, stats, centroids, PixelConnectivity.Connectivity8, MatType.CV_32S);


                var statsIndexer = stats.GetGenericIndexer<int>();
                for (int i = 0; i < nLabels; i++)
                {
                    //Threshold줄때 일정크기 이하의 객체는 노이즈로 간주하고 패스
                    if (statsIndexer[i, 4] < 90) continue;

                    var rect = new OpenCvSharp.Rect
                    {
                        X = statsIndexer[i, 0],
                        Y = statsIndexer[i, 1],
                        Width = statsIndexer[i, 2],
                        Height = statsIndexer[i, 3]
                    };

                    Cv2.Rectangle(OriImageMat, rect, new Scalar(33, 113, 243), 3);

                    obj temp = new obj();

                    //중심좌표 x,y (0 ~ 1)
                    temp.X = ((rect.X + rect.Width) / 2.0) / OriImageMat.Width;
                    temp.Y = ((rect.Y + rect.Height) / 2.0) / OriImageMat.Height;
                    temp.W = (double)rect.Width / (double)OriImageMat.Width;
                    temp.H = (double)rect.Height / (double)OriImageMat.Height;

                    DetectList.Add(temp);
                }



                OriginalImg.Source = OriImageMat.ToWriteableBitmap();
                ThresholdImg.Source = ThrImageMat.ToWriteableBitmap();

            }
        }

        private void Threshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = e.OriginalSource as Slider;
            SelectionChangedEventArgs arg = null;

            if (slider != null)
            {
                SliderValue = slider.Value.ToString();

                //인자 모르겠음.
                ImgList_SelectionChanged(this.Threshold, arg);
            }

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string TxtFN = "0";
            if (CurrentFN.EndsWith(".jpeg"))
                TxtFN = CurrentFN.Replace(".jpeg", ".txt");
            if (CurrentFN.EndsWith(".jpg"))
                TxtFN = CurrentFN.Replace(".jpg", ".txt");
            if (CurrentFN.EndsWith(".bmp"))
                TxtFN = CurrentFN.Replace(".bmp", ".txt");
            if (CurrentFN.EndsWith(".gif"))
                TxtFN = CurrentFN.Replace(".gif", ".txt");
            if (CurrentFN.EndsWith(".png"))
                TxtFN = CurrentFN.Replace(".png", ".txt");


            Stream stream = new FileStream(TxtFN, FileMode.Create);
            StreamWriter sw = new StreamWriter(stream);
            //파일오픈
            foreach (var item in DetectList)
            {
                if (item.W == 1)
                    continue;
                sw.WriteLine("0 " + Math.Round(item.X, 6) + " " + Math.Round(item.Y, 6) + " " + Math.Round(item.W, 6) + " " + Math.Round(item.H, 6));
            }
            sw.Close();
            MessageBox.Show(TxtFN + " 저장완료");
        }

        private void Before_Click(object sender, RoutedEventArgs e)
        {
            if (ImgList.SelectedIndex > 0)
                ImgList.SelectedIndex -= 1;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

            ImgList.SelectedIndex += 1;
        }


    }
}
