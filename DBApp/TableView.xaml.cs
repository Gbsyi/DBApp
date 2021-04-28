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
       
        //Пункт меню "Просмотр"
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


        //Пункт меню "Добавление"
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
        private void AddSquad(object sender, RoutedEventArgs e)
        {
            Refresh();

            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;
            header3.Visibility = Visibility.Visible;
            header4.Visibility = Visibility.Visible;
            header5.Visibility = Visibility.Visible;
            header6.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;
            add3.Visibility = Visibility.Visible;
            add4.Visibility = Visibility.Visible;
            add5.Visibility = Visibility.Visible;
            add6.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "Название";
            header2.Content = "ID Лагеря";
            header3.Content = "ID Вожатого";
            header4.Content = "ID Старшего вожатого";
            header5.Content = "ID Сезона";
            header6.Content = "ID Корпуса";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 1;
            insert.insertCommand = @"insert into [dbo].[squads] values('?',?,?,?,?,?)";

            tableTabs.Items.Add(new TabItem
            {
                Header = "Отряды",
                Content = mainTable
            });
            TableOutput("exec [dbo].[showSquadsTable]", mainTable);

            DataGrid campsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem
            {
                Header = "Лагеря",
                Content = campsTable
            });
            TableOutput("exec [dbo].[showCampsTable]", campsTable);

            DataGrid workersTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem
            {
                Header = "Сотрудники",
                Content = workersTable
            });
            TableOutput("exec [dbo].[showWorkersTable]", workersTable);

            DataGrid seasonsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem
            {
                Header = "Сезоны",
                Content = seasonsTable
            });
            TableOutput("exec [dbo].[showSeasons]", seasonsTable);

            DataGrid housesTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem
            {
                Header = "Корпуса",
                Content = housesTable
            });
            TableOutput("exec [dbo].[showHouses]", housesTable);

            selectVoid = AddSquad;
        }
        private void AddHouse(object sender, RoutedEventArgs e)
        {
            Refresh();

            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;
            
            header1.Content = "Название";
            header2.Content = "Количество комнат";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[houses] values('?',?)";

            tableTabs.Items.Add(new TabItem {Header = "Корпуса", Content = mainTable});
            TableOutput("exec [dbo].[showHouses]", mainTable);

            selectVoid = AddHouse;
        }
        private void AddSeason(object sender, RoutedEventArgs e)
        {
            Refresh();
            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;
            header3.Visibility = Visibility.Visible;


            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;
            add3.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "Название";
            header2.Content = "Дата начала (ГГГГ.ММ.ДД)";
            header3.Content = "Дата окончания (ГГГГ.ММ.ДД)";
 
            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[seasons] values('?','?','?')";

            tableTabs.Items.Add(new TabItem { Header = "Сезоны", Content = mainTable });
            TableOutput("exec [dbo].[showSeasons]", mainTable);

            selectVoid = AddSeason;
        }
        private void AddPosition(object sender, RoutedEventArgs e)
        {
            Refresh();

            header1.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "Название должности";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[positions] values('?')";

            tableTabs.Items.Add(new TabItem { Header = "Должности", Content = mainTable });
            TableOutput("exec [dbo].[showPositions]", mainTable);

            selectVoid = AddPosition;
        }
        private void AddShift(object sender, RoutedEventArgs e)
        {
            Refresh();
            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;
            header3.Visibility = Visibility.Visible;


            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;
            add3.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "ID Сотрудника *";
            header2.Content = "Дата начала (ГГГГ.ММ.ДД ЧЧ:ММ) *";
            header3.Content = "Дата окончания (ГГГГ.ММ.ДД ЧЧ:ММ) *";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[shifts] values(?,'?','?')";

            tableTabs.Items.Add(new TabItem { Header = "Смены", Content = mainTable });
            TableOutput("exec [dbo].[showShifts]", mainTable);

            DataGrid workersTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Сотрудники", Content = workersTable });
            TableOutput("exec [dbo].[showWorkersTable]", workersTable);

            selectVoid = AddShift;

        }
        private void AddClub(object sender, RoutedEventArgs e)
        {
            Refresh();
            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "Название кружка *";
            header2.Content = "Краткое описание *";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[clubs] values('?','?')";

            tableTabs.Items.Add(new TabItem { Header = "Кружки", Content = mainTable });
            TableOutput("exec [dbo].[showClubs]", mainTable);

            selectVoid = AddClub;
        }
        private void AddClubOrganizer(object sender, RoutedEventArgs e)
        {
            Refresh();
            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "ID Кружка *";
            header2.Content = "ID Сотрудника *";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[clubs_organizers] values(?,?)";

            tableTabs.Items.Add(new TabItem { Header = "Организаторы кружков", Content = mainTable });
            TableOutput("exec [dbo].[showClubsOrganizers]", mainTable);

            DataGrid workersTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Сотрудники", Content = workersTable });
            TableOutput("exec [dbo].[showWorkersTable]", workersTable);

            DataGrid clubsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Кружки", Content = clubsTable });
            TableOutput("exec [dbo].[showClubs]", clubsTable);

            selectVoid = AddClubOrganizer;
        }
        private void AddChildren(object sender, RoutedEventArgs e)
        {
            Refresh();
            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;
            header3.Visibility = Visibility.Visible;
            header4.Visibility = Visibility.Visible;
            header5.Visibility = Visibility.Visible;
            header6.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;
            add3.Visibility = Visibility.Visible;
            add4.Visibility = Visibility.Visible;
            add5.Visibility = Visibility.Visible;
            add6.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "Фамилия ребёнка *";
            header2.Content = "Имя ребёнка *";
            header3.Content = "Отчество ребёнка *";
            header4.Content = "Номер телефона *";
            header5.Content = "ID Отряда *";
            header6.Content = "Номер документа *";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[children] values('?','?','?','?',?,'?')";

            tableTabs.Items.Add(new TabItem { Header = "Дети", Content = mainTable });
            TableOutput("exec [dbo].[showChildrenTable]", mainTable);

            DataGrid squadsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Отряды", Content = squadsTable });
            TableOutput("exec [dbo].[showSquadsTable]", squadsTable);

            selectVoid = AddChildren;
        }
        private void AddChildrenToClub(object sender, RoutedEventArgs e)
        {
            Refresh();
            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "ID Ребёнка *";
            header2.Content = "ID Кружка *";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[children_in_clubs] values(?,?)";

            tableTabs.Items.Add(new TabItem { Header = "Дети в кружках", Content = mainTable });
            TableOutput("exec [dbo].[showChildrenInClubs]", mainTable);

            DataGrid clubsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Кружки", Content = clubsTable });
            TableOutput("exec [dbo].[showClubs]", clubsTable);

            DataGrid childrenTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Дети", Content = childrenTable });
            TableOutput("exec [dbo].[showChildrenTable]", childrenTable);

            selectVoid = AddChildrenToClub;
        }
        private void AddBenefitsToChildren(object sender, RoutedEventArgs e)
        {
            Refresh();
            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "ID Ребёнка *";
            header2.Content = "ID Льготы *";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[children_benefits] values(?,?)";

            tableTabs.Items.Add(new TabItem { Header = "Льготы детей", Content = mainTable });
            TableOutput("exec [dbo].[showChildrenBenefitsTable]", mainTable);

            DataGrid benefitsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Льготы", Content = benefitsTable });
            TableOutput("exec [dbo].[showBenefits]", benefitsTable);

            DataGrid childrenTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Дети", Content = childrenTable });
            TableOutput("exec [dbo].[showChildrenTable]", childrenTable);

            selectVoid = AddBenefitsToChildren;
        }
        private void AddBenefit(object sender, RoutedEventArgs e)
        {
            Refresh();

            header1.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "Название льготы";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[benefits] values('?')";

            tableTabs.Items.Add(new TabItem { Header = "Льготы", Content = mainTable });
            TableOutput("exec [dbo].[showBenefits]", mainTable);

            selectVoid = AddBenefit;
        }
        private void AddEvent(object sender, RoutedEventArgs e)
        {
            Refresh();

            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "Название события";
            header2.Content = "Краткое описание";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[events] values('?','?')";

            tableTabs.Items.Add(new TabItem { Header = "События", Content = mainTable });
            TableOutput("exec [dbo].[showEvents]", mainTable);

            selectVoid = AddEvent;
        }
        private void AddCampEvent(object sender, RoutedEventArgs e)
        {
            Refresh();

            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;
            header3.Visibility = Visibility.Visible;
            header4.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;
            add3.Visibility = Visibility.Visible;
            add4.Visibility = Visibility.Visible;

            header1.Content = "ID Группы лагерей *";
            header2.Content = "ID События *";
            header3.Content = "ID Группы организаторов *";
            header4.Content = "Время (ГГГГ.ММ.ДД ЧЧ:ММ) *";

            addButton.Visibility = Visibility.Visible;

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[camp_events] values(?,?,?,'?')";

            tableTabs.Items.Add(new TabItem { Header = "События", Content = mainTable });
            TableOutput("exec [dbo].[showCampEvents]", mainTable);

            DataGrid campGroupsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Группы лагерей", Content = campGroupsTable});
            TableOutput("exec [dbo].[showCampGroups]", campGroupsTable);

            DataGrid eventsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "События", Content = eventsTable });
            TableOutput("exec [dbo].[showEvents]", eventsTable);

            DataGrid organizersTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Организаторы", Content = organizersTable });
            TableOutput("exec [dbo].[showEventOrganizers]", organizersTable);

            selectVoid = AddCampEvent;
        }
        private void AddCampGroup(object sender, RoutedEventArgs e)
        {
            Refresh();

            header1.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "Название группы";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[camp_groups] values('?')";

            tableTabs.Items.Add(new TabItem { Header = "Группы", Content = mainTable });
            TableOutput("exec [dbo].[showGroups]", mainTable);


            selectVoid = AddCampGroup;
        }

        private void AddGroupOfCamps(object sender, RoutedEventArgs e)
        {
            Refresh();

            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;

            header1.Content = "ID Группы";
            header2.Content = "ID Лагеря";

            addButton.Visibility = Visibility.Visible;

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[groups_of_camps] values(?,?)";

            tableTabs.Items.Add(new TabItem { Header = "Группы лагерей", Content = mainTable});
            TableOutput("exec [dbo].[showCampGroups]", mainTable);

            DataGrid campGroupsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Группы", Content = campGroupsTable });
            TableOutput("exec [dbo].[showGroups]", campGroupsTable);

            DataGrid campsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Лагеря", Content = campsTable });
            TableOutput("exec [dbo].[showCampsTable]", campsTable);

            selectVoid = AddGroupOfCamps;
        }
        private void AddOrganizersGroup(object sender, RoutedEventArgs e)
        {
            Refresh();

            header1.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;

            addButton.Visibility = Visibility.Visible;

            header1.Content = "Название группы";

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[event_organizers] values('?')";

            tableTabs.Items.Add(new TabItem { Header = "Группы", Content = mainTable });
            TableOutput("exec [dbo].[showEventOrganizers]", mainTable);


            selectVoid = AddOrganizersGroup;
        }
        private void AddGroupOfOrganizers(object sender, RoutedEventArgs e)
        {
            Refresh();

            header1.Visibility = Visibility.Visible;
            header2.Visibility = Visibility.Visible;

            add1.Visibility = Visibility.Visible;
            add2.Visibility = Visibility.Visible;

            header1.Content = "ID Группы";
            header2.Content = "ID Сотрудника";

            addButton.Visibility = Visibility.Visible;

            tabPanel.SelectedIndex = 1;
            tableTabs.SelectedIndex = 0;
            insert.insertCommand = @"insert into [dbo].[groups_of_organizers] values(?,?)";

            tableTabs.Items.Add(new TabItem { Header = "Группы лагерей", Content = mainTable });
            TableOutput("exec [dbo].[showGroupsOfOrganizers]", mainTable);

            DataGrid groupsTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem { Header = "Группы", Content = groupsTable });
            TableOutput("exec [dbo].[showEventOrganizers]", groupsTable);

            DataGrid workersTable = new() { IsReadOnly = true };
            tableTabs.Items.Add(new TabItem {Header = "Сотрудники", Content = workersTable});
            TableOutput("exec [dbo].[showWorkers]", workersTable);

            selectVoid = AddGroupOfOrganizers;
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (add1.Text != "" 
            && (add2.Text != "" || add2.Visibility == Visibility.Hidden) 
            && (add3.Text != "" || add3.Visibility == Visibility.Hidden) 
            && (add4.Text != "" || add4.Visibility == Visibility.Hidden)
            && (add5.Text != "" || add5.Visibility == Visibility.Hidden)
            && (add6.Text != "" || add6.Visibility == Visibility.Hidden)
            && (add7.Text != "" || add7.Visibility == Visibility.Hidden))
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
