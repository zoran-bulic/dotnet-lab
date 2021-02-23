﻿using Dapper;
using OrdersHandler.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersHandler.DataAccess
{
    public class SqliteDataAccess : IDbDataAccess
    {
        private const string _sqliteConnectionStringId = "SQLite";

        public List<T> LoadData<T>(string sql)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = conn.Query<T>(sql, new DynamicParameters());
                return output.ToList();
            }            
        }

        public T LoadData<T>(string sql, int orderId)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
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
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(sql, order);                
            }
        }

        public void UpdateData<T>(string sql, T order)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.ExecuteScalar(sql, order);
            }
        }

        private string LoadConnectionString(string id = _sqliteConnectionStringId)
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}