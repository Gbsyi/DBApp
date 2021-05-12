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
    /// Логика взаимодействия для EventsWindow.xaml
    /// </summary>
    public partial class EventsWindow : Window
    {
        private string Connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        private DataSet WorkDS = new DataSet();
        private bool IsLogout = false;
        private int SelectedCampId;
        private int SelectedOrganizersGroupId;
        private int eventId;
        private TextBox group;
        private ComboBox workers;
        private TextBox campGroup;
        private ComboBox camps;
        private int LastSelectedOrganizersGroup;
        public EventsWindow()
        {
            InitializeComponent();
        }
        private void ChangeUser(object sender, RoutedEventArgs e)
        {
            //workDS.Clear();
            IsLogout = true;
            this.Close();
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
        private void GetSchedule()
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new("exec [dbo].[getSchedule]", conn);
                WorkDS.Tables["schedule"]?.Clear();
                sda.Fill(WorkDS, "schedule");
                mainTable.ItemsSource = WorkDS.Tables["schedule"].DefaultView;
                conn.Close();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetSchedule();
        }
        private void AddEvent(object sender, RoutedEventArgs e)
        {
            string eventName = EventNameTB.Text;
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"exec [dbo].[checkEvent] \'{eventName}\'", conn);
                WorkDS.Tables["eventId"]?.Clear();
                sda.Fill(WorkDS, "eventId");
                eventId = int.Parse(WorkDS.Tables["eventId"].Rows[0].ItemArray[0].ToString());
                if (eventId == -1)
                {
                    NewGroupDialog.IsOpen = true;
                }
                else
                {
                    AddSchedule();
                }
                conn.Close();
            }
        }
        private void AddSchedule()
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                string eventTime = $"{EventDatePicker.Text} {EventTimePicker.Text}";
                string command = $"insert into [dbo].[camp_events] values({SelectedCampId}, {eventId},  {SelectedOrganizersGroupId}, \'{eventTime}\')";
                SqlCommand cmd = new(command, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            GetSchedule();
        }
        private void AddNewEvent(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter eventSda = new($"exec [dbo].[newEvent] \'{EventNameTB.Text}\', \'{DescriptionTB.Text}\'", conn);
                WorkDS.Tables["newEventId"]?.Clear();
                eventSda.Fill(WorkDS, "newEventId");
                eventId = int.Parse(WorkDS.Tables["newEventId"].Rows[0].ItemArray[0].ToString());
            }
            NewGroupDialog.IsOpen = false;
            AddSchedule();
        }
        //Диалоговое окно "Добавление группы организаторов"
        private void ShowOrganizersGroup(object sender, RoutedEventArgs e)
        {
            OrganizersGroupDialog.IsOpen = true;
            FillOrganizersGroupsListBox();
        }
        private void FillOrganizersGroupsListBox()
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new("exec [dbo].[showEventOrganizers]", conn);
                WorkDS.Tables["organizersGroups"]?.Clear();
                OrganizersGroupsListBox.Items?.Clear();
                sda.Fill(WorkDS, "organizersGroups");
                OrganizersGroupsListBox.SelectionChanged += new SelectionChangedEventHandler(ShowWorkersInGroups);
                for (int i = 0; i < WorkDS.Tables["organizersGroups"].Rows.Count; i++)
                {
                    OrganizersGroupsListBox.Items.Add(WorkDS.Tables["organizersGroups"].Rows[i].ItemArray[1]);
                }
                conn.Close();
            }
            LastSelectedOrganizersGroup = -1;
        }
        private void ShowWorkersInGroups(object sender, SelectionChangedEventArgs e)
        {
            if (OrganizersGroupsListBox.SelectedIndex < 0 || OrganizersGroupsListBox.SelectedItem.GetType().Equals(typeof(TextBox)))
            {
                AddWorkerToGroupButton.IsEnabled = false;
                OrganizersGroupDialogApplyButton.IsEnabled = false;
                OrganizersListBox.Items?.Clear();
                return;       
            }
            AddWorkerToGroupButton.IsEnabled = true;
            OrganizersGroupDialogApplyButton.IsEnabled = true;
            SelectedOrganizersGroupId = int.Parse(WorkDS.Tables["organizersGroups"].Rows[OrganizersGroupsListBox.SelectedIndex].ItemArray[0].ToString());
            GetWorkersInGroup();
            if (NewOrganizersButton.Content.ToString() == "Подтвердить")
            {
                FillOrganizersGroupsListBox();
                NewOrganizersButton.Content = "Добавить";
                NewOrganizersButton.Click -= AddNewOrganizersGroupApply;
                NewOrganizersButton.Click += AddNewOrganizersGroup;
            }
            if (AddWorkerToGroupButton.Content.ToString() == "Подтвердить")
            {
                AddWorkerToGroupButton.Content = "Добавить";
                AddWorkerToGroupButton.Click -= AddWorkerToGroupApply;
                AddWorkerToGroupButton.Click += AddWorkerToGroup;
            }
            LastSelectedOrganizersGroup = OrganizersGroupsListBox.SelectedIndex;
        }
        private void GetWorkersInGroup() 
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"exec [dbo].[showWorkersInGroup] {SelectedOrganizersGroupId}", conn);
                WorkDS.Tables["workersInGroup"]?.Clear();
                OrganizersListBox.Items?.Clear();
                sda.Fill(WorkDS, "workersInGroup");
                for (int i = 0; i < WorkDS.Tables["workersInGroup"].Rows.Count; i++)
                {
                    var table = WorkDS.Tables["workersInGroup"].Rows[i].ItemArray;
                    OrganizersListBox.Items.Add($"{table[0]} {table[1]} {table[2]}");
                }
                if (WorkDS.Tables["workersInGroup"].Rows.Count == 0)
                {
                    OrganizersListBox.Items.Add("Сотрудники отсутствуют");
                }
                conn.Close();
            }

        }
        private void AddWorkerToGroup(object sender, RoutedEventArgs e)
        {
            workers = new();
            using(SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new("select [id], [surname], [name], [fathers_name] from [dbo].[workers] order by [surname]", conn);
                WorkDS.Tables["workers"]?.Clear();
                sda.Fill(WorkDS, "workers");
                var table = WorkDS.Tables["workers"].Rows;
                for(int i = 0; i < WorkDS.Tables["workers"].Rows.Count; i++)
                {
                    workers.Items.Add($"{table[i].ItemArray[1]} {table[i].ItemArray[2]} {table[i].ItemArray[3]}");
                }
                workers.SelectedIndex = 0;
                conn.Close();
            }
            OrganizersListBox.Items.Add(workers);
            AddWorkerToGroupButton.Content = "Подтвердить";
            AddWorkerToGroupButton.Click -= AddWorkerToGroup;
            AddWorkerToGroupButton.Click += AddWorkerToGroupApply;

        }
        private void AddWorkerToGroupApply(object sender, RoutedEventArgs e)
        {
            int newWorkerId = int.Parse(WorkDS.Tables["workers"].Rows[workers.SelectedIndex].ItemArray[0].ToString());
            using(SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlCommand cmd = new($"insert into [dbo].[groups_of_organizers] values({SelectedOrganizersGroupId}, {newWorkerId})",conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            AddWorkerToGroupButton.Content = "Добавить";
            AddWorkerToGroupButton.Click -= AddWorkerToGroupApply;
            AddWorkerToGroupButton.Click += AddWorkerToGroup;
            GetWorkersInGroup();
        }
        private void ChooseOrganizersGroup(object sender, RoutedEventArgs e)
        {
            OrganizersGroupDialog.IsOpen = false;
            OrganizersGroupDialogApplyButton.
                IsEnabled = false;
            ChosenOrganizersGroup.Text = $"Выбрано: {WorkDS.Tables["organizersGroups"].Rows[OrganizersGroupsListBox.SelectedIndex].ItemArray[1]}";
        }
        private void AddNewOrganizersGroup(object sender, RoutedEventArgs e)
        {
            group = new TextBox() { Style = EventNameTB.Style, Padding = new(6), MaxLength = 20, Margin = new Thickness(0, 0, 0, 5), Width = 150 };
            OrganizersGroupsListBox.Items.Add(group);
            NewOrganizersButton.Content = "Подтвердить";
            //Убираем старый обработчик
            NewOrganizersButton.Click -= AddNewOrganizersGroup;
            //Добавляем новый
            NewOrganizersButton.Click += AddNewOrganizersGroupApply;
        }
        private void AddNewOrganizersGroupApply(object sender, RoutedEventArgs e)
        {
            if (group.Text != "")
            {
                using (SqlConnection conn = new(Connection))
                {
                    conn.Open();
                    SqlCommand cmd = new($"insert into [dbo].[event_organizers] values(\'{group.Text}\')", conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                NewOrganizersButton.Content = "Добавить";
                //Убираем старый обработчик
                NewOrganizersButton.Click -= AddNewOrganizersGroupApply;
                //Добавляем новый
                NewOrganizersButton.Click += AddNewOrganizersGroup;
                FillOrganizersGroupsListBox();
            }
        }


        //Диалоговое окно "Добавление группы лагерей"
        private void ShowCampGroups(object sender, RoutedEventArgs e)
        {
            CampGroupsDialog.IsOpen = true;
            FillCampGroupsListBox();
        }
        private void FillCampGroupsListBox()
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new("exec [dbo].[showGroups]", conn);
                WorkDS.Tables["campGroups"]?.Clear();
                CampGroupsListBox.Items?.Clear();
                sda.Fill(WorkDS, "campGroups");
                CampGroupsListBox.SelectionChanged += new SelectionChangedEventHandler(ShowCampsInGroups);
                for (int i = 0; i < WorkDS.Tables["campGroups"].Rows.Count; i++)
                {
                    CampGroupsListBox.Items.Add(WorkDS.Tables["campGroups"].Rows[i].ItemArray[1]);
                }
                conn.Close();
            }
        }
        private void ShowCampsInGroups(object sender, SelectionChangedEventArgs e)
        {
            if (CampGroupsListBox.SelectedIndex < 0 || CampGroupsListBox.SelectedItem.GetType().Equals(typeof(TextBox)))
            {
                //Кнопки на правой панели
                AddCampToGroupButton.IsEnabled = false;
                ChooseGroupDialogApplyButton.IsEnabled = false;
                CampsListBox.Items?.Clear();
                return;
            }
            AddCampToGroupButton.IsEnabled = true;
            ChooseGroupDialogApplyButton.IsEnabled = true;
            SelectedCampId = int.Parse(WorkDS.Tables["campGroups"].Rows[CampGroupsListBox.SelectedIndex].ItemArray[0].ToString());
            GetCampsInGroup();
            if(AddNewCampGroupButton.Content.ToString() == "Подтвердить")
            {
                FillCampGroupsListBox();
                AddNewCampGroupButton.Content = "Добавить";
                AddNewCampGroupButton.Click -= AddNewCampGroupApply;
                AddNewCampGroupButton.Click += AddNewCampGroup;
            }
            if(AddCampToGroupButton.Content.ToString() == "Подтвердить")
            {
                AddCampToGroupButton.Content = "Добавить";
                AddCampToGroupButton.Click -= AddCampToGroupApply;
                AddCampToGroupButton.Click += AddCampToGroup;
            }
        }
        private void GetCampsInGroup()
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"exec [dbo].[showCampsInGroup] {SelectedCampId}", conn);
                WorkDS.Tables["campsInGroup"]?.Clear();
                CampsListBox.Items?.Clear();
                sda.Fill(WorkDS, "campsInGroup");
                for (int i = 0; i < WorkDS.Tables["campsInGroup"].Rows.Count; i++)
                {
                    CampsListBox.Items.Add(WorkDS.Tables["campsInGroup"].Rows[i].ItemArray[0]);
                }
                if(CampsListBox.Items.Count == 0)
                {
                    CampsListBox.Items.Add("Лагеря отсутствуют");
                }
                conn.Close();
            }
        }
        private void AddNewCampGroup(object sender, RoutedEventArgs e)
        {
            campGroup = new() { Style = EventNameTB.Style, Padding = new(6), MaxLength = 20, Margin = new Thickness(0, 0, 0, 5), Width = 150 };
            CampGroupsListBox.Items.Add(campGroup);
            AddNewCampGroupButton.Content = "Подтвердить";
            AddNewCampGroupButton.Click -= AddNewCampGroup;
            AddNewCampGroupButton.Click += AddNewCampGroupApply;
        }
        private void AddNewCampGroupApply(object sender, RoutedEventArgs e)
        {
            if (campGroup.Text != "")
            {
                using(SqlConnection conn = new(Connection))
                {
                    conn.Open();
                    SqlCommand cmd = new($"insert into [dbo].[camp_groups] values(\'{campGroup.Text}\')", conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                AddNewCampGroupButton.Content = "Добавить";
                AddNewCampGroupButton.Click -= AddNewCampGroupApply;
                AddNewCampGroupButton.Click += AddNewCampGroup;
                FillCampGroupsListBox();
            }
        }

        private void AddCampToGroup(object sender, RoutedEventArgs e)
        {
            camps = new();
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new("select [id], [name] from [dbo].[camps] order by [name]", conn);
                WorkDS.Tables["camps"]?.Clear();
                sda.Fill(WorkDS, "camps");
                var table = WorkDS.Tables["camps"].Rows;
                for (int i = 0; i < WorkDS.Tables["camps"].Rows.Count; i++)
                {
                    camps.Items.Add($"{table[i].ItemArray[1]}");
                }
                camps.SelectedIndex = 0;
                conn.Close();
            }
            CampsListBox.Items.Add(camps);
            AddCampToGroupButton.Content = "Подтвердить";
            AddCampToGroupButton.Click -= AddCampToGroup;
            AddCampToGroupButton.Click += AddCampToGroupApply;
        }
        private void AddCampToGroupApply(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new(Connection)) 
            {
                conn.Open();
                SqlCommand cmd = new($"insert into [dbo].[groups_of_camps] values({SelectedCampId}, {WorkDS.Tables["camps"].Rows[camps.SelectedIndex].ItemArray[0]})", conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            AddCampToGroupButton.Content = "Добавить";
            AddCampToGroupButton.Click -= AddCampToGroupApply;
            AddCampToGroupButton.Click += AddCampToGroup;
            GetCampsInGroup();
        }
        private void ChooseGroup(object sender, RoutedEventArgs e)
        {
            CampGroupsDialog.IsOpen = false;
            ChooseGroupDialogApplyButton.IsEnabled = false;
            ChosenCampGroup.Text = $"Выбрано: {WorkDS.Tables["campGroups"].Rows[CampGroupsListBox.SelectedIndex].ItemArray[1]}";
        }
      

    }
}
