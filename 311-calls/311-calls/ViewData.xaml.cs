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
using _311_calls;

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
            SetDataContext();

        }

        public void SetDataContext()
        {
            RowNumbers rows = (RowNumbers)Application.Current.Resources["RowNumbers"];
            List<Json311> data = this.GetData(rows);
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
            SqlConnect getConnection = new SqlConnect();
            List<Json311> displayData;
            Json311 retrieve = new Json311();
            rows.total = getConnection.GetRows();
            displayData = retrieve.GetFullDataset(rows.Curr_min, rows.rowsRemaining);
            return displayData;
        }

        /// <summary>
        /// Gets the previous set of records for the user to look through
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Previous_click(object sender, RoutedEventArgs e)
        {
            RowNumbers rows = Application.Current.Resources["RowNumbers"] as RowNumbers; ;
            bool update = rows.UpdateValuesDown();
            if(update == true)
            {
                SetDataContext();
                GC.Collect();
                this.NavigationService.Refresh();
            }
            Application.Current.Resources["RowNumbers"] = rows;
        }

        /// <summary>
        /// gets the next set of records for the user to look through 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_click(object sender, RoutedEventArgs e)
        {
            RowNumbers rows = Application.Current.Resources["RowNumbers"] as RowNumbers;
            bool update = rows.UpdateValuesUp();
            if(update == true)
            {
                SetDataContext();
                GC.Collect();
                this.NavigationService.Refresh();
            }
            Application.Current.Resources["RowNumbers"] = rows;
        }


        /// <summary>
        /// Displays the map page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_map(object sender, RoutedEventArgs e)
        {
            Map newMap = new Map();
            this.NavigationService.Navigate(newMap);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_dates(object sender, RoutedEventArgs e)
        {
            DateTime? start = Start.SelectedDate;
            DateTime? end = Start.SelectedDate;
            RowNumbers rows = Application.Current.Resources["RowNumbers"] as RowNumbers;
            SqlConnect dbconnect = new SqlConnect();
            Json311 get_data = new Json311();
            List<Json311> show_data = new List<Json311>();
            if (!start.HasValue)
            {
                MessageBox.Show("Select a date on the left");
            }
            else if(start.HasValue && !end.HasValue)
            {
                DateTime new_start = (DateTime)start;
                int year = new_start.Year;
                int month = new_start.Month;
                int date = new_start.Day;
                DateTime date1 = new DateTime(year, month, date, 0, 0, 0);
                DateTime date2 = new DateTime(year, month, date, 23, 59, 59);
                int num_rows = dbconnect.GetRows(date1, date2);
                if(num_rows == 0)
                {
                    MessageBox.Show("There are no values for the selected dates");
                }
                else
                {
                    rows.filter_min = 0;
                    if(num_rows > 500)
                    {
                        rows.filter_max = 500;
                    }
                    else
                    {
                        rows.filter_max = num_rows;
                    }
                    rows.filter_total = num_rows;
                    rows.is_filter = true;
                    show_data =  get_data.GetDateFilteredList(0, 0, date1, date2);
                    DBDataBinding.ItemsSource = show_data;
                    this.NavigationService.Refresh();
                }
            }
            Application.Current.Resources["RowNumbers"] = rows;
        }

        
    }
}
