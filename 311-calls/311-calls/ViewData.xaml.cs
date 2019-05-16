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
using Npgsql;
using JsonUserVariable;
using PgsqlDriver;
using RowManager;

namespace Group7
{
    /// <summary>
    /// Interaction logic for ViewData.xaml
    /// </summary>
    public partial class ViewData : Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ViewData()
        {
            InitializeComponent();
            RowNumbers rows = new RowNumbers();
            try
            {
                rows = Application.Current.Resources["RowNumbers"] as RowNumbers;
            } catch(Exception e)
            {
                log.Info("Exception occured of type: " + e.GetType() + " in ViewData.xaml.cs when accessing application resources");
            }
            List<Json311> data = this.getData(rows);
            OverallData.DataContext = rows;
            DBDataBinding.ItemsSource = data;
            Total.Text = rows.total.ToString();
            Rows_min.Text = rows.Curr_min.ToString();
            Rows_max.Text = rows.Curr_max.ToString();
            Application.Current.Resources["RowNumbers"] = rows;
        }


        /// <summary>
        /// fill a list of our custom varible for us to display to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>Returns the data to populate the table</returns>
        private List<Json311> getData(RowNumbers rows)
        {
            Credentials creds = new Credentials();
            String user = creds.args[0];
            String pass = creds.args[1];
            SqlConnect getConnection = new SqlConnect();
            String connString = getConnection.Connect(user, pass, false);
            List<Json311> displayData = new List<Json311>();
            Json311 retrieve = new Json311();
            rows.total = getConnection.GetRows(connString);
            displayData = retrieve.FillList(connString, rows.Curr_min, rows.rowsRemaining);
            return displayData;
        }

        /// <summary>
        /// Gets the previous set of records for the user to look through
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Previous_click(object sender, RoutedEventArgs e)
        {
            RowNumbers rows = ((Button)sender).DataContext as RowNumbers;
            try
            {
                rows = Application.Current.Resources["RowNumbers"] as RowNumbers;
            }
            catch (Exception error)
            {
                log.Info("Exception occured of type: " + error.GetType() + " in ViewData.xaml.cs when accessing application resources");
            }
            bool update = rows.UpdateValuesDown();
            if(update == true)
            {
                ViewData newView = new ViewData();
                System.GC.Collect();
                this.NavigationService.Navigate(newView);
            }
        }

        /// <summary>
        /// gets the next set of records for the user to look through 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_click(object sender, RoutedEventArgs e)
        {
            RowNumbers rows = ((Button)sender).DataContext as RowNumbers;
            try
            {
                rows = Application.Current.Resources["RowNumbers"] as RowNumbers;
            }
            catch (Exception error)
            {
                log.Info("Exception occured of type: " + error.GetType() + " in ViewData.xaml.cs when accessing application resources");
            }
            bool update = rows.UpdateValuesUp();
            if(update == true)
            {
                ViewData newView = new ViewData();
                GC.Collect();
                this.NavigationService.Navigate(newView);
            }
        }


    }
}
