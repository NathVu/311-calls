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
using JsonUserVariable;
using PgsqlDriver;
using RowManager;


namespace Group7
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : Page
    {
        public HomeView()
        {
            InitializeComponent();
            RowNumbers rows = new RowNumbers(0,500);
            Application.Current.Resources["RowNumbers"] = rows;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            ViewData newView = new ViewData();
            this.NavigationService.Navigate(newView);
        }

        /// <summary>
        /// Updates the database when the "update" button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Credentials creds = new Credentials();
            ConsoleApp1.Group7 drive = new ConsoleApp1.Group7();
            drive.Execute(creds.args);
        }


    }
}
