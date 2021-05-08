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
        private void UpdateTable()
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WorkerLabel.Content = $"Сотрудник: {WorkerName}";
            UpdateTable();
        }
        private void ShowAddShiftDialog(object sender, RoutedEventArgs e)
        {
            ShiftsDialog.IsOpen = true;
        }
        private void ApplyFilter(object sender, RoutedEventArgs e)
        {
            if (BeginFilterDate.Text != "")
            {
                string beginDateTime = $"{BeginFilterDate.Text} {BeginFilterTime.Text}";
                string endDateTime = $"{EndFilterDate.Text} {EndFilterTime.Text}";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string command;
                    string t = endDateTime.Replace(" ", "");
                    if (t == "")
                    {
                        command = $"exec [dbo].[showShiftsFiltered] \'{WorkerId}\', \'{beginDateTime}\'";
                    }
                    else
                    {
                        command = $"exec [dbo].[showShiftsFiltered] \'{WorkerId}\', \'{beginDateTime}\',\'{endDateTime}\'";
                    }
                    SqlDataAdapter sda = new SqlDataAdapter(command, conn);
                    DataSet WorkDS = new();
                    sda.Fill(WorkDS, "schedule");
                    table.ItemsSource = WorkDS.Tables["schedule"].DefaultView;
                    conn.Close();
                }
                FilterDialog.IsOpen = false;
            }
            else
            {
                
            }
        }
        private void ShowFilterDialog(object sender, RoutedEventArgs e)
        {
            FilterDialog.IsOpen = true;
        }
        private void AddShift(object sender, RoutedEventArgs e)
        {
            string beginDateTime = $"{BeginAddDate.Text} {BeginAddTime.Text}";
            string endDateTime = $"{BeginAddDate.Text} {BeginAddTime.Text}";
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string command = $"exec [dbo].[addShift] \'{WorkerId}\', \'{beginDateTime}\',\'{endDateTime}\'";
                SqlCommand com = new(command, conn);
                com.ExecuteNonQuery();
                conn.Close();
            }
            UpdateTable();
            ShiftsDialog.IsOpen = false;
        }

    }
}
