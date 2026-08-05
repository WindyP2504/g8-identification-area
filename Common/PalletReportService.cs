using System;
using System.Data;
using System.Data.SqlClient;

namespace VTP_Induction.Common
{
    public class PalletReportService
    {
        private readonly string _connectionString;
        public PalletReportService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // LỆNH ĐANG CHỜ (ĐANG THỰC HIỆN) — từ WCS_Pallet_Prod
        public DataTable LoadPendingPallets(DateTime fromDate, DateTime toDate)
        {
            string sql = @"
SELECT [Id],[Pallet_ID],[Item_Code],[Item_Name],[Ctn],[Qty],[PO_Name],
       [Location],[Status],[WH_Code],[Line_ID],[PO_ID],[CreatedAt],[Inner_Pallet],[Pcs],
       'Prod' AS Source
FROM [G8_WCS].[dbo].[WCS_Pallet_Prod]
WHERE CreatedAt >= @FromDate AND CreatedAt < @ToDateExclusive
ORDER BY CreatedAt DESC;";

            return ExecuteQuery(sql, fromDate, toDate);
        }

        // LỆNH ĐÃ HOÀN THÀNH (ĐÃ XÓA) — từ WCS_Pallet_His
        public DataTable LoadCompletedPallets(DateTime fromDate, DateTime toDate)
        {
            string sql = @"
SELECT [Id],[Pallet_ID],[Item_Code],[Item_Name],[PO_Name],[Ctn],[Qty],[Pcs],
       [Location],[Status],[WH_Code],[Line_ID],[PO_ID],[CreatedAt],
       [DoneAt],[DoneReason],[Inner_Pallet],
       'His' AS Source
FROM [G8_WCS].[dbo].[WCS_Pallet_His]
WHERE CreatedAt >= @FromDate AND CreatedAt < @ToDateExclusive
ORDER BY DoneAt DESC;";

            return ExecuteQuery(sql, fromDate, toDate);
        }

        public int CountDonePalletsByPO(string poName)
        {
            string sql = @"SELECT
    (SELECT COUNT(*)
     FROM [G8_WCS].[dbo].[WCS_Pallet_Prod]
     WHERE PO_Name = @PO_Name
       AND Status = 'DONE')
    +
    (SELECT COUNT(*)
     FROM [G8_WCS].[dbo].[WCS_Pallet_His]
     WHERE PO_Name = @PO_Name
       AND Status = 'DONE');";

            object result = ExecuteScalar(
                sql,
                new SqlParameter("@PO_Name", poName)
            );

            return result == null || result == DBNull.Value
                ? 0
                : Convert.ToInt32(result);
        }

        private object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        private DataTable ExecuteQuery(string sql, DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate.Date;
                cmd.Parameters.Add("@ToDateExclusive", SqlDbType.Date).Value = toDate.Date.AddDays(1);

                conn.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    adapter.Fill(dt);
            }
            return dt;
        }

        public DataTable LoadParcelsByPallet(string palletId, string source, string date)
        {
            string sql;
            if (source == "His")
            {
                sql = "SELECT [Id],[ParcelCode],[ReceivedCode],[Pallet_ID],[Location]," +
                    "[Status],[Line_ID],[CreatedAt],[DoneAt],[DoneReason] FROM [G8_WCS].[dbo].[WCS_Parcels_His] " +
                    "WHERE Pallet_ID = @PalletID AND CreatedAt >= @FromDate AND CreatedAt < @ToDate ORDER BY Id ASC;";
            }
            else // "Prod"
            {
                sql = "SELECT [Id],[ParcelCode],[ReceivedCode],[Pallet_ID],[Location]," +
                    "[Status],[Line_ID],[CreatedAt] FROM [G8_WCS].[dbo].[WCS_Parcels_Prod] " +
                    "WHERE Pallet_ID = @PalletID ORDER BY Id ASC;";
            }

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                DateTime d = DateTime.Parse(date).Date;

                cmd.Parameters.Add("@PalletID", SqlDbType.NVarChar, 50).Value = palletId;
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = d;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = d.AddDays(1);

                conn.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    adapter.Fill(dt);
            }

            // Thêm cột STT, đánh số 1 -> hết theo đúng thứ tự dòng trong DataTable
            dt.Columns.Add("STT", typeof(int));
            dt.Columns["STT"].SetOrdinal(0); // đưa STT lên đầu bảng

            int stt = 1;
            foreach (DataRow row in dt.Rows)
            {
                row["STT"] = stt++;
            }

            return dt;
        }

        public DataTable GetPalletInfo(string pallet)
        {
            string sql = "SELECT TOP 1 Pallet_ID, Item_Code, Location, Status, WH_Code, Inner_Carton, Weight_ref, Item_Name FROM dbo.WCS_Pallet_Prod WHERE Status <> 'WAIT' ORDER BY Id ASC";
            return ExecuteQuery(sql, DateTime.Today, DateTime.Today);
        }

        public class PalletReportSummary
        {
            public int TotalPending { get; set; }     // Tổng số lệnh đang thực hiện
            public int TotalCompleted { get; set; }    // Tổng số lệnh đã qua
            public int TotalCtnCompleted { get; set; } // Tổng số thùng đã hoàn thành
        }

        public PalletReportSummary GetSummary(DateTime fromDate, DateTime toDate)
        {
            var summary = new PalletReportSummary();

            string sql = @"SELECT (SELECT COUNT(*) FROM [G8_WCS].[dbo].[WCS_Pallet_Prod] WHERE CreatedAt >= @FromDate AND CreatedAt < @ToDateExclusive) AS TotalPending, 
(SELECT COUNT(*) FROM [G8_WCS].[dbo].[WCS_Pallet_His] WHERE CreatedAt >= @FromDate AND CreatedAt < @ToDateExclusive) AS TotalCompleted, (SELECT ISNULL(SUM(Ctn), 0) 
FROM [G8_WCS].[dbo].[WCS_Pallet_His] WHERE CreatedAt >= @FromDate AND CreatedAt < @ToDateExclusive) AS TotalCtnCompleted;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate.Date;
                cmd.Parameters.Add("@ToDateExclusive", SqlDbType.DateTime).Value = toDate.Date.AddDays(1);

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        summary.TotalPending = rd["TotalPending"] == DBNull.Value ? 0 : Convert.ToInt32(rd["TotalPending"]);
                        summary.TotalCompleted = rd["TotalCompleted"] == DBNull.Value ? 0 : Convert.ToInt32(rd["TotalCompleted"]);
                        summary.TotalCtnCompleted = rd["TotalCtnCompleted"] == DBNull.Value ? 0 : Convert.ToInt32(rd["TotalCtnCompleted"]);
                    }
                }
            }

            return summary;
        }
    }
}
