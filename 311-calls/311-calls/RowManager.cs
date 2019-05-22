using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RowManager
{
    /// <summary>
    /// To keep track of the records, and display the total number and the records you are currently looking through 
    /// </summary>
    class RowNumbers
    {
        public int total;
        public int Curr_min = 0;
        public int Curr_max = 500;
        public int rowsRemaining = 500;
        public int filter_min = 0;
        public int filter_max = 0;
        public int filter_total = 0;
        public int filter_remaining = 0;

        public RowNumbers() { }

        /// <summary>
        /// Constructor for initialization with values (most of the time this
        /// will be used))
        /// </summary>
        /// <param name="start">The starting value</param>
        /// <param name="max">The Current Max value</param>
        public RowNumbers(int start, int max)
        {
            this.Curr_min = start;
            this.Curr_max = max;
            this.rowsRemaining = max;
        }

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
                this.Curr_min += 500;
                this.Curr_max = total;
                this.rowsRemaining = this.total % 500;
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
        /// Updates the values if the Previous button is clicked % 500)
        /// 
        /// to-do - make sure that Curr_min is not total-500 but total - (total
        /// </summary>
        /// <returns>A bool whether or not to update the values</returns>
        public bool UpdateValuesDown()
        {
            if ((this.Curr_min - 500) < 0)
            {
                MessageBoxResult result = MessageBox.Show("Sorry, you cannot go any further back," +
                    " would you like to go the end of the dataset?", "Our of Bounds", MessageBoxButton.OKCancel);
                return CarryOutDownResult(result);
            }
            else
            {
                this.Curr_min -= 500;
                this.Curr_max -= 500;
                this.rowsRemaining = 500;
                return true;
            }
        }

        /// <summary>
        /// Takes care of the user directing the display out of the current paramaters
        /// i.e. - greater than the total or less than 0
        /// </summary>
        /// <param name="result">The result of the message box the user clicks earlier</param>
        /// <returns>Returns a bool to tell the program whether to get the next data set</returns>
        public bool CarryOutDownResult(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.OK:
                    {
                        this.rowsRemaining = total % 500;
                        this.Curr_max = this.total;
                        this.Curr_min = this.total - (this.total % 500);
                        return true;
                    }

                case MessageBoxResult.Cancel:
                    break;
            }
            return false;
        }

        /// <summary>
        /// Updates the rows for the next set from the filtered data
        /// </summary>
        /// <returns>returns a bool telling the calling function to update the query and page</returns>
        public bool FilterUp()
        {
            if ((this.filter_min + 500) > this.filter_total)
            {
                MessageBoxResult result = MessageBox.Show("You are at the beginning of the dataset" +
                    " would you like to jump to the end?", "Jump to End", MessageBoxButton.YesNo);
                return CarryOutUpFilter(result);
            }
            else if ((this.filter_max + 500) > this.filter_total)
            {
                this.filter_min += 500;
                this.filter_max = this.filter_total;
                this.filter_remaining = this.filter_total % 500;
                return true;
            }
            else
            {
                this.filter_min += 500;
                this.filter_max += 500;
                this.filter_remaining = 500;
                return true;
            }
        }

        /// <summary>
        /// If the filter would overflow, carries out the result of asking the user
        /// if they would like to go back to the beginnign
        /// </summary>
        /// <param name="result">The users choice for going back or staying</param>
        /// <returns>returns a bool telling the calling function whether to update or not</returns>
        public bool CarryOutUpFilter(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        this.filter_min = 0;
                        this.filter_max = 500;
                        this.filter_remaining = 500;
                        return true;
                    }

                case MessageBoxResult.No:
                    break;
            }
            return false;

        }

        /// <summary>
        /// Updates the rows to the previous set from the filtered data
        /// </summary>
        /// <returns>returns a bool telling the calling function to call the query and refresh the page or not</returns>
        public bool FilterDown()
        {
            if((this.filter_min - 500) < 0)
            {
                MessageBoxResult result = MessageBox.Show("You are at the beginning of the dataset, " +
                    "do you want to go to the end?", "Jump to End", MessageBoxButton.YesNo);
                return CarryOutDownFilter(result);
            }
            else
            {
                this.filter_min -= 500;
                this.filter_max -= 500;
                this.filter_remaining = 500;
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool CarryOutDownFilter(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        this.filter_min = this.filter_total - (this.filter_total % 500);
                        this.filter_max = this.filter_total;
                        this.filter_remaining = 0;
                        return true;
                    }
                case MessageBoxResult.No:
                    break;
            }
            return true;
        }
    }
}