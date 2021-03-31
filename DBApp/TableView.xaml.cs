using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DBApp
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class TableView : Window
    {
        public string account { get; set; } = "";
        private string connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        private LoginPage login = new LoginPage();

        public TableView()
        {
            InitializeComponent();
            
            login.Show();
        }
        public void TableViewLoaded(object sender, RoutedEventArgs e)
        {
            login.Owner = this;
            this.Hide();

            //CampsTable();
        }
        private void ChangeAccount(object sender, RoutedEventArgs e)
        {
            this.Hide();
            
        }
        private void ChildrenBenefitsTable(object sender, RoutedEventArgs e)
        {
            using(SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "exec [dbo].[showChildrenBenefitsTable]";
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
            }
        }
        private void SquadsTable(object sender, RoutedEventArgs e)
        {
            using(SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "exec [dbo].[showSquadsTable]";
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
            }
        }
        private void WorkersTable(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "exec [dbo].[showWorkersTable]";
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
            }
        }
        private void ChildrenTable(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "exec [dbo].[showChildrenTable]";
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
            }
        }
        private void CampsTable(object sender, RoutedEventArgs e)
        {
            using(SqlConnection conn = new SqlConnection(connection))
            {
                
                //string sql = "exec [dbo].[showCampsTable]";
                /*string sqlExpression = "showCampsTable";
                conn.Open();
                SqlCommand command = new SqlCommand(sqlExpression, conn);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                var reader = command.ExecuteReader();
                List<Camp> camps = new List<Camp>();
                if (reader.HasRows)
                {
                    Camp camp = new Camp();
                    camp.id = reader.GetInt32(0);
                    camp.name = reader.GetString(1);
                    camp.description = reader.GetString(2);
                    camp.directorName = reader.GetString(3);
                    camp.managerName = reader.GetString(4);
                    camps.Add(camp);
                }
                table.ItemsSource = camps;*/
                string sql = "exec [dbo].[showCampsTable]";
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
                /*
                string sql = "select * from [dbo].[camps]";
                conn.Open();
                SqlCommand select = new SqlCommand(sql, conn);
                SqlDataReader dataReader = select.ExecuteReader();
                List<Camp> camps = new List<Camp>();
                while (dataReader.Read())
                {
                    Camp camp = new Camp();
                    camp.id = int.Parse(dataReader[0].ToString());
                    camp.name = dataReader[1].ToString();
                    camp.description = dataReader[2].ToString();
                    camp.directorId = int.Parse(dataReader[3].ToString());
                    camp.managerID = int.Parse(dataReader[4].ToString());
                    camps.Add(camp);
                }
                table.ItemsSource = camps;
                */
            }
        }
        private void OnClose(object sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
