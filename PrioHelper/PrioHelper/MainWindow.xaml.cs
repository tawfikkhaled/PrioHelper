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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace PrioHelper
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<SUJET> toDoObservable;
        public MainWindow()
        {
            InitializeComponent();
            var toDos = new List<SUJET>();
            toDos = SqlHelper.GetAllToDos();
            toDoObservable = new ObservableCollection<SUJET>(toDos);
            MyDataGrid.DataContext = toDoObservable;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] criterias = EnteredToDo.Text.Split(' ');
            SqlHelper.AddSubject(new SUJET { CRITERE1 = byte.Parse(criterias[0]), CRITERE2 = byte.Parse(criterias[1]), CRITERE3 = byte.Parse(criterias[2]) });
            var toDos = SqlHelper.GetAllToDos();
            toDoObservable.Add(new SUJET { CRITERE1 = byte.Parse(criterias[0]), CRITERE2 = byte.Parse(criterias[1]), CRITERE3 = byte.Parse(criterias[2]) });
        }
    }
}
