using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersHandler.DataAccess
{
    public class SqliteDataAccess : IDbDataAccess
    {
        private const string _sqliteConnectionStringId = "SQLite";
        private string _connString;

        public SqliteDataAccess(ConnectionStringSettings connStringSetting = null)
        {
            if(connStringSetting != null)
            {
                _connString = connStringSetting.ConnectionString;
            }
            else
            {
                _connString = LoadConnectionString("SQLite");
            }
        }

        public List<T> LoadData<T>(string sql)
        {
            using (IDbConnection conn = new SQLiteConnection(_connString))
            {
                var output = conn.Query<T>(sql, new DynamicParameters());
                return output.ToList();
            }            
        }

        public T LoadData<T>(string sql, int orderId)
        {
            using (IDbConnection conn = new SQLiteConnection(_connString))
            {
                var output = conn.Query<T>(sql, orderId);
                if (output != null && output.Count() > 0)
                {
                    return output.First();
                }
                return default(T);
            }
        }

        public void SaveData<T>(string sql, T order) 
        {
            using (IDbConnection conn = new SQLiteConnection(_connString))
            {
                conn.ExecuteScalar<T>(sql, order);                
            }
        }

        public void UpdateData<T>(string sql, T order)
        {
            using (IDbConnection conn = new SQLiteConnection(_connString))
            {
                conn.ExecuteScalar(sql, order);
            }
        }

        public async Task<T> LoadDataAsync<T>(string sql, int orderId)
        {
            using (IDbConnection conn = new SQLiteConnection(_connString))
            {
                var output = await conn.QueryAsync<T>(sql, orderId);
                if (output != null && output.Count() > 0)
                {
                    return output.First();
                }
                return default(T);
            }
        }

        public async Task<int> InsertDataAsync<T>(string sql, T order)
        {
            int createdOrderId;
            using (IDbConnection conn = new SQLiteConnection(_connString))
            {
                createdOrderId = await conn.ExecuteScalarAsync<int>(sql, order);
            }            
            return createdOrderId;
        }

        public async Task UpdateDataAsync<T>(string sql, T order)
        {            
            using (IDbConnection conn = new SQLiteConnection(_connString))
            {
                await conn.ExecuteScalarAsync<int>(sql, order);                
            }            
        }

        #region Helper methods
        private string LoadConnectionString(string id = _sqliteConnectionStringId)
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        #endregion // Helper methods        
    }
}
