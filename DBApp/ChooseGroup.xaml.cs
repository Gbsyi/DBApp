using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace DBApp
{
    /// <summary>
    /// Логика взаимодействия для ChooseGroup.xaml
    /// </summary>
    public partial class ChooseGroup : Window
    {
        private string connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";

        public ChooseGroup()
        {
            InitializeComponent();
        }
        List<string> itemsList = new List<string>() {"1","2","3"};
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

            string sql = "select [name] from [camp_groups]";
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                //groupChoose.DataContext = ds.Tables[0].DefaultView;
                //groupChoose.ItemsSource = ds.Tables[0].DefaultView;
            }
            
        }
        private void Submit(object sender, RoutedEventArgs e)
        {

        }
    }
}
