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
    /// Логика взаимодействия для HRWindow.xaml
    /// </summary>
    public partial class HRWindow : Window
    {
        private string connection = @"Data Source=GBSYIPC\SQLEXPRESS;Initial Catalog=Lager;Integrated Security=True";
        private DataSet workDS = new DataSet();
        private bool IsLogout = false;
        private bool IsNewWorker = false;
        private string workerName;

        private int workerId = 0;

        RoutedEventHandler workerSelectedDelegate;
        object workerSelectedSender;
        RoutedEventArgs workerSelectedArgs;
        public HRWindow()
        {
            InitializeComponent();
        }

        //Мои методы
        /// <summary>
        ///  Проверка на наличие должности
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Наличие должности</returns>
        private int GetPositionID(string position) 
        {
            int returnValue;
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
               
                string command;
                command = $"exec [dbo].[checkPosition] \'{positionTB.Text}\'";
                SqlDataAdapter sda = new SqlDataAdapter(command, conn);
                DataSet ds = new();
                sda.Fill(ds, "value");
                returnValue = int.Parse(ds.Tables["value"].Rows[0].ItemArray[0].ToString());
                conn.Close();
            }
            return returnValue;
        }

        private void ChangeUser(object sender, RoutedEventArgs e)
        {
            //workDS.Clear();
            IsLogout = true;
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowWorkers(sender, e);
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

        private void ShowWorkers(object sender, RoutedEventArgs e)
        {
            plusButton.Visibility = Visibility.Hidden;
            rightPanel.Children.Clear();
            leftPanel.Children.Clear();

            ListBox workers = new() { Margin = new Thickness(10, 0, 0, 0) };
            workers.SelectionChanged += new SelectionChangedEventHandler(WorkerSelected);
            Label workersLabel = new Label { Content = "Сотрудники", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 5, 0, 0) };
            leftPanel.Children.Add(workersLabel);
            leftPanel.Children.Add(workers);
            DockPanel.SetDock(workersLabel, Dock.Top);
            DockPanel.SetDock(workers, Dock.Top);
            //leftPanel.Children.Add(workers);
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                //Вывод сотрудников
                SqlDataAdapter sda = new SqlDataAdapter("exec showWorkers", conn);
                workDS.Tables["workers"]?.Clear();
                sda.Fill(workDS, "workers");
                for (int i = 0; i < workDS.Tables["workers"].Rows.Count; i++)
                {
                    var item = workDS.Tables["workers"].Rows[i].ItemArray;
                    workers.Items.Add($"{item[1]} {item[2]} {item[3]}");
                }
                conn.Close();
            }
            if (IsNewWorker)
            {
                workers.SelectedIndex = workers.Items.Count - 1;
            }
            else { 
                workers.SelectedIndex = 0;
            }
            IsNewWorker = false;
        }

        private void WorkerSelected(object sender, RoutedEventArgs e)
        {
            rightPanel.Children.Clear();
            plusButton.Visibility = Visibility.Visible;

            ListBox workersList = (ListBox)sender;
            workerId = int.Parse(workDS.Tables["workers"].Rows[workersList.SelectedIndex].ItemArray[0].ToString());
            workerName = $"{workDS.Tables["workers"].Rows[workersList.SelectedIndex].ItemArray[1]} {workDS.Tables["workers"].Rows[workersList.SelectedIndex].ItemArray[2]} {workDS.Tables["workers"].Rows[workersList.SelectedIndex].ItemArray[3]}";
            ListBox positionsList = new() { Margin = new Thickness(5, 0, 0, 0) };
            Label label = new() { Content = "Должности", FontWeight = FontWeights.Bold, Margin = new Thickness(5, 5, 0, 0) };
            rightPanel.Children.Add(label);
            rightPanel.Children.Add(positionsList);
            DockPanel.SetDock(label, Dock.Top);
            DockPanel.SetDock(positionsList, Dock.Top);

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                //Вывод сотрудников
                SqlDataAdapter sda = new SqlDataAdapter($"exec [dbo].[getWorkerPositions] {workerId}", conn);
                workDS.Tables["workerPositions"]?.Clear();
                sda.Fill(workDS, "workerPositions");
                for (int i = 0; i < workDS.Tables["workerPositions"].Rows.Count; i++)
                {
                    positionsList.Items.Add(workDS.Tables["workerPositions"].Rows[i].ItemArray[0]);
                }
                if (workDS.Tables["workerPositions"].Rows.Count == 0)
                {
                    MenuItem item = new() { Header = "Добавить должность"};
                    item.Click += AddPosition;
                    positionsList.Items.Add(new ListBoxItem()
                    {
                        Content = "Должность отсутствует",
                        ContextMenu = new() 
                        {
                            Items = { item }
                        }
                    });
                }
            }
            
            workerSelectedDelegate += WorkerSelected;
            workerSelectedSender = sender;
            workerSelectedArgs = e;
        }

        private void AddPosition(object sender, RoutedEventArgs e)
        {
            addPositionDialog.IsOpen = true;
        }
        private void AddWorker(object sender, RoutedEventArgs e)
        {
            addWorkerDialog.IsOpen = true;
        }
        private void CloseDialog(object sender, RoutedEventArgs e)
        {
            addPositionDialog.IsOpen = false;
            addWorkerDialog.IsOpen = false;
        }
        private void SavePosition(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                int posId = GetPositionID(positionTB.Text);
                if (posId == -1)
                {
                    //Добавить новую
                    string command = $"exec [dbo].[insertPosition] \'{positionTB.Text}\'";
                    SqlDataAdapter sda = new SqlDataAdapter(command, conn);
                    DataSet ds = new();
                    sda.Fill(ds);
                    posId = int.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
                    command = $"insert into [dbo].[workers_position] values({workerId}, {posId})";
                    SqlCommand com = new SqlCommand(command, conn);
                    com.ExecuteNonQuery();
                }
                else
                {
                    //Добавить имеющуюся
                    string command = $"insert into [dbo].[workers_position] values({workerId}, {posId})";
                    SqlCommand com = new SqlCommand(command, conn);
                    com.ExecuteNonQuery();
                }
                conn.Close();
            }
            addPositionDialog.IsOpen = false;
            workerSelectedDelegate?.Invoke(workerSelectedSender, workerSelectedArgs);
        }

        private void SaveWorker(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                if (workerSurnameTB.Text != ""
                   && workerNameTB.Text != ""
                   && workerFathersNameTB.Text != "")
                {
                    string command = $"exec [dbo].[insertWorker] \'{workerSurnameTB.Text}\', \'{workerNameTB.Text}\', \'{workerFathersNameTB.Text}\'";
                    SqlCommand com = new SqlCommand(command, conn);
                    com.ExecuteNonQuery();
                }
                conn.Close();
            }
            IsNewWorker = true;
            addWorkerDialog.IsOpen = false;
            ShowWorkers(sender, e);
        }

        private void ShowSchedule(object sender, RoutedEventArgs e)
        {
            ShiftsWindow shifts = new();
            shifts.WorkerId = workerId;
            shifts.WorkerName = workerName;
            shifts.Owner = this;
            shifts.Show();
        }
        private void OpenClubsWindow(object sender, RoutedEventArgs e)
        {
            ClubsWindow window = new();
            window.Owner = this;
            window.Show();
        }
    }
}
