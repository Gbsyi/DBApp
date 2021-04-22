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

namespace DBApp
{
    /// <summary>
    /// Логика взаимодействия для ChooseSquad.xaml
    /// </summary>
    public partial class ChooseSquad : Window
    {   

        public ChooseSquad()
        {
            InitializeComponent();
        }
        private void Submit(object sender, RoutedEventArgs e)
        {
            //chooseInt = int.Parse(choose.Text);
            TableView parent = (TableView)this.Owner;
            parent.ShowChildrenInSquadTable(int.Parse(choose.Text));
            this.Close();
        }
    }
}
