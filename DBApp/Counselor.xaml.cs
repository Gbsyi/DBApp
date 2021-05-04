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
    /// Логика взаимодействия для Counselor.xaml
    /// </summary>
    public partial class Counselor : Window
    {
        private string connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        private DataSet campsDS = new DataSet();
        int selectedCamp, selectedSquad;
        bool IsLogout = false;
        public Counselor()
        {
            InitializeComponent();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsLogout)
            {
                this.Owner.Show();
                this.Owner = null;
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
             

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chooseCampModal.IsOpen = true;
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                //Заполнение лагерями
                SqlDataAdapter sda = new SqlDataAdapter("select [name], [id] from [dbo].[camps]", conn);
                sda.Fill(campsDS, "camps");
                campsList.SelectionChanged += new SelectionChangedEventHandler(CampSelect);
                for (int i = 0; i < campsDS.Tables["camps"].Rows.Count; i++)
                {
                    campsList.Items.Add(campsDS.Tables["camps"].Rows[i].ItemArray[0] );
                }

            }
        }
        private void SquadSelect(object sender, RoutedEventArgs e)
        {
            ListBox squad = (ListBox)sender;
            selectedSquad = squad.SelectedIndex;
            chosenSquad.Content = $"Выбранный отряд: {squad.SelectedItem}";
            resultPanel.Visibility = Visibility.Visible;
        }
        private void CampSelect(object sender, RoutedEventArgs e)
        {
            ListBox camp = (ListBox)sender;
            selectedCamp = camp.SelectedIndex;
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

        private void chooseSquad_Click(object sender, RoutedEventArgs e)
        {
            chooseCampModal.IsOpen = true;
        }
        private void ChangeUser(object sender, RoutedEventArgs e)
        {
            IsLogout = true;
            this.Close();
            
        }
        private void Apply(object sender, RoutedEventArgs e)
        {
            chooseCampModal.IsOpen = false;
            label.Content = $"Лагерь: \"{campsDS.Tables["camps"].Rows[selectedCamp].ItemArray[0]}\" Отряд: \"{campsDS.Tables["squads"].Rows[selectedSquad].ItemArray[0]}\"";
        }
    }
}
