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
            if ((this.Curr_min - 500) < 0)
            {
                MessageBoxResult result = MessageBox.Show("Sorry, you cannot go any further back, would you like to go the end of the dataset?", "Our of Bounds", MessageBoxButton.OKCancel);
                return CarryOutDownResult(result);
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
                        this.rowsRemaining = total % 500;
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
