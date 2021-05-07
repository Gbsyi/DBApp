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
        delegate void LastCommand(object sender, RoutedEventArgs e);

        private string connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        private DataSet workDS = new DataSet();
        int selectedCamp, selectedSquad;
        int squadId;
        int campId;
        int groupId;
        bool IsLogout = false;
        LastCommand lastCommand;
        object lastSender;
        RoutedEventArgs lastArgs;
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
                sda.Fill(workDS, "camps");
                campsList.SelectionChanged += new SelectionChangedEventHandler(CampSelect);
                for (int i = 0; i < workDS.Tables["camps"].Rows.Count; i++)
                {
                    campsList.Items.Add(workDS.Tables["camps"].Rows[i].ItemArray[0] );
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
            squadsList.Items.Clear();
            //Заполнение отрядами
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter($"select [name], [id] from [dbo].[squads] where [squads].[camp_id] = {workDS.Tables["camps"].Rows[camp.SelectedIndex].ItemArray[1]}", conn);
                workDS.Tables["squads"]?.Clear();
                sda.Fill(workDS, "squads");
                squadsList.SelectionChanged += new SelectionChangedEventHandler(SquadSelect);
                for (int i = 0; i < workDS.Tables["squads"].Rows.Count; i++)
                {
                    squadsList.Items.Add(workDS.Tables["squads"].Rows[i].ItemArray[0]);
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
            workDS.Clear();
            IsLogout = true;
            this.Close();   
        }
        private void Apply(object sender, RoutedEventArgs e)
        {
            chooseCampModal.IsOpen = false;
            squadId = int.Parse(workDS.Tables["squads"].Rows[selectedSquad].ItemArray[1].ToString());
            campId = int.Parse(workDS.Tables["camps"].Rows[selectedCamp].ItemArray[1].ToString());
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter($"select [group_id] from [dbo].[groups_of_camps] where [groups_of_camps].[camp_id] = {campId}", conn);
                DataSet ds = new();
                sda.Fill(ds,"group");
                groupId = int.Parse(ds.Tables["group"].Rows[0].ItemArray[0].ToString());
                conn.Close();
            }
            leftPanel.Children.Clear();
            leftPanel.Children.Add(new Label()
            {
                Content = $"Лагерь: \"{workDS.Tables["camps"].Rows[selectedCamp].ItemArray[0]}\"",
                Margin = new Thickness(10, 10, 0, 0),
                FontWeight = FontWeights.Bold
            });
            leftPanel.Children.Add(new Label()
            {
                Content = $"Отряд: \"{workDS.Tables["squads"].Rows[selectedSquad].ItemArray[0]}\"",
                Margin = new Thickness(10, 10, 0, 0),
                FontWeight = FontWeights.Bold
            });
            lastCommand?.Invoke(lastSender, lastArgs);
        }

        private void ShowChildren(object sender, RoutedEventArgs e)
        {
            rightPanel.Children.Clear();
            ListBox childrenList = new();
            rightPanel.Children.Add(new Label() { Content = "Список детей", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10,10,0,0), FontWeight = FontWeights.Bold});
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter($"select [surname],[name],[fathers_name] from [dbo].[children] where [children].[squad_id] = {squadId}", conn);
                workDS.Tables["children"]?.Clear();
                sda.Fill(workDS, "children");
                squadsList.SelectionChanged += new SelectionChangedEventHandler(SquadSelect);
                for (int i = 0; i < workDS.Tables["children"].Rows.Count; i++)
                {
                    childrenList.Items.Add($"{i+1}.  {workDS.Tables["children"].Rows[i].ItemArray[0]} {workDS.Tables["children"].Rows[i].ItemArray[1]} {workDS.Tables["children"].Rows[i].ItemArray[2]}");
                }
                conn.Close();
            }
            childrenList.Margin = new Thickness(10,10,10,10);
            childrenList.VerticalAlignment = VerticalAlignment.Center;
            rightPanel.Children.Add(childrenList);


            lastCommand = ShowChildren;
            lastArgs = e;
            lastSender = sender;
        }
        private void GetTodaySchedule(object sender, RoutedEventArgs e)
        {
            rightPanel.Children.Clear();

            DataGrid schedule = new() { IsReadOnly = true};
            rightPanel.Children.Add(new Label() { Content = "Расписание", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 10, 0, 0), FontWeight = FontWeights.Bold });
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter($"exec [dbo].[getTodaySchedule] {groupId}", conn);
                workDS.Tables["schedule"]?.Clear();
                sda.Fill(workDS, "schedule");
                schedule.ItemsSource = workDS.Tables["schedule"].DefaultView;
                conn.Close();
            }
            schedule.Margin = new Thickness(10, 10, 10, 10);
            rightPanel.Children.Add(schedule);

            lastCommand = GetTodaySchedule;
            lastArgs = e;
            lastSender = sender;
        }
    }   

}
