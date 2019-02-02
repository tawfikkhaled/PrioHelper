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
using System.Windows.Input;

namespace PrioHelper
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<TabItem> _tabItems;
        private List<TabItem> _allTabItems;
        private TabItem _tabAdd;
        AppModelPresenter appModelPresenter;
        ObservableCollection<SUJET> toDoObservable;
        public MainWindow()
        {

            appModelPresenter = new AppModelPresenter();
           
            try
            {
                InitializeComponent();
                var toDos = new List<SUJET>();
                toDos = SqlHelper.GetAllToDos();
                toDoObservable = new ObservableCollection<SUJET>(toDos);
                MyDataGrid.DataContext = toDoObservable;
                // initialize tabItem array
                _tabItems = new List<TabItem>();

                // add a tabItem with + in header 
                TabItem tabAdd = new TabItem();
                tabAdd.Header = "+";

                _tabItems.Add(tabAdd);
                AddTabItem(tab1);
                AddTabItem(tab2);

                // add first tab
                this.AddTabItem();

                // bind tab control
                //tabDynamic.DataContext = new object();
                _allTabItems = tabDynamic.Items.Cast<TabItem>().ToList();
                tabDynamic.Items.Clear();
                //var tab = (List<TabItem>)tabDynamic.DataContext;
                //tab.Clear();
                tabDynamic.DataContext =_tabItems;

                tabDynamic.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private TabItem AddTabItem()
        {
            int count = _tabItems.Count;

            // create new tab item
            TabItem tab = new TabItem();
            tab.Header = string.Format("Tab {0}", count);
            tab.Name = string.Format("tab{0}", count);
            tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;

            // add controls to tab item, this case I added just a textbox
            TextBox txt = new TextBox();
            txt.Name = "txt";
            tab.Content = txt;

            // insert tab item right before the last (+) tab item
            _tabItems.Insert(count - 1, tab);
            return tab;
        }
        private void AddTabItem(TabItem tabItem)
        {
            tabItem.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;
            int count = _tabItems.Count;

            _tabItems.Insert(count - 1, tabItem);
        }
        private void Key_Pressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                foreach( var item in _allTabItems) 
                {
                    if (item.Name == Screen.Text)
                    {
                        // clear tab control binding
                        tabDynamic.DataContext = null;

                        // add new tab
                        AddTabItem(item);

                        // bind tab control
                        tabDynamic.DataContext = _tabItems;

                        // select newly added tab item
                        tabDynamic.SelectedItem = item;
                    }
                }
            }
        }

        // Remove the clicked tab.
        private void TabItem_RemoveClicked(object sender,
        MouseButtonEventArgs e)
        {
            // Find the clicked Tab.
            TabItem tab_item = sender as TabItem;

            // Remove the TabItem.
            tabDynamic.Items.Remove(tab_item);

            e.Handled = true;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] criterias = EnteredToDo.Text.Split(' ');
            SqlHelper.AddSubject(new SUJET { CRITERE1 = byte.Parse(criterias[0]), CRITERE2 = byte.Parse(criterias[1]), CRITERE3 = byte.Parse(criterias[2]) });
            var toDos = SqlHelper.GetAllToDos();
            toDoObservable.Add(new SUJET { CRITERE1 = byte.Parse(criterias[0]), CRITERE2 = byte.Parse(criterias[1]), CRITERE3 = byte.Parse(criterias[2]) });
        }

        private void tabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = tabDynamic.SelectedItem as TabItem;

            if (tab != null && tab.Header != null)
            {
                if (tab.Header.Equals("+"))
                {
                    // clear tab control binding
                    tabDynamic.DataContext = null;

                    // add new tab
                    TabItem newTab = this.AddTabItem();

                    // bind tab control
                    tabDynamic.DataContext = _tabItems;

                    // select newly added tab item
                    tabDynamic.SelectedItem = newTab;
                }
                else
                {
                    // your code here...
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string tabName = (sender as Button).CommandParameter.ToString();

            var item = tabDynamic.Items.Cast<TabItem>().Where(i => i.Name.Equals(tabName)).SingleOrDefault();

            TabItem tab = item as TabItem;

            if (tab != null)
            {
                if (_tabItems.Count < 3)
                {
                    MessageBox.Show("Cannot remove last tab.");
                }
                else if (MessageBox.Show(string.Format("Are you sure you want to remove the tab '{0}'?", tab.Header.ToString()),
                    "Remove Tab", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // get selected tab
                    TabItem selectedTab = tabDynamic.SelectedItem as TabItem;

                    // clear tab control binding
                    tabDynamic.DataContext = null;

                    _tabItems.Remove(tab);

                    // bind tab control
                    tabDynamic.DataContext = _tabItems;

                    // select previously selected tab. if that is removed then select first tab
                    if (selectedTab == null || selectedTab.Equals(tab))
                    {
                        selectedTab = _tabItems[0];
                    }
                    tabDynamic.SelectedItem = selectedTab;
                }
            }
        }

    private void DrowButton_Click(object sender, RoutedEventArgs e)
        {
            MyCanvasChart.Children.Clear();
            byte[] widths = new byte[toDoObservable.Count];
            byte[] heights = new byte[toDoObservable.Count];
            byte[] colors = new byte[toDoObservable.Count];

            for(int i=0;i<toDoObservable.Count;i++)
            {
                widths[i] = (byte)toDoObservable[i].CRITERE1;
                heights[i] = (byte)toDoObservable[i].CRITERE2;
                colors[i] = (byte)toDoObservable[i].CRITERE3;
            }

            for(int i=0;i<toDoObservable.Count;i++)
            {
                var color = appModelPresenter.GetRgb(colors, colors[i]);
                int sum = 0;
                Array.ForEach(widths, x => sum += x); 
                var rec = new Rectangle
                {
                    Width = ((double)widths[i]/sum)*MyCanvasChart.Width,
                    Height = ((double)heights[i]/heights.Max())*MyCanvasChart.Height,
                    Fill = new SolidColorBrush(Color.FromRgb(color[0], color[1], color[2]))
                };
                MyCanvasChart.Children.Add(rec);
                Canvas.SetTop(rec, appModelPresenter.GetTop(heights,heights[i],MyCanvasChart.Height));
                Canvas.SetLeft(rec, appModelPresenter.GetLeft(widths,widths[i],MyCanvasChart.Width));

            }

        }
    }
}
