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
using System.Windows.Shapes;
using Winforms = System.Windows.Forms;
using System.IO;

using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Deep_WPF
{
    /// <summary>
    /// LabelingTool.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LabelingTool : System.Windows.Window
    {
        string folderPath;
        System.Windows.Point prePosition;
        Rectangle currentRect;

        public LabelingTool()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.ToolWindow;
            //ResizeMode = ResizeMode.NoResize;
        }

        private void btn_InputDir_Click(object sender, RoutedEventArgs e)
        {
            Winforms.FolderBrowserDialog folderDlg = new Winforms.FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = false;
            folderDlg.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            Winforms.DialogResult result = folderDlg.ShowDialog();

            if (result == Winforms.DialogResult.OK)
            {
                lb_filelist.Items.Clear();
                tb_InputDir.Text = folderPath = folderDlg.SelectedPath;
                string[] filePaths = Directory.GetFiles(folderDlg.SelectedPath, "*.jpg");
                foreach (var name in filePaths)
                {
                    string[] split = name.Split('\\');
                    lb_filelist.Items.Add(split[split.Length - 1]);
                }
            }

        }

        private void lb_filelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ImageMat = new Mat(folderPath + "\\" + lb_filelist.SelectedItem.ToString());
            int ImageW = ImageMat.Width;
            int ImageH = ImageMat.Height;
            Im_Image.Width = ImageW;
            Im_Image.Height = ImageH;
            Im_Image.Source = ImageMat.ToWriteableBitmap();
        }
        
        private void Im_Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Im_Image.ReleaseMouseCapture();
            SetRectangleProperty();
            currentRect = null;
        }


        private void Im_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            prePosition = e.GetPosition(this.drawGrid);
            this.Im_Image.CaptureMouse();
            if (currentRect == null)
            {
                CreteRectangle();
            }
        }


        private void Im_Image_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point currnetPosition = e.GetPosition(this.drawGrid);
            //tb_InputDir.Text = string.Format("마우스 좌표 : [{0},{1}]", currnetPosition.X, currnetPosition.Y);

            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                if (currentRect != null)
                {
                    double left = prePosition.X;
                    double top = prePosition.Y;

                    if (prePosition.X > currnetPosition.X)
                    {
                        left = currnetPosition.X;
                    }
                    if (prePosition.Y > currnetPosition.Y)
                    {
                        top = currnetPosition.Y;
                    }
                    currentRect.Margin = new Thickness(left, top, 0, 0);
                    currentRect.Width = Math.Abs(prePosition.X - currnetPosition.X);
                    currentRect.Height = Math.Abs(prePosition.Y - currnetPosition.Y);
                }
            }
        }


        private void SetRectangleProperty()
        {
            currentRect.Opacity = 1;
            //currentRect.Fill = new SolidColorBrush(Colors.LightYellow);
            currentRect.StrokeDashArray = new DoubleCollection();
        }

        private void CreteRectangle()
        {
            currentRect = new Rectangle();
            currentRect.Stroke = new SolidColorBrush(Colors.DarkGreen);
            currentRect.StrokeThickness = 2;
            currentRect.Opacity = 0.7;

            DoubleCollection dashSize = new DoubleCollection();
            dashSize.Add(1);
            dashSize.Add(1);
            currentRect.StrokeDashArray = dashSize;

            currentRect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            currentRect.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            this.drawGrid.Children.Add(currentRect);
        }

    }
}
