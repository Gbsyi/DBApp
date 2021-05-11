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
    /// Логика взаимодействия для CampsWindow.xaml
    /// </summary>
    public partial class CampsWindow : Window
    {
        private bool IsLogout = false;
        private string Connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        private DataSet WorkDS = new();
        private ListBox LeftListBox = new();    
        private int CampID;
        private bool IsNewWorker = false;
        public CampsWindow()
        {
            InitializeComponent();
        }
        private void ChangeUser(object sender, RoutedEventArgs e)
        {
            WorkDS.Clear();
            IsLogout = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowCamps(sender,e);
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

        private void ShowCamps(object sender, RoutedEventArgs e)
        {
            LeftPanel.Children?.Clear();
            using(SqlConnection conn = new(Connection))
            {
                conn.Open();
                //Вывод лагерей
                string command = $"select [name],[id] from [dbo].[camps]";
                SqlDataAdapter sda = new(command,conn);
                WorkDS.Tables["camps"]?.Clear();
                sda.Fill(WorkDS, "camps");
                //Ввод в лист
                Label CampsLabel = new Label { Content = "Лагеря", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 5, 0, 0) };
                LeftPanel.Children.Add(CampsLabel);
                ListBox CampsListBox = new() { Margin = new Thickness(10, 0, 0, 0) };
                DockPanel.SetDock(CampsLabel, Dock.Top);
                DockPanel.SetDock(CampsListBox, Dock.Top);

                CampsListBox.SelectionChanged += new SelectionChangedEventHandler(CampSelected);
                for(int i = 0; i < WorkDS.Tables["camps"].Rows.Count; i++)
                {
                    CampsListBox.Items.Add(WorkDS.Tables["camps"].Rows[i].ItemArray[0]);
                }
                LeftPanel.Children.Add(CampsListBox);
                CampID = int.Parse(WorkDS.Tables["camps"].Rows[0].ItemArray[1].ToString());
                if (IsNewWorker)
                {
                    CampsListBox.SelectedIndex = WorkDS.Tables["camps"].Rows.Count - 1;
                }
                else
                {
                    CampsListBox.SelectedIndex = 0;
                }
                conn.Close();
            }
        }
        private void CampSelected(object sender, RoutedEventArgs e)
        {
            RightPanel.Children?.Clear();
            ListBox CampListBox = (ListBox)sender;
            CampID = int.Parse(WorkDS.Tables["camps"].Rows[CampListBox.SelectedIndex].ItemArray[1].ToString());

            /*using(SqlConnection conn = new(Connection))
            {
                conn.Open();
                //Вывод информации о лагере
                string command = $"exec [dbo].[showCampsTable]";
                SqlDataAdapter sda = new(command, conn);
                WorkDS.Tables["campsInfo"]?.Clear();
                sda.Fill(WorkDS, "campsInfo");
                //Вывод таблицы
                Label CampsTableLabel = new Label { Content = "Информация о лагере", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 5, 0, 0) };
                RightPanel.Children.Add(CampsTableLabel);
                DataGrid CampsTable = new() { Margin = new Thickness(10, 0, 0, 0)};
                DockPanel.SetDock(CampsTableLabel, Dock.Top);
                DockPanel.SetDock(CampsTable, Dock.Top);
                CampsTable.ItemsSource = WorkDS.Tables["campsInfo"].DefaultView;
                RightPanel.Children.Add(CampsTable);
                conn.Close();
            }*/
            Label CampsTableLabel = new Label { Content = "Отряды", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 5, 0, 0) };
            RightPanel.Children.Add(CampsTableLabel);
            ListBox SquadsListBox = new() { Margin = new Thickness(10, 0, 0, 0) };
            DockPanel.SetDock(CampsTableLabel, Dock.Top);
            DockPanel.SetDock(SquadsListBox, Dock.Top);
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter($"select [name], [id] from [dbo].[squads] where [squads].[camp_id] = {CampID}", conn);
                WorkDS.Tables["squads"]?.Clear();
                sda.Fill(WorkDS, "squads");
                SquadsListBox.SelectionChanged += new SelectionChangedEventHandler(SquadSelect);
                for (int i = 0; i < WorkDS.Tables["squads"].Rows.Count; i++)
                {
                    SquadsListBox.Items.Add(WorkDS.Tables["squads"].Rows[i].ItemArray[0]);
                }
                conn.Close();
            }
        }
        private void SquadSelect(object sender, RoutedEventArgs e)
        {

        }
    }
}
