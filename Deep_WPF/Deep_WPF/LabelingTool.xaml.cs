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

        List<System.Windows.Point> Spointlist = new List<System.Windows.Point>();
        List<System.Windows.Point> Epointlist = new List<System.Windows.Point>();
        List<Rectangle> Rectlist = new List<Rectangle>();
        
        
        

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
                Im_Image.Source = null;
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
            if(lb_filelist.Items.Count != 0)
            {
                var ImageMat = new Mat(folderPath + "\\" + lb_filelist.SelectedItem.ToString());
                int ImageW = ImageMat.Width;
                int ImageH = ImageMat.Height;
                Im_Image.Width = ImageW;
                Im_Image.Height = ImageH;
                Im_Image.Source = ImageMat.ToWriteableBitmap();
                ResizeWindow(ImageH);
            }
            
        }
        
        private void drawGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Im_Image.ReleaseMouseCapture();
            SetRectangleProperty();
            currentRect = null;
            Epointlist.Add(e.GetPosition(this.Im_Image));
            lb_rectlist.Items.Add(string.Format("{0},{1}", Convert.ToInt32(Spointlist.Last().X), Convert.ToInt32(Epointlist.Last().Y)));
            ListboxAutoScroll(lb_rectlist);
        }
        
        private void drawGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            prePosition = e.GetPosition(this.Im_Image);
            this.Im_Image.CaptureMouse();
            if (currentRect == null)
            {
                Spointlist.Add(e.GetPosition(this.Im_Image));
                CreteRectangle();
            }
        }
        
        private void drawGrid_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point currnetPosition = e.GetPosition(this.Im_Image);            
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

        private void drawGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(Rectlist.Count != 0)
            {
                drawcanvas.Children.Remove(Rectlist.Last());
                Rectlist.Remove(Rectlist.Last());
                lb_rectlist.Items.RemoveAt(lb_rectlist.Items.Count - 1);
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

            drawcanvas.Children.Add(currentRect);
            Rectlist.Add(currentRect);
        }


        private void ListboxAutoScroll(ListBox lb)
        {
            lb.Items.MoveCurrentToLast();
            lb.ScrollIntoView(lb.Items.CurrentItem);
        }

        private void ResizeWindow(int ImageH)
        {
            if(ImageH > drawcanvas.ActualHeight)
            {
                Application.Current.MainWindow.Height += ImageH - drawcanvas.ActualHeight;
            }
        }

       
    }
}
