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
    /// Логика взаимодействия для ClubsWindow.xaml
    /// </summary>
    public partial class ClubsWindow : Window
    {
        private string Connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        private DataSet WorkDS = new();
        private int ClubIndex;
        private int ClubID;
        int WorkerId;
        bool IsChildren = false;
        public ClubsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowClubs(); MainTable.Columns.RemoveAt(0);
        }
        private void ShowClubs()
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new("exec [dbo].[showClubs]", conn);
                WorkDS.Tables["clubs"]?.Clear();
                sda.Fill(WorkDS, "clubs");
                MainTable.ItemsSource = WorkDS.Tables["clubs"].DefaultView;
                
                conn.Close();
            }
        }

        private void SelectedCamp(object sender, SelectedCellsChangedEventArgs e)
        {
            ClubIndex = MainTable.SelectedIndex;
            ClubID = int.Parse(WorkDS.Tables["clubs"].Rows[MainTable.SelectedIndex].ItemArray[0].ToString());
            Menu2.IsEnabled = true;
            Menu3.IsEnabled = true;
            Menu4.IsEnabled = true;
        }
        private void ShowOrganizers(object sender, RoutedEventArgs e)
        {
            OrganizersDialog.IsOpen = true;
            using(SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"exec [dbo].[showClubOrganizers] {ClubID}", conn);
                WorkDS.Tables["organizers"]?.Clear();
                sda.Fill(WorkDS, "organizers");
                OrganizersTable.ItemsSource = WorkDS.Tables["organizers"].DefaultView;
                conn.Close();
            }
        }
        private void ShowAddClubDialog(object sender, RoutedEventArgs e)
        {
            AddClubDialog.IsOpen = true;
        }
        private void AddClub(object sender,RoutedEventArgs e)
        {
            using(SqlConnection conn = new(Connection))
            {
                conn.Open();
                string command = $"insert into [dbo].[clubs] values(\'{ClubName.Text}\',\'{ClubDescription.Text}\')";
                SqlCommand cmd = new(command, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            AddClubDialog.IsOpen = false;
            ShowClubs();
        }
        private void ShowAddOrganizerDialog(object sender, RoutedEventArgs e)
        {
            AddOrganizerDialog.IsOpen = true;
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"select [id], [surname], [name], [fathers_name] from [dbo].[workers] order by [surname]", conn);
                WorkDS.Tables["workers"]?.Clear();
                WorkersListBox.Items?.Clear();
                sda.Fill(WorkDS, "workers");
                var table = WorkDS.Tables["workers"].Rows;
                WorkersListBox.SelectionChanged += new SelectionChangedEventHandler(WorkerSelected);
                for (int i = 0; i < WorkDS.Tables["workers"].Rows.Count; i++) 
                {
                    WorkersListBox.Items.Add($"{i+1}. {table[i].ItemArray[1]} {table[i].ItemArray[2]} {table[i].ItemArray[3]}");
                }
                conn.Close();
            }
        }
        private void WorkerSelected(object sender, RoutedEventArgs e)
        {
            if (WorkersListBox.SelectedIndex >= 0)
            {
                ApplyButton.IsEnabled = true;
                WorkerId = int.Parse(WorkDS.Tables["workers"].Rows[WorkersListBox.SelectedIndex].ItemArray[0].ToString());
            }
        }
        private void AddOrganizer(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                string command = $"insert into [dbo].[clubs_organizers] values({ClubID},{WorkerId})";
                SqlCommand cmd = new(command, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            AddOrganizerDialog.IsOpen = false;
        }
        private void ShowChildrenDialog(object sender, RoutedEventArgs e)
        {
            ShowChildren.IsOpen = true;
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"exec [dbo].[showChildrenInClub] {ClubID}", conn);
                WorkDS.Tables["children"]?.Clear();
                ChildrenListBox.Items?.Clear();
                sda.Fill(WorkDS, "children");
                var table = WorkDS.Tables["children"].Rows;
                for (int i = 0; i < WorkDS.Tables["children"].Rows.Count; i++)
                {
                    ChildrenListBox.Items.Add($"{i + 1}. {table[i].ItemArray[0]} {table[i].ItemArray[1]} {table[i].ItemArray[2]}");
                }
                conn.Close();
            }
        }
    }
}
