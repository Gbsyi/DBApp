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
        private void HidePanelContent()
        {
            //Вкладка "Фильтр"
            filterTitle.Visibility = Visibility.Hidden;
            chooseGroup.Visibility = Visibility.Hidden;
            chouseGroupLabel.Visibility = Visibility.Hidden;

            //Вкладка "Добавление"
            addTitle.Visibility = Visibility.Hidden;

            header1.Visibility = Visibility.Hidden;
            header2.Visibility = Visibility.Hidden;
            header3.Visibility = Visibility.Hidden;
            header4.Visibility = Visibility.Hidden;
            header5.Visibility = Visibility.Hidden;
            header6.Visibility = Visibility.Hidden;
            header7.Visibility = Visibility.Hidden;

            add1.Visibility = Visibility.Hidden;
            add2.Visibility = Visibility.Hidden;
            add3.Visibility = Visibility.Hidden;
            add4.Visibility = Visibility.Hidden;
            add5.Visibility = Visibility.Hidden;
            add6.Visibility = Visibility.Hidden;
            add7.Visibility = Visibility.Hidden;

            addButton.Visibility = Visibility.Hidden;
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
            HidePanelContent();
            tabPanel.SelectedIndex = 0;
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
            chooseGroup.ItemsSource = groups;
            
            //Grid.SetRow(chooseGroup, 1);
            //Через доп. окно
            /*ChooseGroup chooseGroup = new ChooseGroup();
            chooseGroup.Owner = this;
            chooseGroup.Show();*/
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
            HidePanelContent();
            tabPanel.SelectedIndex = 0;
            TableOutput("exec [dbo].[showChildrenBenefitsTable]");
        }
        private void WorkersTable(object sender, RoutedEventArgs e)
        {
            HidePanelContent();
            tabPanel.SelectedIndex = 0;
            TableOutput("exec [dbo].[showWorkersTable]");

        }
        private void SquadsTable(object sender, RoutedEventArgs e)
        {
            HidePanelContent();
            tabPanel.SelectedIndex = 0;
            TableOutput("exec [dbo].[showSquadsTable]");
        }
        private void ChildrenTable(object sender, RoutedEventArgs e)
        {
            HidePanelContent();
            tabPanel.SelectedIndex = 0;
            TableOutput("exec [dbo].[showChildrenTable]");
        }
        private void CampsTable(object sender, RoutedEventArgs e)
        {
            HidePanelContent();
            tabPanel.SelectedIndex = 0;
            TableOutput("exec [dbo].[showCampsTable]");
        }
        private void ChildrenInSquadTable(object sender, RoutedEventArgs e)
        {

            ChooseSquad chooseSquad = new ChooseSquad();
            chooseSquad.Show();
            chooseSquad.Owner = this;

           
        }
        
        public void AddWorker(object sender, RoutedEventArgs e)
        {
            HidePanelContent();

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
            TableOutput("exec [dbo].[showWorkers]");

        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if(add1.Text != "" && add2.Text != "" && add3.Text != ""){
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    List<string> vars = new();

                    vars.Add(add1.Text);
                    vars.Add(add2.Text);
                    vars.Add(add3.Text);

                    insert.SetVars(vars);
                    
                    string command = insert.GetTableCommand();
                    SqlCommand com = new SqlCommand(command, conn);
                    com.ExecuteNonQuery();
                }
                add1.Clear();
                add2.Clear();
                add3.Clear();
                TableOutput("exec [dbo].[showWorkers]");
            }
            else
            {
                MessageBox.Show("Необходимо заполнить все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void ShowChildrenInSquadTable(int squad)
        {
            HidePanelContent();
            TableOutput($"exec [dbo].[showChildrenInSquad] {squad}");
        }
        
        private void OnClose(object sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
