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
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Deep_WPF
{
    /// <summary>
    /// LabelingTool.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LabelingTool : UserControl
    {
        #region 변수들

        string folderPath;
        bool isdraw = false;
        bool move = false;
        System.Windows.Point prePosition;
        Rectangle currentRect;

        List<System.Windows.Point> Spointlist = new List<System.Windows.Point>();
        List<System.Windows.Point> Epointlist = new List<System.Windows.Point>();
        List<Rectangle> Rectlist = new List<Rectangle>();
        List<string> filelist = new List<string>();
        List<string> classlist = new List<string>();
        Thread t1 = null;

        DoubleAnimation In = new DoubleAnimation() { From = 0 , To = 1, Duration = TimeSpan.FromSeconds(0.3) };

        DoubleAnimation Out = new DoubleAnimation() { From = 1, To = 0, Duration = TimeSpan.FromSeconds(0.3) };

        #endregion


        #region init

        public LabelingTool()
        {
            InitializeComponent();
        }
        
        #endregion


        #region 이벤트

        private void SelectClass_ClassEvent(string Class)
        {
            if (Class == "")
            {
                Spointlist.Remove(Spointlist.Last());
                Epointlist.Remove(Epointlist.Last());
                drawcanvas.Children.Remove(Rectlist.Last());
                Rectlist.Remove(Rectlist.Last());
            }
            else
            {
                lv_rectlist.Items.Add(new RectListView
                {
                    Name = Class,
                    SPosX = (Convert.ToInt32(Spointlist.Last().X) + Convert.ToInt32(Epointlist.Last().X)) / 2,
                    SPosY = (Convert.ToInt32(Spointlist.Last().Y) + Convert.ToInt32(Epointlist.Last().Y)) / 2,
                    W = Math.Abs((Convert.ToInt32(Epointlist.Last().X) - Convert.ToInt32(Spointlist.Last().X))),
                    H = Math.Abs((Convert.ToInt32(Epointlist.Last().Y) - Convert.ToInt32(Spointlist.Last().Y)))
                });
            }
        }

        private void SetClass_WritePathEvent(string path)
        {
            tb_InputCls.Text = path;
            Application.Current.MainWindow.Height = 165;
            btn_Labeling.IsEnabled = true;
        }

        #endregion


        #region 마우스 클릭 관련

        private void drawGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Cursor_Arrow();
            if (isdraw)
            {
                System.Windows.Point temp = e.GetPosition(this.Im_Image);

                this.Im_Image.ReleaseMouseCapture();
                SetRectangleProperty();
                currentRect = null;

                if (temp.X > Im_Image.Width || temp.X < 0)
                {
                    temp.X = temp.X < 0 ? 0 : Im_Image.Width;
                }

                if (temp.Y > Im_Image.Height || temp.Y < 0)
                {
                    temp.Y = temp.Y < 0 ? 0 : Im_Image.Height;
                }

                Epointlist.Add(temp);
                GetClass();
                ListViewAutoScroll(lv_rectlist);
                isdraw = false;
                SaveLabeling();
            }

            if (move)
            {
                move = false;
            }
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
                isdraw = true;
                if (currentRect != null)
                {
                    if (currnetPosition.X > Im_Image.Width || currnetPosition.X < 0)
                    {
                        currnetPosition.X = currnetPosition.X < 0 ? 0 : Im_Image.Width;
                    }

                    if (currnetPosition.Y > Im_Image.Height || currnetPosition.Y < 0)
                    {
                        currnetPosition.Y = currnetPosition.Y < 0 ? 0 : Im_Image.Height;
                    }

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

            else
            {
                RectListView rl = lv_rectlist.SelectedItem as RectListView;
                if (rl == null)
                {
                    return;
                }

                if (move || rl.SPosX - rl.W / 2 < e.GetPosition(Im_Image).X && rl.SPosX + rl.W / 2 > e.GetPosition(Im_Image).X &&
                    rl.SPosY - rl.H / 2 < e.GetPosition(Im_Image).Y && rl.SPosY + rl.H / 2 > e.GetPosition(Im_Image).Y)
                {
                    Cursor_Hand();
                    if (e.MouseDevice.MiddleButton == MouseButtonState.Pressed)
                    {
                        move = true;
                        Rectlist[lv_rectlist.SelectedIndex].Margin = new Thickness(e.GetPosition(Im_Image).X + (rl.SPosX - rl.W / 2) - rl.SPosX, e.GetPosition(Im_Image).Y + (rl.SPosY - rl.H / 2) - rl.SPosY, rl.W, rl.H);

                    }
                }
                else
                {
                    Cursor_Cross();
                }


            }

        }

        private void drawGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Rectlist.Count != 0)
            {
                drawcanvas.Children.Remove(Rectlist.Last());
                Rectlist.Remove(Rectlist.Last());
                lv_rectlist.Items.RemoveAt(lv_rectlist.Items.Count - 1);
                SaveLabeling();
            }

        }

        #endregion


        #region 마우스 커서 변경

        private void drawGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor_Cross();
        }

        private void drawGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor_Arrow();
        }

        private void Cursor_Hand()
        {
            Cursor = Cursors.Hand;
        }

        private void Cursor_Arrow()
        {
            Cursor = Cursors.Arrow;
        }

        private void Cursor_Cross()
        {
            Cursor = Cursors.Cross;
        }

        #endregion


        #region 키보드 프레스

        private void tab_Labeling_KeyDown(object sender, KeyEventArgs e)
        {
            if (lv_rectlist.SelectedItem != null)
            {
                if (e.Key == Key.Delete)
                {
                    drawcanvas.Children.Remove(Rectlist[lv_rectlist.SelectedIndex]);
                    Rectlist.RemoveAt(lv_rectlist.SelectedIndex);
                    lv_rectlist.Items.RemoveAt(lv_rectlist.SelectedIndex);
                }

                if (e.Key == Key.R)
                {
                    lv_rectlist.Items.Clear();
                    Clear_draw();
                    Rectlist.Clear();
                    SaveLabeling();
                }
            }

        }


        #endregion


        #region 컨트롤 기능
        
        //버튼클릭
        private void btn_InputDir_Click(object sender, RoutedEventArgs e)
        {
            if(t1 == null)
            {
                t1 = new Thread(CheckSetting);
                t1.Start();
            }

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
            }

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
            }
        }

        private void btn_InputCls_new_Click(object sender, RoutedEventArgs e)
        {
            SetClass SetClass = new SetClass();
            SetClass.WritePathEvent += SetClass_WritePathEvent;
            SetClass.ShowDialog();
        }

        private void btn_Labeling_Click(object sender, RoutedEventArgs e)
        {
            t1.Abort();
            
            bd_Labeling.BeginAnimation(OpacityProperty, In);
            bd_Setting.BeginAnimation(OpacityProperty, Out);

            
            bd_Setting.Visibility = Visibility.Collapsed;
            bd_Labeling.Visibility = Visibility.Visible;
            Load_Classes();
        }

        private void btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            if (lb_filelist.SelectedIndex != 0)
            {
                btn_Next.IsEnabled = true;
                lb_filelist.SelectedIndex -= 1;
            }

            if (lb_filelist.SelectedIndex == 0)
                btn_Prev.IsEnabled = false;

        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (lb_filelist.SelectedIndex+2 == lb_filelist.Items.Count)
            {
                btn_Next.IsEnabled = false;
                lb_filelist.SelectedIndex += 1;
                return;
            }

            btn_Prev.IsEnabled = true;
            lb_filelist.SelectedIndex += 1;
        }


        //인덱스 변경
        private void lb_filelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_filelist.Items.Count != 0 && lb_filelist.SelectedIndex > -1)
            {
                Print_Image();

                if (CheckLabeling(lb_filelist.SelectedItem.ToString().Split('.')[0]))
                {
                    lv_rectlist.Items.Clear();
                    Clear_draw();
                    Rectlist.Clear();

                    string[] Data = File.ReadAllLines(tb_InputDir.Text + "\\" + lb_filelist.SelectedItem.ToString().Split('.')[0] + ".txt");
                    foreach(var line in Data)
                    {
                        string[] rectdata = line.Split(' ');
                        int x, y, w, h;
                        x = Convert.ToInt32(Convert.ToDouble(rectdata[1]) * Im_Image.Width);
                        y = Convert.ToInt32(Convert.ToDouble(rectdata[2]) * Im_Image.Height);
                        w = Convert.ToInt32(Convert.ToDouble(rectdata[3]) * Im_Image.Width);
                        h = Convert.ToInt32(Convert.ToDouble(rectdata[4]) * Im_Image.Height);
                        lv_rectlist.Items.Add(new RectListView
                        {
                            Name = classlist[int.Parse(rectdata[0])],
                            SPosX =x,
                            SPosY = y,
                            W = w,
                            H = h,
                        });

                        CreteRectangle();
                        currentRect.Margin = new Thickness(x-w/2, y-h/2, 0, 0);
                        currentRect.Width = w;
                        currentRect.Height = h;
                        currentRect.Opacity = 1;
                        currentRect.StrokeDashArray = new DoubleCollection();
                        currentRect = null;
                    }
                }
                else
                {
                    lv_rectlist.Items.Clear();
                    Clear_draw();
                    Rectlist.Clear();
                }
            }            
        }

        private void lv_rectlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RectListView rl = ((ListView)sender).SelectedItem as RectListView;

            if (rl == null)
            {
                return;
            }

            foreach (var rect in Rectlist)
            {
                rect.Stroke = new SolidColorBrush(Colors.DarkGreen);
            }

            Rectlist[lv_rectlist.SelectedIndex].Stroke = new SolidColorBrush(Colors.Aqua);

        }
        #endregion


        #region 사각형 그리기

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

        private void SetRectangleProperty()
        {
            currentRect.Opacity = 1;
            currentRect.StrokeDashArray = new DoubleCollection();
        }

        #endregion


        #region 기타 유틸리티 함수

        private void GetClass()
        {
            SelectClass SelectClass = new SelectClass();
            SelectClass.CheckboxInit(classlist);
            SelectClass.ClassEvent += SelectClass_ClassEvent;
            SelectClass.Owner = Application.Current.MainWindow;
            SelectClass.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            SelectClass.ShowDialog();
        }

        private void ListViewAutoScroll(ListView lv)
        {
            lv.Items.MoveCurrentToLast();
            lv.ScrollIntoView(lv.Items.CurrentItem);
            lv.SelectedIndex = lv.Items.Count - 1;
        }

        private void ResizeWindow(int ImageW, int ImageH)
        {
            if (ImageH > drawcanvas.ActualHeight)
            {
                Application.Current.MainWindow.Height += ImageH - drawcanvas.ActualHeight;
                drawcanvas.Height = ImageH;                
            }
            if (ImageW > drawcanvas.ActualWidth)
            {
                Application.Current.MainWindow.Width += ImageW - drawcanvas.ActualWidth;
                drawcanvas.Width = ImageW;                
            }
        }

        private void Load_Classes()
        {
            string text = File.ReadAllText(tb_InputCls.Text);
            string[] split = text.Trim().Split('\n');
            foreach (var name in split)
            {
                classlist.Add(name.Trim());
            }

        }

        private void SaveLabeling()
        {
            string labeling_Data = null;
            string[] Name = lb_filelist.SelectedItem.ToString().Split('.');
            try
            {
                foreach (var data in lv_rectlist.Items)
                {
                    var temp = data as RectListView;
                    int index = classlist.FindIndex(r => r == temp.Name);
                    Name = lb_filelist.SelectedItem.ToString().Split('.');


                    labeling_Data += index + " " + DoubletoString(temp.SPosX / Im_Image.Width) + " " +
                       DoubletoString(temp.SPosY / Im_Image.Height) + " " + DoubletoString(temp.W / Im_Image.Width) + " " + DoubletoString(temp.H / Im_Image.Height) + "\r";

                }
                File.WriteAllText(tb_InputDir.Text + "\\" + Name[0] + ".txt", labeling_Data);
            }
            catch
            {
                File.WriteAllText(tb_InputDir.Text + "\\" + Name[0] + ".txt", "");
            }
            
        }

        private int ClasstoNum(string Name)
        {
            return classlist.FindIndex(r => r == Name);
        }

        private string DoubletoString(double value)
        {
            return string.Format("{0:0.000000}", value);
        }

        private bool CheckLabeling(string name)
        {
            if (File.Exists(tb_InputDir.Text + "\\" + name + ".txt"))
            {
                return true;
            }
            return false;
        }

        private void Print_Image()
        {
            var ImageMat = new Mat(filelist[lb_filelist.SelectedIndex]);
            int ImageW = ImageMat.Width;
            int ImageH = ImageMat.Height;
            Im_Image.Width = ImageW;
            Im_Image.Height = ImageH;
            Im_Image.Source = ImageMat.ToWriteableBitmap();
            ResizeWindow(ImageW, ImageH);
        }

        private void Clear_draw()
        {
            foreach (var rect in Rectlist)
            {
                drawcanvas.Children.Remove(rect);
            }
        }

        void CheckSetting()
        {
            while (true)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    if (tb_InputDir.Text != "" && tb_InputCls.Text != "")
                    {
                        btn_Labeling.IsEnabled = true;
                    }
                }));
            }
        }

        #endregion


    }
}
