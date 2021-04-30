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
using System.Data;
using System.Data.SqlClient;

namespace DBApp
{
    /// <summary>
    /// Логика взаимодействия для ChooseCampPage.xaml
    /// </summary>
    public partial class ChooseCampPage : Page
    {
        private string connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        public ChooseCampPage()
        {
            InitializeComponent();
        }
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            //using (SqlConnection conn = new SqlConnection(connection))
            //{
            //    conn.Open();
            //    SqlDataAdapter sda = new SqlDataAdapter("select [name] from [dbo].[camps]", conn);
            //    DataSet ds = new DataSet();
            //    sda.Fill(ds);
            //    camps.ItemsSource = ds.Tables[0].DefaultView;
            //    //Вывод

            //    conn.Close();
            //}
        }
    }
}
