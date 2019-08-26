using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace Deep_WPF
{
    /// <summary>
    /// SetClass.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetClass : Window
    {
        string path;


        public delegate void PathEventHandler(string path);
        public event PathEventHandler WritePathEvent;

        public SetClass()
        {
            InitializeComponent();
            txt.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(txt.Text != "")
            {
                SaveFileDialog saveDlg = new SaveFileDialog();
                saveDlg.Filter = "텍스트 파일|*.txt";

                if (saveDlg.ShowDialog().GetValueOrDefault())
                {
                    path = saveDlg.FileName;
                }
                else return;

                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    sw.Write(txt.Text);
                    WritePathEvent(path);
                }
            }
        }
    }
}
