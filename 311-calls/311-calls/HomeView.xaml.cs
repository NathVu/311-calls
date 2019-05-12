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
using ConsoleApp1;
using System.Threading;

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
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Thread t1 = new Thread(new ThreadStart(Thread1Next));
            t1.Start();
            ViewData newView = new ViewData();
            this.NavigationService.Navigate(newView);
            t1.Join();
        }

        public void Thread1Next()
        {
            MessageBox.Show("Getting data", "Next", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Updates the database when the "update" button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Thread t1 = new Thread(new ThreadStart(Thread1Up));
            Thread t2 = new Thread(new ThreadStart(Thread2Up));
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
        }


        /// <summary>
        /// First thread for the Update_click button
        /// </summary>
        public void Thread1Up()
        {
            Credentials creds = new Credentials();
            ConsoleApp1.Group7 drive = new ConsoleApp1.Group7();
            drive.Execute(creds.args);
        }

        /// <summary>
        /// First thread for the Update_click button
        /// </summary>
        public void Thread2Up()
        {
            MessageBox.Show("Updating Database", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
        }


    }
}
