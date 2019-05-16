using System;
using System.Collections.Generic;
using System.Linq;
using SODA;
using JsonUserVariable;
using PgsqlDriver;
using Npgsql;
using NUnit.Framework;
using System.Globalization;
using System.Windows;


namespace ConsoleApp1
{
    /// <summary>
    /// our driver class, contains our main and calls of all the functions we need in order to get the data
    /// from the 311 database into our psql db
    ///
    /// todo - write a ping function to test internet connection
    /// </summary>
    class Group7
    {

        /// <summary>
        /// Since there is no main this fulfills the purpose for the update button
        /// it handles the connection, API call and update to the gcloud database
        /// </summary>
        /// <param name="arguments">the credentials</param>
        public void Execute()
        {
            SqlConnect dBConnect = new SqlConnect();
            String connString = (string)Application.Current.Resources["ConnString"];
            dBConnect.CheckDate(connString, out DateTime date);
            DataFormat test = new DataFormat();
            Dictionary<string, object>[] rarr = test.GetData(date);
            List<Json311> forDB = test.ParseData(rarr);
            dBConnect.Import(forDB, connString);
        }

        /// <summary>
        /// if the date is not a full day since the database is last updated
        /// there is no point in updating it again yet
        /// 
        /// This function is to check if it is worth updating
        /// </summary>
        /// <param name="arguments">the credentials to access the database</param>
        /// <returns>returns true if the database should be updated and false if not</returns>
        public bool CheckDateForUpdate()
        {
            DateTime test = DateTime.Now;
            DateTime now = DateTime.Now;
            SqlConnect dBConnect = new SqlConnect();
            String connString = (string)Application.Current.Resources["ConnString"];
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (NpgsqlCommand getDate = new NpgsqlCommand("SELECT * FROM checktime", conn))
                using (NpgsqlDataReader reader = getDate.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        test = Convert.ToDateTime(reader.GetTimeStamp(0));
                    }
                }

            }
            if(now > (test.AddDays(1)))
            {
                return true;
            }
            return false;
        }

    }




    /// <summary>
    ///  A class to take the data recieved from the API Query
    ///  And format it usiing our user defined class to
    /// </summary>
    class DataFormat
    {

        /// <summary>
        /// used to parse our data into our user created type instead of the dictionary that the API
        /// call returns
        /// </summary>
        /// <param name="dataset"> recieves out dataset, formatted in getData into an array of Dictionary objects</param>
        /// <returns>The data parsed into our user created class
        /// </returns>
        public List<Json311> ParseData(Dictionary<string, object>[] dataset)
        {
            List<Json311> dataList = new List<Json311>();
            DateTime date = DateTime.Today.Date.AddDays(-1);
            for (int i = 0; i < dataset.Length; i++)
            {
                Json311 dItem = new Json311(dataset[i]);
                if (dItem.Created_date < date)
                {
                    dataList.Add(dItem);
                }
            }
            return dataList;
        }

        /// <summary>
        /// Manages our Database function call and gets back the Data as they return us
        /// And converts it to an array which we return
        /// </summary>
        /// <returns>A Dictionary Array which we can read through</returns>
        public Dictionary<string, object>[] GetData(DateTime date)
        {
            ManageDB test = new ManageDB();
            IEnumerable<Dictionary<string, object>> results = test.LoadDB(date);

            /// <remarks>
            /// Allows us to read the data
            /// We convert the dara into an array so it is not in an interface and we are able
            /// to read the data that our query returned
            /// </remarks>
            Dictionary<string, object>[] results_arr = results.ToArray();
            return results_arr;
        }
    }





    /// <summary> Class for managing the Database
    /// created for code clean-up and easier readability
    /// </summary>
    class ManageDB
    {
        public ManageDB() { }

        /// <summary>
        /// Assisted by code from https://dev.socrata.com/foundry/data.cityofnewyork.us/fhrw-4uyv
        /// queried DB - https://data.cityofnewyork.us
        /// API KEY - PVGjhHLj8Svy7Ryz0uJgW9IBh
        /// loadDB connects to the database, sends the query and then returns the data
        /// </summary>
        /// <returns>Returns the dataset of the query to main</returns>
        public IEnumerable<Dictionary<string, object>> LoadDB(DateTime date)
        {
            SODA.SodaClient client = new SodaClient("https://data.cityofnewyork.us", "PVGjhHLj8Svy7Ryz0uJgW9IBh");

            /// <remarks>
            /// The documentation on the web is outdated.
            /// The .NET library has been updated to no longer allow generic typing.
            /// You must use either Dictionary(String,Object) - use <> but not allowed in XML comments
            /// OR a user-defined json serializable class - their documentation does not explain how to do this
            /// well enough, however so we are sticking with the Generic Collection specified
            /// </remarks>>
            SODA.Resource<Dictionary<string, object>> dataset = client.GetResource<Dictionary<string, object>>("fhrw-4uyv");
            SoqlQuery soql = this.GetQueryDate(date);
            IEnumerable<Dictionary<string, object>> results = dataset.Query<Dictionary<string, object>>(soql);
            return results;
        }

        /// <summary>
        /// This function was written to increase readability of the code
        /// It gets the current data (and yesterdays date) so that the code does not need to be updated every day
        /// and composes a query object (SODA.SoqlQuery) to reurn to LoadDB to query the dataset
        /// Utilizes the DateTime type to get todays date
        /// </summary>
        /// <returns>our Query in their defined type - SODA.SoqlQuery </returns>
        public SODA.SoqlQuery GetQueryDate(DateTime date)
        {

            /// <remarks>
            /// Get Date and Time of system now so we do not have to keep changing the code
            /// </remarks>
            DateTime today = date;
            String year = today.Year.ToString();

            /// <remarks>
            /// Their query requires typing with a 0 in front of the month so we
            /// check the formatting and add a 0 if necessary
            /// </remarks>
            String month;
            if (today.Month / 10 != 0)
            {
                month = today.Month.ToString();
            }
            else
            {
                month = "0" + today.Month.ToString();
            }

            /// <remarks>
            /// Since the data is only updated every day for the day before in part and fully for
            /// 2 days before we get the dates for yesterday and the day before for filtering
            /// </remarks>
            String day = (today.Day - 2).ToString();
            String yday = (today.Day - 3).ToString();

            /// <remarks>
            ///The date field in the Query needs to be of type
            ///yyyy-mm-dd + T + hh:mm:ss.nnn (hours, minutes, seconds and nanoseconds.
            ///We format the data here to be in the correct type as the ToString method provided by
            ///the DateTime class does not allow for formatting in this manner
            /// </remarks>
            String tdate = "\"" + year + "-" + month + "-" + day + "T00:00:00.000\"";
            String ydate = "\"" + year + "-" + month + "-" + yday + "T00:00:00.000\"";

            /// <remarks>
            /// A test case just to test that the connection is working
            /// will be removed in later revisions
            /// </remarks>
            //SODA.SoqlQuery soql = new SoqlQuery().Select("*").Limit(10);

            /// <remarks>
            /// for increased readability and easier formatting
            /// </remarks>
            string cdate = "created_date";

            /// <remarks>
            /// Usage of creating a new SoqlQuery -
            /// Note: TimeStamp (or other literal) must be in quotes or else it gets treated like a variable which will throw an error
            /// Field + "function" + "comparison type"
            /// Select can be used to select specific fields but we want all data associated with the calls
            /// Hour compensates for my estimated time the database is updated with new info
            /// Will be updated again when we find out when exactly the data is updated every day
            /// </remarks>
            SODA.SoqlQuery soql;
            if (today.Hour < 10)
            {
                soql = new SoqlQuery().Select("*").Where(cdate + ">" + date);
            }
            else
            {
                soql = new SoqlQuery().Select("*").Where(cdate + ">" + ydate);
            }

            return soql;
        }
    }

}
