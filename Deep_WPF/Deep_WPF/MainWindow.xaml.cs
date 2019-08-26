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
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_labeling_Click(object sender, RoutedEventArgs e)
        {
            Labeling Labeling = new Labeling();
            Labeling.Show();
        }

        private void btn_train_Click(object sender, RoutedEventArgs e)
        {
            Test Test = new Test();
            Test.Show();
        }

        private void btn_test_Click(object sender, RoutedEventArgs e)
        {
            Train Train = new Train();
            Train.Show();
        }
    }
}
