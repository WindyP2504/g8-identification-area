using System;
using System.Data.SqlClient;

namespace VTP_Induction.Device
{
    public class IotWarehouse
    {
        public class IOT_wareHouse
        {
            public class IOTWareHouseDTO
            {
                public int Id { get; set; }                // Primary key, auto-incremented
                public float Temperature { get; set; }     // Temperature value
                public float Light { get; set; }           // Light value
                public float Humidity { get; set; }        // Humidity value
                public DateTime CreateTime { get; set; }   // Creation time
            }

        }
        public class IOTWareHouseDAL
        {
            private string _connectionString;

            public IOTWareHouseDAL()
            {
                _connectionString = "Data Source=database-rfid.cf8icqgqga7c.us-east-1.rds.amazonaws.com;Initial Catalog=RFID_demo;Persist Security Info=True;User ID=admin;Password=vtp092024";
                // _connectionString = connectionString;
            }

            // Add a new record to IOT_wareHouse using DTO
            //public bool AddIOTWareHouse(IOTWareHouseDTO iotWareHouse)
            //{
            //    using (SqlConnection conn = new SqlConnection(_connectionString))
            //    {
            //        string query = "INSERT INTO IOT_wareHouse (Temperature, Light, Humidity, CreateTime) " +
            //                       "VALUES (@Temperature, @Light, @Humidity, @CreateTime)";

            //        SqlCommand cmd = new SqlCommand(query, conn);

            //        cmd.Parameters.AddWithValue("@Temperature", iotWareHouse.Temperature);
            //        cmd.Parameters.AddWithValue("@Light", iotWareHouse.Light);
            //        cmd.Parameters.AddWithValue("@Humidity", iotWareHouse.Humidity);
            //        cmd.Parameters.AddWithValue("@CreateTime", iotWareHouse.CreateTime != null ? iotWareHouse.CreateTime : (object)DBNull.Value);


            //        conn.Open();
            //        return cmd.ExecuteNonQuery() > 0; // Return true if a row was added
            //    }
            //}

            //// Update a record in IOT_wareHouse using DTO
            //public bool UpdateIOTWareHouse(IOTWareHouseDTO iotWareHouse)
            //{
            //    using (SqlConnection conn = new SqlConnection(_connectionString))
            //    {
            //        string query = "UPDATE IOT_wareHouse SET Temperature = @Temperature, Light = @Light, Humidity = @Humidity, CreateTime = @CreateTime " +
            //                       "WHERE Id = @Id";

            //        SqlCommand cmd = new SqlCommand(query, conn);
            //        // Check if each field is null and assign DBNull.Value if necessary
            //        cmd.Parameters.AddWithValue("@Id", iotWareHouse.Id);
            //        cmd.Parameters.AddWithValue("@Temperature", iotWareHouse.Temperature);
            //        cmd.Parameters.AddWithValue("@Light", iotWareHouse.Light);
            //        cmd.Parameters.AddWithValue("@Humidity", iotWareHouse.Humidity);
            //        cmd.Parameters.AddWithValue("@CreateTime", iotWareHouse.CreateTime != null ? iotWareHouse.CreateTime : (object)DBNull.Value);

            //        conn.Open();
            //        return cmd.ExecuteNonQuery() > 0; // Return true if a row was updated
            //    }
            //}

            // Delete a record from IOT_wareHouse using DTO (only needs Id)
            public bool DeleteIOTWareHouse(int Id)
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "DELETE FROM IOT_wareHouse WHERE Id = @Id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", Id);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0; // Return true if a row was deleted
                }
            }

            // Add a new record to IOT_wareHouse using individual parameters
            public bool AddIOTWareHouse(string area, double temp, double light, double humidity, DateTime createTime)
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "INSERT INTO IOT_wareHouse (Area,Temperature, Light, Humidity, CreateTime) " +
                                   "VALUES (@Area,@Temperature, @Light, @Humidity, @CreateTime)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Area", area);
                    cmd.Parameters.AddWithValue("@Temperature", temp);
                    cmd.Parameters.AddWithValue("@Light", light);
                    cmd.Parameters.AddWithValue("@Humidity", humidity);
                    cmd.Parameters.AddWithValue("@CreateTime", createTime != null ? createTime : (object)DBNull.Value);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0; // Return true if a row was added
                }
            }

            // Update a record in IOT_wareHouse using individual parameters
            public bool UpdateIOTWareHouse(int id, double temp, double light, double humidity, DateTime createTime)
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE IOT_wareHouse SET Temperature = @Temperature, Light = @Light, Humidity = @Humidity, CreateTime = @CreateTime " +
                                   "WHERE Id = @Id";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Temperature", temp);
                    cmd.Parameters.AddWithValue("@Light", light);
                    cmd.Parameters.AddWithValue("@Humidity", humidity);
                    cmd.Parameters.AddWithValue("@CreateTime", createTime != null ? createTime : (object)DBNull.Value);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0; // Return true if a row was updated
                }
            }


            //public void AddIOTWareHouse(string p1, float p2, double p3, double p4, System.DateTime dateTime)
            //{
            //    throw new System.NotImplementedException();
            //}
        }
    }
}
