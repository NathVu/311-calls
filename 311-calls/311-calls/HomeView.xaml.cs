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
            WhichDatabase();
            RowNumbers rows = new RowNumbers(0,500);
            Application.Current.Resources["RowNumbers"] = rows;
        }

        /// <summary>
        /// Checks to see which database the user wants to use, will be deprecated later on
        /// and switched to just GCP, local is just here for testing
        /// </summary>
        public void WhichDatabase()
        {
            SqlConnect connect = new SqlConnect();
            Credentials creds = new Credentials();
            String user = creds.GCPargs[0];
            String pass = creds.GCPargs[1];
            Application.Current.Resources["connString"] = connect.ConnectGCP(user, pass, false);

            /*
            MessageBoxResult result = MessageBox.Show("Would you like to use the GCP database?" +
                 " \n If not it will use the local database", "Database Prompt", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        SqlConnect connect = new SqlConnect();
                        Credentials creds = new Credentials();
                        String user = creds.GCPargs[0];
                        String pass = creds.GCPargs[1];
                        Application.Current.Resources["connString"] = connect.ConnectGCP(user, pass, false);
                    }
                    break;

                case MessageBoxResult.No:
                    {
                        SqlConnect connect = new SqlConnect();
                        Credentials creds = new Credentials();
                        String user = creds.LocalArg[0];
                        String pass = creds.LocalArg[1];
                        Application.Current.Resources["connString"] = connect.ConnectLocal(user, pass, false);
                    }
                    break;
            } 
            */
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            ViewData newView = new ViewData();
            this.NavigationService.Navigate(newView);
        }

        /// <summary>
        /// Updates the database when the "update" button is clicked
        /// Checks to see if the user wants to use the GCP Database or the local
        /// database, will be changed when program is completed to default to GCP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            ConsoleApp1.Group7 drive = new ConsoleApp1.Group7();
            drive.Execute();
        }
    }
}
