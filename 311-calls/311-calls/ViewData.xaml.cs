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
        public ViewData()
        {
            InitializeComponent();
            SetDataConext();

        }

        public void SetDataConext()
        {
            RowNumbers rows = (RowNumbers)Application.Current.Resources["RowNumbers"];
            List<Json311> data = this.GetData(rows);
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
        private List<Json311> GetData(RowNumbers rows)
        {
            String connString = (string)Application.Current.Resources["connString"];
            SqlConnect getConnection = new SqlConnect();
            List<Json311> displayData;
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
            rows = Application.Current.Resources["RowNumbers"] as RowNumbers;
            bool update = rows.UpdateValuesDown();
            if(update == true)
            {
                SetDataConext();
                GC.Collect();
                this.NavigationService.Refresh();
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
            rows = Application.Current.Resources["RowNumbers"] as RowNumbers;
            bool update = rows.UpdateValuesUp();
            if(update == true)
            {
                SetDataConext();
                GC.Collect();
                this.NavigationService.Refresh();
            }
        }


    }
}
