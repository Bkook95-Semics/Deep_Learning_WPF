using BeautySolutions.View.ViewModel;
using MaterialDesignThemes.Wpf;
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
using System.Windows.Media.Animation;
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

            var menuLabeling = new List<SubItem>();
            menuLabeling.Add(new SubItem("Using Tool", new LabelingTool()));
            menuLabeling.Add(new SubItem("Using OPENCV", new OPCV()));
            var item0 = new ItemMenu("Labeling", menuLabeling, PackIconKind.Comment);

            var menuTrain = new List<SubItem>();
            menuTrain.Add(new SubItem("Train", new Training()));
            var item1 = new ItemMenu("Train", menuTrain, PackIconKind.Schedule);

            var menuTest = new List<SubItem>();
            menuTest.Add(new SubItem("Test", new Test()));
            var item2 = new ItemMenu("Test", menuTest, PackIconKind.FileReport);
            
            Menu.Children.Add(new UserControlMenuItem(item0, this));
            Menu.Children.Add(new UserControlMenuItem(item1, this));
            Menu.Children.Add(new UserControlMenuItem(item2, this));
        }

        internal void SwitchScreen(object sender)
        {
            var screen = ((UserControl)sender);

            if (screen != null)
            {
                StackPanelMain.Children.Clear();

                DoubleAnimation Ani = new DoubleAnimation();
                Ani.From = 0;
                Ani.To = 1;
                Ani.Duration = new Duration(TimeSpan.FromSeconds(0.3));
                screen.BeginAnimation(OpacityProperty, Ani);
                
                StackPanelMain.Children.Add(screen);
            }
        }

        private void ButtonFechar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
