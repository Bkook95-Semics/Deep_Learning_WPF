using Alturos.Yolo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Deep_WPF
{
    /// <summary>
    /// Test.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Test : UserControl
    {
        string selectedFileName;
        Thread t1 = null;
        public DetectionSystem DetectionSystem = DetectionSystem.Unknown;
        YoloWrapper Wrapper;
        List<string> classList = new List<string>();
        Brush[] brushList;

        YoloData RigthPin = null;
        YoloData LeftPin = null;
        YoloData BotPin = null;
        YoloData SenPin1 = null;
        YoloData SenPin2 = null;

        public Test()
        {
            InitializeComponent();
        }

        private void btn_cfg_Click(object sender, RoutedEventArgs e)
        {
            if(t1 == null)
            {
                t1 = new Thread(CheckSetting);
                t1.Start();
            }
            
            Microsoft.Win32.OpenFileDialog fileDlg = new Microsoft.Win32.OpenFileDialog();
            fileDlg.DefaultExt = ".cfg";
            fileDlg.Filter = "cfg파일 (.cfg)|*.cfg";
            Nullable<bool> result = fileDlg.ShowDialog();

            if (result == true)
            {
                tb_cfg.Text = fileDlg.FileName;
            }
        }

        private void CheckSetting()
        {
            while (true)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    if (tb_cfg.Text != "" && tb_names.Text != "" && tb_weights.Text != "")
                    {
                        bd_Setting.Visibility = Visibility.Collapsed;
                        bd_Testing.Visibility = Visibility.Visible;

                        Wrapper = new YoloWrapper(tb_cfg.Text, tb_weights.Text, tb_names.Text,0,false);
                        DetectionSystem = Wrapper.DetectionSystem;
                        
                        switch (DetectionSystem)
                        {
                            case DetectionSystem.CPU:
                                tb_state.Text = "CPU 환경 : ";
                                break;
                            case DetectionSystem.GPU:
                                tb_state.Text = "GPU 환경 : ";
                                break;
                            default:
                                break;
                        }
                        t1.Abort();
                    }
                }));
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
                Load_Classes();
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

            var sw = new Stopwatch();
            sw.Start();
            var items = Wrapper.Detect(selectedFileName);
            foreach (Alturos.Yolo.Model.YoloItem s in items)
            {
                if (s.Type == "pattern")
                    continue;
                view.Items.Add(new YoloData(s.Type, s.Confidence, s.X, s.Y, s.Width, s.Height));
            }
            sw.Stop();
            tb_state1.Text = sw.Elapsed.TotalMilliseconds + "ms";
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
                view.Items.Clear();
                selectedFileName = fileDlg.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                image.Source = bitmap;
                btn_det.IsEnabled = true;
            }
        }

        public void DrawBox()
        {
            var src = new BitmapImage(new Uri(selectedFileName, UriKind.Absolute));
            Pen pen;
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawImage(src, new Rect(0, 0, src.PixelWidth, src.PixelHeight));
                foreach (YoloData data in view.Items)
                {
                    pen = new Pen(brushList[classList.IndexOf(data.Type)], 3);
                    dc.DrawRectangle(Brushes.Transparent, pen, new Rect(data.X, data.Y, data.Width, data.Height));
                }

                if(Find_Pattern())
                {
                    pen = new Pen(Brushes.Blue, 4);
                    Point Left_s = new Point(LeftPin.X + LeftPin.Width / 2, LeftPin.Y + LeftPin.Height / 2);
                    Point Right_s = new Point(RigthPin.X + RigthPin.Width / 2, RigthPin.Y + RigthPin.Height / 2);
                    Point Sen1_s = new Point(SenPin1.X + SenPin1.Width / 2, SenPin1.Y + SenPin1.Height / 2);
                    Point Sen2_s = new Point(SenPin2.X + SenPin2.Width / 2, SenPin2.Y + SenPin2.Height / 2);
                    Point Bot_s = new Point(BotPin.X + BotPin.Width / 2, BotPin.Y + BotPin.Height / 2);
                    dc.DrawLine(pen, Left_s, Sen1_s);
                    dc.DrawLine(pen, Sen1_s, Right_s);
                    dc.DrawLine(pen, Sen1_s, Right_s);
                    dc.DrawLine(pen, Sen1_s, Sen2_s);
                    dc.DrawLine(pen, Sen2_s, Bot_s);
                }                
            }
            RenderTargetBitmap rtb = new RenderTargetBitmap(src.PixelWidth, src.PixelHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            image.Source = rtb;
            Find_Pattern();
        }

        private void SetPenBrush(string name)
        {
            PropertyInfo[] brushInfo = typeof(Brushes).GetProperties();
            Random rng = new Random();
            int n = brushInfo.Length;
            PropertyInfo temp;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                temp = brushInfo[k];
                brushInfo[k] = brushInfo[n];
                brushInfo[n] = temp;
            }

            brushList = new Brush[brushInfo.Length];
            for (int i = 0; i < brushInfo.Length; i++)
            {
                brushList[i] = (Brush)brushInfo[i].GetValue(null, null);
            }
        }

        private void Load_Classes()
        {
            string text = File.ReadAllText(tb_names.Text);
            string[] split = text.Trim().Split('\n');
            foreach (var name in split)
            {
                classList.Add(name.Trim());
                SetPenBrush(name);
            }
        }

        private bool Find_Pattern()
        {
            List<YoloData> datas = new List<YoloData>();
            foreach (YoloData data in view.Items)
            {
                datas.Add(data);
            }

            int min_r = Int32.MaxValue;
            int min_b = Int32.MaxValue;
            int min_s1 = Int32.MaxValue;
            int min_s2 = Int32.MaxValue;

            RigthPin = null;
            LeftPin = null;
            BotPin = null;
            SenPin1 = null;
            SenPin2 = null;

            // 3개 핀으로 패턴 범위 찾기
            for (int i = 0; i < datas.Count; i ++)
            {
                if (datas[i].Type == "pin")
                {
                    for (int j = 0; j < datas.Count; j++)
                    {
                        if (datas[j].X > datas[i].X)
                        {
                            int temp_r = Math.Abs(datas[j].X - datas[i].X - 103) + Math.Abs(datas[j].Y - datas[i].Y - 5); 
                            int temp_b = Math.Abs(datas[j].X - datas[i].X - 24) + Math.Abs(datas[j].Y - datas[i].Y - 135);
                            if (min_r > temp_r)
                            {
                                min_r = temp_r;
                                RigthPin = datas[j];
                                LeftPin = datas[i];
                            }

                            if(min_b > temp_b)
                            {
                                min_b = temp_b;
                                BotPin = datas[j];
                            }
                        }
                    }
                }
            }

            // 찾은 범위 안에 존재하는 핀 갯수 세기
            int count = 0;
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].Type != "pin")
                    datas.Remove(datas[i]);
                    
                if(datas[i].X >= LeftPin.X && datas[i].X <= RigthPin.X+RigthPin.Width  && datas[i].Y >= LeftPin.Y && datas[i].Y <= BotPin.Y+BotPin.Height)
                {
                    count++;
                }
            }

            if (count == 5) 
            {
                datas.Remove(RigthPin);
                datas.Remove(LeftPin);
                datas.Remove(BotPin);

                for (int i = 0; i < datas.Count; i++) 
                {
                    if (LeftPin.X < datas[i].X)
                    {
                        int temp_s1 = Math.Abs(datas[i].X - LeftPin.X - 28) + Math.Abs(datas[i].Y - LeftPin.Y - 28);
                        if (min_s1 > temp_s1)
                        {
                            min_s1 = temp_s1;
                            SenPin1 = datas[i];
                        }
                    }
                }


                datas.Remove(SenPin1);

                for (int i = 0; i < datas.Count; i++)
                {
                    int temp_s2 = Math.Abs(SenPin1.X - datas[i].X-1)+ Math.Abs(datas[i].Y - SenPin1.Y-56);
                    if (min_s2 > temp_s2)
                    {
                        min_s2 = temp_s2;
                        SenPin2 = datas[i];
                    }
                }
            }



            //오차범위, 오직 5개의 핀으로 구성되어있는 패턴
            if (min_b < 8 && min_r < 8 && min_s1 < 8 && min_s2 < 8 && count == 5)
            {
                return true;
                //var src = new BitmapImage(new Uri(selectedFileName, UriKind.Absolute));
                //Pen pen;
                //DrawingVisual dv = new DrawingVisual();
                //using (DrawingContext dc = dv.RenderOpen())
                //{
                //    dc.DrawImage(src, new Rect(0, 0, src.PixelWidth, src.PixelHeight));
                //    pen = new Pen(Brushes.Blue, 3);
                //    //dc.DrawRectangle(Brushes.Transparent, pen, new Rect(LeftPin.X, LeftPin.Y, RigthPin.X - LeftPin.X + RigthPin.Width, BotPin.Y - LeftPin.Y + BotPin.Height));
                //    Point Left_s = new Point(LeftPin.X + LeftPin.Width / 2, LeftPin.Y + LeftPin.Height / 2);
                //    Point Right_s = new Point(RigthPin.X + RigthPin.Width / 2, RigthPin.Y + RigthPin.Height / 2);
                //    Point Sen1_s = new Point(SenPin1.X + SenPin1.Width / 2, SenPin1.Y + SenPin1.Height / 2);
                //    Point Sen2_s = new Point(SenPin2.X + SenPin2.Width / 2, SenPin2.Y + SenPin2.Height / 2);
                //    Point Bot_s = new Point(BotPin.X + BotPin.Width / 2, BotPin.Y + BotPin.Height / 2);
                //    dc.DrawLine(pen, Left_s, Sen1_s);
                //    dc.DrawLine(pen, Sen1_s, Right_s);
                //    dc.DrawLine(pen, Sen1_s, Right_s);
                //    dc.DrawLine(pen, Sen1_s, Sen2_s);
                //    dc.DrawLine(pen, Sen2_s, Bot_s);
                //}
                //RenderTargetBitmap rtb = new RenderTargetBitmap(src.PixelWidth, src.PixelHeight, 96, 96, PixelFormats.Pbgra32);
                //rtb.Render(dv);
                //image.Source = rtb;                
            }

            return false;
        }
        
    }
}
