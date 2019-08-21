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

        public LabelingTool()
        {
            InitializeComponent();
        }

        private void btn_InputDir_Click(object sender, RoutedEventArgs e)
        {
            Winforms.FolderBrowserDialog folderDlg = new Winforms.FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = false;
            folderDlg.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            Winforms.DialogResult result = folderDlg.ShowDialog();

            if(result == Winforms.DialogResult.OK)
            {
                lb_filelist.Items.Clear();
                tb_InputDir.Text = folderPath = folderDlg.SelectedPath;
                string[] filePaths = Directory.GetFiles(folderDlg.SelectedPath, "*.jpg");
                foreach(var name in filePaths)
                {
                    string[] split = name.Split('\\');
                    lb_filelist.Items.Add(split[split.Length-1]);
                }
            }

        }

        private void lb_filelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bigImageMat = new Mat(folderPath + "\\" + lb_filelist.SelectedItem.ToString());
            Im_Image.Source = bigImageMat.ToWriteableBitmap();
        }


        Boolean isDragging = false;
        System.Windows.Point ptStart;
        private void Im_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(isDragging == false)
            {
                ptStart = e.GetPosition(Im_Image);
                isDragging = true;
            }
        }

        private void Im_Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(isDragging)
            {
                isDragging = false;
            }
        }

        private void Im_Image_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (isDragging)
            {
                double x = e.GetPosition(Im_Image).X;
                double y = e.GetPosition(Im_Image).Y;
                
                recSelection.Margin = new Thickness(ptStart.X+5, ptStart.Y+5, ptStart.Y+5, ptStart.X+5);
                recSelection.Width = Abs(ptStart.X - x);
                recSelection.Height = Abs(ptStart.Y - y);
                
                if (recSelection.Visibility != Visibility.Visible)
                {
                    recSelection.Visibility = Visibility.Visible;
                }
            }
        }

        public static double Abs(double number)
        {
            return (number > 0) ? number : -number; 
        }
    }
}
