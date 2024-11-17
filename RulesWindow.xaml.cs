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

namespace Black0w0Jack
{
    /// <summary>
    /// Логика взаимодействия для RulesWindow.xaml
    /// </summary>
    public partial class RulesWindow : Window
    {
        public RulesWindow()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            StartMenu startMenu = new StartMenu();
            startMenu.Show();
            this.Close();
        }
    }
}
