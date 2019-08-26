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
        List<string> filelist = new List<string>();
        List<string> classlist = new List<string>();


        public LabelingTool()
        {
            InitializeComponent();
            SettingInit();
            WindowStyle = WindowStyle.ToolWindow;
        }

        private void SettingInit()
        {
            Application.Current.MainWindow.MinWidth = 50;
            Application.Current.MainWindow.Width = 500;
            Application.Current.MainWindow.Height = 105;
            //tx_class.Visibility = Visibility.Hidden;
            //tb_InputCls.Visibility = Visibility.Hidden;
            //btn_InputCls_new.Visibility = Visibility.Hidden;
            //btn_InputCls.Visibility = Visibility.Hidden;
        }

        private void btn_InputDir_Click(object sender, RoutedEventArgs e)
        {
            Winforms.FolderBrowserDialog folderDlg = new Winforms.FolderBrowserDialog();
            folderDlg.Description = "이미지 폴더를 선택하세요.";
            folderDlg.ShowNewFolderButton = false;
            folderDlg.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            Winforms.DialogResult result = folderDlg.ShowDialog();

            if (result == Winforms.DialogResult.OK)
            {
                lb_filelist.Items.Clear();
                filelist.Clear();
                Im_Image.Source = null;
                tb_InputDir.Text = folderPath = folderDlg.SelectedPath;
                string[] filePaths = Directory.GetFiles(folderDlg.SelectedPath, "*.jpg", SearchOption.AllDirectories);
                foreach (var name in filePaths)
                {
                    string[] split = name.Split('\\');
                    lb_filelist.Items.Add(split[split.Length - 1]);
                    filelist.Add(name);
                }

                //tx_class.Visibility = Visibility.Visible;
                //tb_InputCls.Visibility = Visibility.Visible;
                //btn_InputCls_new.Visibility = Visibility.Visible;
                //btn_InputCls.Visibility = Visibility.Visible;
                Application.Current.MainWindow.Height = 135;

            }

        }

        private void lb_filelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lb_filelist.Items.Count != 0)
            {
                var ImageMat = new Mat(filelist[lb_filelist.SelectedIndex]);
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
            GetClass();
            Epointlist.Add(e.GetPosition(this.Im_Image));
            lb_rectlist.Items.Add(string.Format("{0},{1}", Convert.ToInt32(Spointlist.Last().X), Convert.ToInt32(Epointlist.Last().Y)));
            ListboxAutoScroll(lb_rectlist);
        }

        private void GetClass()
        {
            SelectClass SelectClass = new SelectClass();
            SelectClass.CheckboxInit(classlist);
            SelectClass.ClassEvent += SelectClass_ClassEvent;
            SelectClass.ShowDialog();
        }

        private void SelectClass_ClassEvent(string Class)
        {

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

        private void drawGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Cross;
        }

        private void drawGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void btn_InputCls_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDlg = new Microsoft.Win32.OpenFileDialog();
            fileDlg.DefaultExt = ".txt";
            fileDlg.Filter = "텍스트 파일 (.txt)|*.txt";
            Nullable<bool> result = fileDlg.ShowDialog();
            
            if (result == true)
            {
                tb_InputCls.Text = fileDlg.FileName;
                Application.Current.MainWindow.Height = 165;
                btn_Labeling.IsEnabled = true;
            }
        }

        private void btn_InputCls_new_Click(object sender, RoutedEventArgs e)
        {
            SetClass SetClass = new SetClass();
            SetClass.WritePathEvent += SetClass_WritePathEvent;
            SetClass.ShowDialog();            
        }

        private void SetClass_WritePathEvent(string path)
        {
            tb_InputCls.Text = path;
            Application.Current.MainWindow.Height = 165;
            btn_Labeling.IsEnabled = true;
        }

        private void btn_Labeling_Click(object sender, RoutedEventArgs e)
        {
            tab_Labeling.Visibility = Visibility.Visible;

            Application.Current.MainWindow.MinWidth = 1000;
            Application.Current.MainWindow.Height = 500;
            tab_Setting.IsEnabled = false;
            tab_Labeling.Focus();
            Load_Classes();
        }

        private void Load_Classes()
        {
            string text = File.ReadAllText(tb_InputCls.Text);
            string[] split = text.Trim().Split('\n');
            foreach(var name in split)
            {
                classlist.Add(name);
            }
        }
    }
}
