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

namespace DBApp.CounselorPages
{
    /// <summary>
    /// Логика взаимодействия для ChooseSquad.xaml
    /// </summary>
    public partial class ChooseSquad : UserControl
    {
        public ChooseSquad()
        {
            InitializeComponent();
        }
        private string connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        private DataSet campsDS = new DataSet();
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                //Заполнение лагерями
                SqlDataAdapter sda = new SqlDataAdapter("select [name], [id] from [dbo].[camps]", conn);
                sda.Fill(campsDS, "camps");
                campsList.SelectionChanged += new SelectionChangedEventHandler(CampSelect);
                for (int i = 0; i < campsDS.Tables["camps"].Rows.Count; i++)
                {
                    campsList.Items.Add(campsDS.Tables["camps"].Rows[i].ItemArray[0]);
                }

            }
        }
        private void CampSelect(object sender, RoutedEventArgs e)
        {
            ListBox camp = (ListBox)sender;
            chosenCamp.Content = $"Выбранный лагерь: {camp.SelectedItem}";
            squadsList.Visibility = Visibility.Visible;
            //Заполнение отрядами
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter($"select [name] from [dbo].[squads] where [squads].[camp_id] = {campsDS.Tables["camps"].Rows[camp.SelectedIndex].ItemArray[1]}", conn);
                sda.Fill(campsDS, "squads");
                squadsList.SelectionChanged += new SelectionChangedEventHandler(SquadSelect);
                for (int i = 0; i < campsDS.Tables["squads"].Rows.Count; i++)
                {
                    squadsList.Items.Add(campsDS.Tables["squads"].Rows[i].ItemArray[0]);
                }
                conn.Close();
            }
        }

        private void SquadSelect(object sender, RoutedEventArgs e)
        {
            ListBox squad = (ListBox)sender;
            chosenSquad.Content = $"Выбранный отряд: {squad.SelectedItem}";
            resultPanel.Visibility = Visibility.Visible;
        }
        private void Apply(object sender, RoutedEventArgs e)
        {
            //Uri mainPageAddress = new Uri("CounselorPages/MainPage.xaml",UriKind.Relative);
            MessageBox.Show(this.Parent.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
