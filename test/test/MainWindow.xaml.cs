using Alturos.Yolo;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace test
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    
    public partial class MainWindow : Window
    {
        string selectedFileName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Images";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPEG |*.jpg|PNG |*.png";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                selectedFileName = dlg.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                image.Source = bitmap;
            }
            
        }

        private void det_Click(object sender, RoutedEventArgs e)
        {
            view.Items.Clear();

            using (var yoloWrapper = new YoloWrapper("my_yolov3-tiny-pinresize.cfg", "my_yolov3-tiny-pinresize_last.weights", "pinresize.names"))
            {
                var items = yoloWrapper.Detect(selectedFileName);
                foreach(Alturos.Yolo.Model.YoloItem s in items)
                {
                    //data Data = new data();
                    //Data.Type = s.Type;
                    //Data.X = s.X;
                    //Data.Y = s.Y;
                    //Data.Width = s.Width;
                    //Data.Height = s.Height;
                    //Data.Confidence = s.Confidence;
                    //view.Items.Add(Data);
                    view.Items.Add(new YoloData(s.Type, s.Confidence, s.X, s.Y, s.Width, s.Height));                    
                }
                
            }
            DrawBox();
        }


        public void DrawBox()
        {
            var src = new BitmapImage(new Uri(selectedFileName, UriKind.Absolute));
            Pen pen = new Pen(Brushes.Blue, 3);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawImage(src, new Rect(0, 0, src.PixelWidth, src.PixelHeight));
                foreach (YoloData data in view.Items)
                {
                    dc.DrawRectangle(Brushes.Transparent, pen, new Rect(data.X, data.Y, data.Width, data.Height));
                }
            }
            RenderTargetBitmap rtb = new RenderTargetBitmap(src.PixelWidth, src.PixelHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            image.Source = rtb;
        }
        private void view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            YoloData rl = ((ListView)sender).SelectedItem as YoloData;

            if (rl == null)
            {
                return;
            }

            DrawBox();

            var src = new BitmapImage(new Uri(selectedFileName, UriKind.Absolute));
            Pen pen;
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawImage(image.Source, new Rect(0, 0, src.PixelWidth, src.PixelHeight));

                    pen = new Pen(Brushes.Green, 3);
                    var overLayBrush = new SolidColorBrush(Color.FromArgb(150, 255, 255, 102));
                    dc.DrawRectangle(overLayBrush, pen, new Rect(rl.X, rl.Y, rl.Width, rl.Height));
            }
            RenderTargetBitmap rtb = new RenderTargetBitmap(src.PixelWidth, src.PixelHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            image.Source = rtb;


        }

        
    }
}
