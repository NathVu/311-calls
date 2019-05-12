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
            RowNumbers rows = new RowNumbers();
            this.getData(rows);
            OverallData.DataContext = rows;
        }


        /// <summary>
        /// fill a list of our custom varible for us to display to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getData(RowNumbers rows)
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
            DBDataBimnding.ItemsSource = displayData;
            Total.Text = rows.total.ToString();
            Rows_min.Text = rows.Curr_min.ToString();
            Rows_max.Text = rows.Curr_max.ToString();
            rows.UpdateValuesUp();


        }

        /// <summary>
        /// Gets the previous set of records for the user to look through
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Previous_click(object sender, RoutedEventArgs e)
        {
            RowNumbers rows = ((Button)sender).DataContext as RowNumbers;
            bool update = rows.UpdateValuesDown();
            if (update == true)
            {
                getData(rows);
            }
            Rows_min.Text = rows.Curr_min.ToString();
            Rows_max.Text = rows.Curr_max.ToString();

        }

        /// <summary>
        /// gets the next set of records for the user to look through 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Clicked");
            RowNumbers rows = ((Button)sender).DataContext as RowNumbers;
            bool update = rows.UpdateValuesUp();
            if (update == true)
            {
                getData(rows);
            }
            Rows_min.Text = rows.Curr_min.ToString();
            Rows_max.Text = rows.Curr_max.ToString();
        }


    }

    /// <summary>
    /// To keep track of the records, and display the total number and the records you are currently looking through 
    /// </summary>
    public class RowNumbers
    {

        public int total;
        public int Curr_min = 0;
        public int Curr_max = 500;
        public int rowsRemaining = 500;

        /// <summary>
        /// Updates the values if the next button is clicked
        /// </summary>
        /// <returns>a bool whether or not to update the values</returns>
        public bool UpdateValuesUp()
        {
            if ((this.Curr_max + 500) > this.total && (this.Curr_min + 500) > this.total)
            {
                MessageBoxResult result = MessageBox.Show("Sorry, you cannot go any further, would you like to go back to the beginning", "Out of Bounds", MessageBoxButton.OKCancel);
                return CarryOutUpResult(result);
            }
            else if ((this.Curr_max + 500) > this.total)
            {
                int remaining = this.total % 500;
                this.Curr_max = total;
                this.rowsRemaining = remaining;
                return true;
            }
            else
            {
                this.Curr_min += 500;
                this.Curr_max += 500;
                return true;
            }
        }

        /// <summary>
        /// If it overflows this function will go back to the beginning of the table
        /// or stay on the page if the user does not want to go back to row 0
        /// </summary>
        /// <param name="result">the result of the users press</param>
        /// <returns>a bool that tells the calling function whether to update or not</returns>
        public bool CarryOutUpResult(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.OK:
                    {
                        this.Curr_min = 0;
                        this.Curr_max = 500;
                        this.rowsRemaining = 500;
                        return true;
                    }

                case MessageBoxResult.Cancel:
                    break;
            }
            return false;
        }

        /// <summary>
        /// Updates the values if the Previous button is clicked
        /// </summary>
        /// <returns>A bool whether or not to update the values</returns>
        public bool UpdateValuesDown()
        {
            if ((this.Curr_min - 500) < 500)
            {
                MessageBox.Show("Sorry, you cannot go any further back, would you like to go the end of the dataset?", "Our of Bounds", MessageBoxButton.OKCancel);
                return false;
            }
            else
            {
                this.Curr_min -= 500;
                this.Curr_max -= 500;
                return true;
            }
        }

        public bool CarryOutDownResult(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.OK:
                    {
                        int remaining = total % 500;
                        this.rowsRemaining = remaining;
                        this.Curr_max = this.total;
                        this.Curr_min = this.total - 500;
                        return true;
                    }

                case MessageBoxResult.Cancel:
                    break;
            }
            return false;
        }
    }
}
