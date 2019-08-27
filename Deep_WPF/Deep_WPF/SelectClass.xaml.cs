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
    /// SelectClass.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelectClass : Window
    {
        public delegate void ClassEventHandler(string Class);
        public event ClassEventHandler ClassEvent;
        

        public SelectClass()
        {
            InitializeComponent();
            lb_classes.SelectedIndex = 0;
            btn_ok.Focus();
        }
        

        public void CheckboxInit(List<string> classlist)
        {
            foreach (var Class in classlist)
            {
                lb_classes.Items.Add(Class.Trim());
            }
        }
        
        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            ClassEvent(lb_classes.SelectedItem.ToString());
            this.Close();
        }

        private void btn_x_Click(object sender, RoutedEventArgs e)
        {
            ClassEvent("");
            this.Close();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btn_ok_Click(sender, e);
            }
        }
    }
}
