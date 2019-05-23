using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;
using JsonUserVariable;
using System.Windows;

namespace PgsqlDriver
{

    /// <summary>
    ///  Class to connect our program to postgres DB and load our data into it
    /// </summary>
    class SqlConnect
    {
        /// <summary>
        /// Establishes a connection between the program and our database (which will hopefully be hosted on Google Cloud Compute - completed)
        /// Also check the username/password pair and prompts the user to re-enter if they are incorrect  
        /// This Connect version is to connect to the GCP database 
        /// </summary>  
        public String ConnectGCP(String user = "", String pass = "", bool newCredentials = true)
        {
            if(newCredentials == true)
            {
                user = Authenticate(out pass);
            }
            pass.Replace(" ", ""); //remove accidental whitespace
            String connString = "Host=127.0.0.1;Port=5433;Username=" + user + ";Password=" + pass + ";Database=postgres";

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    this.CheckConnection(conn);
                    conn.Close();
                    return connString;
                }
            }    catch(Npgsql.PostgresException a) when (newCredentials == true)
                 {
                    this.ConnectGCP();
                }

            return "failed";
            
        }

        /// <summary>
        /// To connect to the database stored on localhost for quicker testing
        /// and development
        /// </summary>
        /// <param name="user">The username for the connection string</param>
        /// <param name="pass">The password for the connection string</param>
        /// <param name="newCredentials">For testing purposes</param>
        /// <returns></returns>
        public String ConnectLocal(String user = "", String pass = "", bool newCredentials = true)
        {
            if (newCredentials == true)
            {
                user = Authenticate(out pass);
            }
            pass.Replace(" ", ""); //remove accidental whitespace
            String connString = "Host=localhost;Port=5432;Username=" + user + ";Password=" + pass + ";Database=calls311";

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    this.CheckConnection(conn);
                    conn.Close();
                    return connString;
                }
            }
            catch (Npgsql.PostgresException a) when (newCredentials == true)
            {
                //Console.WriteLine("Your username and password do not match, try again " + a.GetType());
                this.ConnectLocal();
            }

            return "failed";

        }


        /// <summary>
        /// make sure a connection has been established to the database
        /// Will also attempt to reconnect if the connection is checked and is no longer working
        /// Althogh connection is dropped when you leave scope since we are still inside the function
        /// when we make this call the connection stays open
        /// </summary>
        /// <param name="conn">The connection we are testing</param>
        private void CheckConnection(NpgsqlConnection conn)
        {
            if(conn.State == System.Data.ConnectionState.Open)
            {
               // Console.WriteLine("Connection Established");
            }
            else
            {
               // Console.WriteLine("Connection Dropped, Please re-enter credentials");
                this.ConnectGCP();
            }
        }


        /// <summary>
        /// Responsible for user authentication to the psql database 
        /// </summary>
        /// <param name="pass">An out param to return the password to the calling function</param>
        /// <returns>Returns the username to the calling function</returns>
        private String Authenticate(out String pass)
        {
            String user = this.GetUser();
            pass = this.GetPassword(user);
            return user;
        }

        /// <summary>
        /// Get username for the database we are connecting to 
        /// and require that user enter a real username
        /// </summary>
        /// <returns>The username entered</returns>
        public String GetUser()
        {
            String user = "";
            Console.Write("Enter Username: ");
            while (user == "" || string.IsNullOrWhiteSpace(user))
            {
                user = Console.ReadLine();
            }
            return user;
            
        }
        
        /// <summary>
        /// Get the Password for the User specified to connect to our postgresDB
        /// </summary>
        /// <param name="user">The username we are getting the password for</param>
        /// <returns>returns the password to the calling function</returns>
        public String GetPassword(String user)
        {
            Console.Write("Enter Password for " + user + ": ");
            /// <remarks> 
            /// This is to make sure the password is not visible as it is typed 
            /// </remarks>
            String pass = null;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                pass += key.KeyChar;
            }
            Console.WriteLine();
            return pass;
        }


        /// <summary>
        /// Our import command to import the data for the day into our pgsql database,
        /// will be broken up into multiple functions for increased readability 
        /// </summary>
        /// <param name="dataset">Our current DataSet to load into our DB</param>
        /// <param name="connString">The string to initialize the connection to the database</param>
        /// <param name="tableName">The tableName, set to calls by default, allows for testing
        ///     to a different table without adding fake data to the database, the testing database is 
        ///     otherwise functionally identical to the calls database
        /// </param>
        public void Import(List<Json311> dataset, String connString, string tableName = "calls", Boolean updateTime = true)
        {
            using (var conn = new NpgsqlConnection(connString))
            {

                /// <remarks>
                /// Make Sure data is not duplicated
                /// </remarks>
                conn.Open();
                this.CheckConnection(conn);
                //conn.TypeMapper.UseJsonNet();
                NpgsqlDateTime last_date = new NpgsqlDateTime();
                DateTime construct = new DateTime(0);
                NpgsqlDateTime most_recent = new NpgsqlDateTime(construct);

                /// <remarks> 
                /// Get the DateStamp frmo our checktime table
                /// We do this in order to check the stored datestamp so as not to import duplicate DATA
                /// We cannot read and write over the same connection so we need to retrieve the dateTime so we can check it against the timestamp
                /// of the entries later
                /// </remarks>
                if (updateTime == true)
                {
                    using (NpgsqlCommand checkDate = new NpgsqlCommand("SELECT * FROM checktime", conn))
                    using (NpgsqlDataReader reader = checkDate.ExecuteReader())
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                last_date = reader.GetTimeStamp(0);
                                reader.Close();
                            }
                        }
                        catch (Exception a) { Console.WriteLine(a); }
                    }
                }
                int oldC = 0, newC = 0;
                /// <remarks>
                /// postgres does not support nullable datetimes so we check the DateTime fields and
                /// write a null if the value is null
                /// if not we cast the nullable datetime to datetime and then write that
                /// Using the tablename variable allows us to change the target table without changing all the code
                /// </remarks>
                using (var writer = conn.BeginBinaryImport("COPY " + tableName + " FROM STDIN (FORMAT BINARY)"))
                {
                    foreach (Json311 entry in dataset)
                    {
                      
                        /// <remarks> 
                        /// Check the retrieved dateTime against the entries dateTime
                        /// </remarks>
                        NpgsqlDateTime entryDate = Convert.ToDateTime(entry.Created_date);
                        if (entryDate < last_date)
                        {
                            if(most_recent < entryDate)
                            {
                                most_recent = entryDate;
                            }
                            oldC++;
                            continue;
                       }

                         
                        writer.StartRow();
                        writer.Write(entry.Unique_key);

                        if (entry.Created_date == null)
                        {
                            writer.WriteNull();
                        }
                        else
                        {
                            NpgsqlDateTime cdate = Convert.ToDateTime(entry.Created_date);
                            writer.Write(cdate);
                        }


                        if (entry.Closed_date == null)
                        {
                            writer.WriteNull();
                        }
                        else
                        {
                            NpgsqlDateTime cdate = Convert.ToDateTime(entry.Closed_date);
                            writer.Write(cdate);
                        }

                        writer.Write(entry.Agency);
                        writer.Write(entry.Agency_name);
                        writer.Write(entry.Complaint_type);
                        writer.Write(entry.Descriptor);
                        writer.Write(entry.Location_type);
                        writer.Write(entry.Incident_zip);
                        writer.Write(entry.Incident_address);
                        writer.Write(entry.Street_name);
                        writer.Write(entry.Cross_street_1);
                        writer.Write(entry.Cross_street_2);
                        writer.Write(entry.Intersection_street_1);
                        writer.Write(entry.Intersection_street_2);
                        writer.Write(entry.Address_type);
                        writer.Write(entry.City);
                        writer.Write(entry.Landmark);
                        writer.Write(entry.Facility_type);
                        writer.Write(entry.Status);


                        if (entry.Due_date == null)
                        {
                            writer.WriteNull();
                        }
                        else
                        {
                            NpgsqlDateTime dDate = Convert.ToDateTime(entry.Due_date);
                            writer.Write(dDate);
                        }

                        writer.Write(entry.Resolution_description);


                        if (entry.Resolution_action_updated_date == null)
                        {
                            writer.WriteNull();
                        }
                        else
                        {
                            NpgsqlDateTime rDate = Convert.ToDateTime(entry.Resolution_action_updated_date);
                            writer.Write(rDate);
                        }

                        writer.Write(entry.Community_board);
                        writer.Write(entry.Bbl);
                        writer.Write(entry.Borough);
                        writer.Write(entry.X_coordinate_state_plane);
                        writer.Write(entry.Y_coordinate_state_plane);
                        writer.Write(entry.Open_data_channel_type);
                        writer.Write(entry.Park_facility_name);
                        writer.Write(entry.Park_borough);
                        writer.Write(entry.Vehicle_type);
                        writer.Write(entry.Taxi_company_borough);
                        writer.Write(entry.Taxi_pick_up_location);
                        writer.Write(entry.Bridge_highway_name);
                        writer.Write(entry.Bridge_highway_direction);
                        writer.Write(entry.Road_ramp);
                        writer.Write(entry.Bridge_highway_segment);
                        writer.Write(entry.Latitude);
                        writer.Write(entry.Longitude);
                        writer.Write(entry.Location_city);


                        if (entry.Location == null)
                        {
                            writer.WriteNull();
                        }
                        else
                        {
                            var dat = entry.Location.SelectToken("coordinates");
                            double x = (double)dat[0];
                            double y = (double)dat[1];
                            NpgsqlPoint nPoint = new NpgsqlPoint(x, y);
                            writer.Write(nPoint);
                        }

                        writer.Write(entry.Location_zip);
                        writer.Write(entry.Location_state);

                        newC++;
                    }
                    writer.Complete();
                }
                Console.WriteLine("New Records:" + newC + " Old Records: " + oldC);
                /// <remarks> 
                /// Update the stored Date in the checktime table, only update the time
                /// if we are actually adding data and not just for a test
                /// </remarks>
                if(updateTime == true)
                {
                    NpgsqlCommand dropCheck = new NpgsqlCommand("DROP TABLE checktime", conn);
                    dropCheck.ExecuteNonQuery();
                    NpgsqlCommand newCheck = new NpgsqlCommand("CREATE TABLE checktime (curr_up_date timestamp)", conn);
                    newCheck.ExecuteNonQuery();
                    using (var writer = conn.BeginBinaryImport("COPY checktime FROM STDIN (FORMAT BINARY)"))
                    {
                        writer.StartRow();
                        writer.Write(most_recent);
                        writer.Complete();
                    }
                }
               
                /// <remarks> 
                /// Closes the connection when we are finished with it
                /// </remarks>
                conn.Close();
            }
        }

        /// <summary>
        /// Checks the date to see if we need to make a new API call for today
        /// </summary>
        /// <param name="conn">recieves the connection to our database as a parameter</param>
        public void CheckDate(String connString, out DateTime date)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                date = DateTime.Now;
                conn.Open();
                this.CheckConnection(conn);
                using (NpgsqlCommand checkDate = new NpgsqlCommand("SELECT * FROM checktime", conn))
                using (NpgsqlDataReader reader = checkDate.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            //Console.WriteLine(reader.GetTimeStamp(0));
                            date = Convert.ToDateTime(reader.GetTimeStamp(0));
                        }
                    }
                    catch (Exception a)
                    { //Console.WriteLine(a); }
                    }
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Gets the rows so we know the total number of rows so we can accurately move around
        /// the dataset
        /// </summary>
        /// <returns>returns an int which is the number of rows in the dataset</returns>
        public int GetRows()
        {
            String connString = (string)Application.Current.Resources["connString"];
            int total = 0;
            using(var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using(NpgsqlCommand totalRows = new NpgsqlCommand("SELECT COUNT(*) FROM calls WHERE Created_date IS NOT NULL AND" +
                    " Closed_date IS NOT NULL AND Due_date IS NOT NULL AND Resolution_action_updated_date IS NOT NULL", conn))
                using(NpgsqlDataReader reader = totalRows.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        total = reader.GetInt32(0);
                    }
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the total number of rows matching the query
        /// </summary>
        /// <param name="filterField">The field we are searching</param>
        /// <param name="filterSelection">The value we want from the field</param>
        /// <returns>Returns the number of rows in the set</returns>
        public int GetRows(String filterField, String filterSelection)
        {
            String connString = (string)Application.Current.Resources["connString"];
            int total = 0;
            using(NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (NpgsqlCommand filterRows = new NpgsqlCommand("SELECT COUNT(*) FROM calls WHERE " + filterField + 
                    " = '" + filterSelection + "' AND Created_date IS NOT NULL", conn))
                using (NpgsqlDataReader reader = filterRows.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        total = reader.GetInt32(0);
                    }
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the number of rows that match the time period selected
        /// </summary>
        /// <param name="start">The Start Date</param>
        /// <param name="end">The End Date</param>
        /// <returns>returns the number of rows in the set</returns>
        public int GetRows(DateTime date1, DateTime date2)
        {
            String connString = (string)Application.Current.Resources["connString"];
            NpgsqlDateTime start = date1, end = date2;
            int total = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (NpgsqlCommand filterRows = new NpgsqlCommand("SELECT COUNT(*) FROM calls WHERE Created_date IS NOT NULL " +
                    "AND Created_date <= " + end + " AND Created_date >= " + start, conn))
                using (NpgsqlDataReader reader = filterRows.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        total = reader.GetInt32(0);
                    }
                }
            }
            return total;
        }
    }
}