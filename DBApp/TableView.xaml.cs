﻿using System;
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
        public int choose;
        public string account { get; set; } = "";
        private string connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        private LoginPage login = new LoginPage();
        private Insert insert = new();
        //private int visiblePanels = 0;
        private DataGrid mainTable = new DataGrid() { IsReadOnly = true};
        public TableView()
        {
            InitializeComponent();
            
            login.Show();
        }
        //Мои методы
        
        private void TableOutput(string sql)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
                conn.Close();
            }
        }
        private void TableOutput(string sql, DataGrid dataGrid)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                dataGrid.ItemsSource = ds.Tables[0].DefaultView;
                conn.Close();
            }
        }
        private void Refresh()
        {
            //Вкладка "Фильтр"
            filterTitle.Visibility = Visibility.Hidden;
            chooseGroup.Visibility = Visibility.Hidden;
            chouseGroupLabel.Visibility = Visibility.Hidden;
            filterButton.Visibility = Visibility.Hidden;

            //Вкладка "Добавление"
            addTitle.Visibility = Visibility.Hidden;

            comboHeader1.Visibility = Visibility.Hidden;
            comboHeader1.Height = 0; // Базовый = 25.96
            header1.Visibility = Visibility.Hidden;
            header2.Visibility = Visibility.Hidden;
            header3.Visibility = Visibility.Hidden;
            header4.Visibility = Visibility.Hidden;
            header5.Visibility = Visibility.Hidden;
            header6.Visibility = Visibility.Hidden;
            header7.Visibility = Visibility.Hidden;

            combo1.Visibility = Visibility.Hidden;
            combo1.Height = 0; //Базовый = 21.96

            add1.Visibility = Visibility.Hidden;
            add2.Visibility = Visibility.Hidden;
            add3.Visibility = Visibility.Hidden;
            add4.Visibility = Visibility.Hidden;
            add5.Visibility = Visibility.Hidden;
            add6.Visibility = Visibility.Hidden;
            add7.Visibility = Visibility.Hidden;

            addButton.Visibility = Visibility.Hidden;

            //
            tableTabs.Items.Clear();
            //tableTabs.Items;
        }

        //Методы, связанные с .xaml
        public void TableViewLoaded(object sender, RoutedEventArgs e)
        {
            login.Owner = this;
            this.Hide();
        }
        private void ChangeAccount(object sender, RoutedEventArgs e)
        {
            this.Hide();

        }
       
        private void ScheduleTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            /*tabPanel.SelectedIndex = 0;

            filterTitle.Visibility = Visibility.Visible;
            filterHeader1.Visibility = Visibility.Visible;
            filterHeader1.Content = "ID Группы:";
            filter1.Visibility = Visibility.Visible;
            filterButton.Visibility = Visibility.Visible;
            */
            ChooseSquad chooseSquad = new();
            chooseSquad.command = "select * from [camp_groups]";
            chooseSquad.Owner = this;
            chooseSquad.output = showSchedule;
            chooseSquad.Show();
            /*
            tableTabs.Items.Add(new TabItem
            {
                Header = "Список групп",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput("select * from [camp_groups]", mainTable);*/

            /*tabPanel.SelectedIndex = 0;
            List<string> groups = new List<string>();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter("select [name] from [camp_groups]", conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                for(int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    groups.Add(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                }
            }
            filterTitle.Visibility = Visibility.Visible;
            chooseGroup.Visibility = Visibility.Visible;
            chouseGroupLabel.Visibility = Visibility.Visible;
            chooseGroup.ItemsSource = groups;*/
        }
        private void showSchedule(string id)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Расписание для группы",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showCampSchedule] {id}", mainTable);
        }
        private void ApplyFilter(object sender, RoutedEventArgs e)
        {
            tableTabs.Items.Add(new TabItem
            {
                Header = "Льготы детей",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput("select * from [camp_groups]", mainTable);
        }
        private void groupSelect(object sender, RoutedEventArgs e)
        {
            tabPanel.SelectedIndex = 0;
            int campId = chooseGroup.SelectedIndex + 1;
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter($"exec [dbo].[showCampSchedule] {campId}", conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                table.ItemsSource = ds.Tables[0].DefaultView;
            }
        }
        private void ChildrenBenefitsTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Льготы детей",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput("exec [dbo].[showChildrenBenefitsTable]", mainTable);
        }
        private void WorkersTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Сотрудники",
                Content = mainTable
            });
            tabPanel.SelectedIndex = 0;
            tableTabs.SelectedIndex = 0;
            TableOutput("exec [dbo].[showWorkersTable]", mainTable);

        }
        private void SquadsTable(object sender, RoutedEventArgs e)
        {
            Refresh(); 
            tableTabs.Items.Add(new TabItem
            {
                Header = "Отряды",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput("exec [dbo].[showSquadsTable]", mainTable);
        }
        private void ChildrenTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Дети",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0; 
            tabPanel.SelectedIndex = 0;
            TableOutput("exec [dbo].[showChildrenTable]", mainTable);
        }
        private void CampsTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Лагеря",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput("exec [dbo].[showCampsTable]", mainTable);
        }
        private void ChildrenInSquadTable(object sender, RoutedEventArgs e)
        {

            ChooseSquad chooseSquad = new ChooseSquad();
            chooseSquad.command = "select * from [dbo].[squads]";
            chooseSquad.output = ShowChildrenInSquadTable;
            chooseSquad.Show();
            chooseSquad.Owner = this;

           
        }
        public void ShowChildrenInSquadTable(string squad)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Дети в отряде",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showChildrenInSquad] {squad}", mainTable);
        }
        private void WorkersWithPositionsTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Сотрудники",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput("exec [dbo].[showWorkersWithPosition]", mainTable);
        }
        private void ShiftsTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Смены сотрудников",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showShifts]", mainTable);
        }
        private void HousesTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Корпуса",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showHouses]", mainTable);
        }
        private void SeasonsTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Корпуса",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showSeasons]", mainTable);
        }
        private void ClubsTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Список кружков",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showClubs]", mainTable);
        }
        private void ClubsOrganizersTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Организаторы кружков",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showClubsOrganizers]", mainTable);
        }
        private void ChildrenInClubsTable(object sener,RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Список детей в кружках",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showChildrenInClubs]", mainTable);
        }
        private void BenefitsTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Список льгот",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showBenefits]", mainTable);
        }
        private void CampGroupsTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Список лагерей в группах",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showCampGroups]", mainTable);
        }
        private void GroupsTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Список групп лагерей",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showGroups]", mainTable);
        }
        private void CampEventsTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Расписание",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showCampEvents]", mainTable);
        }
        private void EventsTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "События",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showEvents]", mainTable);
        }
        private void EventOrganizersTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Список групп организаторов",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showEventOrganizers]", mainTable);
        }
        private void GroupsOfOrganizersTable(object sender, RoutedEventArgs e)
        {
            Refresh();
            tableTabs.Items.Add(new TabItem
            {
                Header = "Список организаторов в группах",
                Content = mainTable
            });
            tableTabs.SelectedIndex = 0;
            tabPanel.SelectedIndex = 0;
            TableOutput($"exec [dbo].[showGroupsOfOrganizers]", mainTable);
        }


        //Добавление
        delegate void Select(object sender, RoutedEventArgs e);
        Select selectVoid;

        public void AddWorker(object sender, RoutedEventArgs e)
        {
            Refresh();

            addTitle.Visibility = Visibility.Visible;

            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;
            header3.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;
            add3.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "Фамилия сотрудника *";
            header2.Content = "Имя сотрудника *";
            header3.Content = "Отчество сотрудника *";

            tabPanel.SelectedIndex = 1;
            insert.insertCommand = @"insert into [dbo].[workers] values('?','?','?')";
            tableTabs.Items.Add(new TabItem
            {
                Header = "Сотрудники",
                Content = mainTable
            });
            TableOutput("exec [dbo].[showWorkers]", mainTable);

            selectVoid = AddWorker;

        }
        public void AddWorkerPosition(object sender, RoutedEventArgs e)
        {
            Refresh();

            addTitle.Visibility = Visibility.Visible;

            header1.Content = "ID Сотрудника";
            header1.Visibility = Visibility.Visible;
            add1.Visibility = Visibility.Visible;
            
            header2.Content = "ID Должности";
            header2.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;
            
            addButton.Visibility = Visibility.Visible;
            
            tabPanel.SelectedIndex = 1;
           
            insert.insertCommand = @"insert into [dbo].[workers_position] values(?,?)";
            
            tableTabs.Items.Add(new TabItem
            {
                Header = "Должности сотрудников",
                Content = mainTable
            });
            TableOutput("exec [dbo].[showWorkersTable]", mainTable);
            
            DataGrid workersTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem
            {
                Header = "Сотрудники",
                Content = workersTable
            });
            TableOutput("exec [dbo].[showWorkers]", workersTable);

            DataGrid positionsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem
            {
                Header = "Сотрудники",
                Content = positionsTable
            });
            TableOutput("exec [dbo].[showPositions]", positionsTable);

            selectVoid = AddWorkerPosition;
        }
        private void AddCamp(object sender, RoutedEventArgs e)
        {
            Refresh();
            addTitle.Visibility = Visibility.Visible;

            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;
            header3.Visibility = Visibility.Visible;
            header4.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;
            add3.Visibility = Visibility.Visible;
            add4.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "Название лагеря";
            header2.Content = "Описание";
            header3.Content = "ID Директора";
            header4.Content = "ID Управляющего";

            tabPanel.SelectedIndex = 1;
            insert.insertCommand = @"insert into [dbo].[camps] values('?','?',?,?)";
            tableTabs.Items.Add(new TabItem
            {
                Header = "Лагеря",
                Content = mainTable
            });
            TableOutput("exec [dbo].[showCampsTable]", mainTable);
            DataGrid directorsTable = new() { IsReadOnly = true};
            tableTabs.Items.Add(new TabItem
            {
                Header = "Директора",
                Content = directorsTable
            });
            TableOutput("exec [dbo].[showWorkersWithChPos] 1", directorsTable);
            DataGrid managersTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem
            {
                Header = "Управляющие",
                Content = managersTable
            });
            TableOutput("exec [dbo].[showWorkersWithChPos] 2", managersTable);

            selectVoid = AddCamp;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (add1.Text != "" 
                && (add2.Text != "" || add2.Visibility == Visibility.Hidden) 
                && (add3.Text != "" || add3.Visibility == Visibility.Hidden) 
                && (add4.Text != "" || add4.Visibility == Visibility.Hidden)
            )
            {
                List<string> vars = new();

                vars.Add(add1.Text);
                vars.Add(add2.Text);
                vars.Add(add3.Text);
                vars.Add(add4.Text);
                vars.Add(add5.Text);
                vars.Add(add6.Text);
                vars.Add(add7.Text);
                

                insert.SetVars(vars);

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string command = insert.GetTableCommand();
                    SqlCommand com = new SqlCommand(command, conn);
                    com.ExecuteNonQuery();
                }

                add1.Clear();
                add2.Clear();
                add3.Clear(); 
                add4.Clear();
                add5.Clear();
                add6.Clear();
                add7.Clear();

                TableOutput("exec [dbo].[showWorkers]");
            }
            else
            {
                MessageBox.Show("Необходимо заполнить все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            selectVoid(sender, e);
        }
        
        
        
        private void OnClose(object sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
