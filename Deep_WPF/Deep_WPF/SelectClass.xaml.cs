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

        string name = null;

        public SelectClass()
        {
            InitializeComponent();
            this.Focus();
        }



        public void CheckboxInit(List<string> classlist)
        {
            var DockPanel = new DockPanel {};            
            foreach (var Class in classlist)
            {
                RadioButton RadioButton = new RadioButton();
                RadioButton.Content = Class;
                RadioButton.Checked += RadioButton_Checked;
                RadioButton.Height = 15;
                RadioButton.Margin = new Thickness(5);
                DockPanel.SetDock(RadioButton, Dock.Top);
                DockPanel.Children.Add(RadioButton);
            }

            Button Button = new Button();
            Button.Background = new SolidColorBrush(Colors.LightGreen);
            Button.Height = 25;
            Button.Content = "확인";
            Button.Click += new RoutedEventHandler(btn_OKClick);
            Button.Margin = new Thickness(5);
            DockPanel.SetDock(Button, Dock.Top);
            DockPanel.Children.Add(Button);
            
            this.Content = DockPanel;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton obj = sender as RadioButton;
            name = obj.Content.ToString();
        }

        void btn_OKClick(object sender, RoutedEventArgs e)
        {
            ClassEvent(name);
            this.Close();
        }
        

    }
}
