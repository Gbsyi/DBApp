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
        private void TableOutput(string sql)
        {
            using(SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
            }
        }
        private void ChildrenBenefitsTable(object sender, RoutedEventArgs e)
        {
            TableOutput("exec [dbo].[showChildrenBenefitsTable]");/*
            using(SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "exec [dbo].[showChildrenBenefitsTable]";
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
            }*/
        }
        private void SquadsTable(object sender, RoutedEventArgs e)
        {
            TableOutput("exec [dbo].[showSquadsTable]");
            /*
            using(SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "exec [dbo].[showSquadsTable]";
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
            }*/
        }
        private void WorkersTable(object sender, RoutedEventArgs e)
        {
            TableOutput("exec [dbo].[showWorkersTable]");
            /*
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "exec [dbo].[showWorkersTable]";
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
            }*/
        }
        private void ChildrenTable(object sender, RoutedEventArgs e)
        {
            TableOutput("exec [dbo].[showChildrenTable]");
            /*
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "exec [dbo].[showChildrenTable]";
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
            }*/
        }
        private void CampsTable(object sender, RoutedEventArgs e)
        {
            TableOutput("exec [dbo].[showCampsTable]");
            /*
            using(SqlConnection conn = new SqlConnection(connection))
            {                
                string sql = "exec [dbo].[showCampsTable]";
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
            }
            */
        }
        private void OnClose(object sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
