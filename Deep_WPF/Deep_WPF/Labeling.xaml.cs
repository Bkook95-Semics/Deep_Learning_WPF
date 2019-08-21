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

namespace Deep_WPF
{
    /// <summary>
    /// Labeling.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Labeling : Window
    {
        public Labeling()
        {
            InitializeComponent();
        }

        private void btn_tool_Click(object sender, RoutedEventArgs e)
        {
            Window LabelingTool = new LabelingTool();
            LabelingTool.Show();
        }
    }
}
