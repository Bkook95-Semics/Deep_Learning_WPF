using Alturos.Yolo;
using System;
using System.Collections.Generic;
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

namespace Deep_WPF
{
    /// <summary>
    /// Test.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Test : UserControl
    {
        string selectedFileName;
        public Test()
        {
            InitializeComponent();
        }
        private void btn_cfg_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDlg = new Microsoft.Win32.OpenFileDialog();
            fileDlg.DefaultExt = ".cfg";
            fileDlg.Filter = "cfg파일 (.cfg)|*.cfg";
            Nullable<bool> result = fileDlg.ShowDialog();

            if (result == true)
            {
                tb_cfg.Text = fileDlg.FileName;
            }
        }

        private void btn_weights_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDlg = new Microsoft.Win32.OpenFileDialog();
            fileDlg.DefaultExt = ".tb_weights";
            fileDlg.Filter = "weights파일 (.weights)|*.weights";
            Nullable<bool> result = fileDlg.ShowDialog();

            if (result == true)
            {
                tb_weights.Text = fileDlg.FileName;
            }
        }

        private void btn_names_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDlg = new Microsoft.Win32.OpenFileDialog();
            fileDlg.DefaultExt = ".names";
            fileDlg.Filter = "names파일 (.names)|*.names";
            Nullable<bool> result = fileDlg.ShowDialog();

            if (result == true)
            {
                tb_names.Text = fileDlg.FileName;
            }
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


        private void det_Click(object sender, RoutedEventArgs e)
        {
            view.Items.Clear();

            using (var yoloWrapper = new YoloWrapper(tb_cfg.Text, tb_weights.Text, tb_names.Text))
            {
                var items = yoloWrapper.Detect(selectedFileName);
                foreach (Alturos.Yolo.Model.YoloItem s in items)
                {
                    view.Items.Add(new YoloData(s.Type, s.Confidence, s.X, s.Y, s.Width, s.Height));
                }

            }
            DrawBox();
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDlg = new Microsoft.Win32.OpenFileDialog();
            fileDlg.DefaultExt = ".jpg";
            fileDlg.Filter = "JPEG |*.jpg|PNG |*.png";
            Nullable<bool> result = fileDlg.ShowDialog();
            if (result == true)
            {
                selectedFileName = fileDlg.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                image.Source = bitmap;
            }
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

    }
}
