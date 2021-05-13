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
    /// Логика взаимодействия для CampsWindow.xaml
    /// </summary>
    public partial class CampsWindow : Window
    {
        private string Connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        private DataSet WorkDS = new DataSet(); 
        bool IsLogout = false;
        private int prevWindow;
        //Добавление лагеря
        
        public CampsWindow()
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillCampsListBox();
            FillSquadListBox();
        }
        

        ////////////////////////////////////////////////////////////////Лагерь/////////////////////////////////////////////////////////////////

        //Переменные
        private int SelectedDirector;
        private int SelectedManager;
        private string CampName;
        private string CampDescription;
        
        //Заполнение ListBox-а
        private void FillCampsListBox()
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter("select [name], [id] from [dbo].[camps] order by [name]", conn);
                WorkDS.Tables["camps"]?.Clear();
                CampsListBox.Items?.Clear();
                sda.Fill(WorkDS, "camps");
                CampsListBox.SelectionChanged += new SelectionChangedEventHandler(CampSelected);
                for (int i = 0; i < WorkDS.Tables["camps"].Rows.Count; i++)
                {
                    CampsListBox.Items.Add(WorkDS.Tables["camps"].Rows[i].ItemArray[0]);
                }
                conn.Close();
            }
        }

        private void OpenAddCampDialog(object sender, RoutedEventArgs e)
        {
            AddCampDialog.IsOpen = true;
            //Заполняем ComboBox-ы
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                //Директор
                SqlDataAdapter sda = new("exec [showWorkersWithChPos] 1", conn);
                WorkDS.Tables["directors"]?.Clear();
                DirectorComboBox.Items?.Clear();
                sda.Fill(WorkDS, "directors");
                var table = WorkDS.Tables["directors"].Rows;
                for (int i = 0; i < WorkDS.Tables["directors"].Rows.Count; i++)
                {
                    DirectorComboBox.Items.Add($"{table[i].ItemArray[1]} {table[i].ItemArray[2]} {table[i].ItemArray[3]}");
                }
                //Управляющий
                sda = new("exec [showWorkersWithChPos] 2", conn);
                WorkDS.Tables["managers"]?.Clear();
                ManagerComboBox.Items?.Clear();
                sda.Fill(WorkDS, "managers");
                table = WorkDS.Tables["managers"].Rows;
                for (int i = 0; i < WorkDS.Tables["managers"].Rows.Count; i++)
                {
                    ManagerComboBox.Items.Add($"{table[i].ItemArray[1]} {table[i].ItemArray[2]} {table[i].ItemArray[3]}");
                }
                conn.Close();
            }
        }
        private void CampSelected(object sender, SelectionChangedEventArgs e)
        {
            //Вывод информации
            //DockPanel - Info
            int campId = int.Parse(WorkDS.Tables["camps"].Rows[CampsListBox.SelectedIndex].ItemArray[1].ToString());
            using(SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"exec [dbo].[showCampInfo] {campId}", conn);
                WorkDS.Tables["campInfo"]?.Clear();
                Info.Children?.Clear();
                sda.Fill(WorkDS, "campInfo");
                var campInfo = WorkDS.Tables["campInfo"].Rows[0];
                TextBlock campName = new() { Text = $"Лагерь: {campInfo.ItemArray[1]}", Margin = new(7) };
                DockPanel.SetDock(campName, Dock.Top);
                Info.Children.Add(campName);
                TextBlock campDescription = new() { Text = $"Описание: {campInfo.ItemArray[2]}", Margin= new(7), TextWrapping = TextWrapping.Wrap };
                DockPanel.SetDock(campDescription, Dock.Top);
                Info.Children.Add(campDescription);
                TextBlock campDirector = new() { Text = $"Директор: {campInfo.ItemArray[3]} {campInfo.ItemArray[4]} {campInfo.ItemArray[5]}", Margin = new(7), 
                    TextWrapping =TextWrapping.Wrap };
                DockPanel.SetDock(campDirector, Dock.Top);
                Info.Children.Add(campDirector);
                TextBlock campManager = new()
                {
                    Text = $"Директор: {campInfo.ItemArray[6]} {campInfo.ItemArray[7]} {campInfo.ItemArray[8]}",
                    Margin = new(7),
                    TextWrapping = TextWrapping.Wrap
                };
                DockPanel.SetDock(campManager, Dock.Top);
                Info.Children.Add(campManager);
                conn.Close();
            }
        }
        private void AddCamp(object sender, RoutedEventArgs e)
        {
            SelectedDirector = int.Parse(WorkDS.Tables["directors"].Rows[DirectorComboBox.SelectedIndex].ItemArray[0].ToString());
            SelectedManager = int.Parse(WorkDS.Tables["managers"].Rows[ManagerComboBox.SelectedIndex].ItemArray[0].ToString());
            CampName = CampNameTB.Text;
            CampDescription = CampDescriptionTB.Text;
            //Вносим данные в бд
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlCommand cmd = new($"insert into [dbo].[camps] values(\'{CampName}\',\'{CampDescription}\',{SelectedDirector},{SelectedManager})",conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            FillCampsListBox();
            AddCampDialog.IsOpen = false;
        }
        /////////////////////////////////////////////////////////////////Отряд/////////////////////////////////////////////////////////////////

        private string SquadName;
        private int CampId;
        private int CounselorId;
        private int SeniorCounselorId;
        private int SeasonId;
        private int HouseId;
        private string Season;
        private string House;

        private void FillSquadListBox()
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter("select [name], [id] from [dbo].[squads] order by [name]", conn);
                WorkDS.Tables["squads"]?.Clear();
                SquadsListBox.Items?.Clear();
                sda.Fill(WorkDS, "squads");
                SquadsListBox.SelectionChanged += new SelectionChangedEventHandler(SquadSelected);
                for (int i = 0; i < WorkDS.Tables["squads"].Rows.Count; i++)
                {
                    SquadsListBox.Items.Add(WorkDS.Tables["squads"].Rows[i].ItemArray[0]);
                }
                conn.Close();
            }
        }
       
        private void OpenAddSquadDialog(object sender, RoutedEventArgs e)
        {
            AddSquadDialog.IsOpen = true;
            //Заполняем ComboBox-ы
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                //Лагерь
                    CampComboBox.Items?.Clear();
                for (int i = 0; i < WorkDS.Tables["camps"].Rows.Count; i++)
                {
                    CampComboBox.Items.Add(WorkDS.Tables["camps"].Rows[i].ItemArray[0]);
                }
                /*SqlDataAdapter sda = new SqlDataAdapter("select [name], [id] from [dbo].[camps] order by [name]", conn);
                sda.Fill(WorkDS, "campsComboBox");
                for (int i = 0; i < WorkDS.Tables["campsComboBox"].Rows.Count; i++)
                {
                    CampComboBox.Items.Add(WorkDS.Tables["campsComboBox"].Rows[i].ItemArray[0]);
                }*/
                //Вожатый
                SqlDataAdapter sda = new("exec [showWorkersWithChPos] 3", conn);
                WorkDS.Tables["counselor"]?.Clear();
                CounselorComboBox.Items?.Clear();
                sda.Fill(WorkDS, "counselor");
                var table = WorkDS.Tables["counselor"].Rows;
                for (int i = 0; i < WorkDS.Tables["counselor"].Rows.Count; i++)
                {
                    CounselorComboBox.Items.Add($"{table[i].ItemArray[1]} {table[i].ItemArray[2]} {table[i].ItemArray[3]}");
                }
                //Старший вожатый
                sda = new("exec [showWorkersWithChPos] 4", conn);
                WorkDS.Tables["seniorCounselor"]?.Clear();
                SeniorCounselorComboBox.Items?.Clear();
                sda.Fill(WorkDS, "seniorCounselor");
                table = WorkDS.Tables["seniorCounselor"].Rows;
                for (int i = 0; i < WorkDS.Tables["seniorCounselor"].Rows.Count; i++)
                {
                    SeniorCounselorComboBox.Items.Add($"{table[i].ItemArray[1]} {table[i].ItemArray[2]} {table[i].ItemArray[3]}");
                }
                conn.Close();
            }
        }
        private void AddSquad(object sender, RoutedEventArgs e)
        {
            SquadName = SquadNameTB.Text;
            CampId = int.Parse(WorkDS.Tables["camps"].Rows[CampComboBox.SelectedIndex].ItemArray[1].ToString());
            CounselorId = int.Parse(WorkDS.Tables["counselor"].Rows[CounselorComboBox.SelectedIndex].ItemArray[0].ToString());
            SeniorCounselorId = int.Parse(WorkDS.Tables["seniorCounselor"].Rows[SeniorCounselorComboBox.SelectedIndex].ItemArray[0].ToString());
            Season = SeasonTextBox.Text;
            SeasonId = CheckSeason(Season);
            House = HouseTextBox.Text; 
            HouseId = CheckHouse(House);
            //Проверка сезона
            if(SeasonId == -1)
            {
                prevWindow = 1;
                OpenAddSeasonDialog();
            }
            //Проверка корпуса
            else if(HouseId == -1)
            {
                prevWindow = 1;
                OpenAddHouseDialog();
            }
            else 
            {
                //Заполнение
                InsertSquad();
            }
        }
        private void InsertSquad()
        {
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlCommand cmd = new($"insert into [dbo].[squads] values(\'{SquadName}\',{CampId},{CounselorId}, {SeniorCounselorId}, {SeasonId}, {HouseId})", conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            FillSquadListBox();
        }
        private void SquadSelected(object sender, SelectionChangedEventArgs e)
        {
            //Вывод информации
            int squadId = int.Parse(WorkDS.Tables["squads"].Rows[SquadsListBox.SelectedIndex].ItemArray[1].ToString());
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"exec [dbo].[showSquadInfo] {squadId}", conn);
                WorkDS.Tables["squadInfo"]?.Clear();
                Info.Children?.Clear();
                sda.Fill(WorkDS, "squadInfo");
                var squadInfo = WorkDS.Tables["squadInfo"].Rows[0];
                TextBlock squadName = new() { Text = $"Лагерь: {squadInfo.ItemArray[1]}", Margin = new(7) };
                DockPanel.SetDock(squadName, Dock.Top);
                Info.Children.Add(squadName);
                TextBlock squadCounselor = new()
                {
                    Text = $"Вожатый: {squadInfo.ItemArray[2]} {squadInfo.ItemArray[3]} {squadInfo.ItemArray[4]}",
                    Margin = new(7),
                    TextWrapping = TextWrapping.Wrap
                };
                DockPanel.SetDock(squadCounselor, Dock.Top);
                Info.Children.Add(squadCounselor);
                TextBlock squadSeniorCounselor = new()
                {
                    Text = $"Старший вожатый: {squadInfo.ItemArray[5]} {squadInfo.ItemArray[6]} {squadInfo.ItemArray[7]}",
                    Margin = new(7),
                    TextWrapping = TextWrapping.Wrap
                };
                DockPanel.SetDock(squadSeniorCounselor, Dock.Top);
                Info.Children.Add(squadSeniorCounselor);
                TextBlock squadSeason = new()
                {
                    Text = $"Сезон: {squadInfo.ItemArray[8]}",
                    Margin = new(7),
                    TextWrapping = TextWrapping.Wrap
                };
                DockPanel.SetDock(squadSeason, Dock.Top);
                Info.Children.Add(squadSeason);
                TextBlock squadSeasonBeginning = new()
                {
                    Text = $"Начало: {squadInfo.ItemArray[9]}",
                    Margin = new(14, 7, 7, 7),
                    TextWrapping = TextWrapping.Wrap
                };
                DockPanel.SetDock(squadSeasonBeginning, Dock.Top);
                Info.Children.Add(squadSeasonBeginning);
                TextBlock squadSeasonEnding = new()
                {
                    Text = $"Конец: {squadInfo.ItemArray[10]}",
                    Margin = new(14, 7, 7, 7),
                    TextWrapping = TextWrapping.Wrap
                };
                DockPanel.SetDock(squadSeasonEnding, Dock.Top);
                Info.Children.Add(squadSeasonEnding);
                TextBlock squadHouse = new()
                {
                    Text = $"Корпус: {squadInfo.ItemArray[11]}",
                    Margin = new(7),
                    TextWrapping = TextWrapping.Wrap
                };
                DockPanel.SetDock(squadHouse, Dock.Top);
                Info.Children.Add(squadHouse);
                TextBlock squadHouseRooms = new()
                {
                    Text = $"Количество комнат: {squadInfo.ItemArray[12]}",
                    Margin = new(14, 7, 7, 7),
                    TextWrapping = TextWrapping.Wrap
                };
                DockPanel.SetDock(squadHouseRooms, Dock.Top);
                Info.Children.Add(squadHouseRooms);
                conn.Close();
            }
        }

        /////////////////////////////////////////////////////////////////Сезон//////////////////////////////////////////////////////////////////

        private int CheckSeason(string season)
        {
            int result;
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"exec [dbo].[checkSeason] \'{season}\'", conn);
                WorkDS.Tables["checkSeason"]?.Clear();
                sda.Fill(WorkDS, "checkSeason");
                result = int.Parse(WorkDS.Tables["checkSeason"].Rows[0].ItemArray[0].ToString());
                conn.Close();
            }
            return result;
        }
        private void OpenAddSeasonDialog()
        {
            AddSquadDialog.IsOpen = false;
            AddSeasonDialog.IsOpen = true;
            SeasonName.Text = Season;
        }

        //Вызов через кнопку
        private void OpenAddSeasonDialog(object sender, RoutedEventArgs e)
        {
            AddSquadDialog.IsOpen = false;
            AddSeasonDialog.IsOpen = true;
        }
        private void AddSeason(object sender, RoutedEventArgs e)
        {
            Season = SeasonName.Text;
            string beginDate = SeasonBegin.Text;
            string endDate = SeasonEnd.Text;
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"exec [dbo].[addSeason] \'{Season}\', \'{beginDate}\', \'{endDate}\'", conn);
                WorkDS.Tables["addSeason"]?.Clear();
                sda.Fill(WorkDS, "addSeason");
                SeasonId = int.Parse(WorkDS.Tables["addSeason"].Rows[0].ItemArray[0].ToString());
                conn.Close();
            }
            if(HouseId == -1)
            {
                AddSeasonDialog.IsOpen = false;
                SeasonBegin.Text = "";
                SeasonEnd.Text = "";
                OpenAddHouseDialog();
            }
            else
            {
                InsertSquad();
            }
        }

        /////////////////////////////////////////////////////////////////Корпус/////////////////////////////////////////////////////////////////

        private int CheckHouse(string house)
        {
            int result;
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"exec [dbo].[checkHouse] \'{house}\'", conn);
                WorkDS.Tables["checkHouse"]?.Clear();
                sda.Fill(WorkDS, "checkHouse");
                result = int.Parse(WorkDS.Tables["checkHouse"].Rows[0].ItemArray[0].ToString());
                conn.Close();
            }
            return result;
        }
        private void OpenAddHouseDialog()
        {
            AddHouseDialog.IsOpen = true;
            HouseName.Text = House;
        }

        //Вызов через кнопку
        private void OpenAddHouseDialog(object sender, RoutedEventArgs e)
        {
            AddHouseDialog.IsOpen = true;
        }
        private void AddNewHouse(object sender, RoutedEventArgs e)
        {
            House = HouseName.Text;
            int rooms = int.Parse(NumRooms.Text.ToString());//For exeption
            
            using (SqlConnection conn = new(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new($"exec [dbo].[addHouse] \'{House}\', {rooms}", conn);
                WorkDS.Tables["addHouse"]?.Clear();
                sda.Fill(WorkDS, "addHouse");
                HouseId = int.Parse(WorkDS.Tables["addHouse"].Rows[0].ItemArray[0].ToString());
                conn.Close();
            }
            InsertSquad();
            AddHouseDialog.IsOpen = false;
        }


    }
}
