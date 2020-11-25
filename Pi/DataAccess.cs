using System;
using System.Data;
using System.Data.SqlClient;

namespace Pi
{
    
    public static class DataAccess
    {
        
        /// <summary>
        /// Insert into [Record] table with insert date.
        /// </summary>
        public static int Insert()
        {
            try
            {
                using var connection = new SqlConnection(SqlConnectionStringBuilder.ConnectionString);
                connection.Open();
                
                const string sql = "INSERT INTO Record ([Id], [OpenOn], [CreateDate], [CreateBy]) VALUES (@id, @openOn, @createDate, @createBy)";
                
                using var command = new SqlCommand(sql, connection);
                command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                command.Parameters.Add("@openOn", SqlDbType.DateTimeOffset).Value = DateTimeOffset.Now;
                command.Parameters.Add("@createBy", SqlDbType.VarChar).Value = "TrusharM";
                command.Parameters.Add("@createDate", SqlDbType.DateTimeOffset).Value = DateTimeOffset.Now;
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                e.FailFastIfCriticalException("Pi.DataAccess.Insert");
                throw;
            }
        }
        
        
        /// <summary>
        /// Insert into [Record] table with insert date.
        /// </summary>
        public static Guid GetLastRecordId()
        {
            try
            {
                using var connection = new SqlConnection(SqlConnectionStringBuilder.ConnectionString);
                connection.Open();
                
                const string sql = "SELECT Id FROM Record WHERE OpenOn IS NOT NULL AND CloseOn IS NULL ORDER BY CreateDate DESC";
                
                using var command = new SqlCommand(sql, connection);

                var id = command.ExecuteScalar();
                return (Guid?) id ?? Guid.Empty ;
            }
            catch (Exception e)
            {
                e.FailFastIfCriticalException("Pi.DataAccess.Insert");
                throw;
            }
        }
        
        
        /// <summary>
        /// Update [Record] table with close date.
        /// </summary>
        public static int Update(Guid id)
        {
            if (id == Guid.Empty) return 0;
            
            try
            {
                using var connection = new SqlConnection(SqlConnectionStringBuilder.ConnectionString);
                connection.Open();
                
                const string sql = "UPDATE Record SET [CloseOn] = @closeOn WHERE [ID] = @id";
                
                using var command = new SqlCommand(sql, connection);
                command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                command.Parameters.Add("@closeOn", SqlDbType.DateTimeOffset).Value = DateTimeOffset.Now;
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                e.FailFastIfCriticalException("Pi.DataAccess.Update");
                throw;
            }
        }


        /// <summary>
        /// Connection string.
        /// </summary>
        private static readonly SqlConnectionStringBuilder SqlConnectionStringBuilder = new SqlConnectionStringBuilder
        {
            DataSource = "192.168.86.44",
            UserID = "sa",
            Password = "<YourStrong@Passw0rd>",
            InitialCatalog = "pi"
        };
    }
    
}
