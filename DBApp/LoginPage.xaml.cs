using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace DBApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void OpenMainMenu(object sender, RoutedEventArgs e)
        {
            TableView tableView = new TableView();
            tableView.Owner = this;
            tableView.Show();
            this.Hide();
            
        }
        private void OpenCounselorMenu(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Counselor counselor = new();
            counselor.Show();
            counselor.Owner = this;
        }
        private void OnClose(object sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OpenHRWindow(object sender, RoutedEventArgs e)
        {
            HRWindow hr = new HRWindow();
            hr.Owner = this;
            hr.Show();
            this.Hide();
        }
        private void OpenEventsWindow(object sender, RoutedEventArgs e)
        {
            EventsWindow ew = new();
            ew.Owner = this;
            ew.Show();
            this.Hide();
        }
        private void OpenCampsWindow(object sender, RoutedEventArgs e)
        {
            CampsWindow camps = new();
            camps.Owner = this;
            camps.Show();
            this.Hide();
        }
    }
}
