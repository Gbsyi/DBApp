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
using System.Data;
using System.Data.SqlClient;

namespace DBApp
{
    /// <summary>
    /// Логика взаимодействия для ShiftsWindow.xaml
    /// </summary>
    public partial class ShiftsWindow : Window
    {
        private string connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        public int WorkerId;
        public string WorkerName;
        private DataSet WorkDS;
        public ShiftsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string command;
                command = $"exec [dbo].[showWorkerSchedule] \'{WorkerId}\'";
                SqlDataAdapter sda = new SqlDataAdapter(command, conn);
                DataSet WorkDS = new();
                sda.Fill(WorkDS, "schedule");
                table.ItemsSource = WorkDS.Tables["schedule"].DefaultView;
                conn.Close();
            }
        }
    }
}
